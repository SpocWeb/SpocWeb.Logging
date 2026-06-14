---
digest:
  local-classes:
    ExcludeFromLoggingAttribute:
      mtime: "2026-06-11T12:19:05Z"
      digest: "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
    LoggingLimitPolicy:
      mtime: "2026-06-11T12:19:05Z"
      digest: "5afdd2e10f6c9a25f6685ea0af707157a09e273b39f5899275e27c060c8ef73f"
  folders: {}
---
# SeriLog

Serilog-specific extensions for `SpocWeb.Logging`:
a destructuring policy that truncates oversized strings and arrays,
and an attribute that suppresses logging of sensitive or irrelevant properties.

See the parent [SpocWeb.Logging ReadMe](../ReadMe.md) for the full architecture
and Quick Start guide.

## Classes

| Class | Responsibility |
|---|---|
| [ExcludeFromLoggingAttribute](ExcludeFromLoggingAttribute.cs) | Flag to suppress logging this Property or Class |
| [LoggingLimitPolicy](LoggingLimitPolicy.cs) | Serilog IDestructuringPolicy forms a Chain of Responsibility for serializing Values |

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

## Architecture

```mermaid
flowchart TD
  ExcludeFromLoggingAttribute[ExcludeFromLoggingAttribute]
  LoggingLimitPolicy[LoggingLimitPolicy]

```
