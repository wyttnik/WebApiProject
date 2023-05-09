namespace RestProject.Models
{
    public class BookToReceive
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Isbn13 { get; set; } = null!;
        public int Num_pages { get; set; }
        public DateTime Publication_date { get; set; }

        public string Publisher_name { get; set; } = null!;

        public List<AuthorToTransfer> Authors { get; set; } = new();
    }
}
