
using Business.Models;
using Business.Services;
using Common.Client;
using Common.Helper;
using Common.Log;
using MMS.Common.CheckDeposit.Services;
using MMS.Service.CheckDeposit.Repository.Common;

namespace MyAPIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                //builder.Services.Configure<CoreAPISettings>(builder.Configuration);
                builder.Services.AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
                builder.Services.AddControllers();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddTransient<IMethodExecutionHelper<TransactionEntity>, MethodExecutionHelper<TransactionEntity>>();
                builder.Services.AddTransient<ITransferService, TransferService>();

                #region client

                builder.Services.AddTransient<HttpClientLoggingHandler>();
                builder.Services.AddHttpClient<IWebApiClient, WebApiClient>(c =>
                        c.Timeout = TimeSpan.FromMilliseconds(10000))
                    .AddHttpMessageHandler<HttpClientLoggingHandler>();
                //services.AddHttpClient("VSoftClient").AddHttpMessageHandler<HttpClientLoggingHandler1>();
                builder.Services.AddHttpClient<ISoapClient, SoapClient>(c =>
                        c.Timeout = TimeSpan.FromMilliseconds(10000))
                    .AddHttpMessageHandler<HttpClientLoggingHandler>();

                #endregion

                builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                builder.Services.AddHostedService<QueuedHostedService>();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
