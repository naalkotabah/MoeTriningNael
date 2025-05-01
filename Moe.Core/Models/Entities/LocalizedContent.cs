namespace Moe.Core.Models.Entities;

public class LocalizedContent : BaseEntity
{
    public LocalizedContent()
    { }
    public LocalizedContent(string? en = null, string? ar = null, string? ku = null)
    {
        En = en;
        Ar = ar;
        Ku = ku;
    }
    
    
    public string? En { get; set; }
    public string? Ar { get; set; }
    public string? Ku { get; set; }
    

    public string? GetLocalized(Lang lang) =>
        lang switch
        {
            Lang.AR => Ar ?? En,
            Lang.KU => Ku ?? En,
            _ => En
        };
}

public enum Lang
{
    EN,
    AR,
    KU
}