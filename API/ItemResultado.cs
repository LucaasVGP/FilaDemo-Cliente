namespace API
{
	public class ItemResultado
	{
		public long SimulacaoId { get; set; }
		public string? SimulacaoJson { get; set; }
		public string? LogsJson { get; set; }

		public override string ToString()
		{
			return $"Simulação: {SimulacaoId} \n  SimulacaoJson:{SimulacaoJson} \n LogsJson: {LogsJson}";
		}

	}
}
