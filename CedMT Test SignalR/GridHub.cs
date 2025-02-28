using Microsoft.AspNetCore.SignalR;

namespace CedMT_Test_SignalR
{
    public sealed class GridHub : Hub
    {
        public async Task GetGridRecords(string hash)
        {
            var records = GetMockedRecords(hash);
            await Clients.Caller.SendAsync("ReceiveRecords", records);
        }

        public async Task GetGridSummary(string hash)
        {
            var summary = GetMockedSummary(hash);
            await Clients.Caller.SendAsync("ReceiveGridSummary", summary);
        }

        private List<string> GetMockedRecords(string hash)
        {
            return hash switch
            {
                "1" => new List<string> { "Registro 1.1", "Registro 1.2", "Registro 1.3" },
                "2" => new List<string> { "Registro 2.1", "Registro 2.2", "Registro 2.3" },
                "3" => new List<string> { "Registro 3.1", "Registro 3.2", "Registro 3.3" },
                _ => new List<string> { "Registro por defecto 1", "Registro por defecto 2" },
            };
        }

        private object GetMockedSummary(string hash)
        {
            var random = new Random();
            var reconciliationsByStates = new
            {
                success = new { display = "Success", quantity = random.Next(300, 500), percent = random.Next(30, 50), amount = random.Next(800, 1200) },
                pending = new { display = "Pending", quantity = random.Next(100, 300), percent = random.Next(10, 30), amount = random.Next(400, 800), ratio = random.NextDouble() },
                failed = new { display = "Failed", quantity = random.Next(200, 400), percent = random.Next(20, 40), amount = random.Next(600, 1000), ratio = random.NextDouble() },
                unadjusted = new { display = "Unadjusted", quantity = random.Next(50, 150), percent = random.Next(5, 15), amount = random.Next(200, 400), ratio = random.NextDouble() },
                ignored = new { display = "Ignored", quantity = random.Next(50, 150), percent = random.Next(5, 15), amount = random.Next(100, 300), ratio = random.NextDouble() }
            };

            var reconciliationsByCriteria = new
            {
                soloN = new { display = "Solo N", description = "Conciliaciones rechazadas en Payment", quantity = random.Next(50, 150), amount = random.Next(50, 150), percent = random.Next(5, 15), ratio = random.NextDouble() },
                soloS = new { display = "Solo S", description = "Conciliaciones aprobadas en Payment", quantity = random.Next(100, 200), amount = random.Next(100, 200), percent = random.Next(10, 20), ratio = random.NextDouble() },
                diferenciaImporte = new { display = "Diferencia de Importe", description = "Conciliaciones con diferencias de importe", quantity = random.Next(200, 300), amount = random.Next(200, 300), percent = random.Next(20, 30), ratio = random.NextDouble() }
            };

            return new
            {
                difference_balance = random.Next(400, 600),
                total_reconciliations = random.Next(800, 1200),
                reconciliations_by_states = reconciliationsByStates,
                reconciliations_by_criteria = reconciliationsByCriteria,
                differences_summary_grid = new { }
            };
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceivedMessage", $"{Context.ConnectionId} has joined");
            await base.OnConnectedAsync();
        }
    }

    // Definir el servicio en segundo plano
    public class GridBackgroundService : BackgroundService
    {
        private readonly IHubContext<GridHub> _hubContext;

        public GridBackgroundService(IHubContext<GridHub> hubContext)
        {
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var random = new Random();
                var hash = random.Next(1, 4).ToString();
                var summary = GetMockedSummary(hash);

                await _hubContext.Clients.All.SendAsync("ReceiveGridSummary", summary, stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private object GetMockedSummary(string hash)
        {
            var random = new Random();
            var reconciliationsByStates = new
            {
                success = new { display = "Success", quantity = random.Next(300, 500), percent = random.Next(30, 50), amount = random.Next(800, 1200) },
                pending = new { display = "Pending", quantity = random.Next(100, 300), percent = random.Next(10, 30), amount = random.Next(400, 800), ratio = random.NextDouble() },
                failed = new { display = "Failed", quantity = random.Next(200, 400), percent = random.Next(20, 40), amount = random.Next(600, 1000), ratio = random.NextDouble() },
                unadjusted = new { display = "Unadjusted", quantity = random.Next(50, 150), percent = random.Next(5, 15), amount = random.Next(200, 400), ratio = random.NextDouble() },
                ignored = new { display = "Ignored", quantity = random.Next(50, 150), percent = random.Next(5, 15), amount = random.Next(100, 300), ratio = random.NextDouble() }
            };

            var reconciliationsByCriteria = new
            {
                soloN = new { display = "Solo N", description = "Conciliaciones rechazadas en Payment", quantity = random.Next(50, 150), amount = random.Next(50, 150), percent = random.Next(5, 15), ratio = random.NextDouble() },
                soloS = new { display = "Solo S", description = "Conciliaciones aprobadas en Payment", quantity = random.Next(100, 200), amount = random.Next(100, 200), percent = random.Next(10, 20), ratio = random.NextDouble() },
                diferenciaImporte = new { display = "Diferencia de Importe", description = "Conciliaciones con diferencias de importe", quantity = random.Next(200, 300), amount = random.Next(200, 300), percent = random.Next(20, 30), ratio = random.NextDouble() }
            };

            return new
            {
                difference_balance = random.Next(400, 600),
                total_reconciliations = random.Next(800, 1200),
                reconciliations_by_states = reconciliationsByStates,
                reconciliations_by_criteria = reconciliationsByCriteria,
                differences_summary_grid = new { }
            };
        }
    }
}