using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting.Server;

namespace api_corelation.Models
{
    public class CustomServer : IServer
    {
        private readonly HttpListener _listener;

        public IFeatureCollection Features {  get; private set; }

        public CustomServer(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/");
        }

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            _listener.Start();
            //Console.WriteLine($"Custom server listening on http://*:5000/");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listener.Stop();
            //Console.WriteLine("Custom server stopped.");
            return Task.CompletedTask;
        }

        public Task HandleRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            string[] headers = request.Headers.AllKeys;
            //foreach (string header in headers)
            //{
            //    Console.WriteLine($"{header}: {request.Headers[header]}");
            //}

            string responseString = $"Hello World! Request received at {DateTime.Now}";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            return Task.CompletedTask;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
