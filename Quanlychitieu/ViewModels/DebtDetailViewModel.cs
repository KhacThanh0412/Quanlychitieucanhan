using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.ViewModels
{
    public partial class DebtDetailViewModel : BaseViewModel
    {
        [ObservableProperty]
        DebtModel _singleDebtDetails;

        public DebtDetailViewModel()
        {

        }

        public override Task InitAsync(object initData)
        {
            if (initData is not null)
                SingleDebtDetails = initData as DebtModel;
            return base.InitAsync(initData);
        }
    }
}
