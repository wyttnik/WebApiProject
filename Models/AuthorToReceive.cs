namespace RestProject.Models
{
    public class AuthorToReceive
    {
        public int Author_id { get; set; }
        public string Author_name { get; set; } = null!;

        public List<BookToTransfer> Books { get; set; } = new();
    }
}
