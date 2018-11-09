using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
    public class TWSHistoricalDataEventArgs : EventArgs
    {
        public string Ticker { get; private set; }
        public object ClientObject { get; private set; }
        public double Close { get; private set; }
        public int Count { get; private set; }
        public double High { get; private set; }
        public double Low { get; private set; }
        public double Open { get; private set; }
        public string Time { get; private set; }
        public long Volume { get; private set; }
        public double WAP { get; private set; }


        public TWSHistoricalDataEventArgs(TWSSubscription subscription, Bar bar)
        {
            Ticker = subscription.Ticker;
            Close = bar.Close;
            Count = bar.Count;
            High = bar.High;
            Low = bar.Low;
            Open = bar.Open;
            Time = bar.Time;
            Volume = bar.Volume;
            WAP = bar.WAP;
        }


        public override string ToString()
        {
            return String.Format("{0}, Ticker = {1}, Close = {2},Count = {3}, High = {4}, Low = {5}, Open = {6}, Time = {7}, Volume = {8}, WAP = {9}",
                Ticker,
                Close,
                Count,
                High,
                Low,
                Open,
                Time,
                Volume,
                WAP);
        }
    }
}
