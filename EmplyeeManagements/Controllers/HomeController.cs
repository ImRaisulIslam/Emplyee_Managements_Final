using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EmplyeeManagements.Models;
using EmplyeeManagements.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace EmplyeeManagements.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
        }






        [AllowAnonymous]
        public IActionResult Index()
        {
            var data = _employeeRepository.GetAllEmployee();
            return View(data);
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        public ViewResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create ( CreateEmployeViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniquePath = null;
                if (model.Photo != null)
                {
                    fileUploadProcess(model);
                }

                EmployeeModel employeeModel = new EmployeeModel
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniquePath,

                };

                _employeeRepository.AddEmployee(employeeModel);

               
            }
            ModelState.Clear();
            return View();
        }

        [AllowAnonymous]
        public IActionResult Detail(int? id)

        {

           
            EmployeeModel employee = _employeeRepository.GetEmployee(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("PageNotFound",id.Value);
            }
        
            return View(employee);
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            EmployeeModel employeeModel = _employeeRepository.GetEmployee(id);
            EditEmployeeViewModel editEmployeeViewModel = new EditEmployeeViewModel
            {
                Id = employeeModel.Id,
                Name = employeeModel.Name,
                Email = employeeModel.Email,
                Department =employeeModel.Department,
                ExistingPhotoPath =employeeModel.PhotoPath,
            };
            return View(editEmployeeViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                EmployeeModel employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Department = model.Department;
                employee.Email = model.Email;
               
                if (model.Photo != null)
                {
                    if(model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                            "images/", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = fileUploadProcess(model);
                }

               

             EmployeeModel updateEmployee =   _employeeRepository.UpdateEmployee(employee);

                return RedirectToAction("Index");
            }
           
            return View(model);
        }

        private string fileUploadProcess(CreateEmployeViewModel model)
        {
            string uniquePath;
            string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/");
            uniquePath = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadFolder, uniquePath);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Photo.CopyTo(fileStream);
            }
           
            return uniquePath;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
