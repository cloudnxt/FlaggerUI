using Gates.Server.Data;
using Gates.Server.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Gates
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase(databaseName: "GatesApp"));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IAppService, AppService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IGateService, GateService>();
            builder.Services.AddScoped<ILoadTestService, LoadTestService>();
            builder.Services.AddScoped<ILogsServices, LogsServices>();
            
            builder.Services.AddHostedService<QueuedHostedService>();
            builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
            {
                if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
                {
                    queueCapacity = 100;
                }

                return new DefaultBackgroundTaskQueue(queueCapacity);
            });
            //builder.Services.AddHostedService<Worker>();
            var app = builder.Build();
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

            //app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}