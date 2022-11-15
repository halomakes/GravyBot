# Commands

GravyBot has an optional [GravyBot.Commands package](https://www.nuget.org/packages/GravyBot.Commands) that makes building your bot a cinch. If you've worked with controllers in ASP.NET, this should provide a familiar environment for you to work in.

## Basic Usage

Install the package.

```bash
dotnet add package GravyBot.Commands
```

### Creating the Command Processor

To create a command processor, just inherit the [CommandProcessor](/api/GravyBot.Commands.CommandProcessor.html) base class.  You can then add methods with a [CommandAttribute](/api/GravyBot.Commands.CommandAttribute.html) to handle invocations.  Your methods should return `IClientMessage` or `Task<IClientMessage>` if you have a single response, or you can use `IEnumerable<IClientMessage>` or `IAsyncEnumerable<IClientMessage>` if you want to send multiple responses.

```csharp
public class HelloCommandProcessor : CommandProcessor
{
    [Command("hello", Description = "Say hello to your GravyBot.")]
    public IClientMessage SayHello() => new PrivateMessage(IncomingMessage.From, $"Hello, {IncomingMessage.From}!");
}
```

You can have multiple methods in a single command processor.
```csharp
public class HelloCommandProcessor : CommandProcessor
{
    [Command("hello", Description = "Say hello to your GravyBot.")]
    public IClientMessage SayHello() => new PrivateMessage(IncomingMessage.From, $"Hello, {IncomingMessage.From}!");

    [Command("goodbye", Description = "Say goodbye to your GravyBot.")]
    public IClientMessage SayGoodBye() => new PrivateMessage(IncomingMessage.From, $"Goodbye, {IncomingMessage.From}!");
}
```

### Registering the Command Processor

Add the command orchestrator rule to your message pipeline:
```csharp
services.AddIrcBot(Configuration.GetSection("Irc"), pipeline =>
{
    pipeline.AddSampleRules();
    /* other custom rules here... */

    pipeline.AddCommandOrchestrator();
});
```

Register your commands to the orchestrator:
```csharp
services.AddCommandOrchestrator(builder =>
{
    builder.RegisterProcessors(Assembly.GetExecutingAssembly());
});
```

You should now be able to interact with your command processor:
```
!hello
```

> Hello, &lt;yourNick&gt;!