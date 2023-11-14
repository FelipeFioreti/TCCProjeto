using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using TCCProjeto.Models;

namespace TCCProjeto.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<Pessoa> userManager;
        private readonly SignInManager<Pessoa> signInManager;

        public AccountController(UserManager<Pessoa> userManager, SignInManager<Pessoa> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Cadastro(CadastroViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copia os dados do CadastroViewModel para a Pessoa
                var user = new Pessoa
                {
                    Nome = model.Nome,
                    UserName = model.Email,
                    Email = model.Email
                };

                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Password);


                Pessoa? EmailExistente = await userManager.FindByEmailAsync(model.Email);


                // Se o usuário foi criado com sucesso, faz o login do usuário usando o serviço SignInManager ...
                // e redireciona para o Método Action Index do controlador Home (HomeController).
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("index", "home");

                }

                
                // Se houver algum erro então inclui o ModelState que ...
                // será exibido pela tag helper summary na validação    
                foreach (var error in result.Errors)
                {   
                    if (EmailExistente?.Email == model.Email)
                    {
                        var email = $"Username '{model.Email}' is already taken.";
                        switch (error.Description)
                        {
                            case "Passwords must be at least 8 characters.":
                                error.Description = "Senhas devem possuir mais de 8 caracteres.";
                                break;
                            case "Passwords must have at least one non alphanumeric character.":
                                error.Description = "Senhas devem possuir pelo menos um caractere alpha numérico.";
                                break;
                            case "Passwords must have at least one lowercase ('a'-'z').":
                                error.Description = "Senhas devem possuir pelo menos uma letra minúscula.";
                                break;
                            case "Passwords must have at least one uppercase ('A'-'Z').":
                                error.Description = "Senhas devem possuir pelo menos uma letra maiúscula.";
                                break;
                            case "Passwords must have at least one digit ('0'-'9').":
                                error.Description = "Senhas devem possuir pelo menos um numeral ('0'-'9').";
                                break;
                            default:
                                error.Description = "Email já foi utilizado.";
                                break;
                        }

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    else
                    {
                        error.Description = error.Description switch
                        {
                            "Passwords must be at least 8 characters." => "Senhas devem possuir mais de 8 caracteres.",
                            "Passwords must have at least one non alphanumeric character." => "Senhas devem possuir pelo menos um caractere alpha numérico.",
                            "Passwords must have at least one lowercase ('a'-'z')." => "Senhas devem possuir pelo menos uma letra minúscula.",
                            "Passwords must have at least one uppercase ('A'-'Z')." => "Senhas devem possuir pelo menos uma letra maiúscula.",
                            "Passwords must have at least one digit ('0'-'9')." => "Senhas devem possuir pelo menos um numeral ('0'-'9').",
                            _ => "Cadastro inválido",
                        };
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Se o modelState for válido
            if (ModelState.IsValid)
            {
                // Irá tentar fazer login usando Email e Password.
                // Deve ser passado como parâmetro também o RememberMe ...
                // e um valor true ou false se deve ou não bloquear a conta do usuário...
                // caso uma tentativa de login seja inválida
                Console.WriteLine(model.Email);
                Console.WriteLine(model.Password);
                Console.WriteLine(model.RememberMe);

                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);
                
                // Se for um sucesso
                if (result.Succeeded)
                {
                    // Irá ser redirecionado para o Método index do controlador home
                    return RedirectToAction("index", "home");
                }
                // Se não for um sucesso
                // Irá adicionar um erro ao Model login inválido
                ModelState.AddModelError(string.Empty, "Login Inválido");
            }
            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [Route("/Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
