using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TCCProjeto.Areas.Admin.Models;
using System.Data;
using System.Dynamic;
using System.Security.Claims;
using TCCProjeto.Context;
using TCCProjeto.Models;

namespace TCCProjeto.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Pessoa> _userManager;
        public AdminController(AppDbContext context, UserManager<Pessoa> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                        pontosParaReceber += pontos.QuantidadePlastico * 0.04;
                    }
                    else
                    {
                        pontos.QuantidadePlastico = 0;
                    }


                    if (pontos.QuantidadeMetal != null)
                    {
                        pontosParaReceber += pontos.QuantidadeMetal * 0.1;
                    }
                    else
                    {
                        pontos.QuantidadeMetal = 0;
                    }
                    
                    
                    if (pontos.QuantidadeVidro != null)
                    {
                        pontosParaReceber += pontos.QuantidadeVidro * 0.06;
                    }
                    else
                    {
                        pontos.QuantidadeVidro = 0;
                    }
                    
                    
                    if (pontos.QuantidadePapel != null)
                    {
                        pontosParaReceber += pontos.QuantidadePapel * 0.03;
                    }
                    else
                    {
                        pontos.QuantidadePapel = 0;
                    }
                    
                    
                    if (pontos.QuantidadeLixoEletronico != null)
                    {
                        pontosParaReceber += pontos.QuantidadeLixoEletronico * 0.2;
                    }
                    else
                    {
                        pontos.QuantidadeLixoEletronico = 0;
                    }

                    
                    if (pontos.QuantidadeOutrosLixos != null)
                    {
                        pontosParaReceber += pontos.QuantidadeOutrosLixos * 0.07;
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

        public async Task<IActionResult> DeleteClaim(string claimValues)
        {
            string[] claimValuesArray = claimValues.Split(";");
            string claimType = claimValuesArray[0].ToString();
            string claimValue = claimValuesArray[1].ToString();
            string userId = claimValuesArray[2].ToString();

            Pessoa user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {

                var userClaims = await _userManager.GetClaimsAsync(user);

                Claim claim = userClaims.FirstOrDefault(x => x.Type.Equals(claimType)
                              && x.Value.Equals(claimValue))!;

                IdentityResult result = await _userManager.RemoveClaimAsync(user, claim);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
            {
                ModelState.AddModelError("", "Usuário não encontrado");
            }

            return View("Index");
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

        void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
