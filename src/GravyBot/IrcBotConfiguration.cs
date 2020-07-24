using System.Collections.Generic;

namespace GravyBot
{
    public class IrcBotConfiguration
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public IEnumerable<string> Channels { get; set; }
        public string Nick { get; set; }
        public string Identity { get; set; }
        public string NickServ { get; set; }
    }
}
