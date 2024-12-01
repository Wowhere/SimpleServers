using System;
using System.Threading.Tasks;
using System.Threading;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using EmbedIO.Files;
using EmbedIO.Authentication;
using EmbedIO.Routing;
using System.Collections.Generic;

namespace simpleserver.Models
{
    public class TestController : WebApiController
    {
        [Route(HttpVerbs.Get, "/t")]
        public string Test() {
            HttpContext.Response.Headers.Clear();
            HttpContext.Response.Headers.Add("Server1: lol123");
            return "testtest";
        }
        public TestController() {
        }

    }
    public class HttpServerRunner
    {
        public string port = "8080";
        public string folder = "";
        public string Status = "";
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
                var server = new WebServer(o => o
                    .WithUrlPrefix("http://*:" + port)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithLocalSessionManager()
                    .WithStaticFolder("/", folder, true, m => m.WithContentCaching())
                    .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })))
                    .WithWebApi("/", m => m.WithController<TestController>());
                Status = "Running";
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