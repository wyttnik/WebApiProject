namespace RestProject.Models
{
    public class AuthorToReceive
    {
        public int Id { get; set; }
        public string Author_name { get; set; } = null!;

        public List<BookToTransfer> Books { get; set; } = new();
    }
}
