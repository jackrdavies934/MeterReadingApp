namespace MeterReadingApp.Models.DTO
{
    public class MeterReadingDTO
    {
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadValue { get; set; }
    }
}
