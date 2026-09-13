using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net;
using System.Text;
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
        private WebClient _uploadClient;
        private CancellationTokenSource _cts;
        private DateTime _uploadStartTime;


        public FrmUploadMedia()
        {
            InitializeComponent();
        }

        // ── Logging ──────────────────────────────────────────────────────────

        private void Log(string message)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {message}\n";
            txtLog.AppendText(line);
            txtLog.ScrollToEnd();
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
                string sourcePath = txtScriptPath.Text;   // capture on UI thread
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
                _uploadClient = new WebClient();
                _uploadClient.Credentials = CredentialCache.DefaultCredentials;
                _uploadClient.UploadProgressChanged += OnUploadProgressChanged;

                string uploadUrl = StaticClass.SERVER_URL + "/deskapi/uploadmedia.php";
                _uploadStartTime = DateTime.Now;

                Log($"Connecting to: {uploadUrl}");
                Log($"Uploading {fileName} ({FormatBytes(_fileSize)})...");
                txtStatus.Text = "Uploading...";

                byte[] responseBytes = await _uploadClient.UploadFileTaskAsync(
                    new Uri(uploadUrl), "POST", tempFile);

                if (_cts.IsCancellationRequested) return;

                string response = Encoding.UTF8.GetString(responseBytes);
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
                _uploadClient?.Dispose();
                _uploadClient = null;
                SetUploadingState(false);
            }
        }

        // ── Progress callback ─────────────────────────────────────────────────

        private void OnUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                progressBar.Value = e.ProgressPercentage;
                txtPercent.Text   = $"{e.ProgressPercentage}%";
                txtTransferred.Text = $"{FormatBytes(e.BytesSent)} / {FormatBytes(e.TotalBytesToSend)}";

                double elapsed = (DateTime.Now - _uploadStartTime).TotalSeconds;
                if (elapsed > 0 && e.BytesSent > 0)
                    txtSpeed.Text = $"{FormatBytes((long)(e.BytesSent / elapsed))}/s";
            });
        }

        // ── Cancel ────────────────────────────────────────────────────────────

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            _cts?.Cancel();
            _uploadClient?.CancelAsync();
            Log("Cancelling upload...");
        }

        // ── Exit ──────────────────────────────────────────────────────────────

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _uploadClient?.CancelAsync();
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
