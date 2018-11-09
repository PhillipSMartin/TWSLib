using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class MarketDataTypeEventArgs : EventArgs
    {
        public int RequestId { get; private set; }
        public int MarketDataType { get; private set; }
        public string MarketDataTypeDescription
        {
            get
            {
                switch (MarketDataType)
                {
                    case 1:
                        return "Real-time";
                    case 2:
                        return "Frozen";
                    case 3:
                        return "Delayed";
                    case 4:
                        return "Delayed-frozen";
                    default:
                        return "Unknown market data type";
                }
            }
        }
        public MarketDataTypeEventArgs(int reqId, int marketDataType)
        {
            RequestId = reqId;
            MarketDataType = marketDataType;
        }

        public override string ToString()
        {
            return String.Format("MarketDataType: Id={0}, Type={1}", RequestId, MarketDataType);
        }
    }
}
