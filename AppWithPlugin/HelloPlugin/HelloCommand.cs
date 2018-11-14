using PluginBase;
using System;

namespace HelloPlugin
{
    class HelloCommand : ICommand
    {
        public string Name { get => "Hello"; }
        public string Description { get => "Displays hello message."; }

        public int Execute()
        {
            Console.WriteLine("Hello !!!");
            return 0;
        }
    }
}
