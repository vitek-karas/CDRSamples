using PluginBase;
using System;
using System.Collections.Generic;
using System.IO;
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

                string pluginLocation = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(typeof(Program).Assembly.Location), 
                    @"..\..\..\..\HelloPlugin\bin\Debug\netcoreapp2.1\HelloPlugin.dll".Replace('\\', Path.DirectorySeparatorChar)));

                Console.WriteLine($"Loading plugin from: {pluginLocation}");
                PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
                Assembly helloPluginAssembly = loadContext.LoadFromAssemblyName(new AssemblyName("HelloPlugin"));
                ICommandFactory commandFactory = CreateCommandFactory(helloPluginAssembly);

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

        static ICommandFactory CreateCommandFactory(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommandFactory).IsAssignableFrom(type))
                {
                    ICommandFactory result = Activator.CreateInstance(type) as ICommandFactory;
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find type which implements ICommandFactory in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }
    }
}
