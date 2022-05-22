using System.ComponentModel.DataAnnotations;

namespace TesteSalut.Models
{
    public class NotaFiscal
    {
        public Guid Id = new Guid();

        [Required]
        public string CNPJ { get; set; }

        [Required]
        [Display(Name = "Canal de Compra")]
        public string CanalDeCompra { get; set; }

        [Display(Name = "Data da Compra")]
        [DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Range(typeof(DateTime), "01/05/2022", "15/06/2022", ErrorMessage = "A Data deve estar entre os dias {1} a {2}")]
        public DateTime DataDaCompra { get; set; }

        [Required]
        [Display(Name = "Numero do Cupom Fiscal")]
        public string NumeroCupomFiscal { get; set; }

        [Required]
        [MinLength(5)]
        public List<ProdutoNotaFiscal> ProdutosNaNota { get; set; }

        [Required]
        [Display(Name = "Quantidade")]
        public int QtdProdutos { get; set; }

        [Display(Name = "Upload Cupom Fiscal")]
        public CupomFiscal UploadCupomFiscal { get; set; }

        [Display(Name = "Valor Total dos Produtos")]
        public double ValorTotal { get; set; }
    }
}
