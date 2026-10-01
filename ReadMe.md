---
facet-complexity: 3
facet-status: active
facet-layer: infrastructure
concepts:
  - structured_logging_bridge
tags:
  - code/cross_cutting_infrastructure
  - code/message_template_parsing
description: "SpocWeb.Logging is a minimal, injection-free logging utility that bridges C# string interpolation with structured logging via `Microsoft.Extensions.Logging` and Serilog. It eliminates the need to inject `ILogger` everywhere by exposing a single static `Log.Logger` dispatcher, while preserving semantic property names using `CallerArgumentExpression` and `CallerFilePath`. The library can optionally be combined with the SpocWeb.Proxies project, which provides a dynamic logging proxy interceptor pluggable via Dependency Injection to log all calls with their parameters and return values."
digest:
  local-classes:
    DestructureWrapper:
      mtime: "2026-08-18T17:17:16Z"
      digest: "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"
    Int:
      mtime: "2026-08-18T17:17:16Z"
      digest: "d13ad519922aaf7d4f69afa6992c98ef31297f521db5df6bd32124b4fdccf629"
    Log:
      mtime: "2026-08-18T17:17:16Z"
      digest: "801899ca26ba34e0994f6a41cde675802c9dd06f5469a1ad3f3661185f8e125b"
    LogX:
      mtime: "2026-08-18T17:17:16Z"
      digest: "390c7304721fa0c125175bc6a7b15ad5cbe02c1ea2a2a2dfccefd2ccd9c5bc43"
    PrefixedStringHandler:
      mtime: "2026-08-18T17:17:16Z"
      digest: "5fa0340681151093ecf9e5d0827c3d518d55b500273ee3a449eeef5a61f0cb3b"
    Program:
      mtime: "2026-08-18T17:17:16Z"
      digest: "8b4e2159ade04ce1383da2aa2e0f47c259eebbe4f7a5d9546c89356042e5e6f4"
    StringInterpolationWithValues:
      mtime: "2026-08-18T17:17:16Z"
      digest: "11eb3dd0720f3c59206440050317d00db09a2a43ca7f287c9412d09053d209b9"
  folders: {}
dv_has_:
  sub_:
    folders: 1
    files: 26
    units: 10
    facet_:
      layer_:
        infrastructure: 7
        domain: 2
        presentation: 1
      status_:
        active: 9
        partial: 1
      complexity_:
        "1": 4
        "2": 2
        "3": 3
        "4": 1
    tag_:
      code_:
        log_destructuring: 4
        logging_exclusion_attribute: 2
        message_template_parsing: 2
        interpolated_string_handler: 2
        logging_dispatcher: 1
        type_safe_wrapper: 1
        entry_point: 1
        value_object: 1
    concept_:
      compile_time_string_interpolation: 1
      destructuring_policy: 1
      library_placeholder: 1
      parsed_template_values: 1
      property_suppression_marker: 1
      semantic_logging_extensions: 1
      serilog_destructuring_marker: 1
      serilog_extensions: 1
      typed_integer_wrapper: 1
      "Technology\\IT\\Software\\Logging.md": 1
has_sub_folders: 1
has_sub_files: 26
has_sub_units: 10
has_sub_facet_layer_infrastructure: 7
has_sub_facet_layer_domain: 2
has_sub_facet_layer_presentation: 1
has_sub_facet_status_active: 9
has_sub_facet_status_partial: 1
has_sub_facet_complexity_1: 4
has_sub_facet_complexity_2: 2
has_sub_facet_complexity_3: 3
has_sub_facet_complexity_4: 1
has_sub_tag_code_log_destructuring: 4
has_sub_tag_code_logging_exclusion_attribute: 2
has_sub_tag_code_message_template_parsing: 2
has_sub_tag_code_interpolated_string_handler: 2
has_sub_tag_code_logging_dispatcher: 1
has_sub_tag_code_type_safe_wrapper: 1
has_sub_tag_code_entry_point: 1
has_sub_tag_code_value_object: 1
has_sub_concept_compile_time_string_interpolation: 1
has_sub_concept_destructuring_policy: 1
has_sub_concept_library_placeholder: 1
has_sub_concept_parsed_template_values: 1
has_sub_concept_property_suppression_marker: 1
has_sub_concept_semantic_logging_extensions: 1
has_sub_concept_serilog_destructuring_marker: 1
has_sub_concept_serilog_extensions: 1
has_sub_concept_typed_integer_wrapper: 1
has_sub_concept_technology_it_software_logging_md: 1
---
# SpocWeb.Logging


SpocWeb.Logging is a minimal, injection-free logging utility
that bridges C# string interpolation with structured logging
via `Microsoft.Extensions.Logging` and Serilog.
It eliminates the need to inject `ILogger` everywhere
by exposing a single static `Log.Logger` dispatcher,
while preserving semantic property names using
`CallerArgumentExpression` and `CallerFilePath`.
The library can optionally be combined with the SpocWeb.Proxies project,
which provides a dynamic logging proxy interceptor
pluggable via Dependency Injection to log all calls
with their parameters and return values.

## Dependencies

### NuGet Packages

- `coverlet.collector`
- `Microsoft.Extensions.Logging.Abstractions`
- `PolySharp`
- `Serilog`

## Architecture

