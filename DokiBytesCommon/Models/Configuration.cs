using System.Collections.Generic;

namespace DokiBytesCommon.Models
{
    public class Configuration
    {
        public string LocalPath { get; set; }
        public string RemotePath { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public List<string> Ignore { get; set; }

    }
}
