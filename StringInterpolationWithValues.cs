using Serilog.Events;

namespace org.SpocWeb.root.Logging;

/// <summary> Encapsulates a parsed StringInterpolation with <see cref="values"/> </summary>
/// <inheritdoc cref="ToString"/>
public record StringInterpolationWithValues(MessageTemplate template, params object?[] values)
{
	/// <summary> The parsed Template of the Interpolation </summary>
    public MessageTemplate Template => template;

	/// <summary> The Values to insert into the Template </summary>
    public object?[] Values => values;

	/// <summary> Optional Exception </summary>
    public Exception? Exception { get ; set ; }

    /// <summary> Formats the <see cref="template"/> with the <see cref="values"/></summary>
	public override string ToString() => _toString ??= template.Format(values);
	private string? _toString;

	/// <summary> Indexes the <see cref="values"/> with the <see cref="template"/> Placeholders </summary>
	/// <remarks>
	/// 0,1,2 for a Format-String, but readable Names for a Serilog string with array.
	/// The Names could be extracted by reading the Source Code File. 
	/// </remarks>
	public Dictionary<string, object?> ToDictionary() => _dictionary ??= template.ToDictionary(values);
    private Dictionary<string, object?>? _dictionary;
}