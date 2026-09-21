namespace TianzigeGenerator.Models;

/// <summary>
/// Types of pages that can be included in the PDF document.
/// </summary>
public enum PageType
{
    /// <summary>Tianzige grid page (田字格 - cross grid)</summary>
    TianzigeGrid,
    /// <summary>Mizige grid page (米字格 - rice grid with diagonals)</summary>
    MizigeGrid,
    /// <summary>Jiugongge grid page (九宫格 - 9-cell grid)</summary>
    JiugonggeGrid,
    /// <summary>Continuous manuscript grid for essays (作文格).</summary>
    EssayGrid,
    /// <summary>Grid paired with pinyin/translation lines.</summary>
    Vocabulary,
    /// <summary>Lined page for free writing practice</summary>
    Lined,
    /// <summary>Blank page</summary>
    Blank,
    /// <summary>Custom page layout (replaces Cover/Ending)</summary>
    Custom
}
