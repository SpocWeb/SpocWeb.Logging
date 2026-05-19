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
public readonly struct Int<T> : IComparable<Int<T>>, IEquatable<Int<T>> {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
	public int Value { get; }

	public Int(int value) => Value = value;

	public static implicit operator Int<T>(int value) => new Int<T>(value);
	public static implicit operator int(Int<T> value) => value.Value;

	public static Int<T> operator +(Int<T> a, Int<T> b) => new Int<T>(a.Value + b.Value);
	public static Int<T> operator -(Int<T> a, Int<T> b) => new Int<T>(a.Value - b.Value);
	public static Int<T> operator *(Int<T> a, Int<T> b) => new Int<T>(a.Value * b.Value);
	public static Int<T> operator /(Int<T> a, Int<T> b) => new Int<T>(a.Value / b.Value);

	public static bool operator ==(Int<T> a, Int<T> b) => a.Value == b.Value;
	public static bool operator !=(Int<T> a, Int<T> b) => a.Value != b.Value;
	public static bool operator <(Int<T> a, Int<T> b) => a.Value < b.Value;
	public static bool operator >(Int<T> a, Int<T> b) => a.Value > b.Value;
	public static bool operator <=(Int<T> a, Int<T> b) => a.Value <= b.Value;
	public static bool operator >=(Int<T> a, Int<T> b) => a.Value >= b.Value;

	public int CompareTo(Int<T> other) => Value.CompareTo(other.Value);
	public bool Equals(Int<T> other) => Value == other.Value;
	public override bool Equals(object? obj) => obj is Int<T> other && Equals(other);
	public override int GetHashCode() => Value.GetHashCode();
	public override string ToString() => Value.ToString();
}