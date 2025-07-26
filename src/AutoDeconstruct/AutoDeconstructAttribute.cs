namespace AutoDeconstruct;

/// <summary>
/// Instructs AutoDeconstruct to generate a <c>Deconstruct()</c> implementation.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
	AllowMultiple = false, Inherited = false)]
public sealed class AutoDeconstructAttribute
	: Attribute
{
	/// <summary>
	/// Creates a new <see cref="AutoDeconstructAttribute"/> instance.
	/// </summary>
	/// <param name="targetType">
	/// Specifies the type to add a <c>Deconstruct()</c> method for.
	/// </param>
	/// <param name="search">
	/// Specifies if AutoDeconstruct should search for existing <c>Deconstruct()</c> extension methods. 
	/// The default is <see cref="SearchForExtensionMethods.No"/>.
	/// </param>
	/// <remarks>
	/// <paramref name="targetType"/> is ignored if <see cref="AutoDeconstructAttribute"/> is declared for a type.
	/// If <see cref="AutoDeconstructAttribute"/> is declared at the assembly level, then <paramref name="targetType"/>
	/// must not be <see langword="null" />.
	/// </remarks>
	public AutoDeconstructAttribute(Type? targetType = null, SearchForExtensionMethods search = SearchForExtensionMethods.No) =>
		this.Search = search;

	/// <summary>
	/// Gets the extension method search value.
	/// </summary>
	public SearchForExtensionMethods Search { get; }

	/// <summary>
	/// Gets the target type for auto-deconstruction.
	/// </summary>
	public Type? TargetType { get; }
}