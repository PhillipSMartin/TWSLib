using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class ValidIdEventArgs : EventArgs
    {
        public int OrderId { get; private set; }

        public ValidIdEventArgs(int orderId)
        {
            OrderId = orderId;
        }
    }
}
