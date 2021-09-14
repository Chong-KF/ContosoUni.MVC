using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUni.Data;
using ContosoUni.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

/// <remarks>
///     In an ASP.NET Core REST API, there is no need to explicitly check if the model state is Valid. 
///     Since the controller class is decorated with the [ApiController] attribute, 
///     it takes care of checking if the model state is valid 
///     and automatically returns 400 response along the validation errors.
///     Example response:
///         {
///             "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
///             "title": "One or more validation errors occurred.",
///             "status": 400,
///             "traceId": "|65b7c07c-4323622998dd3b3a.",
///             "errors": {
///                 "Email": [
///                     "The Email field is required."
///                 ],
///                 "FirstName": [
///                     "The field FirstName must be a string with a minimum length of 2 and a maximum length of 100."
///                 ]
///             }
///         }
/// </remarks>

// Showcases the elaborated version of how to implement a typical API

namespace ContosoUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentsApi2Controller : ControllerBase
    {
        private readonly ILogger<DepartmentsApi2Controller> _logger;
        private readonly ApplicationDbContext _context;

        public DepartmentsApi2Controller(
            ILogger<DepartmentsApi2Controller> logger,
            ApplicationDbContext context
            )
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/DepartmentsApi2
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            _logger.LogInformation("Extracted all the Departments and related Subjects");
            try
            {
                var departments = await _context.Departments
                        .Include(s => s.Subjects.Where(t => t.IsDeleted == false))
                        .Where(d => d.IsDeleted == false)
                        .ToListAsync();
                if (departments == null)
                {
                    return NotFound();
                }
                return Ok(departments);
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: api/DepartmentsApi2/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartment(Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            try
            {
                var department = await _context.Departments.FindAsync(id);

                if (department == null)
                {
                    return NotFound();
                }

                return Ok(department);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT: api/DepartmentsApi2/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(Guid id, Department department)
        {
            if (id != department.DepartmentID)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }

        // POST: api/DepartmentsApi2
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostDepartment(Department department)
        {
            try
            {
                // Update the Database by Inserting the new Department Row information
                _context.Departments.Add(department);
                int countAffected = await _context.SaveChangesAsync();
                if (countAffected > 0)
                {
                    // Return the link to the newly inserted row object,
                    // along with the updated new object (to reflect the new ID field
                    var result = CreatedAtAction("GetDepartment", new { id = department.DepartmentID }, department);
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (System.Exception exp)
            {
                ModelState.AddModelError("Post", exp.Message);
                return BadRequest(ModelState);              // HTTP 400
            }
        }

        // DELETE: api/DepartmentsApi2/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null || department.IsDeleted == true)
                {
                    return NotFound();
                }

                department.IsDeleted = true;
                department.LastUpdatedOn = DateTime.Now;
                department.UpdatedByUserId = new Guid();
                //_context.Departments.Remove(department);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
