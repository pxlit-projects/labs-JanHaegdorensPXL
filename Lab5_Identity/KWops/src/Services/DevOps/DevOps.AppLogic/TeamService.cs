using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("DevOps.AppLogic.Tests")]
[assembly: InternalsVisibleTo("DevOps.Api")]

namespace DevOps.AppLogic
{
    internal class TeamService : ITeamService
    {
        public readonly IDeveloperRepository DeveloperRepository;
        public TeamService(IDeveloperRepository developerRepository)
        {
            DeveloperRepository = developerRepository;
        }

        public async Task AssembleDevelopersAsyncFor(Team team, int requiredNumberOfDevelopers)
        {
            // Fetch a list of free developers without a team.
            var freeDevelopers = await DeveloperRepository.FindDevelopersWithoutATeamAsync();

            // Randomly select the required number of developers from the list of free developers,
            // or add all available developers if there aren't enough.
            var random = new Random();
            var selectedDevelopers = freeDevelopers
                .OrderBy(d => random.Next())
                .Take(Math.Min(requiredNumberOfDevelopers, freeDevelopers.Count))
                .ToList();

            // Add the selected developers to the team.
            foreach (var developer in selectedDevelopers)
            {
                team.Join(developer);
            }

            // Save changes to the database (commit).
            await DeveloperRepository.CommitTrackedChangesAsync();
        }

    }
}
