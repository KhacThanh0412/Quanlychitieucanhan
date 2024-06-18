using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quanlychitieu.Helpers
{
    public sealed class ResolveViewModelException<T> : Exception
    {
        public ResolveViewModelException()
            : base($"Failed to resolve ViewModel for {typeof(T)}")
        {
        }
    }
}
