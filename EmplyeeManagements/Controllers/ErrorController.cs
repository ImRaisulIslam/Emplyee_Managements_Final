using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmplyeeManagements.Controllers
{
    public class ErrorController : Controller
    {

        private readonly ILogger<ErrorController> logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }

        [AllowAnonymous]
       
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            logger.LogError($"The path {exceptionHandlerPathFeature.Path} " +
                $"threw an exception {exceptionHandlerPathFeature.Error}");

            return View("Error");
        }




        [Route("Error/{StatusCode}")]
        public IActionResult HttpCodeHandler(int statusCode)
        {
            switch (statusCode)
            {

                case 404:
                    ViewBag.ErrorMessage = "Sorry Your Request did not Found";
                        break;

                
                default:
                    break;
            }
            return View("ErrorPageShowMessage");
        }
    }
}