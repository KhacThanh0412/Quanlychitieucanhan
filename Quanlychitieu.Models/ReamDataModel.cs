using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Models
{
    public class ReamDataModel
    {
        public ObservableCollection<RealmObject> Data { get; set; }
        public ObservableCollection<RealmObject> CurrentData { get; set; }
        public string DataName { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}
