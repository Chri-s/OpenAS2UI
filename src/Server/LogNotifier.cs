using Microsoft.AspNetCore.SignalR;
using Microsoft.JSInterop.WebAssembly;
using Npgsql;
using OpenAS2UI.Server.Hubs;

namespace OpenAS2UI.Server
{
    internal class LogNotifier : BackgroundService
    {
        private NpgsqlConnection npgsqlConnection;
        private readonly IHubContext<LogHub> logHub;

        public LogNotifier(NpgsqlConnection npgsqlConnection, IHubContext<LogHub> logHub)
        {
            this.npgsqlConnection = npgsqlConnection;
            this.logHub = logHub;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await npgsqlConnection.OpenAsync(stoppingToken);

            npgsqlConnection.Notification += NpgsqlConnection_Notification;

            using (var cmd = npgsqlConnection.CreateCommand())
            {
                try
                {
                    cmd.CommandText = "LISTEN msg_metadata_INSERT";
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = "LISTEN msg_metadata_UPDATE";
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    
                }
            }

            while (true)
                await npgsqlConnection.WaitAsync();
        }

        private async void NpgsqlConnection_Notification(object sender, NpgsqlNotificationEventArgs e)
        {
            await this.logHub.Clients.All.SendAsync("NewLogEntry", e.Payload);
        }
    }
}
