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
using MyFund.DataModel;
using MyFund.Services;
using MyFund.Authorization;

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

        [Authorize]
        public async Task<IActionResult> Dashboard(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DeadlineSortParm"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            var projectContext = _context.Project
                                .Include(p => p.AttatchmentSet)
                                .Include(p => p.ProjectCategory)
                                .Include(p => p.Status)
                                .Include(p => p.User)
                                .Where(p => p.UserId == User.GetUserId());
            await projectContext.LoadAsync();

            projectContext = ApplySortOrder(projectContext, sortOrder);

            return View(projectContext);
        }

        // GET: Projects
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string includeDesChecked, long categoryId)
        {
            var projectContext = _context.Project
                                .Include(p => p.AttatchmentSet)
                                .Include(p => p.ProjectCategory)
                                .Include(p => p.Status)
                                .Include(p => p.User)
                                .Where(p => p.StatusId == (long)Status.StatusDescription.Active);
            await projectContext.LoadAsync();

            #region search
            ViewData["categoryId"] = categoryId;
            ViewData["currentFilter"] = searchString;
            ViewData["includeDesChecked"] = includeDesChecked;
            bool isIncludeDesChecked = includeDesChecked == "on";

            projectContext = FilterProjects(projectContext, searchString, isIncludeDesChecked, categoryId);
            #endregion

            #region sort
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DeadlineSortParam"] = sortOrder == "Deadline" ? "deadline_desc" : "Deadline";
            ViewData["CategorySortParam"] = sortOrder == "Category" ? "category_desc" : "Category";
            ViewData["AmountGatheredSortParam"] = sortOrder == "AmountGathered" ? "amountGathered_desc" : "AmountGathered";
            ViewData["GoalSortParam"] = sortOrder == "Goal" ? "goal_desc" : "Goal";
            ViewData["DateCreatedSortParam"] = sortOrder == "DateCreated" ? "dateCreated_desc" : "DateCreated";

            projectContext = ApplySortOrder(projectContext, sortOrder);
            #endregion

            return View(projectContext);
        }

        private static IQueryable<Project> FilterProjects
            (IQueryable<Project> projectContext, string searchString, bool filterShortDescription, long categoryId)
        {
            var filteredContext = projectContext;
            if (categoryId > 0)
            {
                filteredContext = filteredContext.Where(p => p.ProjectCategoryId == categoryId);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                if (!filterShortDescription)
                {
                    filteredContext = filteredContext.Where(p => p.Name.Contains(searchString)
                                                             || p.Title.Contains(searchString));
                }
                else
                {
                    filteredContext = filteredContext.Where(p => p.Name.Contains(searchString)
                                                             || p.Title.Contains(searchString)
                                                             || p.ShortDescription.Contains(searchString));
                }
            }
            return filteredContext;
        }

        private static IQueryable<Project> ApplySortOrder(IQueryable<Project> projectContext, string sortOrder)
        {
            var sortedContext = projectContext;

            switch (sortOrder)
            {
                case "name_desc":
                    sortedContext = sortedContext.OrderByDescending(p => p.Name);
                    break;
                case "Deadline":
                    sortedContext = sortedContext.OrderBy(p => p.Deadline);
                    break;
                case "deadline_desc":
                    sortedContext = sortedContext.OrderByDescending(p => p.Deadline);
                    break;
                case "Category":
                    sortedContext = sortedContext.OrderBy(p => p.ProjectCategory.Name);
                    break;
                case "category_desc":
                    sortedContext = sortedContext.OrderByDescending(p => p.ProjectCategory.Name);
                    break;
                case "Status":
                    sortedContext = sortedContext.OrderBy(p => p.Status.Name);
                    break;
                case "status_desc":
                    sortedContext = sortedContext.OrderByDescending(p => p.Status.Name);
                    break;
                case "AmountGathered":
                    sortedContext = sortedContext.OrderByDescending(p => p.AmountGathered);
                    break;
                case "amountGathered_desc":
                    sortedContext = sortedContext.OrderBy(p => p.AmountGathered);
                    break;
                case "Goal":
                    sortedContext = sortedContext.OrderByDescending(p => p.Goal);
                    break;
                case "goal_desc":
                    sortedContext = sortedContext.OrderBy(p => p.Goal);
                    break;
                case "DateCreated":
                    sortedContext = sortedContext.OrderBy(p => p.DateCreated);
                    break;
                case "dateCreated_desc":
                    sortedContext = sortedContext.OrderByDescending(p => p.DateCreated);
                    break;
            }

            return sortedContext;
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
                var userBackingQuery = from ub in _context.UserBacking
                                       join bp in project.BackingPackages on ub.BackingId equals bp.Id
                                       where ub.UserId == User.GetUserId()
                                       select ub;

                await userBackingQuery.LoadAsync();

                if (userBackingQuery.Count() == 1)
                {
                    var userBacking = userBackingQuery.Single();
                    project.BackingPackages.FirstOrDefault(bp => bp.Id == userBacking.BackingId)?.UserBackings?.Add(userBacking);
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
        [AuthorizeResource(typeof(ResourceOwnerRequirement), typeof(Project))]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                                    .Include(p=>p.BackingPackages)
                                    .FirstOrDefaultAsync(p=>p.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectCreator");

            if (authorizationResult.Succeeded)
            {
                ViewData["AttatchmentSetId"] = project.AttatchmentSetId;
                ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name");
                ViewData["StatusId"] = project.StatusId;
                ViewData["UserId"] = project.UserId;
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
        [AuthorizeResource(typeof(ResourceOwnerRequirement), typeof(Project))]
        public async Task<IActionResult> Edit(string statusUpdate, long id, Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }
            var existingProject = await _context.Project.FindAsync(id);

            if (existingProject == null)
            {
                return NotFound();
            }

            @ViewData["statusUpdateParam"] = statusUpdate;

            if (ModelState.IsValid)
            {
                var authorizationResult = await _authorizationService.AuthorizeAsync(User, project, "ProjectCreator");

                if (authorizationResult.Succeeded)
                {
                    #region try commit
                    try
                    {
                        if (statusUpdate == "Publish")
                        {
                            existingProject.StatusId = (long)Status.StatusDescription.Active;
                        }
                        existingProject.Deadline = project.Deadline;
                        existingProject.Description = project.Description;
                        existingProject.Goal = project.Goal;
                        existingProject.MediaUrl = project.MediaUrl;
                        existingProject.Name = project.Name;
                        existingProject.ProjectCategoryId = project.ProjectCategoryId;
                        existingProject.ShortDescription = project.ShortDescription;
                        existingProject.Title = project.Title;
                        existingProject.Url = project.Url;
                        _context.Update(existingProject);
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
                    return RedirectToAction(nameof(Dashboard));
                    #endregion
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
            ViewData["AttatchmentSetId"] = new SelectList(_context.AttatchmentSet, "Id", "Id", project.AttatchmentSetId);
            ViewData["ProjectCategoryId"] = new SelectList(_context.ProjectCategory, "Id", "Name", project.ProjectCategoryId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "Name", project.StatusId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", project.UserId);
            @ViewData["statusUpdateParam"] = "publish_done";
            return View(project);
        }

        // GET: Projects/Delete/5
        [AuthorizeResource(typeof(ResourceOwnerRequirement), typeof(Project))]
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
        [AuthorizeResource(typeof(ResourceOwnerRequirement), typeof(Project))]
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
