using System;
using System.Threading.Tasks;
using System.Threading;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using EmbedIO.Files;
using EmbedIO.Authentication;

namespace api_corelation.Models
{
    public class HttpServerRunner
    {
        public string port = "8080";
        public string folder = "";
        public string status = "Not running";
        public bool IsLaunched = false;
        CancellationTokenSource ctSource;
        private WebServer server;
        public HttpServerRunner()
        {
        }
        public void Start()
        {
            ctSource = new CancellationTokenSource();
            var server = new WebServer(o => o
                    .WithUrlPrefix("http://*:" + port)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithLocalSessionManager()
                // First, we will configure our web server by adding Modules.
                .WithStaticFolder("/", folder, true, m => m
                    .WithContentCaching()).WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })));
            server.RunAsync(ctSource.Token).ConfigureAwait(false);
            IsLaunched = true;
        }
        public void Stop()
        {
            ctSource.Cancel();
            IsLaunched = false;
        }
    }
}