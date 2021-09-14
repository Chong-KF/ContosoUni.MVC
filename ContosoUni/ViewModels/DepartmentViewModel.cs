using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using ContosoUni.Areas.Identity.Models;
using System.ComponentModel;
using ContosoUni.Models;

namespace ContosoUni.ViewModels
{
    public class DepartmentViewModel
    {
        [Required]
        [Display(Name = "Department ID")]
        public Guid DepartmentID { get; set; }

        [Required]
        [MinLength(5)]
        [StringLength(60)]
        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Required]
        [Display(Name = "Created by")]
        public Guid CreatedByUserId { get; set; }

        [Display(Name = "Updated by")]
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        [Display(Name = "Last updated on")]
        public DateTime LastUpdatedOn { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public MyIdentityUser CreatedByUser { get; set; }
        public MyIdentityUser UpdatedByUser { get; set; }

        public ICollection<Subject> Subjects { get; set; }
    }
}
