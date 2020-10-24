using System.Reflection;

namespace GravyBot.Commands.Tests
{
    public class MockOrchestratorBuilder : CommandOrchestratorBuilder
    {
        public MockOrchestratorBuilder() : base(default) { }

        public override void RegisterProcessor<TProcessor>()
        {
            foreach (var method in typeof(TProcessor).GetMethods())
                foreach (var command in method.GetCustomAttributes<CommandAttribute>())
                    RegisterCommand(command, method);
        }
    }
}
