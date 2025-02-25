using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TCCProjeto.Areas.Admin.Models;
using System.Security.Claims;
using TCCProjeto.Context;
using TCCProjeto.Models;
using TCCProjeto.DataBase;

namespace TCCProjeto.Controllers
{
    public class PontoController : Controller
    {
        private readonly SignInManager<Pessoa> signInManager;
        private readonly AppDbContext _context;
        private readonly UserManager<Pessoa> _userManager;

        public PontoController(SignInManager<Pessoa> signInManager, UserManager<Pessoa> userManager, AppDbContext context)
        {
            this.signInManager = signInManager;
            this. _userManager = userManager;
            this._context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                string id = _userManager.GetUserId(User)!;

                if (id == null || _context.Pessoas == null)
                {
                    return NotFound();
                }

                var pessoa = await _context.Pessoas.FindAsync(id);

                if (pessoa == null)
                {
                    return NotFound();
                }

                return View(pessoa);
                
            }
            return RedirectToAction("cadastro", "account");
        }

        public async Task<IActionResult> Inventario()
        {
            if (signInManager.IsSignedIn(User))
            {
                string id = _userManager.GetUserId(User)!;

                if (id == null || _context.Pessoas == null)
                {
                    return NotFound();
                }

                var pessoa = await _context.Pessoas.FindAsync(id);

                if (pessoa == null)
                {
                    return NotFound();
                }

                var userClaims = await _userManager.GetClaimsAsync(pessoa);

                var model = new ModelClaims
                {
                    Id = pessoa.Id,
                    Email = pessoa.Email,
                    Nome = pessoa.Nome,
                    UserName = pessoa.UserName,
                    Pontos = pessoa.Pontos,
                    Claims = userClaims.ToList(),
                };

                return View(model);
                
            }
            return RedirectToAction("cadastro", "account");
        }

        public IActionResult NecessarioMaisPontos()
        {
            return View();
        }

        public IActionResult ProdutoNoInventario()
        {
            return View();
        }
        public IActionResult ProdutoIndisponivel()
        {
            return View();
        }

        public IActionResult ProdutoResgatado()
        {
            return View();
        }

        public async Task<IActionResult> ResgatarProdutos(string id,string item)
        {
            if (id == null)
            {
                return NotFound();
            }

            DatabaseManager db = new DatabaseManager();

            using SqlConnection sqlConnection = new SqlConnection(db.GetConnectionString());
            sqlConnection.Open();

            SqlCommand cmd = new("SELECT * FROM Produtos WHERE item = @item", sqlConnection);
            cmd.Parameters.AddWithValue("@item", item);
            SqlDataReader reader = cmd.ExecuteReader();


            while (reader.Read())
            {
                Produto produto = new()
                {
                    Id = Convert.ToInt32(reader[0]),
                    Nome = reader[1].ToString(),
                    Item = reader[2].ToString(),
                    Valor = Convert.ToInt32(reader[3]),
                    Quantidade = Convert.ToInt32(reader[4]),
                    Ativo = Convert.ToBoolean(reader[5]),
                };

                if (produto.Ativo == false)
                {
                    return RedirectToAction("ProdutoIndisponivel", "ponto");
                }

                if (produto == null)
                {
                    return NotFound();
                }

                if (produto!.Ativo == false)
                {
                    return RedirectToAction("ProdutoIndisponivel", "ponto");
                }

                var pessoa = await _context.Pessoas.FindAsync(id);

                if (pessoa is not null)
                {
                    if (pessoa.Pontos < produto.Valor)
                    {
                        return RedirectToAction("NecessarioMaisPontos", "ponto");
                    }

                    var claimList = (await _userManager.GetClaimsAsync(pessoa))
                                                        .Select(p => p.Type);
                    if (!claimList.Contains(produto.Nome))
                    {
                        pessoa.Pontos -= produto.Valor;

                        produto.Quantidade--;

                        if (produto.Quantidade == 0)
                        {
                            produto.Ativo = false;
                        }

                        var claimResult1 = await _userManager.AddClaimAsync(pessoa,
                                    new Claim(produto.Nome!, "1"));

                        _context.Users.Update(pessoa);
                        _context.Produtos.Update(produto);
                    }
                    else
                    {
                        return RedirectToAction("ProdutoNoInventario", "ponto");
                    }
                }
               
            await _context.SaveChangesAsync();

            return RedirectToAction("ProdutoResgatado", "ponto");
            }

            return RedirectToAction("error", "home");
        }


    }
}
