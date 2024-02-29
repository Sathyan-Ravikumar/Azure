namespace DuplicationDetection.Data
{
    public class RfidTag
    {
        public string? TagId { get; set; }
        public string? Product { get; set; }

        public double Price { get; set; }
        public RfidTag()
        {
            TagId = Guid.NewGuid().ToString();
        }
    }
}
