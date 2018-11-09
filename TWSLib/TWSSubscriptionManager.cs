using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
    public class TWSSubscriptionManager
    {
         protected static Dictionary<int, TWSSubscription> s_subscriptionMap = new Dictionary<int, TWSSubscription>();
         protected static Dictionary<string, TWSSubscription> s_tickerMap = new Dictionary<string, TWSSubscription>();
        protected static object s_lockObject = new object();

        #region Public Static Methods
         public static TWSSubscription GetById(int requestId)
        {
            TWSSubscription subscription = null;
            lock (s_lockObject)
            {
                s_subscriptionMap.TryGetValue(requestId, out subscription);
            }
            return subscription;
        }

         public static TWSSubscription GetByTicker(string ticker)
         {
             TWSSubscription subscription = null;
             lock (s_lockObject)
             {
                 s_tickerMap.TryGetValue(ticker, out subscription);
             }
             return subscription;
         }

         public static TWSSubscription GetOrCreateSubscription(string ticker, QuoteType quoteType, TWSSubscription.TWSSubscriptionType subscriptionType, object clientObject=null, DateTime? date=null)
         {
             TWSSubscription oldSubscription = null;
             TWSSubscription newSubscription = null;
             lock (s_lockObject)
             {
                 oldSubscription = GetByTicker(ticker);

                 // for tick data, we reuse subscriptions; for other data, we always create a new subscription
                 if ((subscriptionType == TWSSubscription.TWSSubscriptionType.TickData) && (oldSubscription != null))
                 {
                     newSubscription = oldSubscription;
                 }

                 else
                 {
                     newSubscription = new TWSSubscription(ticker, quoteType, subscriptionType, clientObject, date);

                     // if we already have a contract, we can use it
                     if (oldSubscription != null)
                     {
                         newSubscription.Contract = oldSubscription.Contract;
                     }

                     // subscription map is for all subscriptions
                     s_subscriptionMap.Add(newSubscription.RequestId, newSubscription);

                     // ticker map is for tick data only
                     if (subscriptionType == TWSSubscription.TWSSubscriptionType.TickData)
                     {
                         s_tickerMap.Add(ticker, newSubscription);
                     }
                 }
             }

             return newSubscription;
         }

        public static TWSSubscription RemoveSubscription(string ticker)
        {
            TWSSubscription subscription = null;
            lock (s_lockObject)
            {
                subscription = GetByTicker(ticker);
                if (subscription != null)
                {
                    s_tickerMap.Remove(ticker);
                    s_subscriptionMap.Remove(subscription.RequestId);
                }
            }
            return subscription;
        }

        public static TWSSubscription RemoveSubscription(int requestId)
        {
            TWSSubscription subscription = null;
            lock (s_lockObject)
            {
                subscription = GetById(requestId);
                if (subscription != null)
                {
                    s_subscriptionMap.Remove(subscription.RequestId);
                    if (subscription.SubscriptionType == TWSSubscription.TWSSubscriptionType.TickData)
                    {
                        s_tickerMap.Remove(subscription.Ticker);
                    }
                }
            }
            return subscription;
        }

        public static string[] GetAllTickers()
        {
            lock (s_lockObject)
            {
                return s_tickerMap.Keys.ToArray<string>();
            }
        }

        public static TWSSubscription[] GetAllSubscriptions()
        {
            lock (s_lockObject)
            {
                return s_subscriptionMap.Values.ToArray<TWSSubscription>();
            }
        }
        #endregion

    }
}
