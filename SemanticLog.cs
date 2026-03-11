using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text;

namespace org.SpocWeb.root.logging;

/// <summary> Interpolation Handler to capture the Expression in the Interpolation String </summary>
/// <remarks>
/// Assembles the <see cref="Template"/> while the Format String is parsed.
/// Its Constructor Parameters after the initial formatLength and formatNumber
/// match the Arguments after the String-Interpolation.
/// 
/// <see cref="FormattableString"/> can also capture the format string and values,
/// but it cannot...
/// - capture the names of the arguments
/// - rewriting for e.g. Serilog destructuring marker @,
/// - compile-time gating using `out isEnabled` to avoid allocations.
/// </remarks>
[InterpolatedStringHandler]
public ref struct PrefixedStringHandler {

	/// <summary> Use Numbers instead of Names to build the Format String </summary>
	public static bool UseNumbers { get; set; } = false;//true;

    /// <summary> Optional Prefix for any Name </summary>
	private readonly string _prefix;

    /// <summary> The numbered Format String assembled from parsing the Interpolation String </summary>
    /// <remarks> Can be converted into 
    /// - a semantic Format String by using the <see cref="KeyedValues"/> Keys 
    /// - a log String by using the <see cref="KeyedValues"/> Values. 
    /// </remarks>
	public readonly StringBuilder Template;// = new StringBuilder();

	/// <summary> The Values mapped to the Keys parsed from the Interpolation String </summary>
	public readonly List<KeyValuePair<string, object?>> KeyedValues;// = new();

    /// <summary> Semantic Format String with {bracedKeys} </summary>
    /// <remarks>This can still be converted into <see cref="MessageWithValues"/>
    /// by replacing every "{Key}" in <see cref="KeyedValues"/> with its formatted "Value".
    /// </remarks>
    public string MessageWithKeys() => string.Format(Template.ToString(), KeyedValues.Keys().ToArray());

	/// <summary> Semantic Format String with <see cref="KeyedValues"/> Values </summary>
	public readonly string MessageWithValues() => string.Format(Template.ToString(), KeyedValues.Values().ToArray());

	/// <summary> Semantic Format String with <see cref="KeyedValues"/> Keys </summary>
	public readonly void WriteMessageWithValues(TextWriter writer)
        => writer.Write(Template.ToString(), KeyedValues.Values().ToArray());

	/// <inheritdoc cref="PrefixedStringHandler"/>
	public PrefixedStringHandler(int literalLength, int formatCount, ILogger logger, out bool isEnabled)
		: this(literalLength, formatCount, "", logger, LogLevel.Information, out isEnabled) { }

	/// <inheritdoc cref="PrefixedStringHandler"/>
	public PrefixedStringHandler(int literalLength, int formatCount, string prefix, ILogger logger, out bool isEnabled)
		: this(literalLength, formatCount, prefix, logger, LogLevel.Information, out isEnabled) { }

	/// <inheritdoc cref="PrefixedStringHandler"/>
	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	public PrefixedStringHandler(int literalLength, int formatCount, string prefix, ILogger logger, LogLevel level, out bool isEnabled) {
		_prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix + "_";
		isEnabled = logger.IsEnabled(level);
		Template = new StringBuilder(literalLength << 1);
		KeyedValues = new(formatCount + 1);
	}

	/// <summary> Escapes `{` and `}` by doubling. </summary>
	public void AppendLiteral(string s) => Template.Append(s.Replace("{", "{{").Replace("}", "}}"));

	/// <summary>Generic Formatter for all other Types</summary>
	public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string argName = "") {
		// Microsoft ILogger uses {name} for structured logging
		var key = _prefix + argName;
		var name = UseNumbers ? 
            KeyedValues.Count.ToString() : key;
		Template.Append($"{{{name}}}");
		KeyedValues.Add(new KeyValuePair<string, object?>(key, value));
	}

	/// <summary> Special Array Formatting for Collections (Arrays, Lists, etc.) </summary>
	public void AppendFormatted<TElement>(IEnumerable<TElement> values
		, [CallerArgumentExpression("values")] string argName = "") {
		var key = _prefix + argName;
		var name = UseNumbers ? 
            KeyedValues.Count.ToString() : key;
		// We treat collections similarly to destructured objects 
		// so they stay as arrays in the JSON output.
		Template.Append($"{{{name}}}");

		// To be safe, we convert to an array to "freeze" the collection 
		// so it doesn't change before the log is written.
		KeyedValues.Add(new KeyValuePair<string, object?>(key, values?.ToArray()));
	}

	/// <summary> allows usage like: $"The price is {price:C2}" </summary>
    public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression("value")] string argName = "") {
		// We append the format to the template hole
		var formatSuffix = string.IsNullOrEmpty(format) ? "" : ":" + format;
		var key = _prefix + argName + formatSuffix;
		var name = UseNumbers ? 
            KeyedValues.Count.ToString() : key;
		Template.Append($"{{{name}}}");
		KeyedValues.Add(new KeyValuePair<string, object?>(key, value));
	}

	/// <summary> Specifies the prefix string used for identifying LogX.Destructure operations. </summary>
	public static readonly string PREFIX = nameof(LogX) + '.' + nameof(LogX.Destructure) + '(';

	/// <summary> Custom Formatting for <see cref="DestructureWrapper"/>.</summary>
	/// <remarks>
	/// Adds '@' which most providers (Serilog/OTel) will respect!
	/// </remarks>
	public void AppendFormatted(DestructureWrapper wrapper, [CallerArgumentExpression("wrapper")] string argName = "") {
        var cleanName = argName.Replace(".Destructure()", "").Replace(PREFIX, "").Replace(")", "").Trim();
		var key = _prefix + cleanName;
		var name = UseNumbers ? 
            KeyedValues.Count.ToString() : key;
        Template.Append($"{{@{name}}}");
		KeyedValues.Add(new KeyValuePair<string, object?>(key, wrapper.Value));
	}
}

