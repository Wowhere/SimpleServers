using System;
using System.Threading;
using System.Collections.Generic;
using ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Threading.Tasks;
using api_corelation.Models;
using Microsoft.Extensions.Hosting;

namespace simpleserver.Models
{
    public class HttpServerRunner
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
        private static WebHostBuilder ServerBuilder;
        public HttpServerRunner()
        {
           
        }
        public void Start()
        {
            try
            {
                //var builder = WebApplication.CreateBuilder();

                //// Add services to the container.
                //builder.Services.AddRazorPages();

                //var app = builder.Build();

                //// Configure the HTTP request pipeline.
                //if (!app.Environment.IsDevelopment())
                //{
                //    app.UseExceptionHandler("/Error");
                //}
                //app.UseStaticFiles();

                //app.UseRouting();

                //app.UseAuthorization();

                //app.MapRazorPages();

                //app.Run();
                ServerInstance = new CustomServer(Int32.Parse(port));
                ServerBuilder = new WebHostBuilder();
                ServerBuilder.UseServer(ServerInstance).UseUrls("http://*:"+port).UseContentRoot(folder).Configure(app => app.UsePathBase(""));
                ServerBuilder.Build().RunAsync();
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