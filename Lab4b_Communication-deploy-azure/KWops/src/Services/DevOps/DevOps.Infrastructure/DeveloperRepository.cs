using DevOps.AppLogic;
using DevOps.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevOps.Infrastructure
{
    internal class DeveloperRepository : IDeveloperRepository
    {
        private readonly DevOpsContext _context;

        public DeveloperRepository(DevOpsContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Developer developer)
        {
            _context.Developers.Add(developer);
            await CommitTrackedChangesAsync();
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Developer>> FindDevelopersWithoutATeamAsync()
        {
            return await _context.Developers
                .Where(d => d.TeamId == null)
                .ToListAsync();
        }

        public Task<Developer?> GetByIdAsync(string developerId)
        {
            return _context.Developers
                .SingleOrDefaultAsync(d => d.Id == developerId);
        }
    }
}
