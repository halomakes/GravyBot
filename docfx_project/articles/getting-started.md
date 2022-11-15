# Getting Started

GravyBot is built to play nice with .NET Core's dependency injection container and make adding additional rules super-easy.  
At the center of GravyBot are its *[rule pipeline](/api/GravyBot.BotRulePipeline.html)* and *[message queue](/api/GravyBot.MessageQueueService.html)*.  The message queue tracks incoming and outgoing messages and events.  The rule pipeline allows you to add actions that should happen when a certain type of message comes in using simple, strongly-typed interfaces.  It's easy to raise any kind of event and it's easy to listen for any kind of event.

## Commands

The optional commands makes building your bot even easier with self-documenting attribute-based command binding and typed parameter binding.  See more about using it at (/articles/commands.html)

## Installation and Configuration

To get started, install GravyBot to your project and get some configuration set up. The optional *[GravyBot.SampleRules](https://www.nuget.org/packages/GravyBot.SampleRules/)* package provides exception logging and a basic "Hello, world" rule to help you get started.

### NuGet
```bash
dotnet add package GravyBot
dotnet add package GravyBot.SampleRules
```

### In *appsettings.json*
```json
"Irc": {
    "Port": 6667,
    "Nick": "MyIrcBot",
    "Identity": "My first IRC bot using GravyBot",
    "Server": "irc.myserver.net",
    "Channels": [ "#mainchannel", "#testchannel" ],
    "NotifyChannel": "#testchannel",
    "CommandPrefix": "!",
    "LogChannel": "#testchannel"
}
```

### In *Startup.ConfigureServices*
```csharp
services.AddIrcBot(Configuration.GetSection("Irc"), pipeline => {
    pipeline.AddSampleRules();
});
```

The `Action<BotRulePipeline>` parameter to [`AddIrcBot`](/api/GravyBot.Extensions.html#GravyBot_Extensions_AddIrcBot_IServiceCollection_Action_GravyBot_IrcBotConfiguration__Action_GravyBot_BotRulePipeline__) is where you'll be adding your own custom rules.
With all of that set up, run your app and if you installed and registered the *[GravyBot.SampleRules](https://www.nuget.org/packages/GravyBot.SampleRules/)* package, you should be able to say hello to your bot to make sure everything connected correctly.

```
!hello
```

> Hello, &lt;yourNick&gt;!