/// <summary> Makes the compiler pick a different overload of the <see cref="PrefixedStringHandler.AppendFormatted"/> Method. </summary>
/// <remarks>
/// By wrapping the <paramref name="Value"/> in DestructureWrapper (via the Log.Destructure() helper),
/// you change the type of the argument.
///
/// It enables the "@" prefixing
/// Inside the handler, when the DestructureWrapper overload is hit,
/// we manually inject the @ symbol into the message template that we are building for the underlying logger.
/// </remarks>
public record struct DestructureWrapper(object Value);

/// <summary> Extension Methods to log semantically with String Interpolation. </summary>
/// <remarks>
/// For semantic Logging it is sufficient to provide a Template String with {namedHoles}
/// and an Array of Value aligned to these Holes.
/// The Json can later be restored by parsing the {namedHoles}.
/// </remarks>
#pragma warning disable CA2254
public static class LogX {

	/// <summary> Represents the key used to store the original format of a message. </summary>
	/// <remarks>This Key is conventional across many MEL providers like
	/// console/json console providers and OpenTelemetry logging bridges
	/// </remarks>
	public const string KeyOriginalFormat = "{OriginalFormat}";

	/// <summary> Flag to put only the Values in the <see cref="Serilog.Events.LogEvent"/>,
	/// instead of all Pairs including the <see cref="KeyOriginalFormat"/> </summary>
	public static bool ForDeStructure { get; set; } = true;

	/// <summary> Wraps the <paramref name="value"/> into <see cref="DestructureWrapper"/> to trigger writing an `@` to indicate Destructuring to SeriLog </summary>
	public static DestructureWrapper Destructure(this object value) => new(value);

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/> </summary>
	public static void Logg(this ILogger logger
		, [InterpolatedStringHandlerArgument(nameof(logger))] ref PrefixedStringHandler stringInterpolation
		, Exception? x = null, LogLevel? optLevel = default, EventId eventId = default) {
		var level = optLevel ?? (x == null ? LogLevel.Information : LogLevel.Error);
		if (logger.IsEnabled(level)) {
			if (ForDeStructure) {
				logger.Log(level, eventId, x, stringInterpolation.Template.ToString(), stringInterpolation.KeyedValues.Values().ToArray());
			} else {
				stringInterpolation.KeyedValues.Add(new(KeyOriginalFormat, stringInterpolation.Template.ToString()));
				logger.Log(level, eventId, stringInterpolation.KeyedValues, x, static (pairs, e) => pairs.Last().Value + "");
			}
		}
	}

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/>
	/// with the <paramref name="context"/> </summary>
	public static void Logg(this ILogger logger, LogLevel level, string context
		, [InterpolatedStringHandlerArgument(nameof(context), nameof(logger), nameof(level))] ref PrefixedStringHandler stringInterpolation
		, Exception? x = null, EventId eventId = default) {
		if (!logger.IsEnabled(level)) {
			return;
		}
		using (logger.BeginScope(new Dictionary<string, object?> { [nameof(context)] = context })) {
			if (ForDeStructure) { //DeStructure keeps Value
				logger.Log(level, eventId, x, stringInterpolation.Template.ToString(), stringInterpolation.KeyedValues.Values().ToArray());
			} else {  //DeStructure often becomes a String
				stringInterpolation.KeyedValues.Add(new(KeyOriginalFormat, stringInterpolation.Template.ToString()));
				logger.Log(level, eventId, stringInterpolation.KeyedValues, x, static (pairs, e) => pairs.Last().Value + "");
			}                                                                                                                   
		}
	}

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/>
	/// with the <paramref name="context"/> </summary>
	public static void Logg(this ILogger logger, string context
		, [InterpolatedStringHandlerArgument(nameof(context), nameof(logger))] ref PrefixedStringHandler stringInterpolation
		, Exception? x = null, LogLevel? optLevel = null, EventId eventId = default) {
		var level = optLevel ?? (x == null ? LogLevel.Information : LogLevel.Error);
		if (!logger.IsEnabled(level)) {
			return;
		}
		using (logger.BeginScope(new Dictionary<string, object?> { [nameof(context)] = context })) {
			if (ForDeStructure) { //DeStructure keeps Value
				logger.Log(level, eventId, x, stringInterpolation.Template.ToString(), stringInterpolation.KeyedValues.Values().ToArray()); //DeStructure keeps Value
			} else {
				stringInterpolation.KeyedValues.Add(new(KeyOriginalFormat, stringInterpolation.Template.ToString()));
				logger.Log(level, eventId, stringInterpolation.KeyedValues, x, static (pairs, e) => pairs.Last().Value + ""); //DeStructure becomes a String
			}
		}
	}

	/// <summary> Returns the values from a sequence of key/value pairs. </summary>
	/// <remarks>The same is possible for IReadOnlyList using ReadOnlyListFilter{t} with Delegate. </remarks>
	public static IEnumerable<V> Values<K, V>(this IEnumerable<KeyValuePair<K, V>> keyedValues) => keyedValues.Select(p => p.Value);

	/// <summary> Returns the keys from a sequence of key/value pairs. </summary>
	public static IEnumerable<K> Keys<K, V>(this IEnumerable<KeyValuePair<K, V>> keyedValues) => keyedValues.Select(p => p.Key);

	/// <summary> Returns the keys from a sequence of key/value pairs. </summary>
	public static IList<KeyValuePair<K, V>> Add<K, V>(this IList<KeyValuePair<K, V>> keyedValues
		, K key, V value) {
		keyedValues.Add(new(key, value));
		return keyedValues;
	}
}

