using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyFund.Extensions;
using MyFund.Model;

namespace MyFund.Controllers
{
    [Authorize]
    public class BackingPackagesController : Controller
    {
        private readonly CrowdContext _context;

        public BackingPackagesController(CrowdContext context)
        {
            _context = context;
        }

        // GET: BackingPackages
        [AllowAnonymous]
        public async Task<IActionResult> Index(long? projectId)
        {
            if (projectId.HasValue)
            {
                var projectContext = await _context.Project
                                            .Include(p => p.BackingPackages)
                                            .FirstOrDefaultAsync(p => p.Id == projectId);
                if (projectContext==null)
                {
                    return NotFound();
                }

                if (User.Identity.IsAuthenticated)
                {
                    var userBackingQuery = from ub in _context.UserBacking
                                           join bp in projectContext.BackingPackages on ub.BackingId equals bp.Id
                                           where ub.UserId == User.GetUserId()
                                           select ub;

                    await userBackingQuery.LoadAsync();

                    if (userBackingQuery.Count() == 1)
                    {
                        var userBacking = userBackingQuery.Single();
                        projectContext.BackingPackages.First(bp => bp.Id == userBacking.BackingId).UserBackings.Add(userBacking);
                    }
                }

                return View(projectContext.BackingPackages.OrderBy(bp => bp.BackingAmount));
            }

            return NotFound();
        }

        // GET: BackingPackages/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backingPackage = await _context.BackingPackage
                .Include(b => b.AttatchmentSet)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
       
            if (backingPackage == null)
            {
                return NotFound();
            }

            if (User.Identity.IsAuthenticated && backingPackage.Project.UserId != User.GetUserId())
            {
                var userBacking = await _context.UserBacking.SingleOrDefaultAsync(ub => ub.BackingId == backingPackage.Id && ub.UserId == User.GetUserId());
                backingPackage.UserBackings.Add(userBacking);
            }

            return View(backingPackage);
        }

        // GET: BackingPackages/Create/4
        [HttpGet("BackingPackages/Create/{id?}")]
        public async Task<IActionResult> Create(long? id)
        {
            if (id.HasValue)
            {
                var projectContext = await _context.Project.FindAsync(id.Value);
                if (projectContext == null)
                {
                    return NotFound();
                }

                ViewData["ProjectId"] = id.Value;
                return View();
            }
            else
            {
                return NotFound();
            }
        }

        // POST: BackingPackages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("BackingPackages/Create/{projectId?}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PackageDescription,BackingAmount,RewardDescription,DateCreated,DateUpdated,ProjectId,AttatchmentSetId")] BackingPackage backingPackage)
        {
            if (ModelState.IsValid)
            {
                backingPackage.DateCreated = DateTime.Now;
                _context.Add(backingPackage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProjectsController.Details), "Projects", new { Id = backingPackage.ProjectId });
            }
            return View(backingPackage);
        }

        // GET: BackingPackages/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backingPackage = await _context.BackingPackage.FindAsync(id);
            if (backingPackage == null)
            {
                return NotFound();
            }
            ViewData["AttatchmentSetId"] = new SelectList(_context.AttatchmentSet, "Id", "Id", backingPackage.AttatchmentSetId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", backingPackage.ProjectId);
            return View(backingPackage);
        }

        // POST: BackingPackages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,PackageDescription,BackingAmount,RewardDescription,DateCreated,DateUpdated,ProjectId,AttatchmentSetId")] BackingPackage backingPackage)
        {
            if (id != backingPackage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(backingPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BackingPackageExists(backingPackage.Id))
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
            ViewData["AttatchmentSetId"] = new SelectList(_context.AttatchmentSet, "Id", "Id", backingPackage.AttatchmentSetId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", backingPackage.ProjectId);
            return View(backingPackage);
        }

        // GET: BackingPackages/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var backingPackage = await _context.BackingPackage
                .Include(b => b.AttatchmentSet)
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (backingPackage == null)
            {
                return NotFound();
            }

            return View(backingPackage);
        }

        // POST: BackingPackages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var backingPackage = await _context.BackingPackage.FindAsync(id);
            _context.BackingPackage.Remove(backingPackage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BackingPackageExists(long id)
        {
            return _context.BackingPackage.Any(e => e.Id == id);
        }
    }
}
