using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFund.DataModel;
using MyFund.Models;

namespace MyFund.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly CrowdContext _context;

        public HomeController(CrowdContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topProjects = await _context.Project
                                .Include(p => p.ProjectCategory)
                                .Where(p => p.StatusId == (long)Status.StatusDescription.Active)
                                .OrderByDescending(p => p.AmountGathered)
                                .Take(3)
                                .ToListAsync();

            return View(topProjects);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
