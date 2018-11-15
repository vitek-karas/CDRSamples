using Newtonsoft.Json;
using PluginBase;
using System;

namespace JsonPlugin
{
    public class JsonPlugin : ICommand
    {
        public string Name => "json";

        public string Description => "Outputs JSON value.";

        private struct Info
        {
            public string Machine;
            public string User;
            public DateTime Date;
        }

        public int Execute()
        {
            Info info = new Info()
            {
                Machine = Environment.MachineName,
                User = Environment.UserName,
                Date = DateTime.Now
            };

            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(Console.Out, info);
            Console.WriteLine();

            return 0;
        }
    }
}
