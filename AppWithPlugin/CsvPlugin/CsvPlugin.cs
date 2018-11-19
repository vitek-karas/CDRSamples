using CsvHelper;
using PluginBase;
using System;

namespace CsvPlugin
{
    public class CsvPlugin : ICommand
    {
        public string Name => "csv";

        public string Description => "Writes out CSV using CSVHelper.";

        private class PluginRecord
        {
            public string PluginAssemblyName { get; set; }
            public string PluginAssemblyLocation { get; set; }
        }

        public int Execute()
        {
            using (CsvWriter writer = new CsvWriter(Console.Out))
            {
                writer.WriteHeader<PluginRecord>();
                writer.WriteRecord(new PluginRecord()
                {
                    PluginAssemblyName = this.GetType().Assembly.FullName,
                    PluginAssemblyLocation = this.GetType().Assembly.Location
                });
                writer.WriteRecord(new PluginRecord()
                {
                    PluginAssemblyName = writer.GetType().Assembly.FullName,
                    PluginAssemblyLocation = writer.GetType().Assembly.Location
                });
            }

            return 0;
        }
    }
}
