namespace Moe.Core.Models.DTOs.LocalizedContent;

public class LocalizedContentDTO
{
    public string? En { get; set; }
    public string? Ar { get; set; }
    public string? Ku { get; set; }

    public string? GetLocalized(string? lang) => lang switch
    {
        "ar" => Ar ?? En,
        "ku" => Ku ?? En,
        _ => En
    };
}

public class LocalizedContentFormDTO
{
    public string? En { get; set; }
    public string? Ar { get; set; }
    public string? Ku { get; set; }
}

public class LocalizedContentUpdateDTO
{
     public string? En { get; set; }
     public string? Ar { get; set; } 
     public string? Ku { get; set; }   
}
