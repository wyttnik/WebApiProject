namespace RestProject.Models
{
    public class PublisherToReceive
    {
        public int Publisher_id { get; set; }
        public string Publisher_name { get; set; } = null!;

        public ICollection<BookToTransfer> Books { get; set; } = new List<BookToTransfer>();
    }
}
