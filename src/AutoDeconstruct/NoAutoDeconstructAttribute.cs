namespace AutoDeconstruct;

/// <summary>
/// Prevents AutoDeconstruct from generating a <c>Deconstruct()</c> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = false, Inherited = false)]
public sealed class NoAutoDeconstructAttribute
	: Attribute
{ }