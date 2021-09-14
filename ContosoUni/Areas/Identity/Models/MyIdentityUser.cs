using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using ContosoUni.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace ContosoUni.Areas.Identity.Models
{
    public class MyIdentityUser : IdentityUser<Guid>
    {
        [Required] //NOT NULL constraint for DB Schema
        [Display(Name = "Display Name")]  //Constraint for Razor UI Label
        [MinLength(2, ErrorMessage = "Name must have more then 2 characters")]
        [MaxLength(60, ErrorMessage = "Name must not more then 60 characters")]
        [StringLength(60)]  //constraint for DB Schema
        public string DisplayName { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [ForeignKey(nameof(Department.CreatedByUserId))]
        public ICollection<Department> DepartmentsCreatedByUser { get; set; }
        [ForeignKey(nameof(Department.UpdatedByUserId))]
        public ICollection<Department> DepartmentsUpdatedByUser { get; set; }

        [ForeignKey(nameof(Subject.CreatedByUserId))]
        public ICollection<Subject> SubjectsCreatedByUser { get; set; }
        [ForeignKey(nameof(Subject.UpdatedByUserId))]
        public ICollection<Subject> SubjectsUpdatedByUser { get; set; }
    }
}
