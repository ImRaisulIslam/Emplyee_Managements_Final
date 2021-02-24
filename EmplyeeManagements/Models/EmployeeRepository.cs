using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmplyeeManagements.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public EmployeeModel GetEmployee(int Id)
        {
            return _context.Employees.Find(Id);



        }

        public IEnumerable<EmployeeModel> GetAllEmployee()
        {
            return _context.Employees;


        }

        public EmployeeModel AddEmployee(EmployeeModel addEmployee)
        {
            _context.Employees.Add(addEmployee);
            _context.SaveChanges();
            return addEmployee;


        }

        public EmployeeModel UpdateEmployee(EmployeeModel updateEmplyee)
        {
            var employee = _context.Employees.Attach(updateEmplyee);

            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return updateEmplyee;
        }

        public EmployeeModel DeleteEmployee(int Id)
        {
            EmployeeModel employee = _context.Employees.Find(Id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return employee;

        }
    }
}

