using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog.Parsing;

namespace org.SpocWeb.root.logging;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

/// <summary> Extension Methods to use <see cref="StringInterpolationWithValues"/> for Logging. </summary>
/// <remarks>
/// This makes Log-Statements more readable and re-usable for Exceptions and other Messages.
///
/// ## Meta
/// pass: 2
/// mtime: 2026-03-06T12:13:23Z
/// digest: afae3f6b5902d3e5547e169262035596db690e64ea00858288dd6d86ca7b3a82
/// updated: 2026-05-19
/// </remarks>
/// <seealso cref="StringInterpolationWithValues">StringInterpolationWithValues: parsed message template paired with its argument values.</seealso>
/// <seealso cref="LogX">LogX: semantic interpolation-handler-based logging extension methods.</seealso>
/// <seealso cref="PrefixedStringHandler">PrefixedStringHandler: interpolated string handler that captures argument names and values at call-site.</seealso>
/// <example>
/// <code language="yaml">
/// pass: 2
/// mtime: 2026-06-01T21:44:07Z
/// digest: 6057a5f6267fac2c8bb19fd096a5f717c61697110f525f648c9f1a3b4c49f058
/// </code>
/// </example>
[SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
public static class Log
{
	/// <summary> Central Log Dispatcher and aggregator </summary>
	/// <remarks> for lightweight Coding w/o injecting Loggers everywhere. </remarks>
	public static ILogger? Logger { get; set; } //= new Logger();

#pragma warning disable CA2254
	/// <summary>Gets the _message Template Parser.</summary>
	static readonly MessageTemplateParser _messageTemplateParser = new();

	// Function to generate a formatted string
	// Parse the Serilog message template
	/// <summary>Parses <paramref name="message"/> into a <see cref="MessageTemplate"/>, ignoring the second argument.</summary>
 	public static MessageTemplate ParseTemplate(string message, object _) => _messageTemplateParser.Parse(message);

	/// <summary>Gets the _templates.</summary>
	static readonly Dictionary<string, MessageTemplate> _templates = [];

	/// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
	/// <remarks>
	/// This Serilog-like Parsing retains the Expression names, but at the cost of duplicating them.
	/// Rather use a <see cref="FormattableString"/> with String Interpolation!
	/// </remarks>
	[Obsolete("Rather use String Interpolation like in " + nameof(LogX))]
	public static StringInterpolationWithValues Parse_(string stringInterpolation, params object[] args) {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, "Rather use String Interpolation!", -1, args);// { Exception = x };
	}

	#region Log Statements

	/// <inheritdoc cref="Error(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Error(FormattableString stringInterpolation
		, Exception? x = null,
#if NET6_0_OR_GREATER
        [CallerArgumentExpression(nameof(stringInterpolation))]
#else //NET6_0_OR_GREATER
#endif //NET6_0_OR_GREATER
	string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Logger.Error(stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Error"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Error(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogError(x, messageWithValues.template.Text, messageWithValues.values);
		Logger?.Log(LogLevel.Error, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Critical(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Critical(FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Critical(Logger, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Critical"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Critical(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogCritical(x, parsed.template.Text, parsed.values);
		Logger?.Log(LogLevel.Critical, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Debug(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Debug(FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Debug(Logger, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Debug"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Debug(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogDebug(x, messageWithValues.template.Text, messageWithValues.values);
		Logger?.Log(LogLevel.Debug, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Information(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Information(FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Information(Logger, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Information"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Information(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogInformation(x, messageWithValues.template.Text, messageWithValues.values);
		Logger?.Log(LogLevel.Information, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Warning(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Warning(FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Warning(Logger, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Warning"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Warning(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogWarning(x, messageWithValues.template.Text, messageWithValues.values);
		Logger?.Log(LogLevel.Warning, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Trace(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Trace(FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Trace(Logger, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <summary>Dispatches <paramref name="messageWithValues"/> at see cref="LogLevel.Trace"/> level via <see cref="Logger"/>.</summary>
 	public static StringInterpolationWithValues Trace(StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogTrace(x, messageWithValues.template.Text, messageWithValues.values);
		Logger?.Log(LogLevel.Trace, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	#endregion Log Statements

	#region Log Extension Statements

	/// <inheritdoc cref="Error(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Error(this ILogger log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> log.Error(stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Error(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Error(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogError(x, messageWithValues.template.Text, messageWithValues.values);
		log?.Log(LogLevel.Error, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Critical(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Critical(this ILogger log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Critical(log, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Critical(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Critical(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogCritical(x, parsed.template.Text, parsed.values);
		log?.Log(LogLevel.Critical, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Debug(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Debug(this ILogger? log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Debug(log, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Debug(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Debug(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogDebug(x, messageWithValues.template.Text, messageWithValues.values);
		log?.Log(LogLevel.Debug, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Information(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Information(this ILogger? log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Information(log, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Information(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Information(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogInformation(x, messageWithValues.template.Text, messageWithValues.values);
		log?.Log(LogLevel.Information, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Warning(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Warning(this ILogger? log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Warning(log, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Warning(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Warning(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogWarning(x, messageWithValues.template.Text, messageWithValues.values);
		log?.Log(LogLevel.Warning, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	/// <inheritdoc cref="Trace(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Trace(this ILogger? log, FormattableString stringInterpolation
		, Exception? x = null, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string path = "", [CallerLineNumber] int lineNo = -1)
		=> Trace(log, stringInterpolation.Parse(expression, path, lineNo), x);

	/// <inheritdoc cref="Trace(StringInterpolationWithValues, Exception?)"/>
	public static StringInterpolationWithValues Trace(this ILogger? log, StringInterpolationWithValues messageWithValues, Exception? x = null) {
		//log.LogTrace(x, messageWithValues.template.Text, messageWithValues.values);
		log?.Log(LogLevel.Trace, 0, messageWithValues, x, (m, e) => m.ToString() + e);
		return messageWithValues;
	}

	#endregion Log Extension Statements

	/// <summary>Formats <paramref name="template"/> by substituting each placeholder with the corresponding entry from <paramref name="properties"/>.</summary>
 	public static string Format(this MessageTemplate template, params object?[] properties) {
		var result = new StringBuilder(template.Text);
		var pos = -1;
		foreach (var token in template.Tokens) {
			if (token is not PropertyToken propertyToken) {
				continue;
			}

			var propertyValue = int.TryParse(propertyToken.PropertyName, out var index)
				? properties[index]?.ToString()
				: properties[++pos]?.ToString();
			result = result.Replace(propertyToken.ToString(), propertyValue);
		}

		return result.ToString();
	}

	/// <inheritdoc cref="AddProperties"/>
	public static Dictionary<string, object?> ToDictionary(this MessageTemplate template, object?[] properties) {
		var dictionary = new Dictionary<string, object?>();
		dictionary.AddProperties(template, properties);
		return dictionary;
	}

	/// <summary> Adds the <paramref name="properties"/> to the <paramref name="dictionary"/> </summary>
	public static IDictionary<string, object?> AddProperties(this IDictionary<string, object?>? dictionary, MessageTemplate template, params object?[] properties) {
		dictionary ??= new Dictionary<string, object?>();
		var pos = -1;
		foreach (var token in template.Tokens) {
			if (token is not PropertyToken propertyToken) {
				continue;
			}

			var propertyValue = int.TryParse(propertyToken.PropertyName, out var index)
				? properties[index]?.ToString()
				: properties[++pos]?.ToString();
			dictionary[propertyToken.PropertyName] = propertyValue;
		}

		return dictionary;
	}

	/// <inheritdoc cref="Format(MessageTemplate, object?[])"/>
	public static string Format(this MessageTemplate template, IReadOnlyDictionary<string, object?> properties) {
		var result = new StringBuilder(template.Text);
		var tokens = template.Tokens.OfType<PropertyToken>();
		foreach (var token in tokens) {
			var propertyValue = properties[token.PropertyName]?.ToString();
			result = result.Replace($"{{{token.PropertyName}}}", propertyValue);
		}

		return result.ToString();
	}

	/// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
	/// <remarks>
	/// Tries to read the Expressions from the Log Line.
	/// This requires all Expressions to be written on the same Log Line
	/// and the Log Text should not contain `{` or `}` to simplify Parsing.
	///
	/// For comfortable Logging, the Log-Statements should be extracted into their own Files,
	/// similar to the NDoc.xml Files, and deployed to the run-site,
	/// to improve the Logging Experience.
	///
	/// Alternatively the Log-Evaluation can mix in these Values,
	/// but only if File and Line-Info can be matched.
	/// </remarks>
	public static StringInterpolationWithValues Parse(this FormattableString stringInterpolation//, Exception? x = null
		, [CallerArgumentExpression(nameof(stringInterpolation))] string? expression = null
		, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNo = -1
		) {
		if (!_templates.TryGetValue(stringInterpolation.Format, out var template)) {
			_templates[stringInterpolation.Format] = template = _messageTemplateParser.Parse(stringInterpolation.Format);
			string? line = expression;
			if (string.IsNullOrEmpty(line)) {
				if (lineNo > 0 && File.Exists(filePath)) {
					var lines = File.ReadLines(filePath);
					line = lines.Skip(lineNo - 1).First();
				}
			}

			if (!string.IsNullOrEmpty(line)) {
				var tokens = (MessageTemplateToken[]) template.Tokens;
				var i = -1;
				foreach (Match match in BracedExpression.Matches(line)) { //.IndexOf("Log.Parse($\"", StringComparison.Ordinal))) {
					while (++i < tokens.Length) {
						if (tokens[i] is not PropertyToken property) {
							continue;
						} //keep the RawText `{0}` etc. for later Replacements:
						tokens[i] = new PropertyToken(match.Value.Substring(1, match.Value.Length - 2), property.ToString(), property.Format, property.Alignment, property.Destructuring);
						break;
					}
				}
			}
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, stringInterpolation.GetArguments());
	}

	/// <summary> Matches a braced Expression in String Interpolation </summary>
	//[GeneratedRegex(@"\{[^""]+?\}", RegexOptions.Compiled, 9)]
	public static readonly Regex BracedExpression = new(@"\{[^""]+?\}", RegexOptions.Compiled, TimeSpan.FromMilliseconds(99));

	#region parsing with > 0 Params 

	/// <summary> Parses and caches the <paramref name="stringInterpolation"/> </summary>
	public static StringInterpolationWithValues Parse(string stringInterpolation, object? arg0
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, stringInterpolation, arg0);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	public static StringInterpolationWithValues Parse(string formatString, object? arg0, object? arg1
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(formatString, out var template)) {
			_templates[formatString] = template = _messageTemplateParser.Parse(formatString);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4, arg5);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	[SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	[SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	[SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
	}

	/// <inheritdoc cref="Parse(FormattableString, string?, string, int)"/>
	[SuppressMessage("Major Code Smell", "S107:Method has 10 parameters, which is greater than the 7 authorized.", Justification = "Cannot use params[]")]
	public static StringInterpolationWithValues Parse(string stringInterpolation
		, object? arg0, object? arg1, object? arg2, object? arg3, object? arg4, object? arg5, object? arg6, object? arg7, object? arg8, object? arg9
		, [CallerLineNumber] int lineNo = -1, [CallerFilePath] string filePath = "") {
		if (!_templates.TryGetValue(stringInterpolation, out var template)) {
			_templates[stringInterpolation] = template = _messageTemplateParser.Parse(stringInterpolation);
		}

		return new StringInterpolationWithValues(template, filePath, lineNo, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
	}

	#endregion parsing with > 0 Params 

}
