using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteSalut.Data;
using TesteSalut.Models;

namespace TesteSalut.Controllers
{
    [ApiController]
    [Route("produto")]
    public class ProdutoController : Controller
    {
       
        [HttpGet]
        [Route("listar")]
        public async Task<IActionResult> ListarAsync([FromServices] AppDbContext context)
        {
            var produtos = await context.Produto
                .AsNoTracking()
                .ToListAsync();

            return Ok(produtos);
        }

        [HttpPost]
        [Route("adicionar")]
        public async Task<IActionResult> AdicionarAsync([FromServices] AppDbContext context, [FromBody] Produto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var produto = new Produto { 
                Id = Guid.NewGuid(),
                Nome = model.Nome
            };

            try
            {
                await context.Produto.AddAsync(produto);
                await context.SaveChangesAsync();
                return Created($"produto/adicionar/{produto.Id}", produto);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        [HttpPut]
        [Route("editar/{id}")]
        public async Task<IActionResult> EditarAsync([FromServices] AppDbContext context, [FromBody] Produto model,[FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var produto = await context.Produto
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if(produto == null)
            {
                return NotFound();
            }

            try
            {
                produto.Nome = model.Nome;

                context.Produto.Update(produto);
                await context.SaveChangesAsync();

                return Ok("Produto Alterado! " + produto);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }


        [HttpDelete]
        [Route("excluir/{id}")]
        public async Task<IActionResult> RemoverAsync([FromServices] AppDbContext context ,[FromRoute] Guid id)
        {
            var produto = await context.Produto
                .FirstOrDefaultAsync(p => p.Id.Equals(id));

            if(produto == null)
            {
                return NotFound();
            }

            try
            {
                context.Produto.Remove(produto);
                await context.SaveChangesAsync();

                return Ok("Produto removido com sucesso!");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }



    }
}
