using System.Text;
using OpenAS2UI.Server.Hubs;
using OpenAS2UI.Server;
using Microsoft.AspNetCore.ResponseCompression;
using Npgsql;

namespace OpenAS2UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            string url = builder.Configuration["OpenAS2:Url"];

            builder.Services.AddSingleton(sp =>
            {   
                string user = builder.Configuration["OpenAS2:UserId"];
                string password = builder.Configuration["OpenAS2:Password"];
                string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));

                HttpClient httpClient = new HttpClient { BaseAddress = new Uri(url) };
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
                return httpClient;
            });
            builder.Services.AddTransient<DataService>();
            builder.Services.AddTransient<OpenAs2Client>(s =>
            {
                OpenAs2Client client = new OpenAs2Client(s.GetRequiredService<HttpClient>());
                client.BaseUrl = url;
                return client;
            });
            

            if (!string.IsNullOrEmpty(builder.Configuration.GetConnectionString("LogConnection")))
            {
                builder.Services.AddSingleton(sp => new NpgsqlConnection(builder.Configuration.GetConnectionString("LogConnection")));
                builder.Services.AddHostedService<LogNotifier>();
            }

            builder.Services.AddSignalR();
            builder.Services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
            });

            var app = builder.Build();

            app.UseResponseCompression();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapHub<LogHub>("/loghub");
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}