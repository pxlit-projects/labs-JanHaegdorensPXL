using DevOps.Domain;
using Domain;

public class Team : Entity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private readonly List<Developer> _developers;
    public IReadOnlyList<Developer> Developers => _developers;

    private Team(Guid id, string name)
    {
        _developers = new List<Developer>();
        Id = id;
        Name = name;
    }

    public static Team CreateNew(string name)
    {
        if (name == null || name == "") {  throw new ContractException("name is not correct"); }

        Guid newGuid = Guid.NewGuid();

        Team newTeam = new Team(newGuid, name);

        return newTeam;
    }

    public void Join(Developer developer)
    {
        if (_developers.Contains(developer)) {
            throw new ContractException("already in this team");
        }
        else
        {
            developer.TeamId = Id;
            _developers.Add(developer);
        }
    }

    protected override IEnumerable<object> GetIdComponents()
    {
        yield return Id;
    }
}