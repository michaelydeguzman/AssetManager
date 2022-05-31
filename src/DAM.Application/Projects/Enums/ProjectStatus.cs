using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAM.Application.Projects.Enums
{
    public enum ProjectStatus
    {
        [Description("Draft")]
        Draft,
        [Description("To Do")]
        ToDo,
        [Description("In Progress")]
        InProgress,
        [Description("Complete")]
        Complete,
        [Description("Archived")]
        Archived
    }
}