```mermaid
flowchart TD
    subgraph Core["Core (root)"]
        Log["[Log](Log.cs)
    Static dispatcher and
    FormattableString extensions"]
        SIVW["[StringInterpolationWithValues](StringInterpolationWithValues.cs)
    Parsed template + argument array"]
        LogX["[LogX](SemanticLog.cs)
    Interpolation-handler logging
    extensions on ILogger"]
        PSH["[PrefixedStringHandler](SemanticLog.cs)
    Captures argument names
    and values at call-site"]
        DW["[DestructureWrapper](SemanticLog.cs)
    Marker struct to trigger
    @ destructuring in Serilog"]
        Int["[Int&lt;T&gt;](Int.cs)
    Generic typed integer
    value struct"]
    end

    subgraph SeriLog["SeriLog subfolder"]
        LLP["[LoggingLimitPolicy](SeriLog/LoggingLimitPolicy.cs)
    Serilog IDestructuringPolicy;
    truncates strings and arrays"]
        EFLA["[ExcludeFromLoggingAttribute](SeriLog/ExcludeFromLoggingAttribute.cs)
    Attribute to suppress
    logging of a property or type"]
    end

    Log -->|"parses FormattableString into"| SIVW
    Log -->|"dispatches via ILogger.Log"| SIVW
    LogX -->|"builds template using"| PSH
    PSH -->|"wraps destructured values in"| DW
    LLP -->|"skips properties marked with"| EFLA

linkStyle 4 opacity:1
```

## Entry Points

- [Log.Error(FormattableString)](Log.cs#L59) — log an error from a string interpolation expression, capturing source location automatically.
- [Log.Information(FormattableString)](Log.cs#L103) — log at information level; representative of all six severity-level overloads.
- [Log.Parse(FormattableString)](Log.cs#L293) — parse and cache a `FormattableString` into a `StringInterpolationWithValues`, reading expression names from the call-site source.
- [LogX.Logg(ILogger, PrefixedStringHandler)](SemanticLog.cs#L178) — log a semantically named interpolated string directly on an `ILogger`, with compile-time level gating.
- [LoggingLimitPolicy.TryDestructure](SeriLog/LoggingLimitPolicy.cs#L42) — Serilog destructuring policy entry point; truncates strings, limits arrays, and filters excluded properties.

## Quick Start

### 1. Assign the global logger once at startup

```csharp
using org.SpocWeb.root.logging;

Log.Logger = loggerFactory.CreateLogger("App");
```

### 2. Log with a `FormattableString` — no injection required

```csharp
Log.Information($"Processing order {orderId} for customer {customerName}");
Log.Error($"Failed to save {entity}", exception);
```

The library resolves argument names from the call-site source file
via `CallerArgumentExpression` (NET 6+) or by reading the source line on older targets,
so the Serilog / OTel JSON output contains named properties rather than positional indices.

### 3. Semantic logging on an injected `ILogger`

```csharp
logger.Logg($"User {userId} logged in from {ipAddress}");
```

`PrefixedStringHandler` is resolved at compile time;
the interpolation is skipped entirely when the log level is disabled.

### 4. Control destructuring

```csharp
logger.Logg($"Payload: {payload.Destructure()}");
```

The `@` prefix is injected into the template automatically,
instructing Serilog / OTel providers to serialize the object as a structure.

## Key Concepts

### `StringInterpolationWithValues`

Captures the parsed `MessageTemplate` alongside the raw argument array.
Returned by every `Log.*` method so call-sites can reuse the message
(e.g. as an exception message) without re-parsing.
See [StringInterpolationWithValues.cs](StringInterpolationWithValues.cs).

### `PrefixedStringHandler` / `LogX`

Uses the `[InterpolatedStringHandler]` pattern so the compiler passes
each interpolated segment directly into the handler,
enabling compile-time level gating (`out isEnabled`)
and semantic key capture via `CallerArgumentExpression`.
See [SemanticLog.cs](SemanticLog.cs).

### `LoggingLimitPolicy`

A Serilog `IDestructuringPolicy` that limits string length
(default 100 chars) and array cardinality (default 10 elements),
and omits properties annotated with `[ExcludeFromLogging]`
or listed in `IgnoredProperties`.
See [SeriLog/LoggingLimitPolicy.cs](SeriLog/LoggingLimitPolicy.cs).

### `Int<T>`

A generic, strongly typed `int` wrapper that prevents mixing up
domain-specific integer identifiers (e.g. `Int<OrderId>` vs `Int<CustomerId>`).
Supports arithmetic and comparison operators.
See [Int.cs](Int.cs).

## Further Reading

- [Microsoft.Extensions.Logging abstractions](https://learn.microsoft.com/dotnet/core/extensions/logging)
- [Serilog message templates](https://messagetemplates.org/)
- [InterpolatedStringHandler pattern (C# 10)](https://learn.microsoft.com/dotnet/csharp/advanced-topics/performance/interpolated-string-handler)
- [CallerArgumentExpression attribute](https://learn.microsoft.com/dotnet/csharp/language-reference/attributes/caller-information#callerargumentexpression-attribute)
- SpocWeb.Proxies — dynamic logging proxy interceptor (sibling project)

## Classes

| Class | Responsibility |
|---|---|
| [Int](Int.cs) | Generically typed Int32. |
| [Log](Log.cs) | Extension Methods to use StringInterpolationWithValues for Logging. |
| [Program](Program.cs) | Entry point placeholder for the SpocWeb.Logging project. |
| [PrefixedStringHandler](SemanticLog.cs) | Interpolation Handler to capture the Expression in the Interpolation String |
| [DestructureWrapper](SemanticLog.cs) | Makes the compiler pick a different overload of the AppendFormatted Method. |
| [LogX](SemanticLog.cs) | Extension Methods to log semantically with String Interpolation. |
| [StringInterpolationWithValues](StringInterpolationWithValues.cs) | Encapsulates a parsed StringInterpolation with values |

## Subsystems

| Folder | Domain Role |
|---|---|
| [`SeriLog/`](SeriLog/ReadMe.md) | Serilog-specific extensions for `SpocWeb.Logging`: a destructuring policy that truncates oversized strings and arrays, and an attribute that suppresses logging of sensitive or irrelevant properties. |
