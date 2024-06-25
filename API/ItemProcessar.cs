namespace API
{
	public class ItemProcessar
	{
		public long SimulacaoId { get; set; }
		public long RegraId { get; set; }
		public string Gatilho { get; set; }
		public List<string> Comandos { get; set; }
	}
}
