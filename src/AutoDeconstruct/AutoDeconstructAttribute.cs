namespace AutoDeconstruct;

/// <summary>
/// Instructs AutoDeconstruct to generate a <c>Deconstruct()</c> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = false, Inherited = false)]
public sealed class AutoDeconstructAttribute
	: Attribute
{ }