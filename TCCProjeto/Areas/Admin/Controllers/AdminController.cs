using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Dynamic;
using TCCProjeto.Areas.Admin.Models;
using TCCProjeto.Context;
using TCCProjeto.Entities;
using TCCProjeto.Models;

namespace TCCProjeto.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        List<Pessoa> ListaPessoa = new();

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Pessoas != null ?
                        View(await _context.Pessoas.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.Pessoas'  é null.");
        }


        [HttpGet]
        public async Task<IActionResult> Adicionar(string? id)
        {
            if (id == null || _context.Pessoas == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas.FindAsync(id);
            
            if (pessoa == null)
            {
                return NotFound();
            }
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Adicionar(string id, ModelPontos pontos)
        {

            if (id == null || _context.Pessoas == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas.FindAsync(id);

           if(pessoa == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    double? pontosParaReceber = 0;

                    if (pontos.QuantidadePlastico != null)
                    {
                        pontosParaReceber += pontos.QuantidadePlastico * 40;
                    }
                    else
                    {
                        pontos.QuantidadePlastico = 0;
                    }


                    if (pontos.QuantidadeMetal != null)
                    {
                        pontosParaReceber += pontos.QuantidadeMetal * 100;
                    }
                    else
                    {
                        pontos.QuantidadeMetal = 0;
                    }
                    
                    
                    if (pontos.QuantidadeVidro != null)
                    {
                        pontosParaReceber += pontos.QuantidadeVidro * 60;
                    }
                    else
                    {
                        pontos.QuantidadeVidro = 0;
                    }
                    
                    
                    if (pontos.QuantidadePapel != null)
                    {
                        pontosParaReceber += pontos.QuantidadePapel * 30;
                    }
                    else
                    {
                        pontos.QuantidadePapel = 0;
                    }
                    
                    
                    if (pontos.QuantidadeLixoEletronico != null)
                    {
                        pontosParaReceber += pontos.QuantidadeLixoEletronico * 200;
                    }
                    else
                    {
                        pontos.QuantidadeLixoEletronico = 0;
                    }

                    
                    if (pontos.QuantidadeOutrosLixos != null)
                    {
                        pontosParaReceber += pontos.QuantidadeOutrosLixos * 70;
                    }
                    else
                    {
                        pontos.QuantidadeOutrosLixos = 0;
                    }


                    pessoa.Pontos += (double)pontosParaReceber!;
                    
                    _context.Users.Update(pessoa);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }
        

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.Pessoas == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        public async Task<IActionResult> Edit(string? id)
        {
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

        // POST: Pessoas/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(string id, Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == pessoa.Id);

                    user.Nome = pessoa.Nome;
                    user.Pontos = pessoa.Pontos;
                    user.UserName = pessoa.UserName;
                    user.Email = pessoa.Email;


                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.Pessoas == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        // POST: Pessoas/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Pessoas == null)
            {
                return Problem("Entity set 'AppDbContext.Pessoas'  is null.");
            }
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa != null)
            {
                _context.Pessoas.Remove(pessoa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PessoaExists(string id)
        {
            return (_context.Pessoas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
