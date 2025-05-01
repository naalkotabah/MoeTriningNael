using Moe.Core.Models.Entities;

namespace Moe.Core.Extensions;

public static class LangExtensions
{
    public static string ToNameString(this Lang lang)
    {
        return lang switch
        {
            Lang.EN => "en",
            Lang.AR => "ar",
            Lang.KU => "ku",
        };
    }
}