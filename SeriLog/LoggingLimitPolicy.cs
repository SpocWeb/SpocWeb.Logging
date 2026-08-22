using Serilog.Core;
using Serilog.Events;
using System.ComponentModel;

namespace org.SpocWeb.root.logging.SeriLog;

/// <summary> Serilog <see cref="IDestructuringPolicy"/> forms a Chain of Responsibility for serializing Values </summary>
/// <remarks>
/// By default, Serilog invokes the <see cref="object.ToString"/> Method on each Object.
/// To force destructuring, prepend the `@` to the Log-Property Name.
///
/// Developers can use Serilog's [Destructure] attribute to mark classes or properties
/// for finer control over how data is broken down into logs.  
/// 
/// To register this Policy in the Chain, use this Code:
///
/// ## Meta
/// pass: 2
/// mtime: 2026-03-06T09:42:35Z
/// digest: b74a830f176cf637f66813edf9dcea72b6d085643d39dda456cb3cfc43c8bf0f
/// updated: 2026-05-19
/// </remarks>
/// <example>
/// <code language="yaml">
/// pass: 2
/// mtime: 2026-08-22T17:15:51Z
/// digest: 5afdd2e10f6c9a25f6685ea0af707157a09e273b39f5899275e27c060c8ef73f
/// </code>
/// </example>
/// <code lang='cs'>
/// Log.Logger = new LoggerConfiguration()
///		.Destructure.With{ExcludePropertiesPolicy}()
///		.CreateLogger();
/// </code>
public class LoggingLimitPolicy : IDestructuringPolicy
{
	/// <summary> Limit String Length to reduce Log Size </summary>
	[System.ComponentModel.Description("Limit String Length to reduce Log Size")]
	public static int MaxLengthOfString { get; set; } = 100;

	/// <summary> Limit Array Length to reduce Log Size </summary>
	[System.ComponentModel.Description("Limit Array Length to reduce Log Size")]
	public static int MaxLengthOfArray { get; set; } = 10;

	/// <summary> Names of ignored Properties </summary>
	[System.ComponentModel.Description("Names of ignored Properties")]
	public static HashSet<string> IgnoredProperties { get; } = new(new[] { "PassWord" }, StringComparer.OrdinalIgnoreCase);

	/// <summary> ignored Object Types </summary>
	[System.ComponentModel.Description("ignored Object Types")]
	public static HashSet<Type> IgnoredTypes { get; } = new ();

	/// <summary> Filters Log Values </summary>
	[System.ComponentModel.Description("Filters Log Values")]
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
