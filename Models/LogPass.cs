using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestProject.Models
{
    public class LogPass
    {
        public string login { get; set; }
        public string password { get; set; }
    }
}
