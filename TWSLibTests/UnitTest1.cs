using System;
using TWSLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gargoyle.Common;
using System.Collections.Generic;
using Gargoyle.Messaging.Common;

namespace TWSLibTests
{
    [TestClass]
    public class UnitTest1
    {
//        private static ILog s_logger;
        private static TWSUtilities s_utilities = new TWSUtilities();

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            s_utilities.OnInfo += Utilities_OnInfo;
            s_utilities.OnError += Utilities_OnError;
         }

        private static void Utilities_OnError(object sender, LoggingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message + "=>" + e.Exception.Message);
        }
        private static void Utilities_OnInfo(object sender, LoggingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }

        [TestMethod]
        public void ReportConnectionTest()
        {
            s_utilities.Init(TWSUtilities.REPORT_READER);
            bool expected = true;
            bool actual = s_utilities.Connect();

            Assert.AreEqual(expected, actual, "Report connection test failed");
        }

        [TestMethod]
        public void QuoteConnectionTest()
        {
 //           s_utilities.Init(TWSUtilities.QUOTE_READER, "172.18.0.6");
           s_utilities.Init(TWSUtilities.QUOTE_READER, null, null, TWSLib.TWSUtilities.DELAYED);
            bool expected = true;
            bool actual = s_utilities.Connect();

            Assert.AreEqual(expected, actual, "Quote connection test failed");
        }

        [TestMethod]
        public void HistoricalConnectionTest()
        {
            s_utilities.Init(TWSUtilities.HISTORICAL_READER, "172.18.0.6");
            //         s_utilities.Init(TWSUtilities.HISTORICAL_READER);
            bool expected = true;
            bool actual = s_utilities.Connect();

            Assert.AreEqual(expected, actual, "Historical reader connection test failed");
        }

        [TestMethod]
        public void CorporateActionsConnectionTest()
        {
            s_utilities.Init(TWSUtilities.CORPORATE_ACTIONS_READER, "gargoyle-mw20");
            //          s_utilities.Init(TWSUtilities.CORPORATE_ACTIONS_READER, "172.18.0.6");
            //          s_utilities.Init(TWSUtilities.CORPORATE_ACTIONS_READER, null, null, TWSLib.TWSUtilities.DELAYED);
            bool expected = true;
            bool actual = s_utilities.Connect();

            Assert.AreEqual(expected, actual, "Corporate actions connection test failed");
        }

        [TestMethod]
        public void BadConnectionTest()
        {
            s_utilities.Init(0);
            bool expected = false;
            bool actual = s_utilities.Connect();

            Assert.AreEqual(expected, actual, "Bad connection test failed");
        }

        [TestMethod]
        public void StartReportReaderTest()
        {
            ReportConnectionTest();

            bool expected = true;
            bool actual = s_utilities.StartReportReader();

            Assert.AreEqual(expected, actual, "StartReportReader test failed");
        }

        [TestMethod]
        public void StopReportReaderTest()
        {
            StartReportReaderTest();

            System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get some reports");
            System.Threading.Thread.Sleep(5000);

            bool expected = true;
            bool actual = s_utilities.StopReportReader(true);

            Assert.AreEqual(expected, actual, "StopReportReader test failed");

        }

        [TestMethod]
        public void StartQuoteReaderTest()
        {
            QuoteConnectionTest();

            bool expected = true;
            bool actual = s_utilities.StartQuoteReader();

            Assert.AreEqual(expected, actual, "StartQuoteReader test failed");
        }

        [TestMethod]
        public void StartHistoricalReaderTest()
        {
            HistoricalConnectionTest();

            bool expected = true;
            bool actual = s_utilities.StartHistoricalReader();

            Assert.AreEqual(expected, actual, "StartHistoricalReader test failed");
        }

        [TestMethod]
        public void StartCorporateActionsReaderTest()
        {
            CorporateActionsConnectionTest();

            bool expected = true;
            bool actual = s_utilities.StartCorporateActionsReader();

            Assert.AreEqual(expected, actual, "StartCorporateActionsReader test failed");
        }

        [TestMethod]
        public void StopQuoteReaderTest()
        {
            StartQuoteReaderTest();

            bool expected = true;
            bool actual = s_utilities.StopQuoteReader();

            Assert.AreEqual(expected, actual, "StopQuoteReader test failed");

        }

        [TestMethod]
        public void StopHistoricalReaderTest()
        {
            StartHistoricalReaderTest();

            bool expected = true;
            bool actual = s_utilities.StopHistoricalReader();

            Assert.AreEqual(expected, actual, "StopHistoricalReader test failed");

        }

        [TestMethod]
        public void StopCorporateActionsReaderTest()
        {
            StartHistoricalReaderTest();

            bool expected = true;
            bool actual = s_utilities.StopCorporateActionsReader();

            Assert.AreEqual(expected, actual, "StopCorporateActionsReader test failed");

        }

        [TestMethod]
        public void SubscribeToStocksTest()
        {
            StartQuoteReaderTest();

            bool expected = true;
            bool actual = s_utilities.SubscribeToStocks(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("IBM", null),
                new KeyValuePair<string, object>("GE", null) });

            Assert.AreEqual(expected, actual, "SubscribeToStocks test failed");

            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get some quotes");
                System.Threading.Thread.Sleep(5000);

                actual = s_utilities.Unsubscribe(new string[] { "IBM", "GE" });
                Assert.AreEqual(expected, actual, "Unsubscribe failed");
            }

            s_utilities.StopQuoteReader();
        }

        [TestMethod]
        public void GetCorporateActionsAsXmlTest()
        {
            StartCorporateActionsReaderTest();

            string xml;
            int expected = 1;
            int actual = s_utilities.GetCorporateActions("IBM", null, out xml);

            Assert.AreEqual(expected, actual, "GetCorporateActions test failed");
            if (xml != null)
            {
                System.Diagnostics.Debug.WriteLine(xml);
            }

            expected = 0;
            actual = s_utilities.GetCorporateActions("AN", null, out xml);

            Assert.AreEqual(expected, actual, "GetCorporateActions test failed");
            if (xml != null)
            {
                System.Diagnostics.Debug.WriteLine(xml);
            }

            s_utilities.StopCorporateActionsReader();
        }

        [TestMethod]
        public void GetCorporateActionsTest()
        {
            StartCorporateActionsReaderTest();

            bool expected = true;
            bool actual = s_utilities.GetCorporateActions(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("IBM", null),
                new KeyValuePair<string, object>("AN", 2) });

            Assert.AreEqual(expected, actual, "GetCorporateActions test failed");

            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 10 seconds to get corporate actions");
                System.Threading.Thread.Sleep(10000);
            }

            s_utilities.StopCorporateActionsReader();
        }

        [TestMethod]
        public void SubscribeToIndicesTest()
        {
            StartQuoteReaderTest();

            bool expected = true;
            bool actual = s_utilities.SubscribeToIndices(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("SPX", null),
                new KeyValuePair<string, object>("NDX", 2) });

            Assert.AreEqual(expected, actual, "SubscribeToIndices test failed");

            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get some quotes");
                System.Threading.Thread.Sleep(5000);

                actual = s_utilities.Unsubscribe(new string[] { "SPX", "NDX" });
                Assert.AreEqual(expected, actual, "Unsubscribe failed");
            }

            s_utilities.StopQuoteReader();
        }

        [TestMethod]
        public void SubscribeToOptionsTest()
        {
            StartQuoteReaderTest();

            bool expected = true;
            bool actual = s_utilities.SubscribeToOptions(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("SPX   181116C02900000", null) });

            Assert.AreEqual(expected, actual, "SubscribeToOptions test failed");

            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get some quotes");
                System.Threading.Thread.Sleep(5000);

                actual = s_utilities.Unsubscribe(new string[] { "SPX   181116C02900000" });
                Assert.AreEqual(expected, actual, "Unsubscribe failed");
            }

            s_utilities.StopQuoteReader();
        }

        [TestMethod]
        public void SubscribeToFuturesTest()
        {
            StartQuoteReaderTest();

            bool expected = true;
            bool actual = s_utilities.SubscribeToFutures(new KeyValuePair<string, object>[] { new KeyValuePair<string, object>("ES/18Z", null) });

            Assert.AreEqual(expected, actual, "SubscribeToFutures test failed");

            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get some quotes");
                System.Threading.Thread.Sleep(5000);

                actual = s_utilities.Unsubscribe(new string[] { "ES/18Z" });
                Assert.AreEqual(expected, actual, "Unsubscribe failed");
            }

            s_utilities.StopQuoteReader();
        }


        [TestMethod]
        public void GetFundamentalDataTest()
        {
            StartHistoricalReaderTest();

            int expected = 0;
            int actual = s_utilities.GetFundamentalData("IBM", "ReportRatios");

            Assert.AreEqual(expected, actual, "GetFundamentalData test failed");
            if (expected == actual)
            {
                System.Diagnostics.Debug.WriteLine("Waiting 5 seconds to get data");
                System.Threading.Thread.Sleep(5000);
            }

            s_utilities.StopHistoricalReader();
        }

        [TestMethod]
        public void GetHistoricalDataTest()
        {
            StartHistoricalReaderTest();

            int actual1 = s_utilities.GetHistoricalData("AAPL", QuoteType.Stock);
            int actual2 = s_utilities.GetHistoricalData("IBM", QuoteType.Stock, null, new DateTime(2018,10,1));
            int actual3 = s_utilities.GetHistoricalData("SPY", QuoteType.Stock, null, new DateTime(2018, 10, 2));
            int actual4 = s_utilities.GetHistoricalData("NDX", QuoteType.Index, null);
            int actual5 = s_utilities.GetHistoricalData("BXM", QuoteType.Index, null, new DateTime(2018, 10, 3));
            int actual6 = s_utilities.GetHistoricalData("BadTicker", QuoteType.Stock, null, new DateTime(2018, 10, 3));
            int actual7 = s_utilities.GetHistoricalData("SPX   181116C02900000", QuoteType.Option, null);
            int actual8 = s_utilities.GetHistoricalData("ES/18Z", QuoteType.Future, null);

            Assert.AreEqual(0, actual1, "GetHistoricalData test failed for AAPL");
            Assert.AreEqual(1, actual2, "GetHistoricalData test failed for IBM");
            Assert.AreEqual(2, actual3, "GetHistoricalData test failed for SPY");
            Assert.AreEqual(3, actual4, "GetHistoricalData test failed for NDX");
            Assert.AreEqual(4, actual5, "GetHistoricalData test failed for BXM");
            Assert.AreEqual(5, actual6, "GetHistoricalData test failed for BadTicker");
            Assert.AreEqual(-1, actual7, "GetHistoricalData test failed for SPX   181116C02900000");
            Assert.AreEqual(6, actual8, "GetHistoricalData test failed for ES/18Z");
            System.Diagnostics.Debug.WriteLine("Waiting 10 seconds to get data");
            System.Threading.Thread.Sleep(10000);

            s_utilities.StopHistoricalReader();
        }
     
    }
}
