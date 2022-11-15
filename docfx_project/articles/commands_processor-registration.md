# Processor Wiring

You can register all commands in an assembly automatically using the [RegisterProcessors](/api/GravyBot.Commands.CommandOrchestratorBuilder.html#GravyBot_Commands_CommandOrchestratorBuilder_RegisterProcessors_Assembly_) method.

```csharp
services.AddCommandOrchestrator(builder =>
{
    builder.RegisterProcessors(Assembly.GetExecutingAssembly());
});
```

Or you can register processors manually using the [RegisterProcessor<T>](/api/GravyBot.Commands.CommandOrchestratorBuilder.html#GravyBot_Commands_CommandOrchestratorBuilder_RegisterProcessor__1) method.

```csharp
services.AddCommandOrchestrator(builder =>
{
    builder.RegisterProcessor<HelpCommandProcessor>();
});
```

See [CommandOrchestratorBuilder](/api/GravyBot.Commands.CommandOrchestratorBuilder.html) for full reference.

## Using a Processor From a Rule
The [CommandAliasRule<TProcessor>](/api/GravyBot.Commands.CommandAliasRule-1.html) provides a pre-wired mechanism for invoking commands from custom pipeline rules if you want to trigger them from something outside of the standard command syntax using regular expressions.

```csharp
public class DadJokeRule : CommandAliasRule<JokeCommandProcessor>
{
    public DadJokeRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider) : base(options, serviceProvider)
    {
        Pattern = new Regex($"^{Regex.Escape(options.Value.Nick)},? ?tell.*joke", RegexOptions.IgnoreCase);
    }

    protected override async IAsyncEnumerable<IClientMessage> OnMatch(Match _, JokeCommandProcessor commandProcessor)
    {
        yield return await commandProcessor.GetJoke();
    }
}
```