using System.Collections.Generic;

namespace GravyBot
{
    /// <summary>
    /// Configuration for bot
    /// </summary>
    public class IrcBotConfiguration
    {
        /// <summary>
        /// IP adress or URI of server to initiate a TCP connection to
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// TCP port to initiate connection on
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// List of channels to join by default
        /// </summary>
        public IEnumerable<string> Channels { get; set; }

        /// <summary>
        /// Nick to use for bot
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        /// Real name to use for bot
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// Optional NickServ password
        /// </summary>
        public string NickServ { get; set; }

        /// <summary>
        /// Channel to send log messages to
        /// </summary>
        public string LogChannel { get; set; }

        /// <summary>
        /// Prefix to use for commands
        /// </summary>
        public string CommandPrefix { get; set; }

        /// <summary>
        /// Channel to send notifications to
        /// </summary>
        public string NotifyChannel { get; set; }

        /// <summary>
        /// Maximum number of incoming messages to retain
        /// </summary>
        public int MaxMessageHistory { get; set; } = 2000;
    }
}
