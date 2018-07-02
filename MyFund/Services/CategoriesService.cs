using Microsoft.EntityFrameworkCore;
using MyFund.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MyFund.Services
{
    public class CategoriesService
    {
        private readonly CrowdContext _context;

        public CategoriesService(CrowdContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectCategory>> GetCategoriesAsync()
        {
            return await _context.ProjectCategory.OrderBy(cat => cat.Name).ToListAsync();
        }
    }
}
