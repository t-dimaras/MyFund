using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyFund.Model;

namespace MyFund.Controllers
{
    [Authorize]
    public class UserBackingsController : Controller
    {
        private readonly CrowdContext _context;

        public UserBackingsController(CrowdContext context)
        {
            _context = context;
        }

        // GET: UserBackings
        public async Task<IActionResult> Index()
        {
            var crowdContext = _context.UserBacking.Include(u => u.Backing).Include(u => u.User);
            return View(await crowdContext.ToListAsync());
        }

        // GET: UserBackings/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBacking = await _context.UserBacking
                .Include(u => u.Backing)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            
            if (userBacking == null)
            {
                return NotFound();
            }

            return View(userBacking);
        }

        // GET: UserBackings/Create
        public IActionResult Create(long? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            else
            {
                var projectContext = _context.Project
                                        .Include(p => p.BackingPackages);


            }
            ViewData["BackingId"] = new SelectList(_context.BackingPackage, "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email");
            return View();
        }

        // POST: UserBackings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,BackingId,TransactionId,Amount")] UserBacking userBacking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userBacking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BackingId"] = new SelectList(_context.BackingPackage, "Id", "Name", userBacking.BackingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", userBacking.UserId);
            return View(userBacking);
        }

        // GET: UserBackings/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBacking = await _context.UserBacking.FindAsync(id);
            if (userBacking == null)
            {
                return NotFound();
            }
            ViewData["BackingId"] = new SelectList(_context.BackingPackage, "Id", "Name", userBacking.BackingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", userBacking.UserId);
            return View(userBacking);
        }

        // POST: UserBackings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UserId,BackingId,TransactionId,Amount")] UserBacking userBacking)
        {
            if (id != userBacking.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userBacking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserBackingExists(userBacking.UserId))
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
            ViewData["BackingId"] = new SelectList(_context.BackingPackage, "Id", "Name", userBacking.BackingId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Email", userBacking.UserId);
            return View(userBacking);
        }

        // GET: UserBackings/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userBacking = await _context.UserBacking
                .Include(u => u.Backing)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userBacking == null)
            {
                return NotFound();
            }

            return View(userBacking);
        }

        // POST: UserBackings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userBacking = await _context.UserBacking.FindAsync(id);
            _context.UserBacking.Remove(userBacking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserBackingExists(long id)
        {
            return _context.UserBacking.Any(e => e.UserId == id);
        }
    }
}
