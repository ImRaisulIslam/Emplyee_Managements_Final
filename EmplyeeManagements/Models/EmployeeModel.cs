using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public Dept Department { get; set; }
        public string PhotoPath { get; set; }

       

    }
}
