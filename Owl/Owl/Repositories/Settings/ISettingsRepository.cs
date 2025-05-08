namespace Owl.Repositories.Settings;

public interface ISettingsRepository : IRepository<Models.Settings, Models.Settings>
{
    public Models.Settings Current { get; set; }
}
