using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class AuthorBook
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        public int Book_id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Author_id { get; set; }
    }
}
