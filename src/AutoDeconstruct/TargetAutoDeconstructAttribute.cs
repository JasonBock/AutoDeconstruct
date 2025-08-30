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
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="targetType"/> is <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="targetType"/> is a closed generic.
	/// </exception>
	public TargetAutoDeconstructAttribute(Type targetType)
		: this(targetType, Filtering.None, []) { }

	/// <summary>
	/// Creates a new <see cref="TargetAutoDeconstructAttribute"/> instance,
	/// specifying filtered properties.
	/// </summary>
	/// <param name="targetType">
	/// Specifies the type to add a <c>Deconstruct()</c> method for.
	/// </param>
	/// <param name="filtering">The filter setting.</param>
	/// <param name="properties">The filtered properties, specified by name.</param>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="targetType"/> is <see langword="null" />.
	/// </exception>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="targetType"/> is a closed generic.
	/// </exception>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="properties"/> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="filtering"/> is <see cref="Filtering.None"/> and values exist in <paramref name="properties"/>,
	/// or filtering is enabled and no values exist in <paramref name="properties"/>.
	/// </exception>
	public TargetAutoDeconstructAttribute(Type targetType, Filtering filtering, string[] properties)
	{
		this.Validate(targetType, filtering, properties);
		(this.TargetType, this.Filtering, this.Properties) = (targetType, filtering, properties);
	}

	private void Validate(Type targetType, Filtering filtering, string[] properties)
	{
		if (targetType is null) { throw new ArgumentNullException(nameof(targetType)); }

		if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
		{
			throw new ArgumentException("Cannot provide a closed generic type.", nameof(targetType));
		}

		if (properties is null) { throw new ArgumentNullException(nameof(properties)); }

		if (filtering == Filtering.None && properties.Length > 0)
		{
			throw new ArgumentException("Properties should not be provided if no filtering occurs.", nameof(properties));
		}
		else if (filtering != Filtering.None && properties.Length == 0)
		{
			throw new ArgumentException("Properties must be provided if filtering occurs.", nameof(properties));
		}
	}

	/// <summary>
	/// Gets the filter setting.
	/// </summary>
	public Filtering Filtering { get; private set; }

	/// <summary>
	/// Gets the filtered properties, if any were specified.
	/// </summary>
	public string[] Properties { get; private set; }

	/// <summary>
	/// Gets the target type for auto-deconstruction.
	/// </summary>
	public Type TargetType { get; }
}