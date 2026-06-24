namespace org.SpocWeb.root.logging;

/// <summary> Generically typed <see cref="Int32"/> </summary>
/// <remarks>
/// Simple way to create distinctly typed integer Values
///
/// ## Meta
/// pass: 2
/// mtime: 2026-03-06T12:14:31Z
/// digest: e7a003742c58af98acdca4f9428a9f0bc6558f0323f808fa56cee8cc2ff4dbe9
/// updated: 2026-05-19
/// </remarks>
/// <example>
/// <code language="yaml">
/// pass: 2
/// mtime: 2026-05-22T17:59:52Z
/// digest: 0a4ac7c3a97e22765b8ee28ee4baa9c8a3dac777bcfb9574fda918bf9657883c
/// </code>
/// </example>
//[org.SpocWeb.root.Attributes.Replaces("../../../../NET/_root/Abstracts/Int.cs")]
public readonly struct Int<T> : IComparable<Int<T>>, IEquatable<Int<T>> {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	/// <summary>Gets the value.</summary>
	public int Value { get; }

	/// <summary>Initializes a new instance of <see cref="Int32"/> with the specified <paramref name="value"/>.</summary>
	public Int(int value) => Value = value;

	/// <summary>Implicitly converts <paramref name="value"/> to <see cref="Int{T}"/>.</summary>
	public static implicit operator Int<T>(int value) => new Int<T>(value);
	/// <summary>Implicitly converts <paramref name="value"/> to <see cref="int"/>.</summary>
	public static implicit operator int(Int<T> value) => value.Value;

	/// <summary>Adds <paramref name="a"/> and <paramref name="b"/>.</summary>
	public static Int<T> operator +(Int<T> a, Int<T> b) => new Int<T>(a.Value + b.Value);
	/// <summary>Subtracts <paramref name="b"/> from <paramref name="a"/>.</summary>
	public static Int<T> operator -(Int<T> a, Int<T> b) => new Int<T>(a.Value - b.Value);
	/// <summary>Multiplies <paramref name="a"/> by <paramref name="b"/>.</summary>
	public static Int<T> operator *(Int<T> a, Int<T> b) => new Int<T>(a.Value * b.Value);
	/// <summary>Divides <paramref name="a"/> by <paramref name="b"/>.</summary>
	public static Int<T> operator /(Int<T> a, Int<T> b) => new Int<T>(a.Value / b.Value);

	/// <summary>Determines whether <paramref name="a"/> equals <paramref name="b"/>.</summary>
	public static bool operator ==(Int<T> a, Int<T> b) => a.Value == b.Value;
	/// <summary>Determines whether <paramref name="a"/> does not equal <paramref name="b"/>.</summary>
	public static bool operator !=(Int<T> a, Int<T> b) => a.Value != b.Value;
	/// <summary>Determines whether <paramref name="a"/> is less than <paramref name="b"/>.</summary>
	public static bool operator <(Int<T> a, Int<T> b) => a.Value < b.Value;
	/// <summary>Determines whether <paramref name="a"/> is greater than <paramref name="b"/>.</summary>
	public static bool operator >(Int<T> a, Int<T> b) => a.Value > b.Value;
	/// <summary>Determines whether <paramref name="a"/> is less than or equal to <paramref name="b"/>.</summary>
	public static bool operator <=(Int<T> a, Int<T> b) => a.Value <= b.Value;
	/// <summary>Determines whether <paramref name="a"/> is greater than or equal to <paramref name="b"/>.</summary>
	public static bool operator >=(Int<T> a, Int<T> b) => a.Value >= b.Value;

	/// <summary>TODO: LLM</summary>
 	public int CompareTo(Int<T> other) => Value.CompareTo(other.Value);
	/// <summary>TODO: LLM</summary>
 	public bool Equals(Int<T> other) => Value == other.Value;
	/// <inheritdoc cref="Equals(Int{T})"/>
	public override bool Equals(object? obj) => obj is Int<T> other && Equals(other);
	/// <inheritdoc />
	public override int GetHashCode() => Value.GetHashCode();
	/// <inheritdoc />
	public override string ToString() => Value.ToString();
}
