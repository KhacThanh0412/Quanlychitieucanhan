
namespace Quanlychitieu.DataAccess.Repositories;

public class SettingsServiceRepository : ISettingsServiceRepository
{
    public Task ClearPreferences()
    {
        throw new NotImplementedException();
    }

    public Task<T> GetPreference<T>(string key, T defaultValue)
    {
        throw new NotImplementedException();
    }

    public Task SetPreference<T>(string key, T value)
    {
        throw new NotImplementedException();
    }
}
