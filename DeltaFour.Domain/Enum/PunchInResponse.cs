using System.ComponentModel;

namespace DeltaFour.Domain.Enum
{
    public enum PunchInResponse
    {
        [Description("Você está fora do limite da empresa.")]
        OFR,
        
        [Description("Rosto não identificado, por favor tire uma foto melhor")]
        FNC,
        
        [Description("Sucesso! Ponto batido.")]
        SCC
    }
}
