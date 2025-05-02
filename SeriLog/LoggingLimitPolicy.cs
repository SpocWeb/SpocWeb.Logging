using Serilog.Core;
using Serilog.Events;

namespace org.SpocWeb.root.logging.SeriLog;

/// <summary> Serilog <see cref="IDestructuringPolicy"/> forms a Chain of Responsibility for serializing Values </summary>
/// <remarks>
/// By default, Serilog invokes the <see cref="object.ToString"/> Method on each Object.
/// To force destructuring, prepend the `@` to the Log-Property Name.
///
/// Developers can use Serilog's [Destructure] attribute.
/// Marking classes or properties with this attribute allows finer control over how data is broken down into logs.  
/// 
/// To register this Policy in the Chain, use this Code:
/// </remarks>
/// <code lang='cs'>
/// Log.Logger = new LoggerConfiguration()
///		.Destructure.With{ExcludePropertiesPolicy}()
///		.CreateLogger();
/// </code>
public class LoggingLimitPolicy : IDestructuringPolicy
{
	/// <summary> Limit String Length to reduce Log Size </summary>
	public static int MaxLengthOfString { get; set; } = 100;

	/// <summary> Limit Array Length to reduce Log Size </summary>
	public static int MaxLengthOfArray { get; set; } = 10;

	/// <summary> Names of ignored Properties </summary>
	public static HashSet<string> IgnoredProperties { get; } = new(new[] { "PassWord" }, StringComparer.OrdinalIgnoreCase);

	/// <summary> ignored Object Types </summary>
	public static HashSet<Type> IgnoredTypes { get; } = new ();

	/// <summary> Filters Log Values </summary>
	public bool TryDestructure(object? value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result) {
		if (value == null) {
			result = null!;
			return false; //delegate it to the next 
		}

		if (value is string str) {
			// Limit the string to a maximum length 
			var truncatedStr = (str.Length <= MaxLengthOfString) ? str
				: string.Concat(str.AsSpan(0, MaxLengthOfString).ToString(), "...");
			result = new ScalarValue(truncatedStr);
			return true;
		}

		if (value is Array array) { // Limit the array to a maximum number of elements 
			var properties = array.Cast<object>().Take(MaxLengthOfArray)
				.Select(x => propertyValueFactory.CreatePropertyValue(x)).ToList();
			result = new SequenceValue(properties);
			return true;
		}


		var props = new List<LogEventProperty>();

		foreach (var propertyInfo in value.GetType().GetProperties()) {
			if (IgnoredProperties.Contains(propertyInfo.Name)
			    || Attribute.IsDefined(propertyInfo, typeof(ExcludeFromLoggingAttribute))
				|| IgnoredTypes.Any(typ => typ.IsAssignableFrom(propertyInfo.PropertyType))) {
				continue; // Skip properties with the ExcludeFromLogging attribute
			}

			var propertyValue = propertyInfo.GetValue(value);
			var logEventProperty = new LogEventProperty(propertyInfo.Name
				, propertyValueFactory.CreatePropertyValue(propertyValue));
			props.Add(logEventProperty);
		}

		result = new StructureValue(props);
		return true;
	}
}
