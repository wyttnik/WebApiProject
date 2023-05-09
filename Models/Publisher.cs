using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class Publisher
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Publisher_name { get; set; } = null!;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
