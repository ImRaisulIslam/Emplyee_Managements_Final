using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.ViewModels
{
    public class EditEmployeeViewModel : CreateEmployeViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
