using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class AuthorBook
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BookId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AuthorId { get; set; }
    }
}
