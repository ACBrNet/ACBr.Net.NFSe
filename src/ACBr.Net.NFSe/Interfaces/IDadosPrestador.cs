using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Interfaces
{
	public interface IDadosPrestador
	{
		string ChaveAcesso { get; set; }
		string CpfCnpj { get; set; }
		DadosContato DadosContato { get; }
		Endereco Endereco { get; }
		string FraseSecreta { get; set; }
		string InscricaoMunicipal { get; set; }
		string NomeFantasia { get; set; }
		string RazaoSocial { get; set; }
		string Senha { get; set; }
	}
}