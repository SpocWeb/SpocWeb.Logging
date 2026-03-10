using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;
using System.Text;

namespace org.SpocWeb.root.logging;

/// <summary> Interpolation Handler to capture the Expression in the Interpolation String </summary>
/// <remarks>assembles the <see cref="Template"/> while the Format String is parsed. </remarks>
[InterpolatedStringHandler]
public ref struct PrefixedStringHandler {

	private readonly string _prefix;

	/// <summary> The semantic Format String assembled from parsing the Interpolation String </summary>
	public readonly StringBuilder Template = new StringBuilder();

	/// <summary> The Values mapped to the Keys parsed from the Interpolation String </summary>
	public readonly List<KeyValuePair<string, object?>> KeyedValues = new();

	/// <inheritdoc cref="PrefixedStringHandler"/>
	public PrefixedStringHandler(int litLen, int fmtCount, ILogger logger, out bool isEnabled)
		: this(litLen, fmtCount, "", logger, out isEnabled) { }

	/// <inheritdoc cref="PrefixedStringHandler"/>
	[SuppressMessage("ReSharper", "UnusedParameter.Local")]
	public PrefixedStringHandler(int litLen, int fmtCount, string prefix, ILogger logger, out bool isEnabled) {
		_prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix + "_";
		isEnabled = logger.IsEnabled(LogLevel.Information);
	}

	/// <summary> Escapes `{` and `}` by doubling. </summary>
	public void AppendLiteral(string s) => Template.Append(s.Replace("{", "{{").Replace("}", "}}"));

	/// <summary>Generic Formatter for all other Types</summary>
	public void AppendFormatted<T>(T value, [CallerArgumentExpression("value")] string name = "") {
		// Microsoft ILogger uses {name} for structured logging
		Template.Append($"{{{_prefix}{name}}}");
		KeyedValues.Add(new KeyValuePair<string, object?>(_prefix + name, value));
	}

	/// <summary> Special Array Formatting for Collections (Arrays, Lists, etc.) </summary>
	public void AppendFormatted<TElement>(IEnumerable<TElement> values
		, [CallerArgumentExpression("values")] string name = "") {
		// We treat collections similarly to destructured objects 
		// so they stay as arrays in the JSON output.
		Template.Append($"{{{_prefix}{name}}}");

		// To be safe, we convert to an array to "freeze" the collection 
		// so it doesn't change before the log is written.
        KeyedValues.Add(new KeyValuePair<string, object?>(_prefix + name, values?.ToArray()));
	}

	/// <summary> allows usage like: $"The price is {price:C2}" </summary>
	public void AppendFormatted<T>(T value, string? format, [CallerArgumentExpression("value")] string name = "") {
		// We append the format to the template hole
		var formatSuffix = string.IsNullOrEmpty(format) ? "" : ":" + format;
		Template.Append($"{{{_prefix}{name}{formatSuffix}}}");
        KeyedValues.Add(new KeyValuePair<string, object?>(_prefix + name + formatSuffix, value));
	}

	/// <summary> Specifies the prefix string used for identifying LogX.Destructure operations. </summary>
	public static readonly string PREFIX = nameof(LogX) + '.' + nameof(LogX.Destructure) + '(';

	/// <summary> Custom Formatting for <see cref="DestructureWrapper"/>.</summary>
	/// <remarks>
	/// Adds '@' which most providers (Serilog/OTel) will respect!
	/// </remarks>
	public void AppendFormatted(DestructureWrapper wrapper, [CallerArgumentExpression("wrapper")] string name = "") {
		var cleanName = name.Replace(".Destructure()", "").Replace(PREFIX, "").Replace(")", "").Trim();
		Template.Append($"{{@{_prefix}{cleanName}}}");
        KeyedValues.Add(new KeyValuePair<string, object?>(_prefix + cleanName, wrapper.Value));
	}

	/// <inheritdoc cref="PrefixedStringHandler"/>
	public (string Template, object?[] Args) GetResult() => (Template.ToString(), KeyedValues.Values().ToArray());
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

	/// <summary> Wraps the <paramref name="value"/> into <see cref="DestructureWrapper"/> to trigger writing an `@` to SeriLog </summary>
	public static DestructureWrapper Destructure(this object value) => new(value);

	/// <summary> Log the <paramref name="stringInterpolation"/> to the <paramref name="logger"/> </summary>
	public static void Logg(this ILogger logger
		, [InterpolatedStringHandlerArgument(nameof(logger))] ref PrefixedStringHandler stringInterpolation
		, Exception? x = null, LogLevel? optLevel = default, EventId eventId = default) {
		var level = optLevel ?? (x == null ? LogLevel.Information : LogLevel.Error);
		if (logger.IsEnabled(level)) {
			stringInterpolation.KeyedValues.Add(new(KeyOriginalFormat, stringInterpolation.Template.ToString()));
			//logger.Log(level, eventId, x, stringInterpolation.Template.ToString(), stringInterpolation.KeyedValues.Values().ToArray());
			logger.Log(level, eventId, stringInterpolation.KeyedValues, x, static (pairs, e) => {
				return pairs.Last().Value + "";
				//var original = pairs.FirstOrDefault(kv => kv.Key == KeyOriginalFormat).Value + "";
				//return original ?? string.Empty;
			});
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
			stringInterpolation.KeyedValues.Add(new(KeyOriginalFormat, stringInterpolation.Template.ToString()));
			logger.Log(level, eventId, x, stringInterpolation.Template.ToString(), stringInterpolation.KeyedValues.Values().ToArray()); //DeStructure keeps Value
			//logger.Log(level, eventId, stringInterpolation.KeyedValues, x, static (pairs, e) => pairs.Last().Value + ""); //DeStructure becomes a String
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

