namespace Quanlychitieu.DataAccess.IRepositories;

public interface IDataAccessRepo
{
    Task<T> GetDataFromApiAsync<T>(string endpoint);
    Task<List<T>> GetDataForMultipleIdsAsync<T>(List<int> ids, string endpointTemplate);
    Task<T> GetDataFromApiWithQueryParamsAsync<T>(string endpoint, Dictionary<string, string> queryParams);
    Task<HttpResponseMessage> PostDataToApiAsync<T>(string endpoint, T data);
    Task<HttpResponseMessage> PutDataToApiAsync<T>(string endpoint, T data);
    Task<HttpResponseMessage> DeleteDataFromApiAsync(string endpoint);
}
