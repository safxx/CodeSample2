using System;
using System.Runtime;
using HighLoad.Application.Data;
using HighLoad.Framework.Binding;
using HighLoad.Framework.Data;
using HighLoad.Framework.Data.PrepopulatingData;
using HighLoad.Framework.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace HighLoad
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

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.Formatting = Formatting.None;
                options.SerializerSettings.ContractResolver = new TravelingContractResolver();
                options.SerializerSettings.Converters.Add(new MyDateTimeConverter());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IVisitsRepository visitsRepository, IUsersRepository usersRepository, ILocationsRepository locationsRepository, IVisitViewDbConnectionFactory visitViewDbConnectionFactory, IMarkViewDbConnectionFactory markViewDbConnectionFactory)
        {
            app.UseResponseBuffering();
            app.UseMvc();

            DbInitializer.Initialize(visitsRepository, usersRepository, locationsRepository, visitViewDbConnectionFactory, markViewDbConnectionFactory);
        }
    }
}
