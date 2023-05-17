using System.ComponentModel.DataAnnotations;

namespace RestProject.Models
{
    public class BookLanguageToTransfer
    {
        //[Key]
        public int Language_id { get; set; }
        public string Language_code { get; set; } = null!;

        public string Language_name { get; set; } = null!;
    }
}
