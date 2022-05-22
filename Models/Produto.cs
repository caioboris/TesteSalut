using System.ComponentModel.DataAnnotations;

namespace TesteSalut.Models
{
    public class Produto
    {
        public Guid Id { get; set; }
        
        [Required]
        public string Nome { get; set; }


    }
}
