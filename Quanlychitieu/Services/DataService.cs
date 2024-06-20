using CommunityToolkit.Mvvm.Messaging;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Services
{
    public partial class DataService : BaseDataService, IDataService
    {
        private IRealmService _realmService;
        private CancellationTokenSource _cancellationTokenSourceSaveRealm;
        public bool Initialized => _realmService?.GetCurrentInstanceAsync() != null;
        public event EventHandler ConnectivityChanged;
    }
    public class DataSyncModel : RealmObject
    {
        [PrimaryKey]
        public string DataName { get; set; }
        public bool Expired { get; set; }
    }
}
