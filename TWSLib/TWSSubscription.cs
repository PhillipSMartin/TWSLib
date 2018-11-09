using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
    public class TWSSubscription
    {
        private static int s_previousRequestId = 0;
        public static EventHandler<TWSTickEventArgs> s_subscriptionEndedEventHander;

        private List<string> m_log = new List<string>();
        private object m_logLock = new object();

        public enum TWSSubscriptionType { TickData, FundamentalData, HistoricalData, CorporateActionData };
        public enum TWSSubscriptionStatus { New, AwaitingContractInfo, AwaitingMarketData, AwaitingUnsubscribe, Subscribed, Unsubscribed, Rejected };
        public int RequestId { get; private set; }
        public string Ticker { get; private set; }
        public QuoteType QuoteType { get; private set; }
        public object ClientObject { get; private set; }
        public string ReportType { get; set; }
        public TWSSubscriptionType SubscriptionType { get; set; }
        public DateTime? Date { get; set; }

        private TWSSubscriptionStatus m_status;
        public TWSSubscriptionStatus Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
                switch (value)
                {
                    case TWSSubscriptionStatus.Rejected:
                    case TWSSubscriptionStatus.Unsubscribed:
                        if (s_subscriptionEndedEventHander != null)
                        {
                            s_subscriptionEndedEventHander(this, new TWSTickEventArgs(this));
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public Contract Contract { get; set; }
        public object StatusLock = new object();

        public TWSSubscription(string ticker, QuoteType quoteType, TWSSubscriptionType subscriptionType, object clientObject, DateTime? date)
        {
            Ticker = ticker;
            QuoteType = quoteType;
            SubscriptionType = subscriptionType;
            ClientObject = clientObject;
            Date = date;

            RequestId = s_previousRequestId++;
            Status = TWSSubscriptionStatus.New;
         }

        public override string ToString()
        {
            return String.Format("{0}: {1} ({2}) {3} {4}", SubscriptionTypeMsg, Ticker, RequestId, QuoteTypeMsg, StatusMsg);
        }

        // event fired when status is changed to Unsubscribed or Rejected
        public static event EventHandler<TWSTickEventArgs> OnSubscriptionEnded
        {
            add { s_subscriptionEndedEventHander += value; }
            remove { s_subscriptionEndedEventHander -= value; }
        }

        public void AddLogMsg(string msg)
        {
            lock (m_logLock)
            {
                m_log.Add(msg);
            }
        }
        public string[] GetLog()
        {
            lock (m_logLock)
            {
                return m_log.ToArray();
            }
        }
        private string QuoteTypeMsg
        {
            get
            {
                return Enum.GetName(typeof(QuoteType), QuoteType);
            }
        }
        private string SubscriptionTypeMsg
        {
            get
            {
                return Enum.GetName(typeof(TWSSubscription.TWSSubscriptionType), SubscriptionType);
            }
        }
        private string StatusMsg
        {
            get
            {
                return Enum.GetName(typeof(TWSSubscription.TWSSubscriptionStatus), Status);
            }
        }
    }
}
