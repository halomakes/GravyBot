using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GravyBot
{
    /// <summary>
    /// Service used for queueing messages
    /// </summary>
    public class MessageQueueService
    {
        private const int MAX_HISTORY = 300;

        private List<IClientMessage> queuedMessages = new List<IClientMessage>();
        private List<object> messageHistory = new List<object>();
        private List<IClientMessage> outputHistory = new List<IClientMessage>();
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Event fired whenever a message is added
        /// </summary>
        public event EventHandler MessageAdded;

        public MessageQueueService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get a list of messages queued up to be sent
        /// </summary>
        public IEnumerable<IClientMessage> ViewAll() => queuedMessages;

        /// <summary>
        /// Get a list of recent incoming messages
        /// </summary>
        public IEnumerable<object> GetHistory() => messageHistory;

        /// <summary>
        /// Get a list of recently sent messages
        /// </summary>
        public IEnumerable<IClientMessage> GetOutputHistory() => outputHistory;

        private Task ApplyRules<TMessage>(TMessage message)
        {
            var rules = serviceProvider.GetServices<IMessageRule>();
            var matchingRuleType = typeof(IMessageRule<>).MakeGenericType(message.GetType());
            var applicableRules = rules.Where(r => matchingRuleType.IsAssignableFrom(r.GetType()));
            var ruleTasks = applicableRules.Select(ExecuteRuleAsync);

            return Task.WhenAll(ruleTasks);

            async Task ExecuteRuleAsync(IMessageRule rule)
            {
                try
                {
                    await foreach (var responseMessage in rule.Respond(message))
                    {
                        AddOutput(responseMessage);
                    }
                }
                catch (Exception e)
                {
                    Push(e);
                }

            }
        }

        /// <summary>
        /// Get all pending outbound messages and clear queue
        /// </summary>
        public List<IClientMessage> PopAll()
        {
            var messages = queuedMessages;
            queuedMessages = new List<IClientMessage>();
            return messages;
        }

        /// <summary>
        /// Remove an outbound message from the queue
        /// </summary>
        /// <param name="message">Message to remove</param>
        public void Remove(IClientMessage message) => queuedMessages.Remove(message);

        private void AddOutput(IClientMessage message)
        {
            queuedMessages.Add(message);
            outputHistory.Add(message);
            TrimOutputHistory();
            OnMessageAdded(EventArgs.Empty);
        }

        /// <summary>
        /// Record an inbound message
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="message">Message to record</param>
        public void Push<TMessage>(TMessage message)
        {
            messageHistory.Add(message);
            TrimHistory();
            _ = Task.Run(() => ApplyRules(message));
        }

        /// <summary>
        /// Queue a message to be sent
        /// </summary>
        /// <param name="message">Message to send</param>
        public void PushRaw(IClientMessage message) => AddOutput(message);

        private void OnMessageAdded(EventArgs e)
        {
            MessageAdded?.Invoke(this, e);
        }

        private void TrimHistory()
        {
            var overflow = messageHistory.Count - MAX_HISTORY;
            if (overflow > 0)
            {
                messageHistory = messageHistory.Skip(overflow).Take(MAX_HISTORY).ToList();
            }
        }

        private void TrimOutputHistory()
        {
            var overflow = outputHistory.Count - MAX_HISTORY;
            if (overflow > 0)
            {
                outputHistory = outputHistory.Skip(overflow).Take(MAX_HISTORY).ToList();
            }
        }
    }
}
