using System.Text.RegularExpressions;

namespace Moe.Core.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // Replace spaces and underscores with hyphens
        string kebabCase = Regex.Replace(input, @"[\s_]+", "-");

        // Replace CamelCase with kebab-case
        kebabCase = Regex.Replace(kebabCase, @"([a-z0-9])([A-Z])", "$1-$2");

        // Convert to lowercase
        kebabCase = kebabCase.ToLower();

        return kebabCase;
    }

    public static string ToLocalizedUsername(this string input) =>
        input.Replace(" ", string.Empty);
}