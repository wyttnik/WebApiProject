namespace RestProject.Models
{
    public class PublisherToReceive
    {
        public int Id { get; set; }
        public string Publisher_name { get; set; } = null!;

        public ICollection<BookToReceive> Books { get; set; } = new List<BookToReceive>();
    }
}
