using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class MyWebRequest
    {
        private WebRequest request;
        private Stream dataStream;
        private byte[] _pendingPostData;

        private string status;

        public String Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public MyWebRequest(string url)
        {
            // using System.Net;

            ServicePointManager.Expect100Continue = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

            // Create a request using a URL that can receive a post.

            request = WebRequest.Create(url);
        }

        public MyWebRequest(string url, string method)
            : this(url)
        {
            // using System.Net;

            ServicePointManager.Expect100Continue = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

            if (method.Equals("GET") || method.Equals("POST"))
            {
                // Set the Method property of the request to POST.
                request.Method = method;
            }
            else
            {
                throw new Exception("Invalid Method Type");
            }
        }

        public MyWebRequest(string url, string method, string data)
            : this(url, method)
        {
            try
            {
                // using System.Net;

                ServicePointManager.Expect100Continue = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Use SecurityProtocolType.Ssl3 if needed for compatibility reasons

                // Store POST data — written in GetResponse() so headers can still be set
                _pendingPostData = Encoding.UTF8.GetBytes(data);

                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/x-www-form-urlencoded";

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = _pendingPostData.Length;
            }
            catch (Exception ex)
            {
                //return ex.Message;
                MessageBox.Show(ex.Message);
            }

        }

        public void SetHeader(string name, string value)
        {
            // HttpWebRequest treats some headers as restricted properties
            var http = request as HttpWebRequest;
            if (http != null && name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
                http.Accept = value;
            else
                request.Headers[name] = value;
        }

        public string GetResponse()
        {
            try
            {
                // Write deferred POST body now (after any SetHeader calls)
                if (_pendingPostData != null)
                {
                    dataStream = request.GetRequestStream();
                    dataStream.Write(_pendingPostData, 0, _pendingPostData.Length);
                    dataStream.Close();
                    _pendingPostData = null;
                }

                // Get the original response.
                WebResponse response = request.GetResponse();

                this.Status = ((HttpWebResponse)response).StatusDescription;

                // Get the stream containing all content returned by the requested server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content fully up to the end.
                string responseFromServer = reader.ReadToEnd();

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

                return responseFromServer;
            }
            catch (WebException ex) when (ex.Response != null)
            {
                // Server returned a non-2xx status — read and return the body (usually JSON)
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
