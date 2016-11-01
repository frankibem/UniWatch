using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UniWatch.Models
{
    public class AppDbInitializer : DropCreateDatabaseIfModelChanges<AppDbContext>
    {
    }
}