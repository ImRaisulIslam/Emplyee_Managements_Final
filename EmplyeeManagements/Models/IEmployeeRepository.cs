using System.Collections.Generic;

namespace EmplyeeManagements.Models
{
    public interface IEmployeeRepository
    {
        EmployeeModel AddEmployee(EmployeeModel addEmployee);
        EmployeeModel DeleteEmployee(int Id);
        IEnumerable<EmployeeModel> GetAllEmployee();
        EmployeeModel GetEmployee(int Id);
        EmployeeModel UpdateEmployee(EmployeeModel updateEmplyee);
    }
}