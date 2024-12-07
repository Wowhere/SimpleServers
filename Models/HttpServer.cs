using System;
using System.Threading;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using EmbedIO.Files;
using EmbedIO.Authentication;
using EmbedIO.Routing;
using System.Collections.Generic;
using ReactiveUI;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Swan;
using System.IO;
using System.Text;

namespace simpleserver.Models
{
    public class ServerLogger : Swan.Logging.ILogger
    {
        public Swan.Logging.LogLevel LogLevel =>
            Swan.Logging.LogLevel.Debug;
        private MemoryStream memoryStream = new MemoryStream() {};
        private StreamWriter logWriter;
        //private StreamReader logReader;
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public ServerLogger()
        {
            logWriter = new StreamWriter(memoryStream);
        }
        public void Log(Swan.Logging.LogMessageReceivedEventArgs logEvent)
        {
            using (logWriter)
            {
                logWriter.WriteLine(logEvent.CallerLineNumber);
                logWriter.WriteLine(logEvent.UtcDate);
                logWriter.WriteLine(logEvent.Message);
                logWriter.WriteLine(logEvent.Exception);
            }
            //Debug.Write(logEvent.ToJson());
        }
        public string GetCapturedLogs()
        {
            string line = "";
            using (var reader = new StreamReader(memoryStream))
            {
                line = reader.ReadToEnd();
            }
            return line;
        }
    }
    public class TestController : WebApiController
    {
        [Route(HttpVerbs.Get, "/_test")]
        public string Test() {
            HttpContext.Response.Headers.Clear();
            HttpContext.Response.Headers.Add("Server: Test Server");
            return "Test OK";
        }
        public TestController() {
        }

    }
    public class HttpServerRunner : ReactiveObject
    {
        public string port = "8080";
        public string folder = "";
        public string Status = "Init";
        public string Log = "";
        public bool IsLaunched = false;
        public bool IsSecure = false;
        CancellationTokenSource ctSource;
        private WebServer server;
        public HttpServerRunner()
        {
        }
        public void Start()
        {
            try
            {
                
                ctSource = new CancellationTokenSource();
                //string certPath = $"File://{Application.streamingAssetsPath}/cert.pfx";
                //var cert = new X509Certificate2(new X509Certificate($"{Application.streamingAssetsPath}/cert.pfx"));
                server = new WebServer(o => o
                    .WithUrlPrefix("http://*:" + port)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithLocalSessionManager()
                    .WithStaticFolder("/", folder, true, m => m.WithContentCaching())
                    .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Get Index page" })))
                    .WithWebApi("/_test", m => m.WithController<TestController>());

                //var options = new WebServerOptions()
                //    .WithCertificate()
                //    .WithAutoRegisterCertificate(true);
                Swan.Logging.Logger.RegisterLogger<ServerLogger>();
                Status = "Running";
                ServerLogger logger = new ServerLogger();
                Log = logger.GetCapturedLogs();
                IsLaunched = true;
                server.RunAsync(ctSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                IsLaunched = false;
                Status = ex.Message.ToString();
            }
        }
        public void Stop()
        {
            ctSource.Cancel();
            IsLaunched = false;
            Status = "Stopped";
        }
    }
}