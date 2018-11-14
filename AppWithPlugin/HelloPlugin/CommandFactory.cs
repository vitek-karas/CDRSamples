using HelloPlugin;
using PluginBase;
using System.Collections.Generic;

public class CommandFactory : ICommandFactory
{
    public IEnumerable<ICommand> CreateCommands()
    {
        yield return new HelloCommand();
    }
}
