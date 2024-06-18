using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Handlers
{
    public partial class ShellHandler : ShellRenderer
    {
        public ShellItemHandler ShellItemHandler { get; private set; }

        public ShellHandler()
        {
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            ShellItemHandler = new ShellItemHandler(this);
            return ShellItemHandler;
        }
    }
}
