using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.Models
{
    public static class ModelBuilderExtension
    {

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeModel>().HasData(
                new EmployeeModel
                {
                    Id = 1,
                    Email = "raisul@gmail.com",

                    Name = "Raiul",
                    Department = Dept.HR,
                },

                 new EmployeeModel
                 {
                     Id = 2,
                     Email = "raisul@gmail.com",

                     Name = "Raiul2",
                     Department = Dept.HR,
                 }

                );
        }
    }
}
 