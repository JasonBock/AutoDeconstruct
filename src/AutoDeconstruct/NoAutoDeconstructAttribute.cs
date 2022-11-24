namespace AutoDeconstruct;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = false, Inherited = false)]
public sealed class NoAutoDeconstructAttribute
	: Attribute
{ }