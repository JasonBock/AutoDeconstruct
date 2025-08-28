namespace AutoDeconstruct;

/// <summary>
/// Used to denote if named properties should be included or excluded.
/// </summary>
public enum Filtering
{
	/// <summary>
	/// No filtering is done.
	/// </summary>
	None,
	/// <summary>
	/// Only the specified properties are included.
	/// </summary>
	Include,
	/// <summary>
	/// The specified properties are excluded.
	/// </summary>
	Exclude
}