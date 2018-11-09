using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gargoyle.Common;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
    public class TWSUtilities
    {
        #region Private fields
        private TWSEWrapper m_eWrapper;
        private EReader m_eReader;
        private string m_stopMessage = "";
        private int m_quoteType = REAL_TIME;
        private const int MAX_MESSAGES_PER_SECOND = 50;
        private int m_messageCount;

        private AutoResetEvent m_connectionStartWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent m_readerStartWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent m_readerStopWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent m_replayExecutionsWaitHandle = new AutoResetEvent(false);
        private AutoResetEvent m_corporateActionWaitHandle = new AutoResetEvent(false);

        private bool IsConnecting { get; set; }
        private bool IsReaderStarting { get; set; }
        private bool IsReplayingExecutions { get; set; }

        private bool IsReportReaderStarted { get { return IsReading && (ClientNumber == REPORT_READER); } }
        private bool IsQuoteReaderStarted { get { return IsReading && (ClientNumber == QUOTE_READER) ; } }
        private bool IsHistoricalReaderStarted { get { return IsReading && (ClientNumber == HISTORICAL_READER); } }
        private bool IsCorporateActionsReaderStarted { get { return IsReading && (ClientNumber == CORPORATE_ACTIONS_READER); } }

        private string ReaderType
        {
            get
            {
                switch (ClientNumber)
                {
                    case REPORT_READER:
                        return "ReportReader";
                    case QUOTE_READER:
                        return "QuoteReader";
                    case HISTORICAL_READER:
                        return "HistoricalReader";
                    case CORPORATE_ACTIONS_READER:
                        return "CorporateActionsReader";
                    default:
                        return "Reader";
                }
            }
        }
        #endregion

        #region Event handlers
        private event LoggingEventHandler m_infoEventHandler;
        private event LoggingEventHandler m_errorEventHandler;
        private event EventHandler<TWSReportEventArgs> m_reportEventHandler;
        private event EventHandler<TWSTickEventArgs> m_tickEventHandler;
        private event ServiceStoppedEventHandler m_readerStoppedEventHandler;
        private event EventHandler<FundamentalDataEventArgs> m_fundamentalDataEventHandler;
        private event EventHandler<HistoricalDataEventArgs> m_historicalDataEventHandler;
        private event EventHandler<TWSTickEventArgs> m_corporateActionEventHandler;

        // event fired when an exception occurs
        public event LoggingEventHandler OnError
        {
            add { m_errorEventHandler += value; }
            remove { m_errorEventHandler -= value; }
        }
        // event fired for logging
        public event LoggingEventHandler OnInfo
        {
            add { m_infoEventHandler += value; }
            remove { m_infoEventHandler -= value; }
        }
        // event fired when a report is received
        public event EventHandler<TWSReportEventArgs> OnReport
        {
            add { m_reportEventHandler += value; }
            remove { m_reportEventHandler -= value; }
        }
        // event fired when a tick is received
        public event EventHandler<TWSTickEventArgs> OnTick
        {
            add { m_tickEventHandler += value; }
            remove { m_tickEventHandler -= value; }
        }
        // event fired when fundamental data is received
        public event EventHandler<FundamentalDataEventArgs> OnFundamentalData
        {
            add { m_fundamentalDataEventHandler += value; }
            remove { m_fundamentalDataEventHandler -= value; }
        }
        // event fired when corporate action data is received
        public event EventHandler<TWSTickEventArgs> OnCorporateAction
        {
            add { m_corporateActionEventHandler += value; }
            remove { m_corporateActionEventHandler -= value; }
        }
        // event fired when historical data is received
        public event EventHandler<HistoricalDataEventArgs> OnHistoricalData
        {
            add { m_historicalDataEventHandler += value; }
            remove { m_historicalDataEventHandler -= value; }
        }
        // event fired when reader stops
        public event ServiceStoppedEventHandler OnReaderStopped
        {
            add { m_readerStoppedEventHandler += value; }
            remove { m_readerStoppedEventHandler -= value; }
        }
        #endregion

        #region Public Properties

        // valid client numbers
        public const int REPORT_READER = 1;
        public const int QUOTE_READER = 2;
        public const int HISTORICAL_READER = 3;
        public const int CORPORATE_ACTIONS_READER = 4;

        // quote types
        public const int REAL_TIME = 1;
        public const int DELAYED = 3;

        public string TWSIPAddress { get; private set; }
        public int TWSPort { get; private set; }
        public int ClientNumber { get; private set; }
        public int NextOrderId { get; private set; }

        public bool IsInitialized { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsReading { get; private set; }

        public int WaitMs { get; set; }
        public bool HadError { get; private set; }
        public string LastErrorMessage { get; private set; }
        #endregion

        #region Public Methods
        public TWSUtilities() 
        { 
            WaitMs = 10000;
            TWSSubscription.OnSubscriptionEnded += EWrapper_OnTWSTick;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Init(int clientNumber, string IPAddress = null, int? port = null, int? quoteType = null)
        {
            if (!IsInitialized)
            {
                try
                {
                    if (quoteType.HasValue)
                        m_quoteType = quoteType.Value;

                    switch (clientNumber)
                    {

                        case REPORT_READER:
                        case QUOTE_READER:
                        case HISTORICAL_READER:
                        case CORPORATE_ACTIONS_READER:
                            ClientNumber = clientNumber;
                            TWSIPAddress = IPAddress ?? "127.0.0.1";
                            TWSPort = port.HasValue ? port.Value : 7496;

                            m_eWrapper = new TWSEWrapper();
                            m_eWrapper.OnInfo += EWrapper_OnInfo;
                            m_eWrapper.OnError += EWrapper_OnError;
                            m_eWrapper.OnNotification += EWrapper_OnNotification;
                            m_eWrapper.OnConnectAck += EWrapper_OnConnectAck;
                            m_eWrapper.OnValidId += EWrapper_OnValidId;

                            IsInitialized = true;
                            Info(String.Format("TWSUtilities initialized for {0} - client number {1}", ReaderType, ClientNumber));
                            break;

                        default:
                            Info(String.Format("TWSUtilities not initialized - invalid client number {0}", clientNumber));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    IsInitialized = false;
                    Error(String.Format("Unable to initialize TWSUtilities for {0} - client number {1}", ReaderType, ClientNumber), ex);
                }
            }
            return IsInitialized;
        }

        public bool Connect()
        {
            if (!IsInitialized)
            {
                IsConnected = false;
                Info("TWSUtilities has not been initialized");
            }

            else if (!IsConnected)
            {
                try
                {
                    IsConnecting = true;
                    m_connectionStartWaitHandle.Reset();
                    Throttle();
                    Info(String.Format("Connecting to TWS at {0}:{1}", TWSIPAddress, TWSPort));
                    m_eWrapper.ClientSocket.eConnect(TWSIPAddress, TWSPort, ClientNumber);

                    bool timedOut = !WaitAny(WaitMs, m_connectionStartWaitHandle);

                    if (timedOut)
                    {
                        Info("Timeout attempting to connect to TWS");
                    }
                    else if (IsConnected)
                    {
                        Info("Connected to TWS");
                    }
                    else
                    {
                        Info("Unable to connect to TWS");
                    }
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    Error("Exception in Connect", ex);
                }
            }

            return IsConnected;
        }

        public bool StartReportReader()
        {

            if (ClientNumber != REPORT_READER)
            {
                Info("Invalid Client Number - to start report reader, must specify REPORT_READER as client number in the Init method");
                return false;
            }

            else if (!IsReading)
            {
                try
                {
                    m_eWrapper.OnCommissionReport += EWrapper_OnReport;
                    m_eWrapper.OnOpenOrder += EWrapper_OnReport;
                    m_eWrapper.OnExecDetails += EWrapper_OnReport;
                    m_eWrapper.OnExecDetailsEnd += EWrapper_OnExecDetailsEnd;

                    if (StartReader())
                    {
                        ReplayExecutions();
                    }
                    else
                    {
                        m_eWrapper.OnCommissionReport -= EWrapper_OnReport;
                        m_eWrapper.OnOpenOrder -= EWrapper_OnReport;
                        m_eWrapper.OnExecDetails -= EWrapper_OnReport;
                        m_eWrapper.OnExecDetailsEnd -= EWrapper_OnExecDetailsEnd;
                    }
                }
                catch (Exception ex)
                {
                    Error("Exception in StartReportReader", ex);
                }
            }

            return IsReading;
        }

        public bool StopReportReader(bool bReplay = false)
        {
            if (IsReportReaderStarted)
            {
                m_stopMessage = "ReportReader stop requested";

                if (bReplay)
                {
                    ReplayExecutions();
                }

                if (StopReader())
                {
                    m_eWrapper.OnCommissionReport -= EWrapper_OnReport;
                    m_eWrapper.OnOpenOrder -= EWrapper_OnReport;
                    m_eWrapper.OnExecDetails -= EWrapper_OnReport;
                    m_eWrapper.OnExecDetailsEnd -= EWrapper_OnExecDetailsEnd;
                }
            }
            return !IsReportReaderStarted;
        }

        public bool StartQuoteReader()
        {
            if (ClientNumber != QUOTE_READER)
            {
                Info("Invalid Client Number - to start quote reader, must specify QUOTE_READER as client number in the Init method");
                return false;
            }

            else if (!IsReading)
            {
                try
                {
                    Throttle();
                    m_eWrapper.ClientSocket.reqMarketDataType(m_quoteType);
                    m_eWrapper.OnMarketDataType += EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails += EWrapper_OnContractDetails;
                    m_eWrapper.OnTWSTick += EWrapper_OnTWSTick;
                     if (!StartReader())
                    {
                        m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                        m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                        m_eWrapper.OnTWSTick -= EWrapper_OnTWSTick;
                     }
                }
                catch (Exception ex)
                {
                    Error("Exception in StartQuoteReader", ex);
                }
            }

            return IsReading;
        }

        public bool StartHistoricalReader()
        {
            if (ClientNumber != HISTORICAL_READER)
            {
                Info("Invalid Client Number - to start quote reader, must specify HISTORICAL_READER as client number in the Init method");
                return false;
            }

            else if (!IsReading)
            {
                try
                {
                    Throttle();
                    m_eWrapper.ClientSocket.reqMarketDataType(m_quoteType);
                    m_eWrapper.OnMarketDataType += EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails += EWrapper_OnContractDetails;
                     m_eWrapper.OnFundamentalData += EWrapper_OnFundamentalData;
                    m_eWrapper.OnHistoricalData += EWrapper_OnHistoricalData;
                    if (!StartReader())
                    {
                        m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                        m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                        m_eWrapper.OnFundamentalData -= EWrapper_OnFundamentalData;
                        m_eWrapper.OnHistoricalData -= EWrapper_OnHistoricalData;
                    }
                }
                catch (Exception ex)
                {
                    Error("Exception in StartHistoricalReader", ex);
                }
            }

            return IsReading;
        }

        public bool StartCorporateActionsReader()
        {
            if (ClientNumber != CORPORATE_ACTIONS_READER)
            {
                Info("Invalid Client Number - to start quote reader, must specify CORPORATE_ACTIONS_READER as client number in the Init method");
                return false;
            }

            else if (!IsReading)
            {
                try
                {
                    Throttle();
                    m_eWrapper.ClientSocket.reqMarketDataType(m_quoteType);
                    m_eWrapper.OnMarketDataType += EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails += EWrapper_OnContractDetails;
                    m_eWrapper.OnTWSTick += EWrapper_OnTWSTick;
                    if (!StartReader())
                    {
                        m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                        m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                        m_eWrapper.OnTWSTick -= EWrapper_OnTWSTick;
                    }
                }
                catch (Exception ex)
                {
                    Error("Exception in StartCorporateActionsReader", ex);
                }
            }

            return IsReading;
        }

        public bool StopQuoteReader()
        {
            if (IsQuoteReaderStarted)
            {
                m_stopMessage = "QuoteReader stop requested";

                if (StopReader())
                {
                    m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                    m_eWrapper.OnTWSTick -= EWrapper_OnTWSTick;
                }
            }
            return !IsQuoteReaderStarted;
        }

        public bool StopHistoricalReader()
        {
            if (IsHistoricalReaderStarted)
            {
                m_stopMessage = "HistoricalReader stop requested";

                if (StopReader())
                {
                    m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                    m_eWrapper.OnFundamentalData -= EWrapper_OnFundamentalData;
                    m_eWrapper.OnHistoricalData -= EWrapper_OnHistoricalData;
                }
            }
            return !IsHistoricalReaderStarted;
        }

        public bool StopCorporateActionsReader()
        {
            if (IsCorporateActionsReaderStarted)
            {
                m_stopMessage = "CorporateActionsReade stop requested";

                if (StopReader())
                {
                    m_eWrapper.OnMarketDataType -= EWrapper_OnMarketDataType;
                    m_eWrapper.OnContractDetails -= EWrapper_OnContractDetails;
                    m_eWrapper.OnTWSTick -= EWrapper_OnTWSTick;
                }
            }
            return !IsCorporateActionsReaderStarted;
        }

        // returns request id or -1 on failure
        public int GetFundamentalData(string stock, string reportType)
        {
             try
            {
                if (!IsHistoricalReaderStarted)
                {
                    Info("Cannot get fundamental data - HistoricalReader not started");
                    return -1;
                }
                else
                {
                    Contract contract = new Contract()
                    {
                        Symbol = stock,
                        SecType = "STK",
                        Currency = "USD",
                        Exchange = "SMART"
                    };

                    TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(stock, QuoteType.Stock, TWSSubscription.TWSSubscriptionType.FundamentalData);
                    subscription.ReportType = reportType;
                    ProcessSubscription(contract, subscription);

                    //subscription.Contract = contract;
                    //subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData;
                    //RequestMarketData(subscription);
  
                    return subscription.RequestId;
                }
           }
            catch (Exception ex)
            {
                Error("Error getting fundamental data", ex);
                return -1;
            }
     }

        // returns request id or -1 on failure
        public int GetHistoricalData(string symbol, QuoteType quoteType, object clientObject=null, DateTime? date=null)
        {
            try
            {
                if (!IsHistoricalReaderStarted)
                {
                    Info("Cannot get historical data - HistoricalReader not started");
                    return -1;
                }
                else if (quoteType == QuoteType.Option)
                {
                    Info("Cannot get historical data for options");
                    return -1;
                }
                else
                {
                    Contract contract = ContractFactory.GetContract(symbol, quoteType);
                    TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(symbol, quoteType, TWSSubscription.TWSSubscriptionType.HistoricalData, clientObject, date);
                    ProcessSubscription(contract, subscription);

                    return subscription.RequestId;
                }
            }
            catch (Exception ex)
            {
                Error("Error getting historical data", ex);
                return -1;
            }
        }

        public bool SubscribeToStocks(KeyValuePair<string, object>[] stocks)
        {
            try
            {
                if (!IsQuoteReaderStarted)
                {
                    Info("Cannot subscribe to quotes - QuoteReader not started");
                    return false;
                }
                else
                {
                    foreach (var stock in stocks)
                    {
                        Contract contract = ContractFactory.GetContract(stock.Key, QuoteType.Stock);
                        TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(stock.Key, QuoteType.Stock, TWSSubscription.TWSSubscriptionType.TickData, stock.Value);
                        ProcessSubscription(contract, subscription);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error("Error subscribing to stocks", ex);
                return false;
            }
        }

        public bool SubscribeToOptions(KeyValuePair<string, object>[] options)
        {
            try
            {
                if (!IsQuoteReaderStarted)
                {
                    Info("Cannot subscribe to quotes - QuoteReader not started");
                    return false;
                }
                else
                {
                    foreach (var option in options)
                    {
                        Contract contract = ContractFactory.GetContract(option.Key, QuoteType.Option);
                        TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(option.Key, QuoteType.Option, TWSSubscription.TWSSubscriptionType.TickData, option.Value);
                        ProcessSubscription(contract, subscription);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error("Error subscribing to options", ex);
                return false;
            }
        }

        public bool SubscribeToFutures(KeyValuePair<string, object>[] futures)
        {
            try
            {
                if (!IsQuoteReaderStarted)
                {
                    Info("Cannot subscribe to quotes - QuoteReader not started");
                    return false;
                }
                else
                {
                    foreach (var future in futures)
                    {
                        Contract contract = ContractFactory.GetContract(future.Key, QuoteType.Future);
                        TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(future.Key, QuoteType.Future, TWSSubscription.TWSSubscriptionType.TickData, future.Value);
                        ProcessSubscription(contract, subscription);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error("Error subscribing to futures", ex);
                return false;
            }
        }

        public bool SubscribeToIndices(KeyValuePair<string, object>[] indices)
        {
            try
            {
                if (!IsQuoteReaderStarted)
                {
                    Info("Cannot subscribe to quotes - QuoteReader not started");
                    return false;
                }
                else
                {
                    foreach (var index in indices)
                    {
                        Contract contract = ContractFactory.GetContract(index.Key, QuoteType.Index);
                        TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(index.Key, QuoteType.Index, TWSSubscription.TWSSubscriptionType.TickData, index.Value);
                        ProcessSubscription(contract, subscription);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error("Error subscribing to indices", ex);
                return false;
            }
        }

        // returns corporate action data  by firing OnCorporation events 
        public bool GetCorporateActions(KeyValuePair<string, object>[] stocks)
        {
            try
            {
                if (!IsCorporateActionsReaderStarted)
                {
                    Info("Cannot get corporate actions - CorporateActionsReader not started");
                    return false;
                }
                else
                {
                    foreach (var stock in stocks)
                    {
                        Contract contract = ContractFactory.GetContract(stock.Key, QuoteType.Stock);
                        TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(stock.Key, QuoteType.Stock, TWSSubscription.TWSSubscriptionType.CorporateActionData, stock.Value);
                        ProcessSubscription(contract, subscription);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Error("Error getting corporate actions", ex);
                return false;
            }
        }

        // implementation of CorporateActionsLib.ICorporateActionsReader
        // returns corporate action info as xml string (and an int to indicate number of corporateactions returned)
        // startMinusOneDate is not used in this implementation
        public int GetCorporateActions(string symbol, DateTime? startMinusOneDate, out string xml)
        {
            xml = null;
            int recordCount = 0;

            try
            {
                OnCorporateAction += PopulateCorporateAction;

                if (!IsCorporateActionsReaderStarted)
                {
                    Info("Cannot get corporate actions - CorporateActionsReader not started");
                    return 0;
                }

                Contract contract = ContractFactory.GetContract(symbol, QuoteType.Stock);
                CorporateAction corporateAction = new CorporateAction(symbol);
                TWSSubscription subscription = TWSSubscriptionManager.GetOrCreateSubscription(symbol, QuoteType.Stock, TWSSubscription.TWSSubscriptionType.CorporateActionData, corporateAction);

                m_corporateActionWaitHandle.Reset();
                ProcessSubscription(contract, subscription);

                bool timedOut = !WaitAny(WaitMs, m_corporateActionWaitHandle);
                if (corporateAction.ExDate.HasValue)
                {
                    recordCount++;
                    xml = corporateAction.ToXml();
                }
                
            }
            catch (Exception ex)
            {
                Error("Error getting corporate actions", ex);
            }
            finally
            {
                OnCorporateAction -= PopulateCorporateAction;
            }

            return recordCount;
        }

        private void ProcessSubscription(Contract contract, TWSSubscription subscription)
        {
            // process subscription based on current status
            string msg;
            bool requestContract = false;
            bool requestMarketData = false;

            lock (subscription.StatusLock)
            {
                msg = "Processing subscription for " + subscription.ToString();

                switch (subscription.Status)
                {
                    // if this is a new subscription or old but inactive subscription:
                    //      if we have no contract, request one
                    //      if we do, request market data
                    case TWSSubscription.TWSSubscriptionStatus.New:
                    case TWSSubscription.TWSSubscriptionStatus.Rejected:
                    case TWSSubscription.TWSSubscriptionStatus.Unsubscribed:
                    case TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe:
                        if (subscription.Contract == null)
                        {
                            // request was already made if we are in AwaitingUnsubscribe state
                            requestContract = (subscription.Status != TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe);
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo;
                            msg += " - changing status to AwaitingContractInfo";
                        }
                        else
                        {
                            // request was already made if we are in AwaitingUnsubscribe state
                            requestMarketData = (subscription.Status != TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe);
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData;
                            msg += " - changing status to AwaitingMarketData";
                        }
                        break;

                    // in all other cases, do nothing
                    default:
                        msg += " - no processing necessary";
                        break;
                }
            }

            if (requestContract)
            {
                msg += " and requesting contract info";
                Throttle();
                m_eWrapper.ClientSocket.reqContractDetails(subscription.RequestId, contract);
            }

            if (requestMarketData)
            {
                msg += " and requesting market data";
                RequestMarketData(subscription);
            }

            subscription.AddLogMsg(msg);
            Info(msg);
        }

        private void RequestMarketData(TWSSubscription subscription)
        {
            Throttle();
            switch (subscription.SubscriptionType)
            {
                case TWSSubscription.TWSSubscriptionType.TickData:
                    m_eWrapper.ClientSocket.reqMktData(subscription.RequestId, subscription.Contract, null, false, false, null);
                    break;
                case TWSSubscription.TWSSubscriptionType.CorporateActionData:
                    string genericTickList = string.Format("{0}", TickType.IB_DIVIDENDS);
                    m_eWrapper.ClientSocket.reqMktData(subscription.RequestId, subscription.Contract, genericTickList, false, false, null);
                    break;
                case TWSSubscription.TWSSubscriptionType.HistoricalData:
                    // if no date is specified, we will get two days of data, ending today
                    string endDate = null;
                    string durationString = "2 D";

                    // if a date is specified, we will get only that day
                    if (subscription.Date.HasValue)
                    {
                        endDate = String.Format("{0:yyyyMMdd} 23:59:59 GMT", subscription.Date);
                        durationString = "1 D";
                    }
                    DateTime yesterday = DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0));
                    m_eWrapper.ClientSocket.reqHistoricalData(subscription.RequestId, subscription.Contract, endDate, durationString, "1 day", "TRADES", 1, 1, false, null);
                   break;
                case TWSSubscription.TWSSubscriptionType.FundamentalData:
                    m_eWrapper.ClientSocket.reqFundamentalData(subscription.RequestId, subscription.Contract, subscription.ReportType, null);
                    break;
            }
        }


        //private void GetContractInfo(Contract contract, TWSSubscription subscription)
        //{
        //    string msg;
        //    lock (subscription.StatusLock)
        //    {
        //        msg = String.Format("Contract info needed for {0}", subscription.ToString());
        //        switch (subscription.Status)
        //        {
        //            case TWSSubscription.TWSSubscriptionStatus.New:
        //            case TWSSubscription.TWSSubscriptionStatus.Rejected:
        //                msg += " - changing to AwaitingContractInfo and requesting contract info";
        //                subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo;
        //                Throttle();
        //                m_eWrapper.ClientSocket.reqContractDetails(subscription.RequestId, contract);
        //                break;
        //            case TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe:
        //                if (subscription.Contract == null)
        //                {
        //                    msg += " - changing to AwaitingContractInfo - already requested";
        //                    subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo;
        //                }
        //                else
        //                {
        //                    msg += " - changing to AwaitingMarketDataType - already received contract info";
        //                    subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData;
        //                }
        //                break;
        //            case TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo:
        //            case TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData:
        //            case TWSSubscription.TWSSubscriptionStatus.Subscribed:
        //                msg += " - ignoring request";
        //                break;
        //            case TWSSubscription.TWSSubscriptionStatus.Unsubscribed:
        //                msg += " - changing to AwaitingMarketDataType and requesting market data";
        //                subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData;
        //                Throttle();
        //                m_eWrapper.ClientSocket.reqMktData(subscription.RequestId, subscription.Contract, string.Empty, false, false, null);
        //                 break;
        //        }
        //    }
        //
        //    subscription.AddLogMsg(msg);
        //    Info(msg);
        //}

        private bool UnsubscribeAll()
        {
            try
            {
                return Unsubscribe(TWSSubscriptionManager.GetAllTickers());
            }
            catch (Exception ex)
            {
                Error("Error unsubscribing to all market data", ex);
                return false;
            }
       }

        public bool Unsubscribe(int[] requestIds)
        {
            try
            {
                if (IsReading)
                {
                    foreach (int requestId in requestIds)
                    {
                        string msg = string.Empty;
                        TWSSubscription subscription = TWSSubscriptionManager.GetById(requestId);
                        if (subscription != null)
                        {
                            Unsubscribe(subscription, ref msg);
                        }
                        else
                        {
                            msg = String.Format("Cannot unsubscribe from request {0} - request id not found", requestId);
                        }

                        Info(msg);
                    }
                }
                else
                {
                    Info(string.Format("Cannot unsubscribe - no connection"));
                }
                return true;
            }
            catch (Exception ex)
            {
                Error("Error unsubscribing to market data", ex);
                return false;
            }
        }

        public bool Unsubscribe(string[] symbols)
        {
            try
            {
                if (IsReading)
                {
                    foreach (string symbol in symbols)
                    {
                        string msg = string.Empty;
                        TWSSubscription subscription = TWSSubscriptionManager.GetByTicker(symbol);
                        if (subscription != null)
                        {
                            Unsubscribe(subscription, ref msg);
                        }
                        else
                        {
                            msg = String.Format("Cannot unsubscribe from {0} - ticker not found", symbol);
                        }

                        Info(msg);
                    }
                }
                else
                {
                    Info(string.Format("Cannot unsubscribe - no connection"));
                }
                return true;
            }
            catch (Exception ex)
            {
                Error("Error unsubscribing to market data", ex);
                return false;
            }
        }

        private void Unsubscribe(TWSSubscription subscription, ref string msg)
        {
            bool requestUnsubscribe = false;
            lock (subscription.StatusLock)
            {
                msg = String.Format("Unsubscribe request received for {0}", subscription.ToString());
                switch (subscription.Status)
                {
                    case TWSSubscription.TWSSubscriptionStatus.Subscribed:
                        requestUnsubscribe = true;
                        subscription.Status = TWSSubscription.TWSSubscriptionStatus.Unsubscribed;
                        msg += " - changing to Unsubscribed";
                        break;
                    // if we are awaiting data, we must wait until we receive it to mark ticker as unsubscribed
                    case TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData:
                    case TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo:
                        msg += " - changing to AwaitingUnsubscribe";
                        subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe;
                        break;
                    default:
                        msg += " - ignoring request";
                        break;
                }
            }

            if (requestUnsubscribe)
            {
                msg += " and unsubscribing";
                Throttle();
                m_eWrapper.ClientSocket.cancelMktData(subscription.RequestId);
            }

            subscription.AddLogMsg(msg);
        }
        #endregion

        #region EWrapper Event Handlers
        private void EWrapper_OnError(object sender, LoggingEventArgs e)
        {
            if (m_errorEventHandler != null)
                m_errorEventHandler(sender, e);

            if (IsConnecting)
            {
                IsConnecting = false;
                IsConnected = false;
                m_connectionStartWaitHandle.Set();
            }
            if (IsReaderStarting)
            {
                IsReaderStarting = false;
                IsReading = false;
                m_readerStartWaitHandle.Set();
            }
        }

        private void EWrapper_OnInfo(object sender, LoggingEventArgs e)
        {
            if (m_infoEventHandler != null)
                m_infoEventHandler(sender, e);
        }

        private void EWrapper_OnConnectAck(object sender, EventArgs e)
        {
            if (IsConnecting)
            {
                IsConnecting = false;
                IsConnected = true;
                m_connectionStartWaitHandle.Set();
            }
        }

        private void EWrapper_OnValidId(object sender, ValidIdEventArgs e)
        {
            NextOrderId = e.OrderId;
            Info(String.Format("Next Valid Id: {0}", NextOrderId));

            if (IsReaderStarting)
            {
                IsReaderStarting = false;
                IsReading = true;
                m_readerStartWaitHandle.Set();
            }
        }

        private void EWrapper_OnExecDetailsEnd(object sender, ExecDetailsEndEventArgs args)
        {
            if (IsReplayingExecutions)
            {
                IsReplayingExecutions = false;
                m_replayExecutionsWaitHandle.Set();
            }
        }

        private void EWrapper_OnReport(object sender, TWSReportEventArgs args)
        {
            try
            {
                if (m_reportEventHandler != null)
                    m_reportEventHandler(sender, args);
                else
                    Info(args.Report);
            }
            catch (Exception ex)
            {
                Error("Unhandled exception in Report event handler", ex);
            }
        }

        private void EWrapper_OnMarketDataType(object sender, MarketDataTypeEventArgs e)
        {
            string msg = e.ToString();
            bool requestUnsubscribe = false;

            TWSSubscription subscription = TWSSubscriptionManager.GetById(e.RequestId);
            if (subscription != null)
            {
                lock (subscription.StatusLock)
                {
                    msg += String.Format(", {0}", subscription.ToString());
                    switch (subscription.Status)
                    {
                        case TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData:
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.Subscribed;
                            msg += " - changing status to Subscribed";
                            break;
                        case TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe:
                            requestUnsubscribe = true;
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.Unsubscribed;
                            msg += " - changing status to Unsubscribed";
                            break;
                        default:
                            msg += " - ***ERROR*** expected AwaitingMarketData or AwaitingUnsubscribe";
                            break;
                    }
                }
            }
            else
            {
                msg += " - ***ERROR*** ticker id unknown";
            }

            if (requestUnsubscribe)
            {
                msg += " and canceling subscription";
                Throttle();
                m_eWrapper.ClientSocket.cancelMktData(subscription.RequestId);
            }

            if (subscription != null)
            {
                subscription.AddLogMsg(msg);
            }
            Info(msg);
        }

        void EWrapper_OnNotification(object sender, NotificationEventArgs e)
        {
            string msg = e.ToString();

            // shut down on code 1100
            if (e.Code == 1100)
            {
                msg += " - shutting down";
                IsReading = false;
                Stop(e.Message);
            }

            else if (e.Id >= 0)
            {
                TWSSubscription subscription = TWSSubscriptionManager.GetById(e.Id);
                if (subscription != null)
                {
                    msg += String.Format(", {0}", subscription.ToString());
                    switch (subscription.SubscriptionType)
                    {
                        case TWSSubscription.TWSSubscriptionType.HistoricalData:
                            HistoricalDataEventArgs historicalEventArgs = new HistoricalDataEventArgs(e.Id, null, e.Message);
                            EWrapper_OnHistoricalData(sender, historicalEventArgs);
                            break;

                        case TWSSubscription.TWSSubscriptionType.FundamentalData:
                            FundamentalDataEventArgs fundatmentalEventArgs = new FundamentalDataEventArgs(e.Id, e.Message);
                            EWrapper_OnFundamentalData(sender, fundatmentalEventArgs);
                            break;

                        case TWSSubscription.TWSSubscriptionType.TickData:
                            switch (e.Code)
                            {
                                case 101:
                                    msg += " - changing status to Rejected";
                                    subscription.Status = TWSSubscription.TWSSubscriptionStatus.Rejected;
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }

                }
                else
                {
                    msg += " - ***ERROR*** ticker id unknown";
                }
            }
 
            Info(msg);
         }

        void EWrapper_OnContractDetails(object sender, ContractDetailsEventArgs e)
        {
            string msg = String.Format("ContractDetails: Id={0}", e.RequestId);
            bool requestMarketData = false;

            TWSSubscription subscription = TWSSubscriptionManager.GetById(e.RequestId);
            if (subscription != null)
            {
                lock (subscription.StatusLock)
                {
                    msg += String.Format(", {0}", subscription.ToString());
                    subscription.Contract = e.Contract;
                    switch (subscription.Status)
                    {
                        // if we were awaiting contract info, proceed to requesting market data
                        case TWSSubscription.TWSSubscriptionStatus.AwaitingContractInfo:
                            requestMarketData = true;
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.AwaitingMarketData;
                            msg += " - changing status to AwaitingMarketData";
                            break;

                        // if we have received an unsubscribe request, mark ticker as unsubscribed
                        case TWSSubscription.TWSSubscriptionStatus.AwaitingUnsubscribe:
                            subscription.Status = TWSSubscription.TWSSubscriptionStatus.Unsubscribed;
                            msg += " - changing status to Unsubscribed";
                            break;

                        default:
                            msg += " - ***ERROR*** expected AwaitingContractInfo or AwaitingUnsubscribe";
                            break;
                    }
                }
            }
            else
            {
                msg += " - ***ERROR*** request id unknown";
            }

            if (requestMarketData)
            {
                msg += " and requesting market data";
                RequestMarketData(subscription);
            }

            if (subscription != null)
            {
                subscription.AddLogMsg(msg);
            }
            Info(msg);
        }

        void PopulateCorporateAction(object senter, TWSTickEventArgs e)
        {
            try
            {
                if (e.ClientObject != null)
                {
                    if (e.ClientObject.GetType() == typeof(CorporateAction))
                    {
                        CorporateAction corporateAction = (CorporateAction)e.ClientObject;
                        corporateAction.ExDate = e.ExDate;
                        corporateAction.Dividend = e.Dividend;
                        corporateAction.DividendsPastYear = e.DividendsPastYear;
                        corporateAction.DividendsNextYear = e.DividendsNextYear;

                        m_corporateActionWaitHandle.Set();
                    }
                }
 
            }
            catch (Exception ex)
            {
                Error("Error in OnCorporateActions event handler", ex);
            }
        }

        void EWrapper_OnTWSTick(object sender, TWSTickEventArgs e)
        {
            try
            {
                switch (e.SubscriptionType)
                {
                    case TWSSubscription.TWSSubscriptionType.CorporateActionData:
                        if (e.HasDividends)
                        {
                            if (m_corporateActionEventHandler != null)
                            {
                                m_corporateActionEventHandler(sender, e);
                            }
                            else
                                Info("Corporate action event: " + e.ToString());

                            Info(string.Format("Removing subscription, requestid = {0}", e.RequestId));
                            Unsubscribe(new int[] { e.RequestId });
                            TWSSubscriptionManager.RemoveSubscription(e.RequestId);
                        }
                        break;

                    case TWSSubscription.TWSSubscriptionType.TickData:
                        if (m_tickEventHandler != null)
                        {
                            m_tickEventHandler(sender, e);
                        }
                        else
                            Info("Tick event: " + e.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                Error("Unhandled exception in Tick event handler", ex);
            }
        }
 
        void EWrapper_OnFundamentalData(object sender, FundamentalDataEventArgs e)
        {
            try
            {
                if (m_fundamentalDataEventHandler != null)
                {
                    m_fundamentalDataEventHandler(sender, e);
                }
                else
                    Info(e.ToString());

                Info(string.Format("Removing subscription, requestid = {0}", e.RequestId));
                TWSSubscriptionManager.RemoveSubscription(e.RequestId);
            }
            catch (Exception ex)
            {
                Error("Unhandled exception in Fundamental Data event handler", ex);
            }
        }

        void EWrapper_OnHistoricalData(object sender, HistoricalDataEventArgs e)
        {
            try
            {
                e.UpdateSubscriptionInfo(TWSSubscriptionManager.GetById(e.RequestId));
                if (m_historicalDataEventHandler != null)
                {
                    m_historicalDataEventHandler(sender, e);
                }
                else
                    Info(e.ToString());

                if (e.Bar == null) // signals end of historical data
                {
                    Info(string.Format("Removing subscription, requestid = {0}", e.RequestId));
                    TWSSubscriptionManager.RemoveSubscription(e.RequestId);
                }
            }
            catch (Exception ex)
            {
                Error("Unhandled exception in Historical Data event handler", ex);
            }
        }
        #endregion

       #region Private and internal Methods
        internal static bool TryParseTWSDate(string dateString, out DateTime date)
        {
            date = DateTime.MinValue;
            if (dateString != null)
            {
                if (dateString.Length >= 8)
                    return DateTime.TryParse(dateString.Insert(6, "/").Insert(4, "/"), out date);
            }
            return false;
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop("Disposing");

                if (m_connectionStartWaitHandle != null)
                {
                    m_connectionStartWaitHandle.Dispose();
                    m_connectionStartWaitHandle = null;
                }
                if (m_readerStartWaitHandle != null)
                {
                    m_readerStartWaitHandle.Dispose();
                    m_readerStartWaitHandle = null;
                }
                if (m_readerStopWaitHandle != null)
                {
                    m_readerStopWaitHandle.Dispose();
                    m_readerStopWaitHandle = null;
                }
                if (m_replayExecutionsWaitHandle != null)
                {
                    m_replayExecutionsWaitHandle.Dispose();
                    m_replayExecutionsWaitHandle = null;
                }
                if (m_corporateActionWaitHandle != null)
                {
                    m_corporateActionWaitHandle.Dispose();
                    m_corporateActionWaitHandle = null;
                }
            }
        }

        protected void Info(string msg)
        {
            if (m_infoEventHandler != null)
            {
                m_infoEventHandler(this, new LoggingEventArgs("TWS Utilities", msg));
            }
        }
   
        protected void Error(string msg, Exception ex)
        {
            if (m_errorEventHandler != null)
            {
                m_errorEventHandler(this, new LoggingEventArgs("TWS Utilities", msg, ex));
            }
        }

        private bool WaitAny(int millisecondsTimeout, params System.Threading.WaitHandle[] successConditionHandles)
        {
            int n;
            if (millisecondsTimeout == 0)
                n = System.Threading.WaitHandle.WaitAny(successConditionHandles);
            else
                n = System.Threading.WaitHandle.WaitAny(successConditionHandles, millisecondsTimeout);
            if (n == System.Threading.WaitHandle.WaitTimeout)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool StartReader()
        {
            try
            {
                if (!IsConnected)
                {
                    Info("TWSUtilities is not connected");
                }

                else if (!IsReading)
                {
                    m_eReader = new EReader(m_eWrapper.ClientSocket, m_eWrapper.ReaderSignal);
                    m_eReader.Start();

                    BackgroundWorker readerProcess = new BackgroundWorker();
                    readerProcess.DoWork += new DoWorkEventHandler(ReaderWorker);

                    IsReaderStarting = true;
                    m_readerStartWaitHandle.Reset();
                    readerProcess.RunWorkerAsync();
                    bool timedOut = !WaitAny(WaitMs, m_readerStartWaitHandle);

                    if (IsReading) // will have been set by EWrapper_OnValidId callback if successful
                    {
                        Info(ReaderType + " started");
                    }
                    else
                    {
                        Info("Unable to start " + ReaderType + (timedOut ? " - timed out" : ""));
                    }
                }
            }
            catch (Exception ex)
            {
                IsReading = false;
                Error("Exception in StartReader", ex);
            }

            return IsReading;
        }

        private bool StopReader()
        {
            try
            {
                if (IsReading)
                {
                    UnsubscribeAll();
                    m_readerStopWaitHandle.Reset();
                    m_eWrapper.ClientSocket.eDisconnect();

                    bool timedOut = !WaitAny(WaitMs, m_readerStopWaitHandle);
                    if (!IsReading)
                    {
                        Info(ReaderType + " stopped");
                    }
                    else
                    {
                        Info("Unable to stop " + ReaderType + (timedOut ? " - timed out" : ""));
                    }
                }
            }
            catch (Exception ex)
            {
                Error("Exception in StopReader", ex);
            }
            return !IsReading;
        }

        private void ReplayExecutions()
        {
            try
            {
                if (!IsReplayingExecutions)
                {
                    BackgroundWorker replayProcess = new BackgroundWorker();
                    replayProcess.DoWork += new DoWorkEventHandler(ReplayWorker);

                    IsReplayingExecutions = true;
                    m_replayExecutionsWaitHandle.Reset();
                    replayProcess.RunWorkerAsync();
                }
                bool timedOut = !WaitAny(300000, m_replayExecutionsWaitHandle); // five-minute wait

                if (timedOut)
                {
                    Info("Timeout attempting to replay executions");
                }
                else if (IsReading)
                {
                    Info("Execution replay ended");
                }
                else
                {
                    Info("Unable to replay executions");
                }
            }
            catch (Exception ex)
            {
                Error("Exception in ReplayExecutions", ex);
            }
        }

        private void ReaderWorker(object o, DoWorkEventArgs args)
        {
            try
            {
                Info("ReaderWorker thread started");

                m_stopMessage = "TWS disconnected";
                while (m_eWrapper.ClientSocket.IsConnected())
                {
                    try
                    {
                        m_eWrapper.ReaderSignal.waitForSignal();
                        m_eReader.processMsgs();
                    }
                    catch (Exception ex)
                    {
                        Error("Error in ReaderWorker thread - continuing processing", ex);
                    }
                }

                IsReading = false;
                IsConnected = false;
                if (m_readerStoppedEventHandler != null)
                    m_readerStoppedEventHandler(null, new ServiceStoppedEventArgs("TWSReader", m_stopMessage));
                else
                    Info("ReaderWorker thread stopped");
            }
            catch (Exception ex)
            {
                IsReading = false;
                Error("Error in ReaderWorker thread", ex);
                if (m_readerStoppedEventHandler != null)
                    m_readerStoppedEventHandler(null, new ServiceStoppedEventArgs("TWSReader", ex.Message, ex));
            }
            finally
            {
                m_readerStopWaitHandle.Set();
            }
        }

        private void Stop(string message)
        {
 
            if (ClientNumber == REPORT_READER)
                StopReportReader();
            else
                StopQuoteReader();

             if (m_readerStoppedEventHandler != null)
                m_readerStoppedEventHandler(null, new ServiceStoppedEventArgs("TWSReader", message));

        }

        private void ReplayWorker(object o, DoWorkEventArgs args)
        {
            try
            {
                Info("Replaying executions");

                if (m_eWrapper.ClientSocket.IsConnected())
                {
                    Throttle();
                    m_eWrapper.ClientSocket.reqExecutions(1, new ExecutionFilter());
                }
            }
            catch (Exception ex)
            {
                Error("Error in ReplayWorker thread", ex);
            }
        }

        private object m_throttleLock = new object();
  
        private void Throttle()
        {
            lock (m_throttleLock)
            {
                if ((++m_messageCount % MAX_MESSAGES_PER_SECOND) == 0)
                {
                    Info("Waiting one second before sending next message");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        #endregion
    }
}
