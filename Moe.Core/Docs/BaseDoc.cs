using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Moe.Core.Docs;

public abstract class BaseDoc : IDocument
{
    public int H1 { get; set; } = 24;
    public int H2 { get; set; } = 20;
    public int H3 { get; set; } = 16;
    public int H4 { get; set; } = 12;
    
    public int P1 { get; set; } = 12;
    //public int P2 { get; set; } = 16;

    public int PaddingL1 { get; set; } = 20;
    public int PaddingL2 { get; set; } = 16;
    public int PaddingL3 { get; set; } = 12;
    public int PaddingL4 { get; set; } = 8;
    public int PaddingL5 { get; set; } = 4;
    public int PaddingL6 { get; set; } = 2;
    
    public Color C1 { get; set; } = Colors.Grey.Lighten2;
    public Color C2 { get; set; } = Colors.Grey.Lighten1;
    public Color C3 { get; set; } = Colors.Grey.Darken2;
    public Color C4 { get; set; } = Colors.Grey.Darken4;
    public Color CBoarder { get; set; } = Colors.Black;
    public Color CBackground { get; set; } = Colors.White;

    public int MarginL { get; set; } = 2;
    public int BoarderL { get; set; } = 2;
    public int HeaderHeight { get; set; } = 150;
    public int FooterHeight { get; set; } = 70;

    internal abstract TextStyle TextStyle { get; set; }

    public abstract void Compose(IDocumentContainer container);
}
