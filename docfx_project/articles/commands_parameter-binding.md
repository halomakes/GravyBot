# Parameter Binding

Parameters can be automatically parsed out of command invokations and passed into your methods.  The name in the method declaration must match the value in the command attribute.

## Basic Usage

```csharp
[Command("compliment {nick}", Description = "Pay someone a nice (or awkward) compliment.")]
public async Task<IClientMessage> Compliment(string nick)
{
    var compliment = await service.GetComplimentAsync();
    return new PrivateMessage(IncomingMessage.To, $"{nick}: {compliment}");
}
```

## Validation

Parameters support standard ComponentModel validation. Validation errors are sent back to the user as a NoticeMessage.

```csharp
[Command("compliment {nick}", Description = "Pay someone a nice (or awkward) compliment.")]
public async Task<IClientMessage> Compliment([Required, MaxLength(15)] string nick)
{
    var compliment = await service.GetComplimentAsync();
    return new PrivateMessage(IncomingMessage.To, $"{nick}: {compliment}");
}
```

You can also use custom validation attributes here.

```csharp
public class NickAttribute : RegularExpressionAttribute
{
    public const string Nick = @"[A-z_\-\[\]\\^{}|`][A-z0-9_\-\[\]\\^{}|`]+";

    public NickAttribute(bool allowCaret = false) : base(allowCaret ? @$"{Nick}|\^" : Nick)
    { }

    public override string FormatErrorMessage(string name) => $"The {name} field must be a valid nick.";
}
```

```csharp
[Command("compliment {nick}", Description = "Pay someone a nice (or awkward) compliment.")]
public async Task<IClientMessage> Compliment([Required, Nick] string nick)
{
    var compliment = await service.GetComplimentAsync();
    return new PrivateMessage(IncomingMessage.To, $"{nick}: {compliment}");
}
```

## Type Conversion

By default, your parameters can have any primitive type and will be automatically converted for use.

```csharp
[Command("favenum {number}", Description = "Tell everyone what your favorite number is.")]
public IClientMessage AnnounceFavorite([Required] long number) => 
    new PrivateMessage(IncomingMessage.To, $"{IncomingMessage.From}'s favorite number is {number}!");
```

GravyBot will also respect any custom TypeConverters you specify.

```csharp
[TypeConverter(typeof(TimeSpan?))]
public class LenientTimeSpanConverter : TypeConverter
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
        if (value is string s)
        {
            return TimeSpan.TryParse(s, out var parsed) ? parsed : null;
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
```

```csharp
[Command("timer {timeSpan}", Description = "Set a timer for an amount of time.")]
public async IAsyncEnumerable<IClientMessage> GetMessageRate([Required, TypeConverter(typeof(LenientTimeSpanConverter))] TimeSpan timeSpan)
{
    yield return new PrivateMessage(IncomingMessage.From, $"Starting a timer for {timeSpan}...");
    await Task.Delay(timeSpan);
    yield return new PrivateMessage(IncomingMessage.From, "Time's up!");
}
```