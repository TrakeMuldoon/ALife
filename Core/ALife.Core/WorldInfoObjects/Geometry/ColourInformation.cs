using ALife.Core.WorldInfoObjects.Colours;

namespace ALife.Core.WorldInfoObjects.Geometry;

/// <summary>
/// Information about the colours of an object.
/// </summary>
public class ColourInformation
{
    /// <summary>
    /// The fill colour.
    /// </summary>
    public Colour FillColour;

    /// <summary>
    /// The outline colour.
    /// </summary>
    public Colour OutlineColour;

    /// <summary>
    /// The debug fill colour.
    /// </summary>
    public Colour DebugFillColour;
    
    /// <summary>
    /// The debug outline colour.
    /// </summary>
    public Colour DebugOutlineColour;
}
