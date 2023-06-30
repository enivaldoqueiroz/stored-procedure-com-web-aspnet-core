using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebProcedures.Models;

namespace WebProcedures.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly Contexto _context;

        public ProdutoController(Contexto context)
        {
            _context = context;
        }

        // GET: Produto
        public async Task<IActionResult> Index()
        {
            //return View(await _context.Produto.ToListAsync());

            var produtos = await _context.Produto.FromSqlRaw("GetProdutos").ToListAsync();

            return View(produtos);
        }

        // GET: Produto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
             if (id == null)
                return NotFound();

            //var produto = await _context.Produto
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (produto == null)
            //{
            //    return NotFound();
            //}

            var param = new SqlParameter("@id", id);

            var query = $"EXEC GetProdutoById @id = {id}";
            var produtos = await _context.Produto.FromSqlRaw(query).ToListAsync();

            var produto = produtos.FirstOrDefault();
            if (produto == null)
                return NotFound();

            return View(produto);
            }
            catch (System.Exception ex)
            {

                throw;
            }
            
        }

        // GET: Produto/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Produto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome")] Produto produto)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //_context.Add(produto);

                    var param = new SqlParameter("@nome", produto.Nome);
                    await _context.Database.ExecuteSqlRawAsync("Cadastro @nome", param);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(produto);
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        // GET: Produto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            //var produto = await _context.Produto.FindAsync(id);

            var param = new SqlParameter("@id", id);

            var produto = await _context.Produto.FromSqlRaw("GetProdutoById @id", param).FirstOrDefaultAsync();
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome")] Produto produto)
        {
            if (id != produto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(produto);
                    var param = new SqlParameter("@Id", produto.Id);
                    var param2 = new SqlParameter("@nome", produto.Nome);
                    await _context.Database.ExecuteSqlRawAsync("UpdateProduto @Id, @nome", param, param2);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            //var produto = await _context.Produto
            //    .FirstOrDefaultAsync(m => m.Id == id);


            var param = new SqlParameter("@id", id);

            var produtos = await _context.Produto.FromSqlRaw("GetProdutoById @id", param).ToListAsync();

            var produto = produtos.FirstOrDefault();
            if (produto == null)
                return NotFound();

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                //var produto = await _context.Produto.FindAsync(id);
                //_context.Produto.Remove(produto);

                var param = new SqlParameter("@id", id);
                var produtos = await _context.Produto.FromSqlRaw("GetProdutoById @id", param).ToListAsync();
                var produto = produtos.FirstOrDefault();

                await _context.Database.ExecuteSqlRawAsync("EXEC DeletarProduto @id", produto.Id);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {

                throw;
            }
            
        }

        private bool ProdutoExists(int id)
        {
            //return _context.Produto.Any(e => e.Id == id);

            var param = new SqlParameter("@id", id);
            var produto = _context.Produto.FromSqlRaw("GetProdutoById @id", param).Any(); ;
            return produto;
        }
    }
}
