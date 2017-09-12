using System.IO;
using AutoMapper;
using HighLoad.Application;
using HighLoad.Application.Data;
using HighLoad.Application.Data.ReadModel.MarkView;
using HighLoad.Application.Data.ReadModel.VisitView;
using HighLoad.Application.Entities;
using HighLoad.Framework.Data;
using HighLoad.Framework.Data.ReadModel;
using HighLoad.Framework.Data.Repositories;
using HighLoad.Framework.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using ServiceStack.OrmLite;

namespace HighLoad
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.Add(ServiceDescriptor.Singleton<IUsersRepository, UsersRepository>());
                    services.Add(ServiceDescriptor.Singleton<IVisitsRepository, VisitsRepository>());
                    services.Add(ServiceDescriptor.Singleton<ILocationsRepository, LocationsRepository>());
                    services.Add(ServiceDescriptor.Singleton<IVisitViewUpdater, VisitViewUpdater>());
                    services.Add(ServiceDescriptor.Singleton<IMarkViewUpdater, MarkViewUpdater>());
                    services.Add(ServiceDescriptor.Singleton<IExistingEntitiesLookup<Framework.Data.Entities.User>, ExistingEntitiesLookup<Framework.Data.Entities.User>>());
                    services.Add(ServiceDescriptor.Singleton<IExistingEntitiesLookup<Framework.Data.Entities.Location>, ExistingEntitiesLookup<Framework.Data.Entities.Location>>());
                    services.Add(ServiceDescriptor.Singleton<IExistingEntitiesLookup<Framework.Data.Entities.Visit>, ExistingEntitiesLookup<Framework.Data.Entities.Visit>>());
                    services.Add(ServiceDescriptor.Singleton<IVisitsProcessingQueue, VisitsProcessingQueue>());
                    services.Add(ServiceDescriptor.Singleton<IVisitViewProvider, VisitViewProvider>());
                    services.Add(ServiceDescriptor.Singleton<IMarkViewProvider, MarkViewProvider>());
                    services.Add(ServiceDescriptor.Singleton<IVisitViewsContainerViewModelFactory, VisitViewsContainerViewModelFactory>());

                    Mapper.Initialize(cfg =>
                    {
                        cfg.CreateMap<Location, Framework.Data.Entities.Location>().ReverseMap();
                        cfg.CreateMap<User, Framework.Data.Entities.User>().ReverseMap();
                        cfg.CreateMap<Visit, Framework.Data.Entities.Visit>().ReverseMap();
                    });

                    var connection1 = "Server=localhost;uid=root;Port=3307;pwd=123;charset=utf8;database=mydb1;SslMode=none;Connection Timeout=30;";
                    var connection2 = "Server=localhost;uid=root;Port=3307;pwd=123;charset=utf8;database=mydb2;SslMode=none;Connection Timeout=30;";

                    using (var connection = new MySqlConnection("Server=localhost;Port=3307;uid=root;pwd=123;charset=utf8;SslMode=none;Connection Timeout=30;"))
                    {
                        connection.Open();
                        new MySqlCommand("CREATE DATABASE IF NOT EXISTS `mydb1`;", connection).ExecuteNonQuery();
                        new MySqlCommand("CREATE DATABASE IF NOT EXISTS `mydb2`;", connection).ExecuteNonQuery();
                    }

                    services.AddScoped<IVisitViewDbConnectionFactory>(p => new VisitViewDbConnectionFactory(connection1, MySqlDialect.Provider));
                    services.AddScoped<IMarkViewDbConnectionFactory>(p => new MarkViewDbConnectionFactory(connection2, MySqlDialect.Provider));
                })
                .Build();

            host.Run();
        }
    }
}
