using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    /// <summary>
    /// The command table is used by the command line client to handle the parsing of
    /// commands from the user, and dispatching them to the appropriate handler.
    /// </summary>

    public interface ICommandTable
    {
        /// <summary>
        /// Register a command. Once registered, a command will then appear in the results of
        /// other methods such as GetCommands() etc.
        /// </summary>
        
        void AddCommand(string name, CommandHandler handler, bool readOnly);
        void AddCommand(string name, CommandHandler handler, bool test, bool readOnly);

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <returns>The output lines resulting from command execution.</returns>

        List<string> ExecuteCommand(IMarineLMSClient commandRecorder, string line);
    }

    /// <summary>
    /// This is basically a data bag holding information about a single command. The 
    /// command table maintains a set of these.
    /// </summary>

    public class Command
    {
        public string CasePreservedName;
        public CommandHandler Handler;
        public bool Test;
        public string UsageText;
        public string Description;
        public bool ReadOnly;
    }

    /// <summary>
    /// This struct holds the result of trying to look up the command that corresponds
    /// to a given string. If a unique match for the command cannot be found, then
    /// the potential matches by prefix and by camel case are returned so that they
    /// can be shown to the user.
    /// </summary>
    
    public class CommandLookupResult
    {
        public Command CommendToExecute { get; set; }
        public IEnumerable<Command> MatchesByPrefix { get; set; }
        public IEnumerable<Command> MatchesByCamelCase { get; set; }

        public bool MatchesExist() 
        { 
            return MatchesByPrefix.Count() > 0 || MatchesByCamelCase.Count() > 0;  
        }

        public CommandLookupResult()
        {
            MatchesByPrefix = new List<Command>();
            MatchesByCamelCase = new List<Command>();
        }

        public CommandLookupResult(Command commandToExecute)
            : this()
        {
            CommendToExecute = commandToExecute;
        }

        public CommandLookupResult(IEnumerable<Command> matchesByPrefix, IEnumerable<Command> matchesByCamelCase)
        {
            MatchesByPrefix = matchesByPrefix;
            MatchesByCamelCase = matchesByCamelCase;
        }
    }

    /// <summary>
    /// Type definition for a pointer to a command function
    /// </summary>

    public delegate List<string> CommandHandler(Command command, IEnumerable<string> tokens);
}
