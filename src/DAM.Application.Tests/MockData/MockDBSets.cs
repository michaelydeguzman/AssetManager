using DAM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Application.Tests.MockData
{
    public static class MockDBSets
    {
        public static List<Asset> AssetList()
        {
            return new List<Asset>
                            {
                                new Asset()
                                    {
                                        //FileName = "AssetOne",
                                        Id = 1
                                    }
                            };
        }
    }
}