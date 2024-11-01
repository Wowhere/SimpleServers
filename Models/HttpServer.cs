using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace api_corelation.Models
{
    class SimpleHttpServer
    {
        private HttpListener _listener;
        private string _folderPath;

        public SimpleHttpServer(string folderPath)
        {
            _folderPath = folderPath;
            _listener = new HttpListener();
        }

        public void Start(int port)
        {
            _listener.Prefixes.Add($"http://*:{port}/");
            _listener.Start();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public void Serve()
        {
            while (_listener.IsListening)
            {
                HttpListenerContext context = _listener.GetContext();
                HandleRequest(context);
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string filePath = request.Url.Segments[1].Trim('/');
            string fullPath = Path.Combine(_folderPath, filePath);

            if (!File.Exists(fullPath))
            {
                SendError(response, HttpStatusCode.NotFound);
                return;
            }

            string contentType = GetContentType(Path.GetExtension(filePath));
            if (contentType is null)
            {

            }
            byte[] fileBytes = File.ReadAllBytes(fullPath);
            string mimeType = $"application/octet-stream";

            response.ContentType = contentType ?? mimeType;
            response.ContentLength64 = fileBytes.Length;

            Stream outputStream = response.OutputStream;
            outputStream.Write(fileBytes, 0, fileBytes.Length);
            outputStream.Close();
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                //case ".css": return "text/css";
                //case ".js": return "application/javascript";
                //case ".png": return "image/png";
                //case ".jpg": return "image/jpeg";
                //case ".gif": return "image/gif";
                //case ".ico": return "image/x-icon";
                case ".svg": return "image/svg+xml";
                case ".txt": return "text/plain";
                case ".xml": return "application/xml";
                case ".json": return "application/json";
                default: return null;
            }
        }

        private void SendError(HttpListenerResponse response, HttpStatusCode statusCode)
        {
            response.StatusCode = (int)statusCode;
            response.StatusDescription = statusCode.ToString();
            response.Close();
        }
    }
}
