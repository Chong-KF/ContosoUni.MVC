using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUni.Data;
using ContosoUni.Models;

using ContosoUni.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContosoUni.Areas.Identity.Models;
using Microsoft.Extensions.Logging;
using ContosoUni.Services;

namespace ContosoUni.Controllers
{
    //[Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IAzureQueue _azureQueue;

        public DepartmentsController(
            ApplicationDbContext context,
            UserManager<MyIdentityUser> userManager,
            ILogger<DepartmentsController> logger,
            IAzureQueue azureQueue)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _azureQueue = azureQueue;
        }

        // GET: Departments
        public async Task<IActionResult> Index(bool id)
        {
            //var applicationDbContext = _context.Departments.Include(d => d.CreatedByUser).Include(d => d.UpdatedByUser);
            //return View(await applicationDbContext.ToListAsync());
            if (id && User.Identity.IsAuthenticated)
            {
                IEnumerable<DepartmentViewModel> departmentViewModels
                = await (from d in _context.Departments
                    .Include(u => u.CreatedByUser)
                    .Include(u => u.UpdatedByUser)
                    .Include(s => s.Subjects)
                         select new DepartmentViewModel
                         {
                             DepartmentID = d.DepartmentID,
                             DepartmentName = d.DepartmentName,
                             CreatedByUser = d.CreatedByUser,
                             CreatedByUserId = d.CreatedByUserId,
                             UpdatedByUser = d.UpdatedByUser,
                             UpdatedByUserId = d.UpdatedByUserId,
                             LastUpdatedOn = d.LastUpdatedOn,
                             IsDeleted = d.IsDeleted,
                             Subjects = d.Subjects
                         }).ToListAsync();
                ViewBag.IncludeDelete = false;
                return View(departmentViewModels);
            }
            else
            {
                IEnumerable<DepartmentViewModel> departmentViewModels
                = await (from d in _context.Departments
                    .Include(u => u.CreatedByUser)
                    .Include(u => u.UpdatedByUser)
                    .Include(s => s.Subjects.Where(t => t.IsDeleted==false))
                    .Where(d => d.IsDeleted == false)
                         select new DepartmentViewModel
                         {
                             DepartmentID = d.DepartmentID,
                             DepartmentName = d.DepartmentName,
                             CreatedByUser = d.CreatedByUser,
                             CreatedByUserId = d.CreatedByUserId,
                             UpdatedByUser = d.UpdatedByUser,
                             UpdatedByUserId = d.UpdatedByUserId,
                             LastUpdatedOn = d.LastUpdatedOn,
                             IsDeleted = d.IsDeleted,
                             Subjects = d.Subjects
                         }).ToListAsync();
                ViewBag.IncludeDelete = true;
                return View(departmentViewModels);
            }
        }

        // GET: Departments/Details/5
        //[Authorize(Roles = "Administrator,Staff")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.CreatedByUser)
                .Include(d => d.UpdatedByUser)
                .Include(s => s.Subjects)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            DepartmentViewModel departmentViewModel = new()
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                CreatedByUser = department.CreatedByUser,
                CreatedByUserId = department.CreatedByUserId,
                UpdatedByUser = department.UpdatedByUser,
                UpdatedByUserId = department.UpdatedByUserId,
                LastUpdatedOn = department.LastUpdatedOn,
                IsDeleted = department.IsDeleted,
                Subjects = department.Subjects
            };

            return View(departmentViewModel);
        }

        // GET: Departments/Create
        //[Authorize(Roles = "Administrator,Staff")]
        public IActionResult Create()
        {
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            DepartmentViewModel departmentViewModel = new();
            return View(departmentViewModel);
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Administrator,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,DepartmentName,CreatedByUserId,UpdatedByUserId,LastUpdatedOn,IsDeleted")] DepartmentViewModel departmentViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Create", "User not found. Please log back in!");
            }
            if (!ModelState.IsValid)  //validation valid
            {
                return View(departmentViewModel);
            }
            //Model is valid
            Department department = new()
            {
                DepartmentID = Guid.NewGuid(),
                DepartmentName = departmentViewModel.DepartmentName,
                LastUpdatedOn = DateTime.Now,
                CreatedByUserId = user.Id,
                IsDeleted = false
            };
            try
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                string msg = "CRUD : CREATE\nUser : " + user.UserName + "\nTime : " + department.LastUpdatedOn + "\nDepartmentID : " + department.DepartmentID;
                //AzureQueue azureQueue = new();
                _azureQueue.QueueMessage(msg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Create", "Unable to update the Database");
                _logger.LogError($"Create department failed: {ex.Message}");
                ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.CreatedByUserId);
                ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.UpdatedByUserId);
                return View(departmentViewModel);
            }
        }

        // GET: Departments/Edit/5
        //[Authorize(Roles = "Administrator,Staff")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            DepartmentViewModel departmentViewModel = new()
            {
                DepartmentID = department.DepartmentID,
                DepartmentName = department.DepartmentName,
                CreatedByUser = department.CreatedByUser,
                CreatedByUserId = department.CreatedByUserId,
                UpdatedByUser = department.UpdatedByUser,
                UpdatedByUserId = department.UpdatedByUserId,
                LastUpdatedOn = department.LastUpdatedOn,
                IsDeleted = department.IsDeleted
            };

            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.UpdatedByUserId);
            return View(departmentViewModel);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize(Roles = "Administrator,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DepartmentID,DepartmentName,CreatedByUserId,UpdatedByUserId,LastUpdatedOn,IsDeleted")] DepartmentViewModel departmentViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Edit", "User not found. Please log back in!");
            }
            if (id != departmentViewModel.DepartmentID)
            {
                ModelState.AddModelError("Edit", "id error");
            }
            if (!ModelState.IsValid)  //validation valid
            {
                return View(departmentViewModel);
            }
            //Model is valid
            Department department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            department.DepartmentName = departmentViewModel.DepartmentName;
            department.LastUpdatedOn = DateTime.Now;
            department.UpdatedByUserId = user.Id;
            department.IsDeleted = departmentViewModel.IsDeleted;
            try
            {
                _context.Departments.Update(department);
                await _context.SaveChangesAsync();
                string msg = "CRUD : EDIT\nUser : " + user.UserName + "\nTime : " + department.LastUpdatedOn + "\nDepartmentID : " + department.DepartmentID;
                //AzureQueue azureQueue = new();
                _azureQueue.QueueMessage(msg);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Edit", "Unable to update the Database");
                _logger.LogError($"Edit department failed: {ex.Message}");
                ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.CreatedByUserId);
                ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", department.UpdatedByUserId);
                return View(departmentViewModel);
            }
        }

        // GET: Departments/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var department = await _context.Departments
        //        .Include(d => d.CreatedByUser)
        //        .Include(d => d.UpdatedByUser)
        //        .FirstOrDefaultAsync(m => m.DepartmentID == id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(department);
        //}

        // POST: Departments/Delete/5
        //[Authorize(Roles = "Administrator,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                //ModelState.AddModelError("Delete", "User not found. Please log back in!");
                return RedirectToAction(nameof(Index));
            }
            var department = await _context.Departments.FindAsync(id);
            department.IsDeleted = true;
            department.LastUpdatedOn = DateTime.Now;
            department.UpdatedByUserId = user.Id;
            //_context.Departments.Remove(department);
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            string msg = "CRUD : DELETE\nUser : " + user.UserName + "\nTime : " + department.LastUpdatedOn + "\nDepartmentID : " + department.DepartmentID;
            //AzureQueue azureQueue = new();
            _azureQueue.QueueMessage(msg);
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(Guid id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}