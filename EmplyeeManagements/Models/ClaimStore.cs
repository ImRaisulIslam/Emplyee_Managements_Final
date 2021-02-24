using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmplyeeManagements.Models
{
    public static class ClaimStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim ("Create Role","Create Role"),
             new Claim ("Delete Role", "Delete Role"),
              new Claim ("Update Role", "Update Role"),
              new Claim ("Edit Role", "Edit Role"),

        };
    }
}
