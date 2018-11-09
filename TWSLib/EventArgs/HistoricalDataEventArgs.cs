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
    public class HistoricalDataEventArgs : EventArgs
    {
        public HistoricalDataEventArgs(int requestId, Bar bar, string errorMessage=null)
        {
            RequestId = requestId;
            Bar = bar;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
        public int RequestId { get; private set; }
        public string Ticker { get; private set; }
        public object ClientObject { get; private set; }
        public Bar Bar { get; private set; }

        public DateTime Date
        {
            get
            {
                DateTime date = DateTime.MinValue;
                if (Bar != null)
                    TWSUtilities.TryParseTWSDate(Bar.Time, out date);
                return date;
            }
        }

        public double Open
        {
            get
            {
                if (Bar != null)
                    return Bar.Open;
                else
                    return 0;
            }
        }
        public double High
        {
            get
            {
                if (Bar != null)
                    return Bar.High;
                else
                    return 0;
            }
        }
        public double Low
        {
            get
            {
                if (Bar != null)
                    return Bar.Low;
                else
                    return 0;
            }
        }
        public double Close
        {
            get
            {
                if (Bar != null)
                    return Bar.Close;
                else
                    return 0;
            }
        }
        public double WAP
        {
            get
            {
                if (Bar != null)
                    return Bar.WAP;
                else
                    return 0;
            }
        }
        public long Volume
        {
            get
            {
                if (Bar != null)
                    return Bar.Volume;
                else
                    return 0;
            }
        }


        public void UpdateSubscriptionInfo(TWSSubscription subscription)
        {
            if (subscription != null)
            {
                Ticker = subscription.Ticker;
                ClientObject = subscription.ClientObject;
            }
            else
            {
                Ticker = "Unknown";
            }
        }

        public override string ToString()
        {
            if (ErrorMessage != null)
            {
                if (Ticker == null)
                    return String.Format("Historical data error: RequestId={0}, Message={1}", RequestId, ErrorMessage);
                else
                    return String.Format("Historical data error: Ticker={0} ({2}), Message={1}", Ticker, ErrorMessage, RequestId);
            }
            else if (Bar == null)
            {
                if (Ticker == null)
                    return String.Format("Historical data end: RequestId={0}", RequestId);
                else
                    return String.Format("Historical data end: Ticker={0} ({1})", Ticker, RequestId);
            }
            else
            {
                if (Ticker == null)
                    return String.Format("Historical: RequestId={0}, Count={1}, Time={2:d}, Open={3}, High={4}, Low={5}, Close={6}, WAP={7}, Volume={8}",
                        RequestId, Bar.Count, Date, Open, High, Low, Close, WAP, Volume);
                else
                    return String.Format("Historical: Ticker={0} ({9}), Count={1}, Time={2:d}, Open={3}, High={4}, Low={5}, Close={6}, WAP={7}, Volume={8}",
                        Ticker, Bar.Count, Date, Open, High, Low, Close, WAP, Volume, RequestId);
            }
        }

    }
}
