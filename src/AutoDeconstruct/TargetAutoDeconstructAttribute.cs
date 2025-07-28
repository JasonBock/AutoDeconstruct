namespace AutoDeconstruct;

/// <summary>
/// Instructs AutoDeconstruct to generate a <c>Deconstruct()</c> implementation for the given type.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly,
	AllowMultiple = true, Inherited = false)]
public sealed class TargetAutoDeconstructAttribute
	: Attribute
{
	/// <summary>
	/// Creates a new <see cref="TargetAutoDeconstructAttribute"/> instance.
	/// </summary>
	/// <param name="targetType">
	/// Specifies the type to add a <c>Deconstruct()</c> method for.
	/// </param>
	/// <param name="search">
	/// Specifies if AutoDeconstruct should search for existing <c>Deconstruct()</c> extension methods. 
	/// The default is <see cref="SearchForExtensionMethods.No"/>.
	/// </param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="targetType"/> is <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="targetType"/> is a closed generic.
	/// </exception>
	public TargetAutoDeconstructAttribute(Type targetType, SearchForExtensionMethods search = SearchForExtensionMethods.No)
	{
		if (targetType is null) { throw new ArgumentNullException(nameof(targetType)); }

		if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
		{
			throw new ArgumentException("Cannot provide a closed generic type.", nameof(targetType));
		}

		this.TargetType = targetType;
		this.Search = search;
	}

	/// <summary>
	/// Gets the extension method search value.
	/// </summary>
	public SearchForExtensionMethods Search { get; }

	/// <summary>
	/// Gets the target type for auto-deconstruction.
	/// </summary>
	public Type TargetType { get; }
}