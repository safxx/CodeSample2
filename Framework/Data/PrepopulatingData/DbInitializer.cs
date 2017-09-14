using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using HighLoad.Application.Data;
using HighLoad.Framework.Json;
using Newtonsoft.Json;
using ServiceStack.OrmLite.Dapper;

namespace HighLoad.Framework.Data.PrepopulatingData
{
    public class DbInitializer
    {
        public static void Initialize(IVisitsRepository visitsRepository, IUsersRepository usersRepository, ILocationsRepository locationsRepository,
            IVisitViewDbConnectionFactory visitViewDbConnectionFactory, IMarkViewDbConnectionFactory markViewDbConnectionFactory)
        {
            using (var db = visitViewDbConnectionFactory.OpenDbConnection())
            {
                db.Execute(@"CREATE TABLE IF NOT EXISTS `mydb1`.`VisitView` (
                              `VisitId` int(11) NOT NULL,
                              `Country` varchar(50) CHARACTER SET utf8 NOT NULL,
                              `Distance` int(11) NOT NULL,
                              `LocationId` int(11) NOT NULL,
                              `Mark` tinyint(4) NOT NULL,
                              `Place` varchar(8000) CHARACTER SET utf8 NOT NULL,
                              `UserId` int(11) NOT NULL,
                              `VisitedAt` datetime NOT NULL,
                              PRIMARY KEY (`VisitId`),
                              UNIQUE KEY `IX_VisitViews_VisitId` (`VisitId`),
                              KEY `IX_VisitViews_LocationId` (`LocationId`),
                              KEY `IX_VisitViews_UserId` (`UserId`)
                            ) ENGINE=MyISAM DEFAULT CHARSET=utf8;");

                db.Execute("TRUNCATE TABLE `mydb1`.`VisitView`");
            }

            using (var db = markViewDbConnectionFactory.OpenDbConnection())
            {
                db.Execute(@"CREATE TABLE IF NOT EXISTS `mydb2`.`MarkView` (
                              `VisitId` int(11) NOT NULL,
                              `Age` tinyint(4) NOT NULL,
                              `Gender` bit(1) NOT NULL,
                              `LocationId` int(11) NOT NULL,
                              `Mark` tinyint(4) NOT NULL,
                              `UserId` int(11) NOT NULL,
                              `VisitedAt` datetime NOT NULL,
                              PRIMARY KEY (`VisitId`),
                              UNIQUE KEY `IX_MarkViews_VisitId` (`VisitId`),
                              KEY `IX_MarkViews_LocationId` (`LocationId`),
                              KEY `IX_MarkViews_UserId` (`UserId`)
                            ) ENGINE=MyISAM DEFAULT CHARSET=utf8;");

                db.Execute("TRUNCATE TABLE `mydb2`.`MarkView`");
            }

            var files = new ZipArchive(File.OpenRead("Data/data.zip"));
//            var files = new ZipArchive(File.OpenRead("Data/data2.zip"));
//            var files = new ZipArchive(File.OpenRead("../tmp/data/data.zip"));

            var dueTime = DateTime.Now + new TimeSpan(0, 0, 2, 55);

            PopulateUsers(usersRepository, files);
            PopulateLocations(locationsRepository, files);
            PopulateVisits(visitsRepository, files, dueTime);

            GC.Collect(2, GCCollectionMode.Forced);
            Debug.WriteLine("Finished initializing db");
        }

        private static void PopulateUsers(IUsersRepository usersRepository, ZipArchive files)
        {
            var filesWithUser = files.Entries.Where(e => e.Name.Contains("users"));

            var serializer = new JsonSerializer();
            serializer.ContractResolver = new UserContractResolver();
            serializer.Converters.Add(new MyDateTimeConverter());

            var users = filesWithUser.SelectMany(f =>
                {
                    using (var sr = new StreamReader(f.Open()))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<UsersContainer>(jsonTextReader).Users;
                    }
                }
            ).ToArray();

            usersRepository.CreateBatch(users);
        }

        private static void PopulateLocations(ILocationsRepository locationsRepository, ZipArchive files)
        {
            var serializer = new JsonSerializer();

            var filesWithLocations = files.Entries.Where(e => e.Name.Contains("locations"));
            var locations = filesWithLocations.SelectMany(f =>
                {
                    using (var sr = new StreamReader(f.Open()))
                    using (var jsonTextReader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<LocationsContainer>(jsonTextReader).Locations;
                    }
                }
            ).ToArray();

            locationsRepository.CreateBatch(locations);
        }

        private static void PopulateVisits(IVisitsRepository visitsRepository, ZipArchive files, DateTime dueTime)
        {
            var filesWithVisits = files.Entries.Where(e => e.Name.Contains("visits"));
            var start = DateTime.Now;

            var serializer = new JsonSerializer();
            serializer.ContractResolver = new VisitContractResolver();
            serializer.Converters.Add(new MyDateTimeConverter());

            foreach (var file in filesWithVisits)
            {
                var sw = Stopwatch.StartNew();
                var stream = file.Open();

                using (var sr = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    var visits = serializer.Deserialize<VisitsContainer>(jsonTextReader);
                    visitsRepository.CreateBatch(visits.Visits);
                }
                Console.WriteLine("Finished batch: " + sw.Elapsed);
                if (dueTime < DateTime.Now) break;
            }
            Console.WriteLine("Finished in: " + (DateTime.Now - start));
        }
    }
}