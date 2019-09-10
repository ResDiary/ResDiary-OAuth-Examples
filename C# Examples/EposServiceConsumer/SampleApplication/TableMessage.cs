using System;
using System.Text;

namespace RD.EposServiceConsumer.SampleApplication
{
    internal class TableMessage
    {
        public string Table { get; set; }
        public DateTime CloseDateTime { get; set; }
        public double Spend { get; set; }
        public string ReceiptXml { get; set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Table: " + Table);
            stringBuilder.AppendLine("CloseDateTime: " + CloseDateTime);
            stringBuilder.AppendLine("Spend: " + Spend);
            stringBuilder.AppendLine("ReceiptXml: " + ReceiptXml);

            return stringBuilder.ToString();

        }
    }
}