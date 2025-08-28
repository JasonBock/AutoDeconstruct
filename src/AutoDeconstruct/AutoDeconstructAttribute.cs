namespace AutoDeconstruct;

/// <summary>
/// Instructs AutoDeconstruct to generate a <c>Deconstruct()</c> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = false, Inherited = false)]
public sealed class AutoDeconstructAttribute
	: Attribute
{
	/// <summary>
	/// Creates a new <see cref="AutoDeconstructAttribute"/> instance.
	/// </summary>
	public AutoDeconstructAttribute() { }
}