using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class TickReqParamsEventArgs : EventArgs
    {
        public int TickerId { get; private set; }
        public double MinTick { get; private set; }
        public string BboExchange { get; private set; }
        public int SnapshotPermissions { get; private set; }

        public TickReqParamsEventArgs(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            TickerId = tickerId;
            MinTick = minTick;
            BboExchange = bboExchange;
            SnapshotPermissions = snapshotPermissions;
        }

        public override string ToString()
        {
            return String.Format("TickerId={0} MinTick = {1} BboExchange = {2} SnapshotPermissions = {3}", TickerId, MinTick, BboExchange, SnapshotPermissions);
        }
    }
}
