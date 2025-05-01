using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using Moe.Core.Models.Entities;

namespace Moe.Core.Translations
{
    public static class Localizer
    {
        private static readonly ResourceManager ResourceManager;

        static Localizer()
        {
            ResourceManager = new ResourceManager("elixir.core.Translations.Translation", Assembly.GetExecutingAssembly());
        }

        public static string Translate(string key, string culture = null, params string[] args)
        {
            if (string.IsNullOrEmpty(key))
            {
                return "UNKNOWN_TRANSLATION"; // Avoid passing null to ExtractAbsoluteWords()
            }

            // Extract absolute words (e.g., [[word]])
            var absoluteWords = ExtractAbsoluteWords(key, out string keyWithoutAbsoluteWords);

            // Determine the culture
            var ci = string.IsNullOrEmpty(culture) ? CultureInfo.CurrentCulture : new CultureInfo(culture);

            // Retrieve the translation from the resource file
            var translated = ResourceManager.GetString(keyWithoutAbsoluteWords, ci) ?? keyWithoutAbsoluteWords;
            translated = String.IsNullOrEmpty(translated)
                ? key
                : translated;

            // Replace placeholders for absolute words in the translation
            translated = RestoreAbsoluteWords(translated, absoluteWords);

            translated = translated
                .Replace("[", "")
                .Replace("]", "");

            // Format the translated string with additional arguments, if any
            return args.Length > 0 ? string.Format(translated, args) : translated;
        }

        private static Dictionary<string, string> ExtractAbsoluteWords(string input, out string sanitizedInput)
        {
            var matches = Regex.Matches(input, @"\[\[(.+?)\]\]");
            var absoluteWords = new Dictionary<string, string>();
            sanitizedInput = input;

            int index = 0;
            foreach (Match match in matches)
            {
                var placeholder = $"{{ABSOLUTE_{index}}}";
                absoluteWords[placeholder] = match.Groups[1].Value; // Extract text inside [[...]]
                sanitizedInput = sanitizedInput.Replace(match.Value, placeholder); // Replace with placeholder
                index++;
            }

            return absoluteWords;
        }

        private static string RestoreAbsoluteWords(string input, Dictionary<string, string> absoluteWords)
        {
            foreach (var kvp in absoluteWords)
            {
                input = input.Replace(kvp.Key, kvp.Value); // Replace placeholder with original word
            }

            return input;
        }

        public static void Translate_Test()
        {
            var a1 = Translate("There's a new order by [[Mohammed]] {0} {1}", null, "Hello", "World");
            var a2 = Translate("There's a new order by [[Mohammed]]", "ar");
            Console.WriteLine(a1); // Expected Output: اسمك هو Mohammed
            Console.WriteLine(a2); // Expected Output: اسمك هو John وعمرك هو 18
        }

        public static string? Translate(Lang? lang, string? nameEn, string? nameAr, string? nameKu)
        {
            return lang switch
            {
                Lang.AR => nameAr ?? nameEn, // Default to English if Arabic is null
                Lang.KU => nameKu ?? nameEn, // Default to English if Kurdish is null
                _ => nameEn ?? nameAr ?? nameKu // Default to English
            };
        }

    }
}