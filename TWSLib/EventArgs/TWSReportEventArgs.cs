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
    public class TWSReportEventArgs : EventArgs
    {
        public TWSReportEventArgs(int requestId, Contract contract, Execution execution)
        {
            RequestId = requestId;
            Report = ReportToXml(contract, execution);
        }
        public TWSReportEventArgs(CommissionReport commission)
        {
            RequestId = -1;
            Report = ReportToXml(commission);
        }
        public TWSReportEventArgs(Contract contract, Order order, OrderState orderState)
        {
            RequestId = -1;
            Report = ReportToXml(contract, order, orderState);
        }

        public int RequestId { get; private set; }
        public string Report { get; private set; }

        private static string ReportToXml(Contract contract, Execution execution)
        {
            string lastClassName = "";
            string lastPropertyName = "";

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", null, null));
                XmlElement rootNode = xmlDoc.CreateElement("REPORT");
                xmlDoc.AppendChild(rootNode);

                lastClassName = "Contract";
                XmlElement contractNode = xmlDoc.CreateElement("CONTRACT");
                rootNode.AppendChild(contractNode);
                foreach (var property in contract.GetType().GetProperties())
                {
                    if (property.GetValue(contract) != null)
                    {
                        lastPropertyName = property.Name;
                        XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                        fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(contract).ToString()));
                        contractNode.AppendChild(fieldNode);
                    }
                }

                lastClassName = "Execution";
                XmlElement executionNode = xmlDoc.CreateElement("EXECUTION");
                rootNode.AppendChild(executionNode);
                foreach (var property in execution.GetType().GetProperties())
                {
                    if (property.GetValue(execution) != null)
                    {
                        lastPropertyName = property.Name;
                        XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                        fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(execution).ToString()));
                        executionNode.AppendChild(fieldNode);
                    }
                }

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                    xmlDoc.WriteTo(xmlTextWriter);
                    return stringWriter.ToString();
                }
            }
            catch (Exception )
            {
                throw new Exception(String.Format("Error parsing report after {0} property in {1}", lastPropertyName, lastClassName));
            }
        }
        private static string ReportToXml(CommissionReport commission)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", null, null));
            XmlElement rootNode = xmlDoc.CreateElement("REPORT");
            xmlDoc.AppendChild(rootNode);

            XmlElement contractNode = xmlDoc.CreateElement("COMMISSION");
            rootNode.AppendChild(contractNode);
            foreach (var property in commission.GetType().GetProperties())
            {
                XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(commission).ToString()));
                contractNode.AppendChild(fieldNode);
            }

            using (StringWriter stringWriter = new StringWriter())
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlDoc.WriteTo(xmlTextWriter);
                return stringWriter.ToString();
            }
        }
        private static string ReportToXml(Contract contract, Order order, OrderState orderState)
        {
            string lastClassName = "";
            string lastPropertyName = "";

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", null, null));
                XmlElement rootNode = xmlDoc.CreateElement("REPORT");
                xmlDoc.AppendChild(rootNode);

                lastClassName = "Contract";
                XmlElement contractNode = xmlDoc.CreateElement("CONTRACT");
                rootNode.AppendChild(contractNode);
                foreach (var property in contract.GetType().GetProperties())
                {
                    if (property.GetValue(contract) != null)
                    {
                        lastPropertyName = property.Name;
                        XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                        fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(contract).ToString()));
                        contractNode.AppendChild(fieldNode);
                    }
                }

                lastClassName = "Order";
                XmlElement orderDetailsNode = xmlDoc.CreateElement("ORDERDETAILS");
                rootNode.AppendChild(orderDetailsNode);
                foreach (var property in order.GetType().GetProperties())
                {
                    if (property.GetValue(order) != null)
                    {
                        lastPropertyName = property.Name;
                        XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                        fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(order).ToString()));
                        orderDetailsNode.AppendChild(fieldNode);
                    }
                }

                lastClassName = "OrderState";
                XmlElement orderStateNode = xmlDoc.CreateElement("ORDERSTATE");
                rootNode.AppendChild(orderStateNode);
                foreach (var property in orderState.GetType().GetProperties())
                {
                    if (property.GetValue(orderState) != null)
                    {
                        lastPropertyName = property.Name;
                        XmlElement fieldNode = xmlDoc.CreateElement(property.Name.Replace(" ", "").Replace("/", "").Replace("'", "").Replace("&", "n").Replace("-", "").Replace("%", "Pct"));
                        fieldNode.AppendChild(xmlDoc.CreateTextNode(property.GetValue(orderState).ToString()));
                        orderStateNode.AppendChild(fieldNode);
                    }
                }

                using (StringWriter stringWriter = new StringWriter())
                {
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                    xmlDoc.WriteTo(xmlTextWriter);
                    return stringWriter.ToString();
                }
            }
            catch (Exception )
            {
                throw new Exception(String.Format("Error parsing order after {0} property in {1}", lastPropertyName, lastClassName));
            }
        }
    }
}
