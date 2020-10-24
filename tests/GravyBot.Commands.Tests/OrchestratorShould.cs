using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GravyBot.Commands.Tests
{
    public class OrchestratorShould
    {
        private readonly CommandOrchestratorRule orchestrator;

        public OrchestratorShould()
        {
            var opts = Options.Create(new IrcBotConfiguration
            {
                CommandPrefix = ".cb "
            });
            var builder = new MockOrchestratorBuilder();
            builder.RegisterProcessor<TestProcessor>();
            orchestrator = new CommandOrchestratorRule(opts, builder, new MockProcessorProvider<TestProcessor>());
        }

        [Fact]
        public async Task Invoke_String_Params()
        {
            var msg = new PrivateMessage("#test", ".cb hello potato");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("hello, potato", resp.Message);
        }

        public class TestProcessor : CommandProcessor
        {
            [Command("hello {user}")]
            public PrivateMessage SayHello(string user) => new PrivateMessage("nobody", $"hello, {user}");
        }
    }
}
