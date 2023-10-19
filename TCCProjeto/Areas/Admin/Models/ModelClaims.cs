using System.Security.Claims;

namespace MvcWebIdentity.Areas.Admin.Models;

public class ModelClaims
{
    public ModelClaims()
    {
        Claims = new List<Claim>();
    }
    public string? Id { get; set; }

    public string? Nome { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public double Pontos { get; set; }
    public List<Claim>? Claims { get; set; }
}
