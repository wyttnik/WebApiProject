namespace RestProject.Models
{
    public class BookToTransfer
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Isbn13 { get; set; } = null!;
        public int Num_pages { get; set; }
        public DateTime Publication_date { get; set; }

        public int PublisherId { get; set; }
    }
}
