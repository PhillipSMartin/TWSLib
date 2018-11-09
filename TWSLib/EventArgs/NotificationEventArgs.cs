using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWSLib
{
    public class NotificationEventArgs : EventArgs
    {
        public int Id { get; private set; }
        public int Code { get; private set; }
        public string Message { get; private set; }

        public NotificationEventArgs(int id, int errorCode, string errorMsg)
        {
            Id = id;
            Code = errorCode;
            Message = errorMsg;
        }

        public override string ToString()
        {
            return String.Format("Nofification: Id={0}, Code={1}, Message={2}", Id, Code, Message);
        }
    }
}
  