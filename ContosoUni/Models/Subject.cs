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
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Comment("The Unique ID of the Subject")]
        public Guid SubjectID { get; set; }

        [Required]
        [StringLength(60)]
        [Column(TypeName = "nvarchar")]
        [Comment("The Unique ID of the Subject")]
        public string SubjectName { get; set; }

        [Required]
        [Comment("the Grade of the subject")]
        public short? Grade { get; set; }

        [Required]
        [ForeignKey(nameof(Subject.Departments))]
        [JsonIgnore]
        public Guid DepartmentID { get; set; }

        [Required]
        [JsonIgnore]
        public Guid CreatedByUserId { get; set; }

        [JsonIgnore]
        public Guid? UpdatedByUserId { get; set; }

        [Required]
        [JsonIgnore]
        public DateTime LastUpdatedOn { get; set; }

        [Required]
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public Department Departments { get; set; }
        [JsonIgnore]
        public MyIdentityUser CreatedByUser { get; set; }
        [JsonIgnore]
        public MyIdentityUser UpdatedByUser { get; set; }
    }
}
