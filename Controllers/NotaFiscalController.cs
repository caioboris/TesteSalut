using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TesteSalut.Data;
using TesteSalut.Models;

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
                sql.Open();
                SqlDataReader reader = null;
                SqlCommand sqlCommand = new SqlCommand("SELECT NOME FROM PRODUTO ", sql);
                reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    int i = 0;
                    produtos.Add(new SelectListItem { Text = reader["NOME"].ToString(), Value = i.ToString() });
                    i++;
                }

                sql.Close();
            }
                return new SelectList(produtos, "Value", "Text");
        }

        public IActionResult Index()
        {

            NotaFiscal nota = new NotaFiscal();
            ViewBag.produtos = GetOptions().ToArray();

            return View("Views/Index.cshtml",nota);
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

        [HttpPut]
        [Route("adicionarProdutoNaNota")]
        public IActionResult AdicionarProdutoNaNota([FromBody] ProdutoNotaFiscal produto)
        {            

            if (!ModelState.IsValid)            
                return BadRequest();

            produtosNaNota.Add(produto);

            ViewBag.produtosNota = produtosNaNota.ToList();

            return Created("/adicionarProdutoNaNota", produto);

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
                ProdutosNaNota = model.ProdutosNaNota,
                QtdProdutos = model.QtdProdutos,
                UploadCupomFiscal = model.UploadCupomFiscal
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


        [HttpPost]
        [Route("uploadCupom")]
        public async Task<IActionResult> UploadCupom(IList<IFormFile> cupomFiscal)
        {
            IFormFile cupomCarregado = cupomFiscal.FirstOrDefault();

            if (cupomCarregado != null)
            {
                MemoryStream ms = new MemoryStream();
                cupomCarregado.OpenReadStream().CopyTo(ms);

                if(cupomCarregado.ContentType.Equals("application/pdf") || cupomCarregado.ContentType.Equals("image/png") || cupomCarregado.ContentType.Equals("image/jpg"))
                {
                    CupomFiscal cupom = new CupomFiscal()
                    {
                        Descricao = cupomCarregado.FileName,
                        Dados = ms.ToArray(),
                        ContentType = cupomCarregado.ContentType
                    };

                    _context.CupomFiscal.Add(cupom);
                    await _context.SaveChangesAsync();


                }
                else
                {
                    throw new Exception("Formato de Arquivo não suportado!");
                }
            }
            return RedirectToAction("Index");
        }

        [Route("cupom/visualizar")]
        public IActionResult Visualizar(int id)
        {
            var cupomSalvo = _context.CupomFiscal.FirstOrDefault(c => c.Id == id);

            return File(cupomSalvo.Dados, cupomSalvo.ContentType);
        } 


    }
}
