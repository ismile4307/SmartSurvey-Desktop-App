using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for FrmUploadMedia.xaml
    /// </summary>
    public partial class FrmUploadMedia : Window
    {
        private string myPath;
        private string fileName;
        private long _fileSize;
        private CancellationTokenSource _cts;
        private DateTime _uploadStartTime;

        // Retry delays in seconds: 5s → 15s → 30s (4 total attempts)
        private static readonly int[] RetryDelays = { 5, 15, 30 };

        // Single shared HttpClient — infinite timeout, cancellation via CancellationToken
        // SSL bypass is handled via ServicePointManager (required on .NET 4.5)
        private static readonly HttpClient _httpClient;

        static FrmUploadMedia()
        {
            var handler = new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultCredentials
            };
            _httpClient = new HttpClient(handler)
            {
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public FrmUploadMedia()
        {
            InitializeComponent();
        }

        // ── Logging ──────────────────────────────────────────────────────────

        private void Log(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            Dispatcher.Invoke(() =>
            {
                txtLog.AppendText(line);
                txtLog.ScrollToEnd();
            });
        }

        // ── Browse ────────────────────────────────────────────────────────────

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    InitialDirectory = Properties.Settings.Default.StartupPath,
                    FileName = "",
                    Filter = "Zip File (*.zip)|*.zip|All Files (*.*)|*.*"
                };

                if (dlg.ShowDialog() == true)
                {
                    txtScriptPath.Text = dlg.FileName;
                    myPath   = Path.GetDirectoryName(dlg.FileName);
                    fileName = Path.GetFileName(dlg.FileName);

                    var info = new FileInfo(dlg.FileName);
                    _fileSize = info.Length;

                    txtFileInfo.Text = $"Size: {FormatBytes(_fileSize)}   |   " +
                                       $"Type: {info.Extension.ToUpper()}   |   " +
                                       $"Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm}";
                    txtFileInfo.Foreground = Brushes.DarkSlateGray;

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    Log($"File selected: {fileName}  ({FormatBytes(_fileSize)})");
                }
                else
                {
                    txtScriptPath.Text = "";
                    txtFileInfo.Text   = "No file selected";
                    txtFileInfo.Foreground = Brushes.Gray;
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR browsing: {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Upload ────────────────────────────────────────────────────────────

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            ResetProgress();
            SetUploadingState(true);

            try
            {
                // Prepare temp copy
                Log("Preparing temporary copy of file...");
                string sourcePath = txtScriptPath.Text;
                string tempDir    = Path.Combine(myPath, "temp");
                string tempFile   = Path.Combine(tempDir, fileName);

                await Task.Run(() =>
                {
                    Directory.CreateDirectory(tempDir);
                    File.Copy(sourcePath, tempFile, overwrite: true);
                });
                Log($"Temp file ready: {tempFile}");

                // TLS setup
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol  = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                _cts = new CancellationTokenSource();

                string uploadUrl = StaticClass.SERVER_URL + "/api/upload/media";
                _uploadStartTime = DateTime.Now;

                Log($"Connecting to: {uploadUrl}");
                Log($"Uploading {fileName} ({FormatBytes(_fileSize)})...");
                Dispatcher.Invoke(() => txtStatus.Text = "Uploading... (Attempt 1)");

                string response = await UploadWithRetryAsync(tempFile, uploadUrl, _fileSize);

                if (_cts.IsCancellationRequested) return;

                TimeSpan elapsed = DateTime.Now - _uploadStartTime;

                progressBar.Value = 100;
                txtPercent.Text   = "100%";
                Log($"Server response: {response}");
                Log($"Finished in {elapsed.TotalSeconds:F1}s  |  Avg speed: {FormatBytes((long)(_fileSize / Math.Max(elapsed.TotalSeconds, 0.1)))}/s");

                if (response.Contains("successfully"))
                {
                    txtStatus.Text       = "Upload complete!";
                    txtStatus.Foreground = Brushes.Green;
                    Log("SUCCESS: Media uploaded successfully.");
                    MessageBox.Show("Media uploaded successfully.", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    txtStatus.Text       = "Upload complete with warning.";
                    txtStatus.Foreground = Brushes.OrangeRed;
                    Log("WARNING: Unexpected server response.");
                    MessageBox.Show($"Server responded with:\n{response}", "Upload Result",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (OperationCanceledException)
            {
                txtStatus.Text       = "Upload cancelled.";
                txtStatus.Foreground = Brushes.OrangeRed;
                Log("Upload was cancelled by user.");
            }
            catch (Exception ex)
            {
                txtStatus.Text       = "Upload failed.";
                txtStatus.Foreground = Brushes.Red;
                Log($"ERROR: {ex.Message}");
                MessageBox.Show(ex.Message, "Upload Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetUploadingState(false);
            }
        }

        // ── Retry logic ───────────────────────────────────────────────────────

        private async Task<string> UploadWithRetryAsync(string tempFile, string uploadUrl, long fileSize)
        {
            int maxAttempts = RetryDelays.Length + 1; // 4 total

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                _cts.Token.ThrowIfCancellationRequested();

                if (attempt > 1)
                    Log($"--- Attempt {attempt} of {maxAttempts} ---");

                try
                {
                    using (var fileStream = File.OpenRead(tempFile))
                    using (var content = new MultipartFormDataContent())
                    {
                        var progressStream = new ProgressStream(fileStream, fileSize, OnProgressUpdate);
                        var streamContent  = new StreamContent(progressStream);
                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        content.Add(streamContent, "file", Path.GetFileName(tempFile));

                        using (var response = await _httpClient.PostAsync(uploadUrl, content, _cts.Token))
                        {
                            int statusCode = (int)response.StatusCode;

                            // 5xx: retry if attempts remain
                            if (statusCode >= 500 && attempt < maxAttempts)
                            {
                                Log($"Server error ({statusCode}). Waiting before retry...");
                                await CountdownDelayAsync(RetryDelays[attempt - 1]);
                                continue;
                            }

                            string body = await response.Content.ReadAsStringAsync();

                            if (!response.IsSuccessStatusCode)
                                throw new Exception($"Server returned {statusCode}: {body}");

                            return body;
                        }
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (HttpRequestException ex) when (attempt < maxAttempts)
                {
                    Log($"Network error: {ex.Message}");
                    Log($"Waiting before retry...");
                    await CountdownDelayAsync(RetryDelays[attempt - 1]);
                }
            }

            throw new Exception("Upload failed after all retry attempts.");
        }

        // Countdown shown in status bar while waiting to retry
        private async Task CountdownDelayAsync(int seconds)
        {
            for (int i = seconds; i > 0; i--)
            {
                int remaining = i;
                Dispatcher.Invoke(() => txtStatus.Text = $"Retrying in {remaining}s...");
                await Task.Delay(1000, _cts.Token);
            }
        }

        // ── Progress ──────────────────────────────────────────────────────────

        private void OnProgressUpdate(long bytesSent, long totalBytes)
        {
            Dispatcher.Invoke(() =>
            {
                double percent = totalBytes > 0 ? (double)bytesSent / totalBytes * 100 : 0;
                progressBar.Value   = Math.Min(percent, 99); // reserve 100% for confirmed success
                txtPercent.Text     = $"{percent:F0}%";
                txtTransferred.Text = $"{FormatBytes(bytesSent)} / {FormatBytes(totalBytes)}";

                double elapsed = (DateTime.Now - _uploadStartTime).TotalSeconds;
                if (elapsed > 0 && bytesSent > 0)
                    txtSpeed.Text = $"{FormatBytes((long)(bytesSent / elapsed))}/s";
            });
        }

        // ── Progress stream ───────────────────────────────────────────────────

        private sealed class ProgressStream : Stream
        {
            private readonly Stream _inner;
            private readonly long _totalBytes;
            private long _bytesRead;
            private readonly Action<long, long> _onProgress;

            public ProgressStream(Stream inner, long totalBytes, Action<long, long> onProgress)
            {
                _inner      = inner;
                _totalBytes = totalBytes;
                _onProgress = onProgress;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int n = _inner.Read(buffer, offset, count);
                if (n > 0)
                {
                    _bytesRead += n;
                    _onProgress(_bytesRead, _totalBytes);
                }
                return n;
            }

            public override bool CanRead  => true;
            public override bool CanSeek  => false;
            public override bool CanWrite => false;
            public override long Length   => _totalBytes;
            public override long Position
            {
                get => _bytesRead;
                set => throw new NotSupportedException();
            }
            public override void Flush() { }
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value)                 => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                if (disposing) _inner.Dispose();
                base.Dispose(disposing);
            }
        }

        // ── Cancel ────────────────────────────────────────────────────────────

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            _cts?.Cancel();
            Log("Cancelling upload...");
        }

        // ── Exit ──────────────────────────────────────────────────────────────

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            this.Close();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtScriptPath.Text))
            {
                MessageBox.Show("Please select a file first.", "Validation",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!File.Exists(txtScriptPath.Text))
            {
                MessageBox.Show("Selected file does not exist.", "Validation",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ResetProgress()
        {
            txtLog.Clear();
            progressBar.Value   = 0;
            txtPercent.Text     = "0%";
            txtTransferred.Text = "";
            txtSpeed.Text       = "";
            txtStatus.Text      = "Starting...";
            txtStatus.Foreground = Brushes.Black;
        }

        private void SetUploadingState(bool uploading)
        {
            btnUpload.IsEnabled = !uploading;
            btnBrowse.IsEnabled = !uploading;
            btnCancel.IsEnabled = uploading;
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes >= 1_073_741_824) return $"{bytes / 1_073_741_824.0:F2} GB";
            if (bytes >= 1_048_576)     return $"{bytes / 1_048_576.0:F2} MB";
            if (bytes >= 1_024)         return $"{bytes / 1_024.0:F1} KB";
            return $"{bytes} B";
        }
    }
}
