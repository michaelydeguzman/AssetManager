using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAM.Application.Watermarks.Enums
{
    public enum WatermarkPosition
    {
        [Description("Top Left")]
        TopLeft = 1,
        [Description("Top Right")]
        TopRight,
        [Description("Center")]
        Center,
        [Description("Bottom Left")]
        BottomLeft,
        [Description("Bottom Right")]
        BottomRight
    }
}
