using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class ExecDetailsEndEventArgs : EventArgs
    {
        public ExecDetailsEndEventArgs(int reqId)
        {
            RequestId = reqId;
        }

        public int RequestId { get; private set; }
    }
}
