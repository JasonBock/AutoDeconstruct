using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoDeconstruct.Extensions;

internal static class StringExtensions
{
	internal static string GenerateFileName(this string self) =>
		self.Replace("global::", string.Empty)
			.Replace(":", string.Empty)
			.Replace("<", string.Empty)
			.Replace(">", string.Empty)
			.Replace("?", "_null_")
			.Replace("*", "Pointer");

	// This code came from Humanize:
	// https://github.com/Humanizr/Humanizer/blob/7492f69c25be62c3be8cd435d9ccaa95a2ef20e9/src/Humanizer/InflectorExtensions.cs
	// Trying to reference the package in the source generator
	// just wasn't working, and the implementation is pretty small
	// so I basically copied it here.
	// Giving credit where credit is due.
	// Note that we can't use [GeneratedRegex] here, because that's
	// .NET 7+, and we can't access that in NS 2.0.
	internal static string ToCamelCase(this string self)
	{
		var word = Regex.Replace(self, "(?:^|_| +)(.)", match => match.Groups[1].Value.ToUpper(CultureInfo.CurrentCulture));
		return word.Length > 0 ? word.Substring(0, 1).ToLower(CultureInfo.CurrentCulture) + word.Substring(1) : word;
	}
}