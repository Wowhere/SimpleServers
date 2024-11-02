using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_corelation.Models
{
    public class ServerOptions
    {
        public int Port;
        public string WorkingDirectory;
        public string[] Headers;
        public bool IsLaunched = false;
        public ServerOptions(int port, string directory, string[] headers) { 
            Port = port;
            WorkingDirectory = directory;
            Headers = headers;
        }
    }
}
