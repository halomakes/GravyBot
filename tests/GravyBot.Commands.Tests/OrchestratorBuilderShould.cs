using GravyIrc.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GravyBot.Commands.Tests
{
    public class OrchestratorBuilderShould
    {
        [Fact]
        public void Thow_On_Duplicate_Command()
        {
            var builder = new CommandOrchestratorBuilder();
            Assert.Throws<DuplicateCommandException>(() => builder.RegisterProcessor<DuplicateCommandProcessor>());
        }

        public class DuplicateCommandProcessor : CommandProcessor
        {
            [Command("potato")]
            public IClientMessage GetPotato() => default;

            [Command("potato {spud}")]
            public IClientMessage DoMorePotato() => default;
        }

        [Fact]
        public void Throw_On_Bad_Return()
        {
            var builder = new CommandOrchestratorBuilder();
            Assert.Throws<ArgumentException>(() => builder.RegisterProcessor<BadReturnProcessor>());
        }

        public class BadReturnProcessor : CommandProcessor
        {
            [Command("potato")]
            public string GetPotato() => default;
        }

        [Fact]
        public void Allow_IClientMessage_Return_Types()
        {
            var builder = new CommandOrchestratorBuilder();
            builder.RegisterProcessor<ReturnTestProcessor>();
        }

        public class ReturnTestProcessor : CommandProcessor
        {
            [Command("single")]
            public IClientMessage GetSingle() => default;

            [Command("multi")]
            public IEnumerable<IClientMessage> GetMultiple() => default;

            [Command("async-single")]
            public Task<IClientMessage> GetSingleAsync() => default;

            [Command("async-mulit")]
            public IAsyncEnumerable<IClientMessage> GetMultiAsync() => default;
        }
    }
}
