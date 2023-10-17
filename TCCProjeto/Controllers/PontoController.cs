using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Claims;
using TCCProjeto.Context;
using TCCProjeto.Entities;

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
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (signInManager.IsSignedIn(User))
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Felipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using SqlConnection connection = new(connectionString);
                connection.Open();

                SqlCommand cmd = new($"SELECT Id FROM AspNetUsers WHERE Email = '{User.Identity!.Name}' ", connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string id;
                    id = reader[0].ToString()!;

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
            }
            return RedirectToAction("cadastro", "account");
        }

        public IActionResult NecessarioMaisPontos()
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

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Felipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                SqlCommand cmd = new($"SELECT valor FROM Produtos WHERE item = '{item}' ", connection);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
    
                    double valor = Convert.ToInt32(reader[0]);

                    var pessoa = await _context.Pessoas.FindAsync(id);

                    if (pessoa!.Pontos < valor)
                    {
                        return RedirectToAction("NecessarioMaisPontos", "ponto");
                    }

                        pessoa.Pontos -= valor;
                        _context.Users.Update(pessoa);

                        await SeedUserClaims(pessoa);

                        await _context.SaveChangesAsync();

                    return RedirectToAction("ProdutoResgatado", "ponto");
                }

            }
            return RedirectToAction("index", "home");
        }

        public async Task SeedUserClaims(Pessoa pessoa)
        {
            try
            {
                Pessoa user = await _userManager.FindByEmailAsync(pessoa.Email);
                if (user is not null)
                {
                    var claimList = (await _userManager.GetClaimsAsync(user))
                                                       .Select(p => p.Type);
                    if (!claimList.Contains("Item"))
                    {
                        var claimResult1 = await _userManager.AddClaimAsync(user,
                                 new Claim("Teste", "Passagem de onibus"));
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
