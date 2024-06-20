using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Services
{
    public interface IDataService
    {
        bool Initialized { get; }
        Task<bool> InitAsync(string locale);
    }
}