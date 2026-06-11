# SeriLog

Serilog-specific extensions for `SpocWeb.Logging`:
a destructuring policy that truncates oversized strings and arrays,
and an attribute that suppresses logging of sensitive or irrelevant properties.

See the parent [SpocWeb.Logging ReadMe](../ReadMe.md) for the full architecture
and Quick Start guide.

## Classes

| Class | Responsibility |
|---|---|
| [LoggingLimitPolicy](LoggingLimitPolicy.cs) | Serilog `IDestructuringPolicy` that truncates strings beyond `MaxLengthOfString`, limits arrays to `MaxLengthOfArray` elements, and omits properties listed in `IgnoredProperties` or annotated with `[ExcludeFromLogging]`. |
| [ExcludeFromLoggingAttribute](ExcludeFromLoggingAttribute.cs) | Attribute that marks a property or type to be silently skipped by `LoggingLimitPolicy` during Serilog destructuring. |

## Entry Points

| Method | Description |
|---|---|
| `LoggingLimitPolicy.TryDestructure(object, …)` | Serilog policy entry point; returns `true` and sets `result` when it handles the value (strings, arrays, and filtered objects). |

## Registration

```csharp
Log.Logger = new LoggerConfiguration()
    .Destructure.With<LoggingLimitPolicy>()
    .CreateLogger();
```
