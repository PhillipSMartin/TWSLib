using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gargoyle.Messaging.Common;
using IBApi;

namespace TWSLib
{
   public  class ContractFactory
    {
       public static Contract GetContract(string symbol, QuoteType quoteType)
       {
           switch (quoteType)
           {
               case QuoteType.Stock:
                   return new Contract()
                   {
                       Symbol = symbol,
                       SecType = "STK",
                       Currency = "USD",
                       Exchange = "SMART"
                   };
               case QuoteType.Option:
                   return new Contract()
                   {
                       LocalSymbol = symbol,
                       SecType = "OPT",
                       Currency = "USD",
                       Exchange = "SMART"
                   };
               case QuoteType.Index:
                   return new Contract()
                   {
                       Symbol = symbol,
                       SecType = "IND",
                       Currency = "USD"
                   };
               case QuoteType.Future:
                   return new Contract()
                   {
                       LocalSymbol = String.Format("{0}{1}{2}", symbol.Substring(0, 2), symbol.Substring(5, 1), symbol.Substring(4, 1)),
                       SecType = "FUT",
                       Currency = "USD"
                   };
               default:
                   return null;
           }
       }
    }
}
