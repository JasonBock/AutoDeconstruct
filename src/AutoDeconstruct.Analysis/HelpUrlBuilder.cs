namespace AutoDeconstruct.Analysis;

internal static class HelpUrlBuilder
{
	internal static string Build(string identifier) =>
		$"https://github.com/JasonBock/AutoDeconstruct/tree/main/docs/{identifier}.md";
}