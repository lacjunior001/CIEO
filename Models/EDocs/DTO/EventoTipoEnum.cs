namespace CIEO.Models.EDocs.DTO
{
    /// <summary>
    /// Informação sobre o evento recebido do e-docs ao consultar.
    /// </summary>
    public enum EventoTipoEnum
    {
        CapturaNatoDigitalICPBrasil = 101,
        CapturaNatoDigitalCopia = 102,
        CapturaNatoDigitalMultiplasAssinaturasServidor = 103,
        CapturaNatoDigitalMultiplasAssinaturasCidadao = 104,
        CapturaNatoDigitalAssinaturaEletronicaServidor = 105,
        CapturaNatoDigitalAssinaturaEletronicaCidadao = 106,
        CapturaDigitalizadoServidor = 107,
        CapturaDigitalizadoCidadao = 108,
        EncaminhamentoInicial = 201,
        EncaminhamentoPosterior = 202,
        AutuacaoProcesso = 301,
        DespachoProcesso = 302,
        DesentranhamentoProcesso = 303,
        EntranhamentoProcesso = 304,
        AvocamentoProcesso = 305,
        EncerramentoProcesso = 306,
        ReaberturaProcesso = 307,
        AjusteCustodiaProcesso = 308,
        SobrestamentoProcesso = 309,
        ProsseguimentoProcesso = 310,
        EdicaoProcesso = 311,
        CopiaProcesso = 312
    }
}
