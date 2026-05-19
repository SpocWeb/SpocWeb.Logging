namespace org.SpocWeb.root.logging.SeriLog;

/// <summary> Flag to suppress logging this Property or Class </summary>
/// <remarks>
/// ## Meta
/// pass: 2
/// mtime: 2025-05-02T17:50:18Z
/// digest: a0abcb7a602f933d81f71ef7fcb71b7e0559850e350427f6839c82827a93ffe8
/// updated: 2026-05-19
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class)]
public class ExcludeFromLoggingAttribute : Attribute;