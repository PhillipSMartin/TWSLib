using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Common;
using IBApi;

namespace TWSLib
{
    // class to handle communication between the client and TWS
    public class TWSEWrapper : EWrapper
    {
        private event LoggingEventHandler m_infoEventHandler;
        private event LoggingEventHandler m_errorEventHandler;
        private event EventHandler m_connectAckEventHandler;
        private event EventHandler m_connectionClosedEventHandler;
        private event EventHandler<TWSReportEventArgs> m_commissionReportEventHandler;
        private event EventHandler<TWSReportEventArgs> m_execDetailsEventHandler;
        private event EventHandler<ExecDetailsEndEventArgs> m_execDetailsEndEventHandler;
        private event EventHandler<TWSReportEventArgs> m_openOrderEventHandler;
        private event EventHandler<TWSOrderStatusEventArgs> m_orderStatusEventHandler;
        private event EventHandler<EventArgs> m_openOrderEndEventHandler;
        private event EventHandler<ValidIdEventArgs> m_validIdEventHandler;
        private event EventHandler<ManagedAccountsEventArgs> m_managedAccountsEventHandler;
        private event EventHandler<TWSTickEventArgs> m_twsTickEventHandler;
        private event EventHandler<TickSnapshotEndEventArgs> m_tickSnapshotEndEventHandler;
        private event EventHandler<NotificationEventArgs> m_notificationEventHandler;
        private event EventHandler<TickReqParamsEventArgs> m_tickReqParamsEventHandler;
        private event EventHandler<MarketDataTypeEventArgs> m_marketDataTypeEventHandler;
        private event EventHandler<ContractDetailsEventArgs> m_contractDetailsEventHandler;
        private event EventHandler<ContractDetailsEndEventArgs> m_contractDetailsEndEventHandler;
        private event EventHandler<FundamentalDataEventArgs> m_fundamentalDataEventHandler;
        private event EventHandler<HistoricalDataEventArgs> m_historicalDataEventHandler;

        public TWSEWrapper()
        {
            ReaderSignal = new EReaderMonitorSignal();
            ClientSocket = new EClientSocket(this, ReaderSignal);
        }

        #region Public Properties
        public EClientSocket ClientSocket { get; private set; }   // sends messages to TWS
        public EReaderSignal ReaderSignal { get; private set; }   // signals that a message is in the queue and is ready for processing
        #endregion

        #region Event Handlers
        public event LoggingEventHandler OnInfo
        {
            add { m_infoEventHandler += value; }
            remove { m_infoEventHandler -= value; }
        }
        public event LoggingEventHandler OnError
        {
            add { m_errorEventHandler += value; }
            remove { m_errorEventHandler -= value; }
        }
        public event EventHandler OnConnectAck
        {
            add { m_connectAckEventHandler += value; }
            remove { m_connectAckEventHandler -= value; }
        }
        public event EventHandler OnConnectionClosed
        {
            add { m_connectionClosedEventHandler += value; }
            remove { m_connectionClosedEventHandler -= value; }
        }
        public event EventHandler<TWSReportEventArgs> OnCommissionReport
        {
            add { m_commissionReportEventHandler += value; }
            remove { m_commissionReportEventHandler -= value; }
        }
        public event EventHandler<TWSReportEventArgs> OnExecDetails
        {
            add { m_execDetailsEventHandler += value; }
            remove { m_execDetailsEventHandler -= value; }
        }
        public event EventHandler<ExecDetailsEndEventArgs> OnExecDetailsEnd
        {
            add { m_execDetailsEndEventHandler += value; }
            remove { m_execDetailsEndEventHandler -= value; }
        }
        public event EventHandler<TWSReportEventArgs> OnOpenOrder
        {
            add { m_openOrderEventHandler += value; }
            remove { m_openOrderEventHandler -= value; }
        }
        public event EventHandler<TWSOrderStatusEventArgs> OnOrderStatus
        {
            add { m_orderStatusEventHandler += value; }
            remove { m_orderStatusEventHandler -= value; }
        }
        public event EventHandler<EventArgs> OnOpenOrderEnd
        {
            add { m_openOrderEndEventHandler += value; }
            remove { m_openOrderEndEventHandler -= value; }
        }
        public event EventHandler<ValidIdEventArgs> OnValidId
        {
            add { m_validIdEventHandler += value; }
            remove { m_validIdEventHandler -= value; }
        }
        public event EventHandler<ManagedAccountsEventArgs> OnManagedAccounts
        {
            add { m_managedAccountsEventHandler += value; }
            remove { m_managedAccountsEventHandler -= value; }
        }
        public event EventHandler<TWSTickEventArgs> OnTWSTick
        {
            add { m_twsTickEventHandler += value; }
            remove { m_twsTickEventHandler -= value; }
        }
        public event EventHandler<TickSnapshotEndEventArgs> OnTickSnapshotEnd
        {
            add { m_tickSnapshotEndEventHandler += value; }
            remove { m_tickSnapshotEndEventHandler -= value; }
        }
        public event EventHandler<NotificationEventArgs> OnNotification
        {
            add { m_notificationEventHandler += value; }
            remove { m_notificationEventHandler -= value; }
        }
        public event EventHandler<TickReqParamsEventArgs> OnTickReqParams
        {
            add { m_tickReqParamsEventHandler += value; }
            remove { m_tickReqParamsEventHandler -= value; }
        }
        public event EventHandler<MarketDataTypeEventArgs> OnMarketDataType
        {
            add { m_marketDataTypeEventHandler += value; }
            remove { m_marketDataTypeEventHandler -= value; }
        }
        public event EventHandler<ContractDetailsEventArgs> OnContractDetails
        {
            add { m_contractDetailsEventHandler += value; }
            remove { m_contractDetailsEventHandler -= value; }
        }
        public event EventHandler<ContractDetailsEndEventArgs> OnContractDetailsEnd
        {
            add { m_contractDetailsEndEventHandler += value; }
            remove { m_contractDetailsEndEventHandler -= value; }
        }
        public event EventHandler<FundamentalDataEventArgs> OnFundamentalData
        {
            add { m_fundamentalDataEventHandler += value; }
            remove { m_fundamentalDataEventHandler -= value; }
        }
        public event EventHandler<HistoricalDataEventArgs> OnHistoricalData
        {
            add { m_historicalDataEventHandler += value; }
            remove { m_historicalDataEventHandler -= value; }
        }

        
        #endregion

