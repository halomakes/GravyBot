using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
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
        public async Task Accept_String_Param()
        {
            var msg = new PrivateMessage("#test", ".cb hello potato");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("hello, potato", resp.Message);
        }

        [Fact]
        public async Task Accept_Multiple_Strings()
        {
            var msg = new PrivateMessage("#test", ".cb potato yeet yote");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("got yeet and yote", resp.Message);
        }

        [Fact]
        public async Task Accept_Multiple_Parameterless()
        {
            var msg = new PrivateMessage("#test", ".cb ping");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("pong", resp.Message);
        }

        [Fact]
        public async Task Convert_Standard_Types()
        {
            var msg = new PrivateMessage("#test", ".cb add 420 69");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("489", resp.Message);
        }

        [Fact]
        public async Task Convert_Dates()
        {
            var msg = new PrivateMessage("#test", ".cb schedule potato 12-23-2020");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("potato at 12/23/2020", resp.Message);
        }

        [Fact]
        public async Task Block_DirectOnly_In_Channel()
        {
            var msg = new PrivateMessage("#public", ".cb secret i suck eggs");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
        }

        [Fact]
        public async Task Allow_DirectOnly_In_Direct()
        {
            var msg = new PrivateMessage("private", ".cb secret i suck eggs");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.IsAssignableFrom<PrivateMessage>(responses.FirstOrDefault());
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("secret sealed", resp.Message);
        }

        [Fact]
        public async Task Block_ChannelOnly_In_Direct()
        {
            var msg = new PrivateMessage("private", ".cb publicize i suck eggs");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
        }

        [Fact]
        public async Task Allow_ChannelOnly_In_Channel()
        {
            var msg = new PrivateMessage("#public", ".cb publicize i suck eggs");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.IsAssignableFrom<PrivateMessage>(responses.FirstOrDefault());
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("i suck eggs", resp.Message);
        }

        [Fact]
        public async Task Should_RateLimit()
        {
            var msg = new PrivateMessage("#public", ".cb spam");

            // first message
            var initialResponses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(initialResponses);
            Assert.IsAssignableFrom<PrivateMessage>(initialResponses.FirstOrDefault());

            // follow-up before rate limit
            var spammyResponses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(spammyResponses);
            Assert.IsAssignableFrom<NoticeMessage>(spammyResponses.FirstOrDefault());
        }

        [Fact]
        public async Task Should_Allow_After_RateLimit()
        {
            var msg = new PrivateMessage("#public", ".cb spam");

            // first message
            var initialResponses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(initialResponses);
            Assert.IsAssignableFrom<PrivateMessage>(initialResponses.FirstOrDefault());

            await Task.Delay(TimeSpan.FromSeconds(4));

            // follow-up before rate limit
            var spammyResponses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(spammyResponses);
            Assert.IsAssignableFrom<PrivateMessage>(spammyResponses.FirstOrDefault());
        }


        public class TestProcessor : CommandProcessor
        {
            [Command("hello {user}")]
            public PrivateMessage SayHello(string user) => new PrivateMessage("nobody", $"hello, {user}");

            [Command("potato {first} {second}")]
            public PrivateMessage CheckTwoStrings(string second, string first) => new PrivateMessage("nobody", $"got {first} and {second}");

            [Command("ping")]
            public PrivateMessage PingPong() => new PrivateMessage("nobody", "pong");

            [Command("add {a} {b}")]
            public PrivateMessage Add(int a, int b) => new PrivateMessage("nobody", (a + b).ToString());

            [Command("schedule {meetingName} {time}")]
            public PrivateMessage Schedule(string meetingName, DateTime time) => new PrivateMessage("nobody", $"{meetingName} at {time:d}");

            [Command("secret {dirty}"), DirectOnly]
            public PrivateMessage TakeSecret(string dirty) => new PrivateMessage("nobody", "secret sealed");

            [Command("publicize {commonKnowledge}"), ChannelOnly]
            public PrivateMessage AnnouncePublic(string commonKnowledge) => new PrivateMessage("nobody", commonKnowledge);

            [Command("spam"), RateLimit(3, TimeUnit.Second)]
            public PrivateMessage Spam() => new PrivateMessage("nobody", "spam");
        }
    }
}
