using System;
using System.Threading;
using System.Collections.Generic;
using ReactiveUI;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace simpleserver.Models
{
    public class HttpServerRunner : ReactiveObject
    {
        public string port = "8080";
        public string folder = "";
        public string Status = "Init";
        public string Log = "";
        public bool IsLaunched = false;
        public bool IsSecure = false;
        CancellationTokenSource ctSource;
        ResponseHeaders Headers;
        private IServer ServerInstance;
        private WebHostBuilder ServerBuilder;
        public HttpServerRunner()
        {
           
        }
        public void Start()
        {
            try
            {
                ServerBuilder = new WebHostBuilder();
                ServerBuilder.UseServer(ServerInstance).UseUrls("http://*:"+port).UseContentRoot(folder).Build().Start();
                ctSource = new CancellationTokenSource();
                Status = "Running";
                //ServerLogger logger = new ServerLogger();
                //Log = logger.GetCapturedLogs();
                //IsLaunched = true;
                //server.RunAsync(ctSource.Token).ConfigureAwait(false);
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