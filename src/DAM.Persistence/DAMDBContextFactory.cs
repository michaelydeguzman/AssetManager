using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAM.Persistence
{
    public class DAMDBContextFactory : DesignTimeDbContextFactoryBase<DAMDBContext>
    {
        protected override DAMDBContext CreateNewInstance(DbContextOptions<DAMDBContext> options)
        {
            return new DAMDBContext(options);
        }
    }
}
