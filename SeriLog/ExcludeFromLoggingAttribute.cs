namespace org.SpocWeb.root.logging.SeriLog;

/// <summary> Flag to suppress logging this Property or Class </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class)]
public class ExcludeFromLoggingAttribute : Attribute;