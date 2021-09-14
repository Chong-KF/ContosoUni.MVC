using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

using ContosoUni.Areas.Identity.Models;
using ContosoUni.Models;

namespace ContosoUni.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<MyIdentityUser, MyIdentityRole, Guid>
        //Guid both myIdentity class inherit from guid
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Subject> Subject { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Department>()
                .Property(e => e.LastUpdatedOn)
                .HasDefaultValueSql("getdate()");

            builder.Entity<Department>()
                .Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Department>()                        //child table
                .HasOne<MyIdentityUser>(c => c.CreatedByUser)   //object of parent in Child
                .WithMany(p => p.DepartmentsCreatedByUser)      //collection of children in parent  
                .HasForeignKey(c => c.CreatedByUserId)          //column of child on which FK is established
                .OnDelete(DeleteBehavior.Restrict);             //CASCADE DELETE Behaviour

            builder.Entity<Department>()                        //child table
                .HasOne<MyIdentityUser>(c => c.UpdatedByUser)   //object of parent in Child
                .WithMany(p => p.DepartmentsUpdatedByUser)      //collection of children in parent  
                .HasForeignKey(c => c.UpdatedByUserId)          //column of child on which FK is established
                .OnDelete(DeleteBehavior.Restrict);             //CASCADE DELETE Behaviour

            builder.Entity<Subject>()
                .Property(e => e.LastUpdatedOn)
                .HasDefaultValueSql("getdate()");

            builder.Entity<Subject>()
                .Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            builder.Entity<Subject>()                           //child table
                .HasOne<MyIdentityUser>(c => c.CreatedByUser)   //object of parent in Child
                .WithMany(p => p.SubjectsCreatedByUser)         //collection of children in parent  
                .HasForeignKey(c => c.CreatedByUserId)          //column of child on which FK is established
                .OnDelete(DeleteBehavior.Restrict);             //CASCADE DELETE Behaviour

            builder.Entity<Subject>()                           //child table
                .HasOne<MyIdentityUser>(c => c.UpdatedByUser)   //object of parent in Child
                .WithMany(p => p.SubjectsUpdatedByUser)         //collection of children in parent  
                .HasForeignKey(c => c.UpdatedByUserId)          //column of child on which FK is established
                .OnDelete(DeleteBehavior.Restrict);             //CASCADE DELETE Behaviour

            builder.Entity<Subject>()                           //child table
                .HasOne<Department>(c => c.Departments)         //object of parent in Child
                .WithMany(p => p.Subjects)                      //collection of children in parent  
                .HasForeignKey(c => c.DepartmentID)          //column of child on which FK is established
                .OnDelete(DeleteBehavior.Restrict);             //CASCADE DELETE Behaviour

            base.OnModelCreating(builder);
        }

        
    }
}
