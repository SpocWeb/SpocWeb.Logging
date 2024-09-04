using Serilog.Events;

namespace org.SpocWeb.root.Logging;

/// <summary> Encapsulates a parsed StringInterpolation with <see cref="Values"/> </summary>
/// <inheritdoc cref="ToString"/>
public record StringInterpolationWithValues(MessageTemplate Template, object[] Values)
{

    public MessageTemplate Template { get; } = Template;

    public object[] Values { get; } = Values;

    private string _toString;
    /// <summary> Formats the <see cref="Template"/> with the <see cref="Values"/></summary>
	public override string ToString() => _toString ??= Template.Format(Values);


    private Dictionary<string, object> _dictionary;


    public Dictionary<string, object> ToDictionary() => _dictionary ??= ToDictionary();
}