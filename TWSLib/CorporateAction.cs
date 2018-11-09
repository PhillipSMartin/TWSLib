using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TWSLib
{
    internal class CorporateAction
    {
        public CorporateAction(string ticker)
        {
            Ticker = ticker;
        }
        public string Ticker { get; private set; }

        public DateTime? ExDate { get; set; }
        public double Dividend { get; set; }
        public double DividendsPastYear { get; set; }
        public double DividendsNextYear { get; set; }

        public string ToXml()
        {
            string xml = string.Empty;
            XmlDocument xmlDoc = new XmlDocument();

            // add xml declaration tag: <?xml>
            XmlDeclaration xmlNode = xmlDoc.CreateXmlDeclaration("1.0", null, null);
            xmlDoc.AppendChild(xmlNode);

            // Add root node: <CorporateAction>...</CorporateAction>
            XmlElement rootNode = xmlDoc.CreateElement("CorporateAction");
            xmlDoc.AppendChild(rootNode);

            // Add node for each field
            XmlElement cellNode = xmlDoc.CreateElement("Ticker");
            cellNode.AppendChild(xmlDoc.CreateTextNode(Ticker));
            rootNode.AppendChild(cellNode);

            if (ExDate.HasValue)
            {
                cellNode = xmlDoc.CreateElement("ExDate");
                cellNode.AppendChild(xmlDoc.CreateTextNode(ExDate.Value.ToString("d")));
                rootNode.AppendChild(cellNode);

                cellNode = xmlDoc.CreateElement("Dividend");
                cellNode.AppendChild(xmlDoc.CreateTextNode(Dividend.ToString()));
                rootNode.AppendChild(cellNode);
            }

            cellNode = xmlDoc.CreateElement("DividendsPastYear");
            cellNode.AppendChild(xmlDoc.CreateTextNode(DividendsPastYear.ToString()));
            rootNode.AppendChild(cellNode);

            cellNode = xmlDoc.CreateElement("DividendsNextYear");
            cellNode.AppendChild(xmlDoc.CreateTextNode(DividendsNextYear.ToString()));
            rootNode.AppendChild(cellNode);

            // Write XML content to StringWriter
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                //               xmlTextWriter.Formatting = Formatting.Indented;  // Output formatted XML code
                xmlDoc.WriteTo(xmlTextWriter);

                // Output XML code
                xml = stringWriter.ToString();
            }

            return xml;
        }
    }
}
