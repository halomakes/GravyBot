using GravyIrc;
using GravyIrc.Connection;
using GravyIrc.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace GravyBot
{
    /// <summary>
    /// Hosted service for managing IRC connection and rules
    /// </summary>
    public class IrcBotService : IHostedService, IDisposable
    {
        private IrcClient client;
        private readonly MessageQueueService queueService;
        private readonly IrcBotConfiguration config;
        private Timer timer;
        private bool isRegistered = false;
        private readonly BotRulePipeline pipeline;
        readonly List<Type> registeredSubscriptionTypes = new List<Type>();

        public IrcBotService(MessageQueueService queueService, IOptions<IrcBotConfiguration> options, BotRulePipeline pipeline)
        {
            this.queueService = queueService;
            this.pipeline = pipeline;
            config = options.Value;

            queueService.MessageAdded += QueueService_MessageAdded;

            InitializeClient();
        }

        private void InitializeClient()
        {
            var user = new User(config.Nick, config.Identity);

            client = new IrcClient(user, new TcpClientConnection());
            client.OnRawDataReceived += Client_OnRawDataReceived;
            client.EventHub.Subscribe<RplWelcomeMessage>(Client_OnRegistered);
            client.OnDisconnect += Client_OnDisconnect;

            SubscribeToMessageEvents();
        }

        private async void Client_OnDisconnect(object sender, EventArgs e)
        {
            Console.WriteLine($"{DateTime.Now} Connection lost.");
            await ReconnectAsync();
        }

        private void SubscribeToMessageEvents()
        {
            registeredSubscriptionTypes.Clear();
            var method = typeof(IrcBotService).GetMethod(nameof(IrcBotService.SubscribeToEvent), BindingFlags.NonPublic | BindingFlags.Instance);
            var eligibleTypes = pipeline.SubscribedTypes.Where(t => typeof(IServerMessage).IsAssignableFrom(t)).Distinct();

            foreach (var type in eligibleTypes)
            {
                if (!registeredSubscriptionTypes.Contains(type))
                {
                    registeredSubscriptionTypes.Add(type);
                    var genericMethod = method.MakeGenericMethod(type);
                    genericMethod.Invoke(this, new object[] { });
                }
            }
        }

        private void SubscribeToEvent<TMessage>() where TMessage : IrcMessage, IServerMessage
        {
            client.EventHub.Subscribe<TMessage>((client, args) => queueService.Push(args.IrcMessage));
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await ConnectAsync();
            timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        private async void QueueService_MessageAdded(object sender, EventArgs e)
        {
            await SendQueuedMessagesAsync();
        }

        private async void Client_OnRegistered(object sender, IrcMessageEventArgs<RplWelcomeMessage> e)
        {
            await client.SendAsync(new PrivateMessage("NickServ", $"identify {config.NickServ}"));
            await client.SendAsync(new UserModeMessage(config.Nick, "+B"));
            await JoinDefaultChannels();
            isRegistered = true;
        }

        private async void ProcessQueue(object state)
        {
            if (isRegistered)
            {
                await SendQueuedMessagesAsync();
            }
        }

        private async Task SendQueuedMessagesAsync()
        {
            await JoinDefaultChannels();

            var queue = queueService.PopAll();

            if (queue.Count > 0)
            {
                Console.WriteLine($"{DateTime.Now} Sending {queue.Count()} messages.");
            }

            foreach (var message in queue)
            {
                if (message is IChannelMessage m)
                {
                    if (m.IsChannelMessage)
                    {
                        await JoinChannel(m.Target);
                    }
                }

                await client.SendAsync(message);
            }
        }

        private async Task JoinDefaultChannels()
        {
            foreach (var c in config.Channels)
            {
                await JoinChannel(c);
            }
        }

        /// <summary>
        /// Joins a channel
        /// </summary>
        /// <remarks>Will only join channel if not already joined</remarks>
        /// <param name="channelName">Name of channel including #</param>
        public async Task JoinChannel(string channelName)
        {
            if (!client.Channels.Any(c => c.Name == channelName))
            {
                await client.SendAsync(new JoinMessage(channelName));
            }
        }

        /// <summary>
        /// Connect to the IRC server
        /// </summary>
        public async Task ConnectAsync()
        {
            await client.ConnectAsync(config.Server, config.Port);
        }

        private async Task ReconnectAsync()
        {
            if (pipeline.IsAutoReconnectEnabled)
            {
                Dispose();
                await Task.Delay(1000);
                Console.WriteLine($"{DateTime.Now} Attempting to reconnect to server...");
                InitializeClient();
                try
                {
                    await StartAsync();
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"{DateTime.Now} Couldn't reconnect: {e}");
                    await ReconnectAsync();
                }
            }
        }

        private void Client_OnRawDataReceived(IrcClient client, string rawData)
        {
            Console.WriteLine($"{DateTime.Now} {rawData}");
        }

        public void Dispose()
        {
            client?.Dispose();
            timer?.Dispose();
        }

        /// <summary>
        /// Stop the hosted service and disconnect from the IRC server
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
