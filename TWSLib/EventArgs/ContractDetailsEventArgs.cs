using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace TWSLib
{
    public class ContractDetailsEventArgs : EventArgs
    {
        public int RequestId { get; private set; }
        public ContractDetails ContractDetails { get; private set; }
        public Contract Contract { get { return ContractDetails.Summary; } }

        public ContractDetailsEventArgs(int reqId, ContractDetails contractDetails)
        {
            RequestId = reqId;
            ContractDetails = contractDetails;
        }

        public override string ToString()
        {
            return "ContractDetails begin. ReqId: " + RequestId
             + printContractMsg(Contract)
            + printContractDetailsMsg(ContractDetails)
            + "\nContractDetails end. ReqId: " + RequestId;
        }
 
        private string printContractMsg(Contract contract)
        {
            return "\nConId: " + contract.ConId
            + "\nSymbol: " + contract.Symbol
            + "\nSecType: " + contract.SecType
            + "\nLastTradeDateOrContractMonth: " + contract.LastTradeDateOrContractMonth
            + "\nStrike: " + contract.Strike
            + "\nRight: " + contract.Right
            + "\nMultiplier: " + contract.Multiplier
            + "\nExchange: " + contract.Exchange
            + "\nPrimaryExchange: " + contract.PrimaryExch
            + "\nCurrency: " + contract.Currency
            + "\nLocalSymbol: " + contract.LocalSymbol
            + "\nTradingClass: " + contract.TradingClass;
        }

        private string printContractDetailsMsg(ContractDetails contractDetails)
        {
            return "\nMarketName: " + contractDetails.MarketName
            + "\nMinTick: " + contractDetails.MinTick
            + "\nPriceMagnifier: " + contractDetails.PriceMagnifier
            + "\nOrderTypes: " + contractDetails.OrderTypes
            + "\nValidExchanges: " + contractDetails.ValidExchanges
            + "\nUnderConId: " + contractDetails.UnderConId
            + "\nLongName: " + contractDetails.LongName
            + "\nContractMonth: " + contractDetails.ContractMonth
            + "\nIndystry: " + contractDetails.Industry
            + "\nCategory: " + contractDetails.Category
            + "\nSubCategory: " + contractDetails.Subcategory
            + "\nTimeZoneId: " + contractDetails.TimeZoneId
            + "\nTradingHours: " + contractDetails.TradingHours
            + "\nLiquidHours: " + contractDetails.LiquidHours
            + "\nEvRule: " + contractDetails.EvRule
            + "\nEvMultiplier: " + contractDetails.EvMultiplier
            + "\nMdSizeMultiplier: " + contractDetails.MdSizeMultiplier
            + "\nAggGroup: " + contractDetails.AggGroup
            + "\nUnderSymbol: " + contractDetails.UnderSymbol
            + "\nUnderSecType: " + contractDetails.UnderSecType
            + "\nMarketRuleIds: " + contractDetails.MarketRuleIds
            + "\nRealExpirationDate: " + contractDetails.RealExpirationDate
            + printContractDetailsSecIdList(contractDetails.SecIdList);
        }

        private string printContractDetailsSecIdList(List<TagValue> secIdList)
        {
            string r = "";

            if (secIdList != null && secIdList.Count > 0)
            {
                r += "\nSecIdList: {";
                foreach (TagValue tagValue in secIdList)
                {
                   r += tagValue.Tag + "=" + tagValue.Value + ";";
                }
                r += "}";
            }

            return r;
        }
    }
}
