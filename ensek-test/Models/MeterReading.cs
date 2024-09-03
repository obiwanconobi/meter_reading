namespace ensek_test.Models
{
    public class MeterReading
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }
    }
}
