using System;
using System.Collections.Generic;
using System.Text;

namespace ProxySchedule
{
    public class Proxy
    {
        public string IP { get; set; }
        public int Port { get; set; }

        public Proxy(string ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }
    }
}
