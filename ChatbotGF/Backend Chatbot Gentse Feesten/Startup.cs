using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Chatbot_GF.MessengerManager;
using Chatbot_GF.Data;
using Chatbot_GF.MessageBuilder.Factories;
using Chatbot_GF.Client;

namespace Chatbot_GF
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            // Add framework services.
            services.AddMvc();

            //Add custom services
            services.AddScoped<IPayloadHandler, PayloadHandler>();
            services.AddScoped<IMessageHandler, MessageHandler>();
            services.AddTransient<IReplyManager, ReplyManager>();
            services.AddSingleton<ITempUserData, TempUserData>();
            services.AddSingleton<IDataConstants, DataConstants>();
            services.AddSingleton<ITextHandler, FreeTextHandler>();
            services.AddScoped<IRemoteDataManager, RemoteDataManager>();
            services.AddTransient<ICarouselFactory, CarouselFactory>();
            services.AddTransient<ILocationFactory, LocationFactory>();

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/Chatbot-{Date}.txt");

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
