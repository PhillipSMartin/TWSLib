using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
    public class TWSTickEventArgs : EventArgs
    {
        public string Ticker { get; private set; }
        public int RequestId { get; private set; }
        public SubscriptionStatus SubscriptionStatus { get; private set; }
        public TWSSubscription.TWSSubscriptionType SubscriptionType { get; private set; }
        public object ClientObject { get; private set; }

        public int Field { get; private set; }
        public double? DoubleValue { get; private set; }
        public int? IntValue { get; private set; }
        public string StringValue { get; private set; }
        public TickAttrib Attribs { get; private set; }

        public string FieldName { get { return TickType.getField(Field); } }
        public bool HasAttribs { get { return Attribs != null; } }

        // option Prices
        public double? Ask { get { if ((Field == TickType.ASK) || (Field == TickType.DELAYED_ASK)) return DoubleValue; else return null; } }
        public double? Bid { get { if ((Field == TickType.BID) || (Field == TickType.DELAYED_BID)) return DoubleValue; else return null; } }
        public double? Last { get { if ((Field == TickType.LAST) || (Field == TickType.DELAYED_LAST)) return DoubleValue; else return null; } }
        public double? Open { get { if ((Field == TickType.OPEN) || (Field == TickType.DELAYED_OPEN)) return DoubleValue; else return null; } }
        public double? PrevClose { get { if ((Field == TickType.CLOSE) || (Field == TickType.DELAYED_CLOSE)) return DoubleValue; else return null; } }
 
        // option Greeks
        public bool HasGreeks { get; private set; }
        public double? ImpliedVol { get; private set; }
        public double? Delta { get; private set; }
        public double? OptionPrice { get; private set; }
        public double? PresentValueDividend { get; private set; }
        public double? Gamma { get; private set; }
        public double? Vega { get; private set; }
        public double? Theta { get; private set; }
        public double? UnderlyingPrice { get; private set; }

        // dividends
        public bool HasDividends { get { return Field == TickType.IB_DIVIDENDS; } }
        public DateTime? ExDate { get; private set; }
        public double Dividend { get; private set; }
        public double DividendsPastYear { get; private set; }
        public double DividendsNextYear { get; private set; }

        public TWSTickEventArgs(TWSSubscription subscription)
        {
            Ticker = subscription.Ticker;
            RequestId = subscription.RequestId;
            SubscriptionType = subscription.SubscriptionType;
            ClientObject = subscription.ClientObject;
            switch (subscription.Status)
            {
                case TWSSubscription.TWSSubscriptionStatus.Rejected:
                    SubscriptionStatus = SubscriptionStatus.Rejected;
                    break;
                case TWSSubscription.TWSSubscriptionStatus.New:
                case TWSSubscription.TWSSubscriptionStatus.Unsubscribed:
                case TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe:
                    SubscriptionStatus = SubscriptionStatus.Unsubscribed;
                    break;
                default:
                    SubscriptionStatus = SubscriptionStatus.Subscribed;
                    break;
            }
        }

        public TWSTickEventArgs(TWSSubscription subscription, int field, double price, TickAttrib attribs = null) 
            : this(subscription)
         {
            Field = field;
            DoubleValue = price;
            Attribs = attribs;
        }

        public TWSTickEventArgs(TWSSubscription subscription, int field, int size)
            : this(subscription)
        {
            Field = field;
            IntValue = size;
        }

        public TWSTickEventArgs(TWSSubscription subscription, int field, string value)
            : this(subscription)
        {
            Field = field;
            StringValue = value;

            SaveDividendData();
        }

        private void SaveDividendData()
        {
            if (Field == TickType.IB_DIVIDENDS)
            {
                if (StringValue != null)
                {
                    string[] tokens = StringValue.Split(new char[] { ',' });
                    if (tokens.Length >= 4)
                    {
                        double dividendsPastYear;
                        double dividendsNextYear;
                        DateTime exDate;
                        double dividend;
 
                        if (double.TryParse(tokens[0], out dividendsPastYear))
                            DividendsPastYear = dividendsPastYear;
                        if (double.TryParse(tokens[1], out dividendsNextYear))
                            DividendsNextYear = dividendsNextYear;
                        if (TWSUtilities.TryParseTWSDate(tokens[2], out exDate))
                            ExDate = exDate;
                        if (double.TryParse(tokens[3], out dividend))
                            Dividend = dividend;
                    }
                }
            }
        }

        public TWSTickEventArgs(TWSSubscription subscription, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
            : this(subscription)
        {
            Field = field;
            HasGreeks = true;
            if (impliedVolatility != Double.MaxValue) ImpliedVol = impliedVolatility * 100.0;
            if (delta != Double.MaxValue) Delta = delta;
            if (optPrice != Double.MaxValue) OptionPrice = optPrice;
            if (pvDividend != Double.MaxValue) PresentValueDividend = pvDividend;
            if ((gamma != Double.MaxValue) && (undPrice != Double.MaxValue)) Gamma = gamma * undPrice / 100.0;
            if (vega != Double.MaxValue) Vega = vega;
            if (theta != Double.MaxValue) Theta = theta;
            if (undPrice != Double.MaxValue) UnderlyingPrice = undPrice;
        }

        public override string ToString()
        {
            if (HasGreeks)
            {
                return String.Format("{0}({10}), Field: {1}, ImpliedVolatility: {2}, Delta: {3}, OptionPrice: {4}, PresentValueDividend: {5}, Gamma: {6}, Vega: {7}, Theta: {8}, UnderlyingPrice: {9}",
                    Ticker, FieldName, ImpliedVol, Delta, OptionPrice, PresentValueDividend, Gamma, Vega, Theta, UnderlyingPrice, RequestId);
            }
            else if (HasAttribs)
            {
                return String.Format("{0}({6}), Field: {1}, Value: {2}, CanAutoExecute: {3}, PastLimit: {4}, PreOpen: {5}",
                    Ticker, FieldName, DoubleValue, Attribs.CanAutoExecute, Attribs.PastLimit, Attribs.PreOpen, RequestId);
            }
            else if (HasDividends)
            {
                return String.Format("{0}({5}), ExDate: {1:d}, Dividend: {2}, PastYear: {3}, NextYear: {4}",
                   Ticker, ExDate, Dividend, DividendsPastYear, DividendsNextYear, RequestId);
            }
            else if (DoubleValue.HasValue)
            {
                return String.Format("{0}({3}), Field: {1}, Value: {2}", Ticker, FieldName, DoubleValue, RequestId);
            }
            else if (IntValue.HasValue)
            {
                return String.Format("{0}({3}), Field: {1}, Value: {2}", Ticker, FieldName, IntValue, RequestId);
            }
            else if (StringValue != null)
            {
                return String.Format("{0}({3}), Field: {1}, Value: {2}", Ticker, FieldName, StringValue, RequestId);
            }
            else
            {
                return String.Format("{0}, Status={1}", Ticker, Enum.GetName(typeof(SubscriptionStatus), SubscriptionStatus), RequestId);
            }
        }
    }
}
