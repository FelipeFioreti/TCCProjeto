using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using TCCProjeto.Areas.Admin.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Claims;
using TCCProjeto.Context;
using TCCProjeto.Models;

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
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Felipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
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

        public async Task<IActionResult> Inventario()
        {
            if (signInManager.IsSignedIn(User))
            {
                string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Felipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
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

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Felipe;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using SqlConnection connection = new(connectionString);
            connection.Open();

            SqlCommand cmd = new($"SELECT * FROM Produtos WHERE item = '{item}' ", connection);
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
