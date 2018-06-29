using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyFund.Extensions;
using MyFund.Model;

namespace MyFund.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly CrowdContext _context;
        private readonly IAuthorizationService _authorizationService;

        public ProjectsController(CrowdContext context, IAuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // GET: Projects
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var crowdContext = _context.Project.Include(p => p.AttatchmentSet).Include(p => p.ProjectCategory).Include(p => p.Status).Include(p => p.User);
            return View(await crowdContext.ToListAsync());
        }

        // GET: Projects/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.AttatchmentSet)
                .Include(p => p.ProjectCategory)
                .Include(p => p.Status)
                .Include(p => p.User)
                .Include(p => p.BackingPackages)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            if (User.GetUserId().HasValue && User.GetUserId().Value != project.UserId)
            {
                var userBackingQuery = from bp in _context.BackingPackage
                                       from ub in bp.UserBackings
                                       where ub.UserId == User.GetUserId() && ub.BackingId == bp.Id
                                       select ub;

                await userBackingQuery.LoadAsync();

                if (userBackingQuery.Count() == 1)
                {
                    var userBacking = userBackingQuery.Single();
                    project.BackingPackages.First(bp => bp.Id == userBacking.BackingId).UserBackings.Add(userBacking);
                }
            }
            return View(project);
        }

        ////GET: /Projects/Details/{project object}
        //public IActionResult Details(Project project)
        //{
        //    if (project ==null || !_context.Project.Contains(project))
        //    {
        //        return NotFound();
        //    }

        //    return View(project);
        //}

        // GET: Projects/Create
        public IActionResult Create()
        {
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Title,ShortDescription,Description,Goal,Deadline,StatusId,ProjectCategoryId,Url,MediaUrl")] Project project)
        {

            #region project derived and default values
            project.UserId = User.GetUserId().Value;
            project.DateCreated = DateTime.Now;
            project.AmountGathered = 0;
            project.StatusId = (long)Status.StatusDescription.Inactive;
            #endregion

            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { project.Id });
            }
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name", project.ProjectCategoryId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", project.StatusId);
            return View(project);
        }

        // GET: Projects/Edit/5
        //[Authorize(Policy = "ProjectCreator")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectCreator");

            if (authorizationResult.Succeeded)
            {
                ViewData["AttatchmentSetId"] = new SelectList(_context.AttatchmentSet, "Id", "Id", project.AttatchmentSetId);
                ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name", project.ProjectCategoryId);
                ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", project.StatusId);
                ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", project.UserId);
                return View(project);
            }
            else if (User.Identity.IsAuthenticated)
            {
                return new ForbidResult();
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "ProjectCreator")]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Title,ShortDescription,Description,Goal,AmountGathered,DateCreated,DateUpdated,Deadline,StatusId,ProjectCategoryId,Url,UserId,AttatchmentSetId,MediaUrl")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AttatchmentSetId"] = new SelectList(_context.AttatchmentSet, "Id", "Id", project.AttatchmentSetId);
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name", project.ProjectCategoryId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", project.StatusId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", project.UserId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.AttatchmentSet)
                .Include(p => p.ProjectCategory)
                .Include(p => p.Status)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var project = await _context.Project.FindAsync(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(long id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
