using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Projects.Dtos
{
    public class ProjectUpdateDetailsDto
    {
        public int ProjectId { get; set; }

        public string FieldName { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}
