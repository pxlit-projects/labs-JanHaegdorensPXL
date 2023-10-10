using DevOps.Domain;

namespace DevOps.AppLogic
{
    public interface IDeveloperRepository
    {
        Task<IReadOnlyList<Developer>> FindDevelopersWithoutATeamAsync();
        Task CommitTrackedChangesAsync();

        Task<Developer?> GetByIdAsync(string developerId);
        Task AddAsync(Developer developer);
    }
}