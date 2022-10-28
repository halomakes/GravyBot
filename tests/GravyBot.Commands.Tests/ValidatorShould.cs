using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GravyBot.Commands.Tests
{
    public class ValidatorShould
    {
        private readonly CommandOrchestratorRule orchestrator;

        public ValidatorShould()
        {
            var opts = Options.Create(new IrcBotConfiguration
            {
                CommandPrefix = ".cb "
            });
            var builder = new MockOrchestratorBuilder();
            builder.RegisterProcessor<TestProcessor>();
            builder.AddChannelPolicy("noMain", new string[] { "#main" }, ChannelPolicy.PolicyMode.Blacklist);
            builder.AddChannelPolicy("onlyMain", new string[] { "#main" }, ChannelPolicy.PolicyMode.Whitelist);
            orchestrator = new CommandOrchestratorRule(opts, builder, new MockProcessorProvider<TestProcessor>());
        }

        [Fact]
        public async Task Validate_Type_Conversion()
        {
            var msg = new PrivateMessage("#test", ".cb count 69");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("69 is one of my favorite numbers.", resp.Message);

            msg = new PrivateMessage("#test", ".cb count potato");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            var validationResp = responses.FirstOrDefault() as NoticeMessage;
            Assert.Equal("Cannot convert value potato to Int32.", validationResp.Message);
            Assert.Single(responses);
        }

        [Fact]
        public async Task Use_Custom_Converter()
        {
            var msg = new PrivateMessage("#test", ".cb parse potato");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            var resp = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("Parsed out 00:01:09", resp.Message);
        }

        [Fact]
        public async Task Validate_Required_Param()
        {
            var msg = new PrivateMessage("#test", ".cb pester");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            var valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.Equal("The personName field is required.", valMsg.Message);

            msg = new PrivateMessage("#test", ".cb pester carrots");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<PrivateMessage>(responses.FirstOrDefault());
            var response = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("I am pestering you.", response.Message);
        }

        [Fact]
        public async Task Validate_Maxlength_Param()
        {
            var msg = new PrivateMessage("#test", ".cb throw carrotsasdfasdfasdfasdfasdfasdfasdfasdfasdfafsd");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            var valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.StartsWith("The field personName must", valMsg.Message);

            msg = new PrivateMessage("#test", ".cb throw carrots");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<PrivateMessage>(responses.FirstOrDefault());
            var response = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("I am throwing you.", response.Message);
        }

        [Fact]
        public async Task Validate_Range_Param()
        {
            // too low
            var msg = new PrivateMessage("#test", ".cb donate -1");
            var responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            var valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.StartsWith("The field amount must", valMsg.Message);

            // too high
            msg = new PrivateMessage("#test", ".cb donate 1000000");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.StartsWith("The field amount must", valMsg.Message);

            // bad conversion
            msg = new PrivateMessage("#test", ".cb donate potato");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.StartsWith("Cannot convert", valMsg.Message);

            // required
            msg = new PrivateMessage("#test", ".cb donate");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<NoticeMessage>(responses.FirstOrDefault());
            valMsg = responses.FirstOrDefault() as NoticeMessage;
            Assert.Equal("The amount field is required.", valMsg.Message);

            msg = new PrivateMessage("#test", ".cb donate 69");
            responses = await orchestrator.RespondAsync(msg).ToListAsync();
            Assert.NotEmpty(responses);
            Assert.Single(responses);
            Assert.IsAssignableFrom<PrivateMessage>(responses.FirstOrDefault());
            var response = responses.FirstOrDefault() as PrivateMessage;
            Assert.Equal("Someone donated $69.00", response.Message);
        }

        public class TestProcessor : CommandProcessor
        {
            [Command("count {number}")]
            public PrivateMessage EnjoyNumber(int number) => new PrivateMessage("nobody", $"{number} is one of my favorite numbers.");

            [Command("pester {personName}")]
            public PrivateMessage Pester([Required] string personName) => new PrivateMessage(personName, "I am pestering you.");

            [Command("throw {personName}")]
            public PrivateMessage Throw([Required, MaxLength(15)] string personName) => new PrivateMessage(personName, "I am throwing you.");

            [Command("donate {amount}")]
            public PrivateMessage Donate([Required, Range(0, 500)] decimal amount) => new PrivateMessage("nobody", $"Someone donated {amount:C}");

            [Command("parse {irrelevant}")]
            public PrivateMessage Parse([Required, TypeConverter(typeof(CustomConverter))] TimeSpan? irrelevant) => new PrivateMessage("nobody", $"Parsed out {irrelevant}");
        }

        [TypeConverter(typeof(TimeSpan?))]
        public class CustomConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                {
                    return true;
                }
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string)
                {
                    return TimeSpan.FromSeconds(69);
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return value.ToString();
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
