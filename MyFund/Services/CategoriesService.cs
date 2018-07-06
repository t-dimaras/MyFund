using Microsoft.EntityFrameworkCore;
using MyFund.DataModel;
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

        public IAsyncEnumerable<ProjectCategory> GetCategories()
        {
            var asyncCategories = _context.ProjectCategory.OrderBy(cat => cat.Name).ToAsyncEnumerable();
            return asyncCategories;
        }
    }
}
