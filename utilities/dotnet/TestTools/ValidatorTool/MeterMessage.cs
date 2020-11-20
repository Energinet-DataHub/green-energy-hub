using System;
using System.Text.Json.Serialization;

namespace ValidatorTool
{
    public class MeterMessage
    {
        public MeterMessage()
        {
        }

        public MeterMessage(int meterValue, int meterId, DateTime dateTime, int customerId)
        {
            MeterValue = meterValue;
            MeterId = meterId;
            MeterDateTime = dateTime;
            CustomerId = customerId;
        }

        [JsonPropertyName("transactionId")]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("meterValue")]
        public int MeterValue { get; set; }

        [JsonPropertyName("meterId")]
        public int MeterId { get; set; }

        [JsonPropertyName("meterReadDate")]
        public DateTime MeterDateTime { get; set; }

        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }
    }
}
