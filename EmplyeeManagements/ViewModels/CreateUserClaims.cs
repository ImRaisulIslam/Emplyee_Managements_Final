using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmplyeeManagements.ViewModels
{
    public class CreateUserClaims
    {
        
        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }
}
