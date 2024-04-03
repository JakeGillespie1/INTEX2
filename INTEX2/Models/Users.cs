using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace INTEX2.Models
{
    public class Users : IdentityUser
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
