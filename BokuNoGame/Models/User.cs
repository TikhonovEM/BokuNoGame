using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BokuNoGame.Models
{
    public class User : IdentityUser
    {
        public string Login { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
