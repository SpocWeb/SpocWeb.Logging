using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace org.SpocWeb.root.Logging;

/// <summary> Encapsulates a parsed StringInterpolation with <see cref="values"/> </summary>
/// <remarks>
/// The <paramref name="filePath"/> can act as the Log Category, instead of the Class Name.
/// This further obsoletes the tedious Log Injection.
/// The <paramref name="filePath"/> can be mapped to severity or architectural Levels,
/// possibly by the namespace Parts, or by the Assembly so that Standard Assemblies are exempted.
/// The actual Log Level is a combination of
/// - the Assembly Level, <br/>
/// - the Class Level and <br/>
/// - the <see cref="LogLevel"/>
///
/// ## Unusual Logs: 
/// <see cref="LogLevel.Critical"/>,  <see cref="LogLevel.Error"/> and <see cref="LogLevel.Warning"/> are important,
/// independent of the Assembly or Class, because it indicates wrong / unexpected usage.
///
/// ## These Levels should consider the Assembly/Class much more to control Log Volume:
/// - <see cref="LogLevel.Information"/> <br/>
/// - <see cref="LogLevel.Debug"/> <br/>
/// - <see cref="LogLevel.Trace"/> <br/>
/// 
/// </remarks>
/// <inheritdoc cref="ToString"/>
public record StringInterpolationWithValues(MessageTemplate template//, Exception? exception
	, string filePath, int lineNo, params object?[] values)
{
	/// <summary> The parsed Template of the Interpolation </summary>
    public MessageTemplate Template => template;

	/// <summary> The Values to insert into the Template </summary>
    public object?[] Values => values;

	///// <summary> Optional Exception </summary>
	//public Exception? Exception => exception;

    /// <summary> Formats the <see cref="template"/> with the <see cref="values"/></summary>
	public override string ToString() => _toString ??= template.Format(values);// + exception;
	private string? _toString;

	/// <summary> Indexes the <see cref="values"/> with the <see cref="template"/> Placeholders </summary>
	/// <remarks>
	/// 0,1,2 for a Format-String, but readable Names for a Serilog string with array.
	/// The Names could be extracted by reading the Source Code File. 
	/// </remarks>
	public Dictionary<string, object?> ToDictionary() => _dictionary ??= template.ToDictionary(values);
    private Dictionary<string, object?>? _dictionary;
}