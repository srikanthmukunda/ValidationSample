using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ValidationSample.Models
{
    public class Admin
    {
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter user Id")]
        public string UserId { get; set; }
        
        public string UserName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter password")]
        public string Password { get; set; }
    }
}