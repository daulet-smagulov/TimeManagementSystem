using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeManagementSystem.Models
{
    // Models returned by MeController actions.
    public class MeViewModel
    {
        public string FullName { get; set; }
    }
}