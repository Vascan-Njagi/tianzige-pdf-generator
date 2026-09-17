namespace TianzigeGenerator.Models;

/// <summary>
/// Types of pages that can be included in the PDF document.
/// </summary>
public enum PageType
{
    /// <summary>Cover page with title, name field, date, etc.</summary>
    Cover,
    /// <summary>Tianzige grid page (田字格 - cross grid)</summary>
    TianzigeGrid,
    /// <summary>Mizige grid page (米字格 - rice grid with diagonals)</summary>
    MizigeGrid,
    /// <summary>Jiugongge grid page (九宫格 - 9-cell grid)</summary>
    JiugonggeGrid,
    /// <summary>Blank page</summary>
    Blank,
    /// <summary>Lined page for free writing practice</summary>
    Lined,
    /// <summary>Ending / back cover page</summary>
    Ending
}
