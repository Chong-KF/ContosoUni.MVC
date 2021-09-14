using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using ContosoUni.Areas.Identity.Models;
using ContosoUni.Models;

namespace ContosoUni.ViewModels
{
    public class SubjectViewModel
    {
        [Required]
        [Display(Name = "Subject ID")]
        public Guid SubjectID { get; set; }

        [Required]
        [MinLength(5)]
        [StringLength(60)]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Required]
        [Range(1, 9)]
        [Display(Name = "Grade")]
        public short? Grade { get; set; }

        [Required]
        [Display(Name = "Department")]
        public Guid DepartmentID { get; set; }

        [Required]
        [Display(Name = "Created by")]
        public Guid CreatedByUserId { get; set; }

        [Display(Name = "Updated by")]
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        [Display(Name = "Last updated on")]
        public DateTime LastUpdatedOn { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        public Department Departments { get; set; }
        public MyIdentityUser CreatedByUser { get; set; }
        public MyIdentityUser UpdatedByUser { get; set; }
    }
}
