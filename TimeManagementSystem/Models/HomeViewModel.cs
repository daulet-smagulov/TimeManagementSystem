using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeManagementSystem.Models
{
    public class HomeViewModel
    {
        public ApplicationUser CurrentUser { get; set; }
        public bool HasSubUsers { get; set; }
    }
}