using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class BookLanguage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Key]
        public int Language_id { get; set; }
        public string Language_code { get; set; } = null!;

        public string Language_name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
