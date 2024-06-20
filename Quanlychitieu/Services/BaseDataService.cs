using Newtonsoft.Json;
using Quanlychitieu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Services
{
    public abstract class BaseDataService
    {
        protected static JsonSerializerSettings _jsonSerializerSettings;

        static BaseDataService()
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver()
            };
        }

        public virtual async Task<bool> InitAsync(string locale)
        {
            return await Task.FromResult(true);
        }
    }
}
