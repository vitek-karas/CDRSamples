﻿using CsvHelper;
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

                string[] pluginPaths = new string[]
                {
                    @"HelloPlugin\bin\Debug\netcoreapp2.1\HelloPlugin.dll",
                    @"JsonPlugin\bin\Debug\netcoreapp2.1\JsonPlugin.dll",
                    @"OldJsonPlugin\bin\Debug\netcoreapp2.1\OldJsonPlugin.dll",
                    @"FrenchPlugin\bin\Debug\netcoreapp2.1\FrenchPlugin.dll",
                    @"UVPlugin\bin\Debug\netcoreapp2.1\UVPlugin.dll",
                    @"CsvPlugin\bin\Debug\netcoreapp2.1\CsvPlugin.dll"
                };

                IEnumerable<ICommand> commands = pluginPaths.SelectMany(pluginPath =>
                {
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreateCommands(pluginAssembly);
                }).ToList();

                // Write out the list of plugins and their implementation assemblis.
                // This also acts as a test since it uses CsvHelper package (v 9.1.0).
                using (CsvWriter cmdListWriter = new CsvWriter(Console.Out))
                {
                    Console.WriteLine();
                    Console.WriteLine($"  Using {cmdListWriter.GetType().Assembly.FullName} from {cmdListWriter.GetType().Assembly.Location}");
                    cmdListWriter.WriteHeader<CommandRecord>();
                    cmdListWriter.WriteRecords(commands.Select(c => new CommandRecord() { Name = c.Name, AssemblyName = c.GetType().Assembly.FullName }));
                }

                if (args.Length == 0)
                {
                    // If there are no command specified, show the list of command and their descriptions.
                    Console.WriteLine();
                    Console.WriteLine("Commands: ");
                    foreach (ICommand command in commands)
                    {
                        Console.WriteLine($"{command.Name}\t - {command.Description}");
                    }
                }
                else
                {
                    // Otherwise execute the commands in the order specified.
                    foreach (string commandName in args)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"-- {commandName} --");
                        ICommand command = commands.FirstOrDefault(c => c.Name == commandName);
                        if (command == null)
                        {
                            Console.WriteLine("No such command is known.");
                            return;
                        }

                        command.Execute();
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private class CommandRecord
        {
            public string Name { get; set; }
            public string AssemblyName { get; set; }
        }

        static Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading commands from: {pluginLocation}");
            PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        static IEnumerable<ICommand> CreateCommands(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(ICommand).IsAssignableFrom(type))
                {
                    ICommand result = Activator.CreateInstance(type) as ICommand;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements ICommand in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
