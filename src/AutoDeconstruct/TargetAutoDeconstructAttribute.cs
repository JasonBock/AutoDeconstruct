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
	{
		if (targetType is null) { throw new ArgumentNullException(nameof(targetType)); }

		if (targetType.IsGenericType && !targetType.IsGenericTypeDefinition)
		{
			throw new ArgumentException("Cannot provide a closed generic type.", nameof(targetType));
		}

		this.TargetType = targetType;
	}

	/// <summary>
	/// Gets the target type for auto-deconstruction.
	/// </summary>
	public Type TargetType { get; }
}