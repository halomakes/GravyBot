using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GravyBot.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        private static readonly Regex ParameterRgx = new Regex(@"\{([^\{\}]+)\}+", RegexOptions.Compiled); // params indicated with {paramName}

        public CommandAttribute(string commandFormat)
        {
            CommandFormat = commandFormat;
            ParseCommandFormat(commandFormat);
        }

        public string CommandFormat { get; private set; }

        public string Command { get; private set; }

        public IEnumerable<string> ParameterNames { get; private set; }

        public Regex MatchingPattern { get; private set; }

        private void ParseCommandFormat(string commandFormat)
        {
            var matches = ParameterRgx.Matches(commandFormat);
            ParameterNames = matches.Select(m => m.Groups[1].Value.Trim());

            var duplicateKey = ParameterNames.GroupBy(n => n).FirstOrDefault(g => g.Count() > 1);
            if (duplicateKey != default)
                throw new ArgumentException($"Multiple parameters defined with name {duplicateKey.Key}");

            var paramReplacement = @"(.+)";
            var split = ParameterRgx.Split(commandFormat);
            var commandRgxSegments = split.Select(section => ParameterNames.Contains(section) ? paramReplacement : Regex.Escape(section));
            MatchingPattern = new Regex(string.Join(string.Empty, commandRgxSegments));

            Command = commandFormat.Split(' ').First().Trim().ToLower();
        }
    }
}