        #region Private Methods
        private void Info(string msg)
        {
            if (m_infoEventHandler != null)
            {
                m_infoEventHandler(this, new LoggingEventArgs("TWSEWrapper", msg));
            }
        }
        private void Error(string msg, Exception ex)
        {
            if (m_errorEventHandler != null)
            {
                m_errorEventHandler(this, new LoggingEventArgs("TWSEWrapper", msg, ex));
            }
        }
        #endregion

        #region EWrapper Members

        public void accountDownloadEnd(string account)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in accountDownloadEnd", ex); }
        }

        public void accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in accountSummary", ex); }
        }

        public void accountSummaryEnd(int reqId)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in accountSummaryEnd", ex); }
        }

        public void accountUpdateMulti(int requestId, string account, string modelCode, string key, string value, string currency)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in accountUpdateMulti", ex); }
        }

        public void accountUpdateMultiEnd(int requestId)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in accountUpdateMultiEnd", ex); }
        }

        public void bondContractDetails(int reqId, ContractDetails contract)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in bondContractDetails", ex); }
        }

        public void commissionReport(CommissionReport commissionReport)
        {
            try
            {
                if (m_commissionReportEventHandler != null)
                    m_commissionReportEventHandler(this, new TWSReportEventArgs(commissionReport));
            }
            catch (Exception ex) { Error("Exception in commissionReport callback", ex); }
        }

        public void connectAck()
        {
            try
            {
                if (m_connectAckEventHandler != null)
                    m_connectAckEventHandler(this, new EventArgs());
                else
                    Info("Ack received");

                if (ClientSocket.AsyncEConnect)
                    ClientSocket.startApi();
            }
            catch (Exception ex) { Error("Exception in connectAck callback", ex); }
        }

        public void connectionClosed()
        {
            try
            {
                if (m_connectionClosedEventHandler != null)
                    m_connectionClosedEventHandler(this, new EventArgs());
                else
                    Info("Connection closed");
            }
            catch (Exception ex) { Error("Exception in connectionClosed callback", ex); }
        }

        public void contractDetails(int reqId, ContractDetails contractDetails)
        {
            try
            {
                ContractDetailsEventArgs args = new ContractDetailsEventArgs(reqId, contractDetails);
                if (m_contractDetailsEventHandler != null)
                    m_contractDetailsEventHandler(this, args);
                else
                    Info(args.ToString());
            }
            catch (Exception ex) { Error("Exception in contractDetails callback", ex); }
        }

        public void contractDetailsEnd(int reqId)
        {
            try
            {
                if (m_contractDetailsEndEventHandler != null)
                    m_contractDetailsEndEventHandler(this, new ContractDetailsEndEventArgs(reqId));
                else
                    Info(String.Format("ContractDetailsEnd: {0}", reqId));
            }
            catch (Exception ex) { Error("Exception in contractDetailsEnd callback", ex); }
        }

        public void currentTime(long time)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in currentTime", ex); }
        }

        public void deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in deltaNeutralValidation", ex); }
        }

        public void displayGroupList(int reqId, string groups)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in displayGroupList", ex); }
        }

        public void displayGroupUpdated(int reqId, string contractInfo)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in displayGroupUpdated", ex); }
        }

        public void error(int id, int errorCode, string errorMsg)
        {
            try
            {
                NotificationEventArgs args = new NotificationEventArgs(id, errorCode, errorMsg);
                if (m_notificationEventHandler != null)
                    m_notificationEventHandler(this, args);
                else
                    Info(args.ToString());
            }
            catch (Exception ex) { Error("Exception in error callback", ex); }
        }

        public void error(string str)
        {
            Info("Error in TWSEWrapper: " + str);
        }

        public void error(Exception e)
        {
            Error("Error in TWSEWrapper", e);
        }

        public void execDetails(int reqId, Contract contract, Execution execution)
        {
            try
            {
                if (m_execDetailsEventHandler != null)
                    m_execDetailsEventHandler(this, new TWSReportEventArgs(reqId, contract, execution));
            }
            catch (Exception ex) { Error("Exception in execDetails callback", ex); }
        }

        public void execDetailsEnd(int reqId)
        {
            try
            {
                if (m_execDetailsEndEventHandler != null)
                    m_execDetailsEndEventHandler(this, new ExecDetailsEndEventArgs(reqId));
            }
            catch (Exception ex) { Error("Exception in execDetailsEnd callback", ex); }
        }

        public void familyCodes(FamilyCode[] familyCodes)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in familyCodes", ex); }
        }

        public void fundamentalData(int reqId, string data)
        {

            try
            {
                if (m_fundamentalDataEventHandler != null)
                    m_fundamentalDataEventHandler(this, new FundamentalDataEventArgs(reqId, data));
            }
            catch (Exception ex) { Error("Exception in fundamentalData", ex); }
        }

        public void headTimestamp(int reqId, string headTimestamp)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in headTimestamp", ex); }
        }

        public void histogramData(int reqId, HistogramEntry[] data)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in histogramData", ex); }
        }

        public void historicalData(int reqId, Bar bar)
        {
            try
            {
                if (m_historicalDataEventHandler != null)
                    m_historicalDataEventHandler(this, new HistoricalDataEventArgs(reqId, bar));
            }
            catch (Exception ex) { Error("Exception in historicalData", ex); }
        }

        public void historicalDataEnd(int reqId, string start, string end)
        {
            try
            {
                if (m_historicalDataEventHandler != null)
                    m_historicalDataEventHandler(this, new HistoricalDataEventArgs(reqId, null));
            }
            catch (Exception ex) { Error("Exception in historicalDataEnd", ex); }
        }

        public void historicalDataUpdate(int reqId, Bar bar)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalDataUpdate", ex); }
        }

        public void historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalNews", ex); }
        }

        public void historicalNewsEnd(int requestId, bool hasMore)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalNewsEnd", ex); }
        }

        public void historicalTicks(int reqId, HistoricalTick[] ticks, bool done)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalTicks", ex); }
        }

        public void historicalTicksBidAsk(int reqId, HistoricalTickBidAsk[] ticks, bool done)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalTicksBidAsk", ex); }
        }

        public void historicalTicksLast(int reqId, HistoricalTickLast[] ticks, bool done)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in historicalTicksLast", ex); }
        }

        public void managedAccounts(string accountsList)
        {
            try
            {
                if (m_managedAccountsEventHandler != null)
                    m_managedAccountsEventHandler(this, new ManagedAccountsEventArgs(accountsList));
                else
                    Info(String.Format("Managed Accounts: {0}", accountsList));
            }
            catch (Exception ex) { Error("Exception in managedAccounts callback", ex); }
        }

        public void marketDataType(int reqId, int marketDataType)
        {
            try
            {
                MarketDataTypeEventArgs args = new MarketDataTypeEventArgs(reqId, marketDataType);
                if (m_marketDataTypeEventHandler != null)
                    m_marketDataTypeEventHandler(this, args);
                else
                    Info(args.ToString());
            }
            catch (Exception ex) { Error("Exception in marketDataType callback", ex); }
        }

        public void marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in marketRule", ex); }
        }

        public void mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in mktDepthExchanges", ex); }
        }

        public void newsArticle(int requestId, int articleType, string articleText)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in newsArticle", ex); }
        }

        public void newsProviders(NewsProvider[] newsProviders)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in newsProviders", ex); }
        }

        public void nextValidId(int orderId)
        {
            try
            {
                if (m_validIdEventHandler != null)
                    m_validIdEventHandler(this, new ValidIdEventArgs(orderId));
                else
                    Info(String.Format("Next Valid Id: {0}", orderId));
            }
            catch (Exception ex) { Error("Exception in nextValidId callback", ex); }
        }

        public void openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            try
            {
                TWSReportEventArgs args = new TWSReportEventArgs(contract, order, orderState);
                if (m_openOrderEventHandler != null)
                    m_openOrderEventHandler(this, args);
                else
                    Info(args.Report);
            }
            catch (Exception ex) { Error("Exception in openOrder callback", ex); }

        }

        public void openOrderEnd()
        {
            try
            {
                if (m_openOrderEndEventHandler != null)
                    m_openOrderEndEventHandler(this, new EventArgs());
            }
            catch (Exception ex) { Error("Exception in openOrderEnd callback", ex); }
        }

        public void orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld, double mktCapPrice)
        {
            try
            {
                TWSOrderStatusEventArgs args = new TWSOrderStatusEventArgs(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld, mktCapPrice);
                if (m_orderStatusEventHandler != null)
                    m_orderStatusEventHandler(this, args);
                else
                    Info(args.ToString());
            }
            catch (Exception ex) { Error("Exception in orderStatus callback", ex); }
        }

        public void pnl(int reqId, double dailyPnL, double unrealizedPnL, double realizedPnL)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in pnl", ex); }
        }

        public void pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double realizedPnL, double value)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in pnlSingle", ex); }
        }

        public void position(string account, Contract contract, double pos, double avgCost)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in position", ex); }
        }

        public void positionEnd()
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in positionEnd", ex); }
        }

        public void positionMulti(int requestId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in positionMulti", ex); }
        }

        public void positionMultiEnd(int requestId)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in positionMultiEnd", ex); }
        }

        public void realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in realtimeBar", ex); }
        }

        public void receiveFA(int faDataType, string faXmlData)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in receiveFA", ex); }
        }

        public void rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in rerouteMktDataReq", ex); }
        }

        public void rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in rerouteMktDepthReq", ex); }
        }

        public void scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in scannerData", ex); }
        }

        public void scannerDataEnd(int reqId)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in scannerDataEnd", ex); }
        }

        public void scannerParameters(string xml)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in scannerParameters", ex); }
        }

        public void securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in securityDefinitionOptionParameter", ex); }
        }

        public void securityDefinitionOptionParameterEnd(int reqId)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in securityDefinitionOptionParameterEnd", ex); }
        }

        public void smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in smartComponents", ex); }
        }

        public void softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in softDollarTiers", ex); }
        }

        public void symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in symbolSamples", ex); }
        }

        public void tickByTickAllLast(int reqId, int tickType, long time, double price, int size, TickAttrib attribs, string exchange, string specialConditions)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in tickByTickAllLast", ex); }
        }

        public void tickByTickBidAsk(int reqId, long time, double bidPrice, double askPrice, int bidSize, int askSize, TickAttrib attribs)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in tickByTickBidAsk", ex); }
        }

        public void tickByTickMidPoint(int reqId, long time, double midPoint)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in tickByTickMidPoint", ex); }
        }

        public void tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in tickEFP", ex); }
        }

        public void tickGeneric(int tickerId, int field, double value)
        {
            try
            {
                TWSSubscription subscription = TWSSubscriptionManager.GetById(tickerId);
                if (subscription != null)
                {
                    TWSTickEventArgs args = new TWSTickEventArgs(subscription, field, value);
                    if (m_twsTickEventHandler != null)
                    {
                        m_twsTickEventHandler(this, args);
                    }
                }
                else
                {
                    Info(String.Format("Error - tick received for unknown ticker id {0}", tickerId));
                }
            }
            catch (Exception ex) { Error("Exception in tickGeneric callback", ex); }
        }

        public void tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in tickNews", ex); }
        }

        public void tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            try
            {
                // ignore Bid and Ask computations
                if ((field == TickType.MODEL_OPTION) || (field == TickType.DELAYED_MODEL_OPTION))
                {
                    TWSSubscription subscription = TWSSubscriptionManager.GetById(tickerId);
                    if (subscription != null)
                    {
                        TWSTickEventArgs args = new TWSTickEventArgs(subscription, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice);
                        if (m_twsTickEventHandler != null)
                            m_twsTickEventHandler(this, args);
                    }
                    else
                    {
                        Info(String.Format("Error - tick received for unknown ticker id {0}", tickerId));
                    }
                }
            }
            catch (Exception ex) { Error("Exception in tickOptionComputation callback", ex); }
        }

        public void tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            try
            {
                TWSSubscription subscription = TWSSubscriptionManager.GetById(tickerId);
                if (subscription != null)
                {
                    TWSTickEventArgs args = new TWSTickEventArgs(subscription, field, price, attribs);
                    if (m_twsTickEventHandler != null)
                       if (args.DoubleValue >= 0)
                            m_twsTickEventHandler(this, args);
                        else 
                            Info(String.Format("Ignoring tick price: {0}", args.ToString()));
                }
                else
                {
                    Info(String.Format("Error - tick received for unknown ticker id {0}", tickerId));
                }
            }
            catch (Exception ex) { Error("Exception in tickPrice callback", ex); }
        }

        public void tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            try
            {
                TickReqParamsEventArgs args = new TickReqParamsEventArgs(tickerId, minTick, bboExchange, snapshotPermissions);
                if (m_tickReqParamsEventHandler != null)
                    m_tickReqParamsEventHandler(this, args);
                else
                    Info(args.ToString());
            }
            catch (Exception ex) { Error("Exception in tickReqParams callback", ex); }
        }

        public void tickSize(int tickerId, int field, int size)
        {
            try
            {
                TWSSubscription subscription = TWSSubscriptionManager.GetById(tickerId);
                if (subscription != null)
                {
                    TWSTickEventArgs args = new TWSTickEventArgs(subscription, field, size);
                    if (m_twsTickEventHandler != null)
                        m_twsTickEventHandler(this, args);
                }
                else
                {
                    Info(String.Format("Error - tick received for unknown ticker id {0}", tickerId));
                }
            }
            catch (Exception ex) { Error("Exception in tickSize callback", ex); }
        }

        public void tickSnapshotEnd(int tickerId)
        {
            try
            {
                if (m_tickSnapshotEndEventHandler != null)
                    m_tickSnapshotEndEventHandler(this, new TickSnapshotEndEventArgs(tickerId));
                else
                    Info(String.Format("TickSnapshotEnd: {0}", tickerId));
            }
            catch (Exception ex) { Error("Exception in tickSnapshotEnd callback", ex); }
        }

        public void tickString(int tickerId, int field, string value)
        {
            try
            {
                TWSSubscription subscription = TWSSubscriptionManager.GetById(tickerId);
                if (subscription != null)
                {
                    TWSTickEventArgs args = new TWSTickEventArgs(subscription, field, value);
                    if (m_twsTickEventHandler != null)
                        m_twsTickEventHandler(this, args);
                }
                else
                {
                    Info(String.Format("Error - tick received for unknown ticker id {0}", tickerId));
                }
            }
            catch (Exception ex) { Error("Exception in tickString callback", ex); }
        }

        public void updateAccountTime(string timestamp)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updateAccountTime", ex); }
        }

        public void updateAccountValue(string key, string value, string currency, string accountName)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updateAccountValue", ex); }
        }

        public void updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updateMktDepth", ex); }
        }

        public void updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updateMktDepthL2", ex); }
        }

        public void updateNewsBulletin(int msgId, int msgType, string message, string origExchange)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updateNewsBulletin", ex); }
        }

        public void updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in updatePortfolio", ex); }
        }

        public void verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in verifyAndAuthCompleted", ex); }
        }

        public void verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in verifyAndAuthMessageAPI", ex); }
        }

        public void verifyCompleted(bool isSuccessful, string errorText)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in verifyCompleted", ex); }
        }

        public void verifyMessageAPI(string apiData)
        {
            try { throw new NotImplementedException(); }
            catch (Exception ex) { Error("Exception in verifyMessageAPI", ex); }
        }

        #endregion
    }
}
