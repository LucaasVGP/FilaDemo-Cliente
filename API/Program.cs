using System.Text;
using System.Text.Json;
using API;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };

var opcao = 0;

while (opcao != 6)
{
	var opcaoMenu = ExibeMenu();


	switch (opcaoMenu)
	{
		case 1:
			EnviarItemFila(factory, "Processar");
			break;
		case 2:
			ExibirItensFila(factory, "Resultados");
			break;
		case 3:
			ExibirItemFila(factory, "Resultados");
			break;
		case 4:
			EnviaParamentroFila(factory, "Parametros");
			break;
		case 5:
			Console.Clear();
			break;

	}


	opcao = opcaoMenu;
}



void EnviarItemFila(ConnectionFactory factory, string fila)
{
	var opcao = 0;
	Console.WriteLine();
	Console.Write("Id da simulacao: ");
	var opcaoEntrada = Console.ReadLine();
	if (!string.IsNullOrEmpty(opcaoEntrada))
	{
		try
		{
			opcao = int.Parse(opcaoEntrada);
		}
		catch
		{

			opcao = 0;
		}
	}
	if (opcao != 0)
	{

		var novoItemProcessar = new ItemProcessar
		{
			SimulacaoId = opcao,
			Comandos = new List<string> { "comandoA", "ComandoB" },
			Gatilho = "GatilhoA teste",
			RegraId = 1
		};
		var message = JsonSerializer.Serialize(novoItemProcessar);
		SendMessageToQueue(factory, message, fila);
	}
	else
	{
		Console.WriteLine("Id invalido");
	}
}

void EnviaParamentroFila(ConnectionFactory factory, string fila)
{
	var opcao = 0;
	Console.WriteLine();
	Console.Write("Id da simulacao: ");
	var opcaoEntrada = Console.ReadLine();
	if (!string.IsNullOrEmpty(opcaoEntrada))
	{
		try
		{
			opcao = int.Parse(opcaoEntrada);
		}
		catch
		{

			opcao = 0;
		}
	}
	if (opcao != 0)
	{

		var novoItemParametro = new ItemParametro
		{
			SimulacaoId = opcao,
			PropA = "",
			PropB = ""
		};
		SendMessageToQueue(factory, novoItemParametro.Serializa(), fila);
	}
	else
	{
		Console.WriteLine("Id invalido");
	}
}

void ExibirItemFila(ConnectionFactory factory, string fila)
{
	using var connectionConsumer = factory.CreateConnection();
	using var channelConsumer = connectionConsumer.CreateModel();

	channelConsumer.QueueDeclare(queue: fila, durable: false, exclusive: false, autoDelete: false, arguments: null);

	var result = channelConsumer.BasicGet(queue: fila, autoAck: true);
	if (result != null)
	{
		var body = result.Body.ToArray();
		var message = Encoding.UTF8.GetString(body);
		var itemResultado = JsonSerializer.Deserialize<ItemResultado>(message);
		if (itemResultado != null) Console.WriteLine(itemResultado.ToString());
	}
	else
	{
		Console.WriteLine("nenhum item na fila");
	}
}
void ExibirItensFila(ConnectionFactory factory, string fila)
{
	var opcao = 0;
	Console.WriteLine();
	Console.Write("Id da simulacao: ");
	var opcaoEntrada = Console.ReadLine();
	if (!string.IsNullOrEmpty(opcaoEntrada))
	{
		try
		{
			opcao = int.Parse(opcaoEntrada);
		}
		catch
		{

			opcao = 0;
		}
	}

	var itens = LerFila(factory, "Resultados", opcao);
	if (itens.Count > 0)
	{
		Console.WriteLine(string.Join(",", itens.Select(x => x.ToString())));
	}
	else
	{
		Console.WriteLine("nenhum item na fila");
	}



}
int ExibeMenu()
{
	var opcao = 0;
	Console.WriteLine("opções: \n");
	Console.WriteLine("1 - Enviar item para Fila. \n");
	Console.WriteLine("2 - Ler todos os itens fila. \n");
	Console.WriteLine("3 - Ler primeiro Item fila. \n");
	Console.WriteLine("4 - envia parametro fila \n");
	Console.WriteLine("5 - Limpar Tela. \n");
	Console.WriteLine("6 - Sair. \n");
	Console.Write("Escolha uma opção: \n");
	var opcaoEntrada = Console.ReadLine();
	try
	{
		if (!string.IsNullOrEmpty(opcaoEntrada))
			opcao = int.Parse(opcaoEntrada);
	}
	catch
	{
		opcao = 0;
	}
	return opcao;
}
List<ItemResultado> LerFila(ConnectionFactory factory, string fila, long simulacaoId)
{
	var itensDevolverFila = new List<string>();
	var itensExibir = new List<ItemResultado>();

	using var connectionConsumer = factory.CreateConnection();
	using var channelConsumer = connectionConsumer.CreateModel();

	channelConsumer.QueueDeclare(queue: fila, durable: false, exclusive: false, autoDelete: false, arguments: null);
	while (true)
	{
		var result = channelConsumer.BasicGet(queue: fila, autoAck: true);
		if (result != null)
		{
			var body = result.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);
			var itemResultado = JsonSerializer.Deserialize<ItemResultado>(message);
			if (itemResultado != null)
			{
				if (itemResultado.SimulacaoId == simulacaoId) itensExibir.Add(itemResultado);
				else itensDevolverFila.Add(JsonSerializer.Serialize(itemResultado));
			}
		}
		else
		{
			break;
		}

	};

	foreach (var item in itensDevolverFila)
	{
		SendMessageToQueue(factory, item, fila);
	}
	return itensExibir;

}

void SendMessageToQueue(ConnectionFactory factory, string message, string fila)
{
	using (var connection = factory.CreateConnection())
	using (var channelProducer = connection.CreateModel())
	{
		channelProducer.QueueDeclare(queue: fila, durable: false, exclusive: false, autoDelete: false, arguments: null);

		var body = Encoding.UTF8.GetBytes(message);

		channelProducer.BasicPublish(exchange: "", routingKey: fila, basicProperties: null, body: body);
		Console.WriteLine($"enviado para {fila}");
	}
}
