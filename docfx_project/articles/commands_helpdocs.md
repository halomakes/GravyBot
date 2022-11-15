# Command Help Doc Generation

Information about registered commands can be retrived from the [Bindings](/api/GravyBot.Commands.CommandOrchestratorBuilder.html#GravyBot_Commands_CommandOrchestratorBuilder_Bindings) property on [CommandOrchestratorBuilder](/api/GravyBot.Commands.CommandOrchestratorBuilder.html).  This can be accessed via dependency injection to build documentation.  See [this working example in ChatBeet](https://github.com/halomademeapc/ChatBeet/blob/develop/ChatBeet/Pages/Commands/Index.cshtml).

## Help Command

A command that provides information about commands is provided in the package.  Just include the [HelpCommandProcessor](/api/GravyBot.Commands.HelpCommandProcessor.html) to make it available for use.

```csharp
services.AddCommandOrchestrator(builder =>
{
    builder.RegisterProcessor<HelpCommandProcessor>();
});
```

You should now be able to interact with your command processor:
```
!help potato
```

> **compliment**: Pay someone a nice (or awkward) compliment. | Usage: .cb compliment {nick} | Can only be used in channels | Can only be used every 10s