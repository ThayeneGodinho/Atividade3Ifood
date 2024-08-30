using MySql.Data.MySqlClient;

namespace appEscola
{
    public class alunos
    {
        private static string connectionString =
            "Server=localhost;Port=3306;Database=db_aulas_2024;User=kaique;password=1234567;SslMode=none;";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1 - Cadastrar restaurante");
                Console.WriteLine("2 - Cadastrar um novo prato");
                Console.WriteLine("3 - Cadastrar um cliente");
                Console.WriteLine("4 - Realizar pedido");
                Console.WriteLine("5 - Listar pedidos");
                Console.WriteLine("6 - Gerenciamento de pedidos");
                Console.WriteLine("7 - Sair");
                Console.Write("Escolha uma opção acima: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        CadastrarRestaurante();
                        break;
                    case "2":
                        CadastrarPratos();
                        break;
                    case "3":
                        CadastrarCliente();
                        break;
                    case "4":
                        RealizarPedido();
                        break;
                    case "5":
                        ListarPedidos();
                        break;
                    case "6":
                        GerenciamentoPedidos();
                        break;
                    case "7":
                        Console.WriteLine("Sair");
                        break;
                    default:
                        Console.WriteLine("Opção inválida");
                        break;
                }
            }
        }
        static void CadastrarRestaurante()
        {
            Console.Write("Informe o Nome do restaurante: ");
            string nome = Console.ReadLine();


            Console.Write("Informe o endereço: ");
            string endereco = Console.ReadLine();

            Console.Write("Informe o telefone do restaurante: ");
            float telefone = float.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO restaurantes (nome,endereco, telefone) VALUES (@nome,@endereco,@telefone)";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@endereco", endereco);
                cmd.Parameters.AddWithValue("@telefone", telefone);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Restaurante cadastrado com sucesso");
        }

        static void CadastrarPratos()
        {

            Console.Write("Informe o id do restaurante deste prato: ");
            int idrestaurantes = int.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM restaurantes WHERE id_restaurantes = @id";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", idrestaurantes);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        Console.WriteLine("O Id do restaurante informado não existe!");
                    }
                }
            }

            Console.Write("Informe o Nome do prato: ");
            string nome = Console.ReadLine();

            Console.Write("Informe a descrição: ");
            string descricao = Console.ReadLine();

            Console.Write("Informe o preço do prato : ");
            float preco = float.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO pratos (id_restaurantes,nome, descricao, preco) VALUES (@idrestaurantes,@nome, @descricao,@preco)";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@idrestaurantes", idrestaurantes);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@descricao", descricao);
                cmd.Parameters.AddWithValue("@preco", preco);

                cmd.ExecuteNonQuery();

                Console.WriteLine("Prato cadastrado com sucesso");
            }

        }

        static void CadastrarCliente()
        {
            Console.Write("Informe o Nome do cliente: ");
            string nome = Console.ReadLine();


            Console.Write("Informe o endereço do cliente: ");
            string endereco = Console.ReadLine();

            Console.Write("Informe o telefone do cliente: ");
            float telefone = float.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO clientes (nome,endereco, telefone) VALUES (@nome,@endereco,@telefone)";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@endereco", endereco);
                cmd.Parameters.AddWithValue("@telefone", telefone);

                cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Restaurante cadastrado com sucesso");
        }

        static void RealizarPedido()
        {
            Console.Write("ID do Cliente: ");
            int clienteId = int.Parse(Console.ReadLine());

            Console.Write("ID do Restaurante: ");
            int restauranteId = int.Parse(Console.ReadLine());

            decimal total = 0;
            List<int> pratoIds = new List<int>();
            List<int> quantidades = new List<int>();

            string opcao;
            do
            {
                Console.Write("ID do Prato: ");
                int pratoId = int.Parse(Console.ReadLine());

                Console.Write("Quantidade: ");
                int quantidade = int.Parse(Console.ReadLine());

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Preco FROM pratos WHERE id_pratos = @PratoId";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PratoId", pratoId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            decimal preco = reader.GetDecimal("Preco");
                            total += preco * quantidade;
                        }
                    }
                }

                pratoIds.Add(pratoId);
                quantidades.Add(quantidade);

                Console.Write("Adicionar mais pratos? (s/n): ");
                opcao = Console.ReadLine();
            } while (opcao.ToLower() == "s");

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO pedidos (id_clientes, id_restaurantes, total) VALUES (@ClienteId, @RestauranteId, @Total)";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteId", clienteId);
                command.Parameters.AddWithValue("@RestauranteId", restauranteId);
                command.Parameters.AddWithValue("@Total", total);

                try
                {
                    command.ExecuteNonQuery();
                    int pedidoId = (int)command.LastInsertedId;

                    for (int i = 0; i < pratoIds.Count; i++)
                    {
                        var itemQuery = "INSERT INTO itens_pedidos (id_pedidos, id_pratos, quantidade, preco) VALUES (@PedidoId, @PratoId, @Quantidade, @Preco)";
                        var itemCommand = new MySqlCommand(itemQuery, connection);
                        itemCommand.Parameters.AddWithValue("@PedidoId", pedidoId);
                        itemCommand.Parameters.AddWithValue("@PratoId", pratoIds[i]);
                        itemCommand.Parameters.AddWithValue("@Quantidade", quantidades[i]);

                        using (var priceCommand = new MySqlCommand("SELECT Preco FROM pratos WHERE id_pratos = @PratoId", connection))
                        {
                            priceCommand.Parameters.AddWithValue("@PratoId", pratoIds[i]);
                            decimal preco = (decimal)priceCommand.ExecuteScalar();
                            itemCommand.Parameters.AddWithValue("@Preco", preco);

                            itemCommand.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Pedido realizado com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao realizar pedido: " + ex.Message);
                }
            }
        }

        static void ListarPedidos()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM pedidos";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader["id_pedidos"]}, Idcliente: {reader["id_clientes"]}, Idrestaurante: {reader["id_restaurantes"]},Total: {reader["total"]}");

                        }
                    }
                    else
                    {
                        Console.WriteLine("Não existe pedidos cadastrado");
                    }

                }

            }
        }


        static void GerenciamentoPedidos()
        {
            Console.Write("Informe o ID do pedido que deseja gerenciar: ");
            int pedidoId = int.Parse(Console.ReadLine());

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Mostrar detalhes do pedido
                string query = @"SELECT p.id_pedidos AS PedidoId, c.nome AS ClienteNome, r.id_restaurantes AS RestauranteNome, datapedido,statuspedido, total
                                 FROM pedidos p
                                 INNER JOIN clientes c ON p.id_clientes = c.id_clientes
                                 INNER JOIN restaurantes r ON p.id_restaurantes = r.id_restaurantes
                                 WHERE p.id_pedidos = @PedidoId";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@PedidoId", pedidoId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Pedido ID: {reader["PedidoId"]}");
                        Console.WriteLine($"Cliente: {reader["ClienteNome"]}");
                        Console.WriteLine($"Restaurante: {reader["RestauranteNome"]}");
                        Console.WriteLine($"Data do Pedido: {reader["datapedido"]}");
                        Console.WriteLine($"Status do Pedido: {reader["statuspedido"]}");
                        Console.WriteLine($"Total: {reader["total"]}");
                    }
                    else
                    {
                        Console.WriteLine("Pedido não encontrado.");
                        return;
                    }
                }

                // Atualizar o status do pedido
                Console.Write("Informe o novo status do pedido: ");
                string statuspedido = Console.ReadLine();

                string updateQuery = "UPDATE pedidos SET statuspedido = @NovoStatus WHERE statuspedido = @PedidoId";
                MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@NovoStatus", statuspedido);
                updateCmd.Parameters.AddWithValue("@PedidoId", pedidoId);

                try
                {
                    updateCmd.ExecuteNonQuery();
                    Console.WriteLine("Status do pedido atualizado com sucesso!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao atualizar o status do pedido: " + ex.Message);
                }
            }
        }
    }
}