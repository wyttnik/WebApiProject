using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Book_id { get; set; }
        public string Title { get; set; } = null!;
        public string Isbn13 { get; set; } = null!;


        public int Num_pages { get; set;  }
        public DateTime Publication_date { get; set; }

        public int Language_id { get; set; }
        public int Publisher_id { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public BookLanguage BookLanguage { get; set; } = null!;

        public List<Author> Authors { get; set; } = new();

    }
}
