using System;
using System.Linq;
using Xunit;

namespace GravyBot.Commands.Tests
{
    public class CommandAttributeShould
    {
        [Fact]
        public void Parse_Command()
        {
            var command = "booru";
            Assert.Equal(command.ToLower(), new CommandAttribute(command).CommandName);
            Assert.Equal(command.ToLower(), new CommandAttribute($"{command} {{id}}").CommandName);
            Assert.Equal(command.ToLower(), new CommandAttribute($"{command} {{id}} {{value}}").CommandName);
            Assert.Equal(command.ToLower(), new CommandAttribute($"{command} {{id}} = {{value}}").CommandName);
        }

        [Fact]
        public void Parse_No_Named_Args()
        {
            var command = new CommandAttribute("booru");
            Assert.Empty(command.ParameterNames);
        }

        [Fact]
        public void Parse_Single_Named_Arg()
        {
            var command = new CommandAttribute("booru {id}");
            Assert.NotEmpty(command.ParameterNames);
            Assert.Contains("id", command.ParameterNames);
        }

        [Fact]
        public void Parse_Multiple_Named_Args()
        {
            var command = new CommandAttribute("booru {id} {potato}");
            Assert.NotEmpty(command.ParameterNames);
            Assert.Contains("id", command.ParameterNames);
            Assert.Contains("potato", command.ParameterNames);
            Assert.Equal(2, command.ParameterNames.Count());
        }

        [Fact]
        public void Throw_Duplicate_Named_Args()
        {
            Assert.Throws<ArgumentException>(() => new CommandAttribute("booru {id} {id}"));
        }

        [Fact]
        public void Build_Regex_No_Named_Args()
        {
            var command = new CommandAttribute("booru");
            Assert.Equal("booru", command.MatchingPattern.ToString());
        }

        [Fact]
        public void Escape_Non_Arg_Segments()
        {
            var command = new CommandAttribute("(booru) {id}");
            Assert.Equal(@"\(booru\)\ (.+)", command.MatchingPattern.ToString());
        }

        [Fact]
        public void Build_Regex_Single_Arg()
        {
            var command = new CommandAttribute("booru {id}");
            Assert.Equal(@"booru\ (.+)", command.MatchingPattern.ToString());
        }

        [Fact]
        public void Compiled_Regex_Match_Command_No_Arg()
        {
            var command = new CommandAttribute("booru");
            Assert.Matches(command.MatchingPattern, "booru");
        }

        [Fact]
        public void Compiled_Regex_Match_Command_Single_Arg()
        {
            var command = new CommandAttribute("booru {id}");
            Assert.Matches(command.MatchingPattern, "booru 12345");
        }

        [Fact]
        public void Compiled_Regex_Match_Command_Multiple_Args()
        {
            var command = new CommandAttribute("booru {id} {name}");
            Assert.Matches(command.MatchingPattern, "booru 12345 potato");
        }

        [Fact]
        public void Compiled_Regex_Parse_Args()
        {
            var command = new CommandAttribute("booru {id} {name}");
            var match = command.MatchingPattern.Match("booru 12345 potato");
            Assert.Equal("12345", match.Groups[1].Value);
            Assert.Equal("potato", match.Groups[2].Value);
        }

        [Fact]
        public void Compiled_Regex_Parse_Formatted_Args()
        {
            var command = new CommandAttribute("booru {id} => {name}");
            var match = command.MatchingPattern.Match("booru 12345 => potato");
            Assert.Equal("12345", match.Groups[1].Value);
            Assert.Equal("potato", match.Groups[2].Value);
        }
    }
}
