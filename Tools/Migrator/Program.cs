using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using DbUp;

namespace Migrator
{
    class Program
    {

        static string connectionString = "Server= localhost; Database= SlotsDev; Integrated Security=True;";

        static int Main(string[] args)
        {
            if (args.Any(x => x == "--drop"))
            {
                Drop();
                return 0;
            }
            else if (args.Any(x => x == "--new"))
            {
                Drop();
                return Migrate();
            }
            else
            {
                return Migrate();
            }
        }

        static int Migrate()
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To.SqlDatabase(connectionString)
                                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                                .WithTransactionPerScript()
                                .LogToConsole()
                                .Build();

            Console.WriteLine("Is upgrade required: " + upgrader.IsUpgradeRequired());

            var result = upgrader.PerformUpgrade();
            if (!result.Successful)
            {
                LogError(result.Error);
                return -1;
            }
            else
            {
                LogSuccess();
                return 0;
            }
        }

        static int Drop()
        {
            DropDatabase.For.SqlDatabase(connectionString);
            return 0;
        }

        static void LogError(Exception err)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(err);
            Console.WriteLine("Failed!");
            Console.ResetColor();
        }

        static void LogSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
        }
    }
}
