using System;
using System.Collections.Generic;

namespace DAM.Application.Projects.Dtos
{
    public class ImportAssetsToProjectDto
    {
        public List<int> AssetIds { get; set; }

        public List<int> ProjectIds { get; set; }
    }
}
