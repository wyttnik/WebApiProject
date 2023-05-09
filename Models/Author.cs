using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Author_name { get; set; } = null!;

        public List<Book> Books { get; set; } = new();
    }
}
