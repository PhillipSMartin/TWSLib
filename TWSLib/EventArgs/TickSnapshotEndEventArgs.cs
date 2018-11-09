using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class TickSnapshotEndEventArgs : EventArgs
    {
        public int TickerId { get; private set; }

        public TickSnapshotEndEventArgs(int tickerId)
        {
            TickerId = tickerId;
        }
    }
}
