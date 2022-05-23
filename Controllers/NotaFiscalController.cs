using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteSalut.Data;
using TesteSalut.Models;
using TesteSalut.ViewModels;

namespace TesteSalut.Controllers
{
    [ApiController]
    [Route("notaFiscal")]
    public class NotaFiscalController : Controller
    {
        
        private readonly IConfiguration _config;

        public AppDbContext _context { get; }

        List<SelectListItem> produtos = new List<SelectListItem>();

         List<ProdutoNotaFiscal> produtosNaNota = new List<ProdutoNotaFiscal>();


        public NotaFiscalController(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        private SelectList GetOptions()
        {
            string connection = _config["ConnectionString"];

            using (SqlConnection sql = new SqlConnection(connection))
            {
                int i = 0;
                sql.Open();
                SqlDataReader reader = null;
                SqlCommand sqlCommand = new SqlCommand("SELECT NOME FROM PRODUTO ", sql);
                reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    
                    produtos.Add(new SelectListItem { Text = reader["NOME"].ToString(), Value = reader["NOME"].ToString() });
                    i++;
                }

                sql.Close();
            }
                return new SelectList(produtos, "Value", "Text");
        }

        [Route("tabelaProdutos")]
        public PartialViewResult TabelaProdutos()
        {
            return PartialView("Views/TabelaProdutos.cshtml");
        }

        public IActionResult Index()
        {

            ProdutoNotaViewModel produtoNotaViewModel = new ProdutoNotaViewModel(); 
            ViewBag.produtos = GetOptions().ToArray();

            return View("Views/Index.cshtml", produtoNotaViewModel);
        }



        [HttpGet]
        [Route("listar")]
        public async Task<IActionResult> ListarAsync([FromServices] AppDbContext context)
        {
            var notasFiscais = await context.NotaFiscal
                .AsNoTracking()
                .ToListAsync();

            return Ok(notasFiscais);
        }



        [HttpPost]
        [Route("adicionarProdutoNaNota")]
        public async Task<IActionResult> AdicionarProdutoNaNota([FromServices] AppDbContext context, [FromForm] string produto, [FromForm] int quantidade)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            var produtoInserido = await context.Produto
                .FirstOrDefaultAsync(p => p.Nome.Equals(produto));

            ProdutoNotaFiscal produtoAdicionado = new ProdutoNotaFiscal
            {
                Produto = produtoInserido,
                Quantidade = quantidade
            };

            produtosNaNota.Add(produtoAdicionado);

            ViewBag.QtdProdutos = produtosNaNota.Count();

            return Ok(produtosNaNota);

        }

        [HttpGet]
        [Route("listarProdutosNaNota")]
        public JsonResult ListarProdutosNaNota()
        {
            return Json(produtosNaNota);
        }

        [HttpPost]
        [Route("adicionar")]
        public async Task<IActionResult> AdicionarAsync([FromServices] AppDbContext context, [FromBody] NotaFiscal model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var notaFiscal = new NotaFiscal
            {
                Id = Guid.NewGuid(),
                CNPJ = model.CNPJ,
                CanalDeCompra = model.CanalDeCompra,
                DataDaCompra = model.DataDaCompra,
                NumeroCupomFiscal = model.NumeroCupomFiscal,
                ProdutosNaNota = produtosNaNota,
                QtdProdutos = model.QtdProdutos,
            };

            try
            {
                await context.NotaFiscal.AddAsync(notaFiscal);
                await context.SaveChangesAsync();
                return Created($"notaFiscal/adicionar/{notaFiscal.Id}", notaFiscal);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

            foreach(var item in produtosNaNota)
            {
                produtosNaNota.Remove(item);
            }

        }



        [HttpDelete]
        [Route("excluir/{id}")]
        public async Task<IActionResult> RemoverAsync([FromServices] AppDbContext context, [FromRoute] Guid id)
        {
            var produto = await context.NotaFiscal
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if (produto == null)
            {
                return NotFound();
            }

            try
            {
                context.NotaFiscal.Remove(produto);
                await context.SaveChangesAsync();

                return Ok("Nota Fiscal removida com sucesso!");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


    }
}
