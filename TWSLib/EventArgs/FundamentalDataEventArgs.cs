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
    public class FundamentalDataEventArgs : EventArgs
    {
        public FundamentalDataEventArgs(int requestId, string data)
        {
            RequestId = requestId;
            Data = data;
        }
 
        public int RequestId { get; private set; }
        public string Data { get; private set; }

        public override string ToString()
        {
            return String.Format("Fundamentals: Id={0}, Data={1}", RequestId, Data);
        }

      }
}
