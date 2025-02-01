namespace AutoDeconstruct;

/// <summary>
/// Allows the user to search for <c>Deconstruct()</c> extension methods.
/// By default, AutoDeconstruct will not do this search as it's not typical
/// for developers to create <c>Deconstruct()</c> extension methods.
/// </summary>
public enum SearchForExtensionMethods
{
	/// <summary>
	/// Do not search for <c>Deconstruct()</c> extension methods.
	/// </summary>
	No,
	/// <summary>
	/// Search for <c>Deconstruct()</c> extension methods.
	/// </summary>
	Yes
}
