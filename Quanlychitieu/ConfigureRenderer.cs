using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using Microsoft.Maui.Handlers;
using Quanlychitieu.Handlers;
using System.Threading.Tasks;

namespace Quanlychitieu
{
    public static class ConfigureRenderer
    {
        public static MauiAppBuilder RegisterCompatibilityRenderer(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers((handlers) =>
            {
                handlers.AddHandler(typeof(Entry), typeof(CustomEntryHandler));
            });

            return builder;
        }
    }
}
