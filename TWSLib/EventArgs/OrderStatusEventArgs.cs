using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IBApi;

namespace TWSLib
{
    public class TWSOrderStatusEventArgs : EventArgs
    {
        public TWSOrderStatusEventArgs(int orderId, string status, double filled, double remaining, double avgFillPrice, int permid, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            OrderId = orderId;
            Status = status;
            Filled = filled;
            Remaining = remaining;
            AvgFillPrice = avgFillPrice;
            Permid = permid;
            ParentId = parentId;
            LastFillPrice = lastFillPrice;
            ClientId = clientId;
            WhyHeld = whyHeld;
            MktCapPrice = mktCapPrice;
        }

        public int OrderId { get; private set; }
        public string Status { get; private set; }
        public double Filled { get; private set; }
        public double Remaining { get; private set; }
        public double AvgFillPrice { get; private set; }
        public int Permid { get; private set; }
        public int ParentId { get; private set; }
        public double LastFillPrice { get; private set; }
        public int ClientId { get; private set; }
        public string WhyHeld { get; private set; }
        public double MktCapPrice { get; private set; }

        public override string ToString()
        {
           return String.Format("OrderStatus: OrderId={0}, Status={1}, Filled={2}, Remaining={3}, AvgFillPrice={4}, Permid={5}, ParentId={6}, LastFillPrice={7}, ClientId={8}, WhyHeld={9}, MktCapPrice={10}",
            OrderId, Status, Filled, Remaining, AvgFillPrice, Permid, ParentId, LastFillPrice, ClientId, WhyHeld, MktCapPrice);
        }
    }
}
