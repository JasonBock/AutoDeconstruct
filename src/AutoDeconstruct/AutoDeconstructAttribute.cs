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
	/// <param name="search">
	/// Specifies if AutoDeconstruct should search for existing <c>Deconstruct()</c> extension methods. 
	/// The default is <see cref="SearchForExtensionMethods.No"/>.
	/// </param>
	public AutoDeconstructAttribute(SearchForExtensionMethods search = SearchForExtensionMethods.No) =>
		this.Search = search;

	/// <summary>
	/// Gets the extension method search value.
	/// </summary>
	public SearchForExtensionMethods Search { get; }
}