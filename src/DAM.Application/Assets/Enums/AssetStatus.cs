using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAM.Application.Assets.Enums
{
    public enum AssetStatus
    {
        [Description("Draft")]
        Draft,
        [Description("Archived")]
        Archived,
        [Description("Deleted")]
        Deleted,
        [Description("Submitted For Review")]
        Submitted,
        [Description("Approved")]
        Approved,
        [Description("Rejected")]
        Rejected
    }
}
