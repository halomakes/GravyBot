using GravyIrc.Messages;
using System.Collections.Generic;

namespace GravyBot.Commands
{
    public class HelpCommandProcessor : CommandProcessor
    {
        private readonly ICommandOrchestratorBuilder builder;
        private readonly IrcBotConfiguration botConfiguration;

        public HelpCommandProcessor(ICommandOrchestratorBuilder builder, IrcBotConfiguration botConfiguration)
        {
            this.builder = builder;
            this.botConfiguration = botConfiguration;
        }

        [Command("help {commandName}", Description = "Get information about a command")]
        public IClientMessage GetCommandInfo(string commandName)
        {
            if (!string.IsNullOrEmpty(commandName))
                commandName = "help";

            if (builder.Bindings.ContainsKey(commandName))
            {
                var binding = builder.Bindings[commandName];
                var segments = new List<string>();

                if (!string.IsNullOrEmpty(binding.Command?.Description))
                    segments.Add(binding.Command.Description);

                if (!string.IsNullOrEmpty(binding.Command?.CommandFormat))
                    segments.Add($"Usage: {botConfiguration.CommandPrefix}{binding.Command.CommandFormat}");

                if (binding.IsChannelOnly)
                    segments.Add("Can only be used in channels");

                if (binding.IsDirectOnly)
                    segments.Add("Can only be used in direct messages");

                if (binding.IsRateLimited)
                    segments.Add($"Can only be used every {binding.RateLimitPeriod.Value.ToFriendlyString()}");

                return new NoticeMessage(IncomingMessage.From, string.Join(" | ", segments));
            }
            else
            {
                return new NoticeMessage(IncomingMessage.From, $"No command {IrcValues.BOLD}{commandName}{IrcValues.RESET} registered.");
            }
        }
    }
}
