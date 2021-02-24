using EmplyeeManagements.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.ViewModels
{
    public class CreateEmployeViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dept Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
