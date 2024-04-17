using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Services
{
    public class FinanceService
    {
        public IEnumerable<string> GetTransactionTypes()
        {
            return new List<string>
            {
                "All",
                "Income",
                "Expense"
            };
        }
    }
}
