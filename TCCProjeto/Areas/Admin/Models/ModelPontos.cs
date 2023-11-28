using System.ComponentModel.DataAnnotations;

namespace TCCProjeto.Areas.Admin.Models
{
    public class ModelPontos
    {
        public double? QuantidadePlastico { get; set; }
        public double? QuantidadeMetal { get; set; }
        public double? QuantidadeVidro { get; set; }
        public double? QuantidadePapel { get; set; }
        public double? QuantidadeLixoEletronico { get; set; }
        [Display(Name = "Outros Lixos")]
        public double? QuantidadeOutrosLixos { get; set; }
    }
}
