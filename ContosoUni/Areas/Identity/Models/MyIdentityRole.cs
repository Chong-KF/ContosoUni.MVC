using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUni.Areas.Identity.Models
{
    public class MyIdentityRole : IdentityRole<Guid>
    {
        [MaxLength(100)]    //constraint on the UI
        [StringLength(100)] //constraint on the DB Schema
        public string Description { get; set; }
    }
}
