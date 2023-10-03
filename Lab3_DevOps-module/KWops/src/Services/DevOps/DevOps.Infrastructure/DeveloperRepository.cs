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
    }
}
