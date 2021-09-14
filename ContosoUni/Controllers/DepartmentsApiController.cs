
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

namespace ContosoUni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsApiController : ControllerBase
    {
        private readonly ILogger<DepartmentsApiController> _logger;
        private readonly ApplicationDbContext _context;

        public DepartmentsApiController(
            ILogger<DepartmentsApiController> logger,
            ApplicationDbContext context
            )
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/DepartmentsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            _logger.LogInformation("Extracted all the Departments and related Subjects");
            return await _context.Departments
                .Include(s => s.Subjects.Where(t => t.IsDeleted == false))
                .Where(d => d.IsDeleted == false)
                .ToListAsync();
        }

        // GET: api/DepartmentsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(Guid id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null || department.IsDeleted == true)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/DepartmentsApi/5
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
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/DepartmentsApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Department>> PostDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = department.DepartmentID }, department);
        }

        // DELETE: api/DepartmentsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(Guid id)
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
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            //_context.Departments.Remove(department);
            //await _context.SaveChangesAsync();

            return Ok();
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}

