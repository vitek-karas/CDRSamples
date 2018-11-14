using PluginBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppWithPlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "/d")
                {
                    Console.WriteLine("Waiting for any key...");
                    Console.ReadLine();
                }

                PluginLoadContext loadContext = new PluginLoadContext(@"F:\vitkaras\CDRSamples\AppWithPlugin\HelloPlugin\bin\Debug\netcoreapp2.2\HelloPlugin.dll");
                Assembly helloPluginAssembly = loadContext.LoadFromAssemblyName(new AssemblyName("HelloPlugin"));
                ICommandFactory commandFactory = (ICommandFactory)helloPluginAssembly.CreateInstance("HelloPlugin.CommandFactory");

                IEnumerable<ICommand> commands = commandFactory.CreateCommands();

                if (args.Length == 0)
                {
                    Console.WriteLine("Commands: ");
                    foreach (ICommand command in commands)
                    {
                        Console.WriteLine($"{command.Name}\t - {command.Description}");
                    }
                }
                else
                {
                    ICommand command = commands.FirstOrDefault(c => c.Name == args[0]);
                    if (command == null)
                    {
                        Console.WriteLine("No such command is known.");
                        return;
                    }

                    command.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
