using System.Text;
using Newtonsoft.Json;

namespace Quanlychitieu.DataAccess
{
    public class DataAccessRepo : IDataAccessRepo
    {
        private readonly HttpClient _httpClient;

        public DataAccessRepo()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://192.168.1.220:5000/")
            };
        }

        public async Task<T> GetDataFromApiAsync<T>(string endpoint)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }

        public async Task<List<T>> GetDataForMultipleIdsAsync<T>(List<int> ids, string endpointTemplate)
        {
            var results = new List<T>();

            foreach (var id in ids)
            {
                var endpoint = string.Format(endpointTemplate, id);
                var data = await GetDataFromApiAsync<T>(endpoint);
                results.Add(data);
            }

            return results;
        }

        public async Task<T> GetDataFromApiWithQueryParamsAsync<T>(string endpoint, Dictionary<string, string> queryParams)
        {
            var query = new FormUrlEncodedContent(queryParams);
            string queryString = await query.ReadAsStringAsync();
            string fullEndpoint = $"{endpoint}?{queryString}";
            return await GetDataFromApiAsync<T>(fullEndpoint);
        }

        public async Task<HttpResponseMessage> PostDataToApiAsync<T>(string endpoint, T data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(endpoint, httpContent);
        }

        public async Task<HttpResponseMessage> PutDataToApiAsync<T>(string endpoint, T data)
        {
            var jsonContent = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.PutAsync(endpoint, httpContent);
        }

        public async Task<HttpResponseMessage> DeleteDataFromApiAsync(string endpoint)
        {
            return await _httpClient.DeleteAsync(endpoint);
        }
    }
}
