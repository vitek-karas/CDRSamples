using System.Collections.Generic;

namespace PluginBase
{
    public interface ICommandFactory
    {
        IEnumerable<ICommand> CreateCommands();
    }
}
