using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class ManagedAccountsEventArgs : EventArgs
    {
        public string AccountsList { get; private set; }

        public ManagedAccountsEventArgs(string accountsList)
        {
            AccountsList = accountsList;
        }
    }
}
