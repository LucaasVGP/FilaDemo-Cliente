using System.Text.Json;

namespace API
{
	public class ItemParametro
	{

		public long SimulacaoId { get; set; }
		public string PropA { get; set; }
		public string PropB { get; set; }

		public override string ToString()
		{
			return $"Prop A:{PropA} \n Prop B:{PropB} ";
		}

		public string Serializa()
		{
			return JsonSerializer.Serialize(this);
		}
	}



}
