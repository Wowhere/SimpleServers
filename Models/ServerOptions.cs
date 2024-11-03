using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_corelation.Models
{
    public class ServerOptions
    {
        public string Port;
        public string WorkingDirectory;
        public string[] Headers;
        public bool IsLaunched = false;
        public SimpleHttpServer server;
        public ServerOptions(string port, string directory, string[] headers) { 
            Port = port;
            WorkingDirectory = directory;
            Headers = headers;
            server = new SimpleHttpServer(port, WorkingDirectory);
        }
    }
}
