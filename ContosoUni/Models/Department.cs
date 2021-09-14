using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ContosoUni.Areas.Identity.Models;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ContosoUni.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("The Unique ID of the Department")]
        public Guid DepartmentID { get; set; }

        [Required]
        [StringLength(60)]
        [Column(TypeName = "nvarchar")]
        [Comment("The Unique ID of the Department")]
        public string DepartmentName { get; set; }

        [Required]
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }

        [JsonIgnore]
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        [JsonIgnore]
        public DateTime LastUpdatedOn { get; set; }

        [Required]
        [DefaultValue(false)]
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public MyIdentityUser CreatedByUser { get; set; }

        [JsonIgnore]
        public MyIdentityUser UpdatedByUser { get; set; }

        [ForeignKey(nameof(Subject.DepartmentID))]
        public ICollection<Subject> Subjects { get; set; }
    }
}
