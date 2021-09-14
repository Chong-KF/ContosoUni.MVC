using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUni.Data;
using ContosoUni.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ContosoUni.Areas.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using ContosoUni.ViewModels;

namespace ContosoUni.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly ILogger<DepartmentsController> _logger;

        public SubjectsController(
            ApplicationDbContext context,
            UserManager<MyIdentityUser> userManager,
            ILogger<DepartmentsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Subjects
        public async Task<IActionResult> Index(bool id)
        {
            //var applicationDbContext = _context.Subject.Include(s => s.CreatedByUser).Include(s => s.Departments).Include(s => s.UpdatedByUser);
            //return View(await applicationDbContext.ToListAsync());
            if (id && User.Identity.IsAuthenticated)
            {
                IEnumerable<SubjectViewModel> subjectViewModels
                = await (from s in _context.Subject
                    .Include(s => s.CreatedByUser)
                    .Include(s => s.Departments)
                    .Include(s => s.UpdatedByUser)
                         select new SubjectViewModel
                         {
                             SubjectID = s.SubjectID,
                             SubjectName = s.SubjectName,
                             Grade = s.Grade,
                             Departments = s.Departments,
                             DepartmentID = s.DepartmentID,
                             CreatedByUser = s.CreatedByUser,
                             CreatedByUserId = s.CreatedByUserId,
                             UpdatedByUser = s.UpdatedByUser,
                             UpdatedByUserId = s.UpdatedByUserId,
                             LastUpdatedOn = s.LastUpdatedOn,
                             IsDeleted = s.IsDeleted
                         }).ToListAsync();
                ViewBag.IncludeDelete = false;
                return View(subjectViewModels);
            }
            else
            {
                IEnumerable<SubjectViewModel> subjectViewModels
                = await (from s in _context.Subject
                    .Include(s => s.CreatedByUser)
                    .Include(s => s.Departments)
                    .Include(s => s.UpdatedByUser)
                    .Where(s => s.IsDeleted == false)
                         select new SubjectViewModel
                         {
                             SubjectID = s.SubjectID,
                             SubjectName = s.SubjectName,
                             Grade = s.Grade,
                             Departments = s.Departments,
                             DepartmentID = s.DepartmentID,
                             CreatedByUser = s.CreatedByUser,
                             CreatedByUserId = s.CreatedByUserId,
                             UpdatedByUser = s.UpdatedByUser,
                             UpdatedByUserId = s.UpdatedByUserId,
                             LastUpdatedOn = s.LastUpdatedOn,
                             IsDeleted = s.IsDeleted
                         }).ToListAsync();
                ViewBag.IncludeDelete = true;
                return View(subjectViewModels);
            }
        }

        // GET: Subjects/Details/5
        [Authorize(Roles = "Administrator,Staff")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subject
                .Include(s => s.CreatedByUser)
                .Include(s => s.Departments)
                .Include(s => s.UpdatedByUser)
                .FirstOrDefaultAsync(m => m.SubjectID == id);
            if (subject == null)
            {
                return NotFound();
            }

            SubjectViewModel subjectViewModel = new()
            {
                SubjectID = subject.SubjectID,
                SubjectName = subject.SubjectName,
                Grade = subject.Grade,
                Departments = subject.Departments,
                DepartmentID = subject.DepartmentID,
                CreatedByUser = subject.CreatedByUser,
                CreatedByUserId = subject.CreatedByUserId,
                UpdatedByUser = subject.UpdatedByUser,
                UpdatedByUserId = subject.UpdatedByUserId,
                LastUpdatedOn = subject.LastUpdatedOn,
                IsDeleted = subject.IsDeleted
            };

            return View(subjectViewModel);
        }

        // GET: Subjects/Create
        [Authorize(Roles = "Administrator,Staff")]
        public IActionResult Create()
        {
            var departmentsQuery = from d in _context.Departments
                                   where d.IsDeleted.Equals(false)
                                   select d;
            
            ViewData["DepartmentID"] = new SelectList(departmentsQuery, "DepartmentID", "DepartmentName");
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName");
            SubjectViewModel subjectViewModel = new();
            return View(subjectViewModel);
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectID,SubjectName,Grade,DepartmentID,CreatedByUserId,UpdatedByUserId,LastUpdatedOn,IsDeleted")] SubjectViewModel subjectViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Create", "User not found. Please log back in!");
            }
            if (!ModelState.IsValid)  //validation valid
            {
                return View(subjectViewModel);
            }
            //Model is valid
            Subject subject = new()
            {
                SubjectID = Guid.NewGuid(),
                SubjectName = subjectViewModel.SubjectName,
                Grade = subjectViewModel.Grade,
                DepartmentID = subjectViewModel.DepartmentID,
                LastUpdatedOn = DateTime.Now,
                CreatedByUserId = user.Id,
                IsDeleted = false
            };
            try
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Create", "Unable to update the Database");
                _logger.LogError($"Create subject failed: {ex.Message}");
                
                var departmentsQuery = from d in _context.Departments
                                       where d.IsDeleted.Equals(false)
                                       select d;
                ViewData["DepartmentID"] = new SelectList(departmentsQuery, "DepartmentID", "DepartmentName", subject.DepartmentID);
                ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.CreatedByUserId);
                ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.UpdatedByUserId);
                return View(subjectViewModel);
            } 
        }

        // GET: Subjects/Edit/5
        [Authorize(Roles = "Administrator,Staff")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                return NotFound();
            }

            SubjectViewModel subjectViewModel = new()
            {
                SubjectID = subject.SubjectID,
                SubjectName = subject.SubjectName,
                Grade = subject.Grade,
                Departments = subject.Departments,
                DepartmentID = subject.DepartmentID,
                CreatedByUser = subject.CreatedByUser,
                CreatedByUserId = subject.CreatedByUserId,
                UpdatedByUser = subject.UpdatedByUser,
                UpdatedByUserId = subject.UpdatedByUserId,
                LastUpdatedOn = subject.LastUpdatedOn,
                IsDeleted = subject.IsDeleted
            };
            
            var departmentsQuery = from d in _context.Departments
                                   where d.IsDeleted.Equals(false)
                                   select d;
            ViewData["DepartmentID"] = new SelectList(departmentsQuery, "DepartmentID", "DepartmentName", subject.DepartmentID);
            ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.CreatedByUserId);
            ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.UpdatedByUserId);
            return View(subjectViewModel);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SubjectID,SubjectName,Grade,DepartmentID,CreatedByUserId,UpdatedByUserId,LastUpdatedOn,IsDeleted")] SubjectViewModel subjectViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Edit", "User not found. Please log back in!");
            }
            if (id != subjectViewModel.SubjectID)
            {
                ModelState.AddModelError("Edit", "id error");
            }
            if (!ModelState.IsValid)  //validation valid
            {
                return View(subjectViewModel);
            }
            //Model is valid
            Subject subject = await _context.Subject.FindAsync(id);
            if (subject== null)
            {
                return NotFound();
            }

            subject.DepartmentID = subjectViewModel.DepartmentID;
            subject.SubjectName = subjectViewModel.SubjectName;
            subject.Grade = subjectViewModel.Grade;
            subject.LastUpdatedOn = DateTime.Now;
            subject.UpdatedByUserId = user.Id;
            subject.IsDeleted = subjectViewModel.IsDeleted;
            try
            {
                _context.Subject.Update(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Edit", "Unable to update the Database");
                _logger.LogError($"Edit subject failed: {ex.Message}");
                
                var departmentsQuery = from d in _context.Departments
                                       where d.IsDeleted.Equals(false)
                                       select d;
                ViewData["DepartmentID"] = new SelectList(departmentsQuery, "DepartmentID", "DepartmentName", subject.DepartmentID);
                ViewData["CreatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.CreatedByUserId);
                ViewData["UpdatedByUserId"] = new SelectList(_context.Users, "Id", "DisplayName", subject.UpdatedByUserId);
                return View(subjectViewModel);

            }
        }

        //// GET: Subjects/Delete/5
        //[Authorize(Roles = "Administrator,Staff")]
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var subject = await _context.Subject
        //        .Include(s => s.CreatedByUser)
        //        .Include(s => s.Departments)
        //        .Include(s => s.UpdatedByUser)
        //        .FirstOrDefaultAsync(m => m.SubjectID == id);
        //    if (subject == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(subject);
        //}

        // POST: Subjects/Delete/5
        [Authorize(Roles = "Administrator,Staff")]
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
            var subject = await _context.Subject.FindAsync(id);
            subject.IsDeleted = true;
            subject.LastUpdatedOn = DateTime.Now;
            subject.UpdatedByUserId = user.Id;
            _context.Subject.Update(subject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectExists(Guid id)
        {
            return _context.Subject.Any(e => e.SubjectID == id);
        }
    }
}
