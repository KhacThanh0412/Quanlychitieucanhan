using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Messages
{
    public class RealmDataMessage : ValueChangedMessage<ReamDataModel>
    {
        public RealmDataMessage(ReamDataModel value)
            : base(value)
        {
        }
    }
}
