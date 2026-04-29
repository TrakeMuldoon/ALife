using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry
{
    public struct ShapeColouration
    {
        public Colour OutlineColour;
        public double OutlineWidth;
        public Colour FillColour;

        public Colour DebugOutlineColour;
        public double DebugOutlineWidth;
        public Colour DebugFillColour;
        
        public void SetFillColour(Colour colour)
        {
            FillColour = colour;
        }
        
        public void SetOutlineColour(Colour colour)
        {
            OutlineColour = colour;
        }
        
        public void SetDebugFillColour(Colour colour)
        {
            DebugFillColour = colour;
        }
        
        public void SetDebugOutlineColour(Colour colour)
        {
            DebugOutlineColour = colour;
        }
        
        public void SetOutlineWidth(double width)
        {
            OutlineWidth = width;
        }
        
        public void SetDebugOutlineWidth(double width)
        {
            DebugOutlineWidth = width;
        }
        
        public ShapeColouration(Colour outlineColour, double outlineWidth, Colour fillColour, Colour debugOutlineColour, double debugOutlineWidth, Colour debugFillColour)
        {
            OutlineColour = outlineColour;
            OutlineWidth = outlineWidth;
            FillColour = fillColour;
            DebugOutlineColour = debugOutlineColour;
            DebugOutlineWidth = debugOutlineWidth;
            DebugFillColour = debugFillColour;
        }
        
        public ShapeColouration(Colour outlineColour, double outlineWidth, Colour fillColour)
        {
            OutlineColour = outlineColour;
            OutlineWidth = outlineWidth;
            FillColour = fillColour;
            DebugOutlineColour = outlineColour;
            DebugOutlineWidth = outlineWidth;
            DebugFillColour = fillColour;
        }
        
        public ShapeColouration(Colour outlineColour, double outlineWidth)
        {
            OutlineColour = outlineColour;
            OutlineWidth = outlineWidth;
            FillColour = outlineColour;
            DebugOutlineColour = outlineColour;
            DebugOutlineWidth = outlineWidth;
            DebugFillColour = outlineColour;
        }
    }
}