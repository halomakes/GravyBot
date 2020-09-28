# Adding Custom Rules

Two interfaces are provided for adding your own rules: [`IMessageRule`](/api/GravyBot.IMessageRule-1.html) and [`IAsyncMessageRule`](/api/GravyBot.IAsyncMessageRule-1.html).  These generic interfaces take one type parameter that is used to specify what type of incoming messages they will respond to.  If a rule can handle multiple types of incoming messages, simply implement the generic for those other message types.  This is especially useful if you have rules that can be triggered both from an incoming message or from a WebUI or other source.

[`IAsyncMessageRule`](/api/GravyBot.IAsyncMessageRule-1.html) has a boolean method [`Matches(TMessage)`](/api/GravyBot.IAsyncMessageRule-1.html#GravyBot_IAsyncMessageRule_1_Matches__0_) that is used for synchronous condition matching to determine whether or not an async rule should be processed.  This prevents the application from wasting threads on unnecessary tasks for rules that are not needed on a given message. 

Rules don't run in any specific order and have access to a scoped service container, so you have freedom to use DI to your heart's content.  You can also emit multiple messages using `yield return` as [`IMessageRule`](/api/GravyBot.IMessageRule-1.html) and [`IAsyncMessageRule`](/api/GravyBot.IAsyncMessageRule-1.html) implement `IEnumerable` and `IAsyncEnumerable` respectively.

## Creating the Rule

Let's look at an example rule.  This excerpt is an abridged example from [ChatBeet](https://github.com/halomademeapc/ChatBeet).

```csharp
public class ArtistRule : IAsyncMessageRule<PrivateMessage>
{
    private readonly IrcBotConfiguration config;
    private readonly LastFmService lastFm;
    private readonly Regex rgx;

    public ArtistRule(LastFmService lastFm, IOptions<IrcBotConfiguration> options)
    {
        this.lastFm = lastFm;
        config = options.Value;
        rgx = new Regex($"^{Regex.Escape(config.CommandPrefix)}artist (.*)", RegexOptions.IgnoreCase);
    }

    public bool Matches(PrivateMessage incomingMessage) => pattern.IsMatch(incomingMessage.Message);

    public async IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
    {
        var match = rgx.Match(incomingMessage.Message);
        var artistName = match.Groups[1].Value.Trim();
        var artist = await lastFm.GetArtistInfo(artistName);

        if (artist != null)
        {
            yield return new PrivateMessage(incomingMessage.From, $"{artist.Bio?.Summary}");

            if (artist.Tags.Any())
                yield return new PrivateMessage(incomingMessage.From, $"{IrcValues.BOLD}Related tags{IrcValues.RESET}: { string.Join(", ", artist.Tags.Select(t => t.Name))}");
        }
        else
            yield return new PrivateMessage(incomingMessage.From, "Sorry, couldn't find that artist.");
    }
}
```

We can see here that this rule will only respond to a [`PrivateMessage`](https://gravyirc.halomademeapc.com/api/GravyIrc.Messages.PrivateMessage.html) coming in.  The type parameter can be any custom type that you've raised or one of the built-in [`IServerMessage`](https://gravyirc.halomademeapc.com/api/GravyIrc.Messages.IServerMessage.html) implementations from [GravyIrc](https://gravyirc.halomademeapc.com). See a list of message types from GravyIrc [here](https://gravyirc.halomademeapc.com/api/GravyIrc.Messages.html).
A regular expression is used to determine if the rule should take action before the main response method is triggered.  We can also see that we're able to use dependency injection like normal and that we can also emit multiple messages from out response method if we so choose.

## Registering the Rule

Registering a rule to our pipeline is pretty simple.  Just add a line in your `ConfigureServices` method for the app.

```csharp
services.AddIrcBot(Configuration.GetSection("Irc"), pipeline => {
    pipeline.AddSampleRules();
    pipeline.RegisterAsyncRule<ArtistRule, PrivateMessage>();
});
```

You would then be able to trigger the rule as such:
```
!artist Styx
```

> Styx /stɪks/ is an American rock band from Chicago that formed in 1972 and became famous for its albums released in the late 1970s and early 1980s. They are best known for melding hard rock guitar balanced with acoustic guitar, synthesizers mixed with acoustic piano, upbeat tracks with power ballads, and incorporating elements of international musical theatre.\[5\] The band established itself with a progressive rock sound in the 1970s

> **Related tags**: classic rock, rock, Progressive rock, 80s, hard rock