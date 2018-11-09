using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class ContractDetailsEndEventArgs : EventArgs
    {
        public int RequestId { get; private set; }

        public ContractDetailsEndEventArgs(int reqId)
        {
            RequestId = reqId;
        }
    }
}
