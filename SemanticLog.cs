using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

namespace org.SpocWeb.root.logging; 

/// <summary> Interpolation Handler to capture the Expression in the Interpolation String </summary>
[InterpolatedStringHandler]
public ref struct MicrosoftPrefixedHandler {
	private readonly string _prefix;
	private string _template = "";
	private readonly List<object?> _values = new();

	/// <inheritdoc cref="MicrosoftPrefixedHandler"/>
	public MicrosoftPrefixedHandler(int litLen, int fmtCount, ILogger logger, out bool isEnabled)
		: this(litLen, fmtCount, "", logger, out isEnabled) { }

	/// <inheritdoc cref="MicrosoftPrefixedHandler"/>
	public MicrosoftPrefixedHandler(int litLen, int fmtCount, string prefix, ILogger logger, out bool isEnabled) {
		_prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix + "_";
		isEnabled = logger.IsEnabled(LogLevel.Information);
	}

	/// <summary> Escapes `{` and `}` by doubling. </summary>
	public void AppendLiteral(string s) => _template += s.Replace("{", "{{").Replace("}", "}}");

	/// <summary>Generic Formatter for all other Types</summary>
	public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string name = "") {
		// Microsoft ILogger uses {name} for structured logging
		_template += $"{{{_prefix}{name}}}";
		_values.Add(value);
	}

	/// <summary> Special Array Formatting for Collections (Arrays, Lists, etc.) </summary>
	public void AppendFormatted<TElement>(IEnumerable<TElement> values
		, [CallerArgumentExpression("values")] string name = "") {
		// We treat collections similarly to destructured objects 
		// so they stay as arrays in the JSON output.
		_template += $"{{{_prefix}{name}}}";

		// To be safe, we convert to an array to "freeze" the collection 
		// so it doesn't change before the log is written.
		_values.Add(values?.ToArray());
	}

	/// <summary> allows usage like: $"The price is {price:C2}" </summary>
	public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression("value")] string name = "") {
		// We append the format to the template hole
		var formatSuffix = string.IsNullOrEmpty(format) ? "" : ":" + format;
		_template += $"{{{_prefix}{name}{formatSuffix}}}";
		_values.Add(value);
	}

	/// <summary> Specifies the prefix string used for identifying LogX.Destructure operations. </summary>
	public static readonly string PREFIX = nameof(LogX) + '.' + nameof(LogX.Destructure) + '(';

	/// <summary> Custom Formatting for <see cref="DestructureWrapper"/>.</summary>
	/// <remarks>
	/// Adds '@' which most providers (Serilog/OTel) will respect!
	/// </remarks>
	public void AppendFormatted(DestructureWrapper wrapper, [CallerArgumentExpression("wrapper")] string name = "") {
		var cleanName = name.Replace(".Destructure()", "").Replace(PREFIX, "").Replace(")", "")
			.Trim();
		_template += $"{{@{_prefix}{cleanName}}}";
		_values.Add(wrapper.Value);
	}

	/// <inheritdoc cref="MicrosoftPrefixedHandler"/>
	public (string Template, object?[] Args) GetResult() => (_template, _values.ToArray());
}

/// <summary>
/// 
/// </summary>
/// <remarks>
/// By wrapping the <paramref name="Value"/> in DestructureWrapper (via the Log.Destructure() helper),
/// you change the type of the argument.
/// This allows the compiler to pick a different overload of the AppendFormatted method.
///
/// It enables the "@" prefixing
/// Inside the handler, when the DestructureWrapper overload is hit,
/// we manually inject the @ symbol into the message template that we are building for the underlying logger.
/// </remarks>
public record struct DestructureWrapper(object Value);

/// <summary> Extension Methods to log semantically with String Interpolation. </summary>
#pragma warning disable CA2254
public static class LogX {
	/// <summary> Wraps the <paramref name="value"/> into <see cref="DestructureWrapper"/> to trigger writing an `@` to SeriLog </summary>
	public static DestructureWrapper Destructure(this object value) => new(value);

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/> </summary>
	public static void LogEvent(this ILogger logger
		, [InterpolatedStringHandlerArgument(nameof(logger))] ref MicrosoftPrefixedHandler stringInterpolation) {
		var (template, args) = stringInterpolation.GetResult();

		logger.Log(LogLevel.Information, template, args);
	}

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/> </summary>
	public static void LogEvent(this ILogger logger, LogLevel level
		, [InterpolatedStringHandlerArgument(nameof(logger))] ref MicrosoftPrefixedHandler stringInterpolation) {
		var (template, args) = stringInterpolation.GetResult();

		logger.Log(LogLevel.Information, template, args);
	}

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/>
	/// with the <paramref name="context"/> </summary>
	public static void LogEvent(this ILogger logger, string context
		, [InterpolatedStringHandlerArgument(nameof(context), nameof(logger))] ref MicrosoftPrefixedHandler stringInterpolation) {
		var (template, args) = stringInterpolation.GetResult();

		// We use BeginScope to add the EventName to every log in this call
		using (logger.BeginScope(new Dictionary<string, object> { [nameof(context)] = context })) {
			//logger.LogInformation(template, args);
			logger.Log(LogLevel.Information, template, args);
		}
	}
}

