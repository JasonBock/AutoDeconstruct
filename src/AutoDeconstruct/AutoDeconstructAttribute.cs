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
	public AutoDeconstructAttribute()
		: this(Filtering.None, []) { }

	/// <summary>
	/// Creates a new <see cref="AutoDeconstructAttribute"/> instance,
	/// specifying filtered properties.
	/// </summary>
	/// <param name="filtering">The filter setting.</param>
	/// <param name="properties">The filtered properties, specified by name.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="properties"/> is <see langword="null" />.</exception>
	/// <exception cref="ArgumentException">
	/// Thrown if <paramref name="filtering"/> is <see cref="Filtering.None"/> and values exist in <paramref name="properties"/>,
	/// or filtering is enabled and no values exist in <paramref name="properties"/>.
	/// </exception>
	public AutoDeconstructAttribute(Filtering filtering, string[] properties)
	{
		this.Validate(filtering, properties);
		(this.Filtering, this.Properties) = (filtering, properties);
	}

	private void Validate(Filtering filtering, string[] properties)
	{
		if (properties is null) { throw new ArgumentNullException(nameof(properties)); }

		if (filtering == Filtering.None && properties.Length > 0)
		{
			throw new ArgumentException("Properties should not be provided if no filtering occurs.", nameof(properties));
		}
		else if (properties.Length == 0)
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
}