using EC.Errors;
using EC.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Core.Common
{
    [TransientType]
    [RegisterAsType(typeof(ICommandTable))]

    public class CommandTable : ICommandTable
    {
        /// <summary>
        /// Register a command. Once registered, a command will then appear in the results of
        /// other methods such as GetCommands() etc. <paramref name="test"/> allows certain
        /// commands to be marked as 'for testing only' and they are typically suppressed in
        /// command listings.
        /// </summary>

        public void AddCommand(string name, CommandHandler handler, bool test, string usage, string description, bool readOnly)
        {
            Command c = new Command();
            c.Test = test;
            c.UsageText = usage;
            c.Description = description;
            c.CasePreservedName = name;
            c.Handler = handler;
            c.ReadOnly = readOnly;
            CommandMap.Add(name.ToLower(), c);
        }

        /// <summary>
        /// Register a command. Once registered, a command will then appear in the results of
        /// other methods such as GetCommands() etc. <paramref name="test"/> allows certain
        /// commands to be marked as 'for testing only' and they are typically suppressed in
        /// command listings.
        /// </summary>

        public void AddCommand(string name, CommandHandler handler, bool test, bool readOnly)
        {
            string usageText = GetUsageAttribute(handler);
            string description = GetCommandDescription(handler);
            AddCommand(name, handler, test, usageText, description, readOnly);
        }

        /// <summary>
        /// Register a command. Once registered, a command will then appear in the results of
        /// other methods such as GetCommands() etc. 
        /// </summary>

        public void AddCommand(string name, CommandHandler handler, bool readOnly)
        {
            string usageText = this.GetUsageAttribute(handler);
            string description = GetCommandDescription(handler);
            AddCommand(name, handler, false, usageText, description, readOnly);
        }

        /// <summary>
        /// Execute a command, given a complete command line as a string. 
        /// Processor tries to match commands in following order: 
        ///     (1) Full Name Match
        ///     (2) Camel Case 
        ///     (3) Initial Prefix
        /// If unique match not found, help displayed with info on potentially matching commands.
        /// </summary>

        public List<string> ExecuteCommand(IMarineLMSClient commandRecorder, string line)
        {
            List<string> result = new List<string>();
            var tokens = ParseArguments(line);

            if (tokens.Count() == 0)
            {
                result.Add("Error: no command given");
                return result;
            }

            var cmd = tokens.First();
            tokens = tokens.Skip(1);

            try
            {
                var lookupResult = LookupCommand(cmd);
                if (lookupResult.CommendToExecute != null) { return ExecuteCommand(lookupResult.CommendToExecute, tokens, commandRecorder, line); }
                if (lookupResult.MatchesExist()) { return DisplayMatches(cmd, lookupResult); }
                if (cmd.ToLower() == "help") { return DisplayHelp(); }
                if (cmd.ToLower() == "testhelp") { return DisplayHelpWithTestCommands(); }
                if (cmd.ToLower().StartsWith("?")) { return DisplayMatchingCommands(cmd.Substring(1)); }
                if (true) { return DisplayAllCommands(cmd); }
            }
            catch (Exception e)
            {
                result.Add("EXCEPTION");
                result.Add(string.Format("   -> {0}", e.Message));
                result.Add(string.Format("   Stack Trace:\n{0}", e.StackTrace));

                commandRecorder.RecordCommandExecution(line, true, result);

                return result;
            }
        }

        /// <summary>
        /// Lookup command in command table using following matching algorithm in order: 
        ///     (1) Matche complete command name
        ///     (2) Matche camel case representation of command (IE getCertificateAudit = gca)
        ///     (3) Matche initial prefix of command name
        /// Returns struct with command to execute (if unique command found) or potentially matching commands.
        /// </summary>

        private CommandLookupResult LookupCommand(string cmdText)
        {
            cmdText = cmdText.ToLower();

            // Exact
            if (CommandMap.Keys.Contains(cmdText))
            {
                return new CommandLookupResult(CommandMap[cmdText]);
            }

            // Camel Case
            var matchesbyCamelCase = GetCommandsByCamelCase(cmdText, true);

            if (matchesbyCamelCase.Count() == 1)
            {
                return new CommandLookupResult(matchesbyCamelCase.First());
            }

            // Prefix
            var matchesByPrefix = GetCommandsByPrefix(cmdText, true);

            if (matchesByPrefix.Count() == 1)
            {
                return new CommandLookupResult(matchesByPrefix.First());
            }

            return new CommandLookupResult(matchesByPrefix: matchesByPrefix, matchesByCamelCase: matchesbyCamelCase);
        }

        /// <summary>
        /// Handles argument parsing. This handles treating quoted strings as single
        /// arguments, and also removing extraneous whitespace.
        /// </summary>

        private static IEnumerable<string> ParseArguments(string commandLine)
        {
            char[] parmChars = commandLine.ToCharArray();
            bool inQuote = false;

            for (int index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"') { inQuote = !inQuote; }
                if (!inQuote && parmChars[index] == ' ') { parmChars[index] = '\n'; }
            }

            var result = (new string(parmChars)).Split('\n');
            return result.Where(c => !string.IsNullOrEmpty(c)).Select(c => c.Trim(new char[] { '\"' }));
        }

        /// <summary>
        /// Return all the registered commands (including test commands if the
        /// parameter is true).
        /// </summary>

        public List<Command> GetCommands(bool includeTest)
        {
            IEnumerable<Command> commands = CommandMap.Values;
            if (!includeTest) { commands = commands.Where(c => !c.Test); }
            List<Command> result = new List<Command>();
            result.AddRange(commands);
            result.Sort((c1, c2) => c1.CasePreservedName.CompareTo(c2.CasePreservedName));
            return result;
        }

        /// <summary>
        /// Return the set of commands that match the given prefix 
        /// </summary>

        public List<Command> GetCommandsByPrefix(string prefix, bool includeTest)
        {
            prefix = prefix.ToLower();
            var matchingCommands = CommandMap.Where(p => p.Key.StartsWith(prefix)).Select(p => p.Value);
            if (!includeTest) { matchingCommands = matchingCommands.Where(c => !c.Test); }
            var commandList = matchingCommands.ToList();
            commandList.Sort((c1, c2) => c1.CasePreservedName.CompareTo(c2.CasePreservedName));
            return commandList;
        }

        /// <summary>
        /// Return the set of commands that match the given camel-case shorthand.
        /// </summary>

        private List<Command> GetCommandsByCamelCase(string cmdText, bool includeTest)
        {
            cmdText = cmdText.ToLower();
            var commands = CommandMap.Select(p => p);
            if (!includeTest) { commands = commands.Where(p => !p.Value.Test); }
            var camelMap = commands.Select(p => { return new KeyValuePair<string, Command>(BuildCamelCaseFor(p.Value.CasePreservedName), p.Value); });
            var commandList = camelMap.Where(p => p.Key == cmdText).Select(p => p.Value).ToList();
            commandList.Sort((c1, c2) => c1.CasePreservedName.CompareTo(c2.CasePreservedName));
            return commandList;
        }

        /// <summary>
        /// Return a list of all commands whose name contains the given string.
        /// </summary>

        private IEnumerable<Command> GetCommandsByStringMatch(string matchStr)
        {
            var matchingCommands = CommandMap.Where(p => p.Key.Contains(matchStr)).Select(p => p.Value);
            var commandList = matchingCommands.ToList();
            commandList.Sort((c1, c2) => c1.CasePreservedName.CompareTo(c2.CasePreservedName));
            return commandList;
        }

        /// <summary>
        /// This returns the camelcase string for a given command. This string is
        /// composed of the first character of the command name, followed by any
        /// upper-case characters from the command name. E.g. GetCertificateAudits becomes
        /// 'gca'
        /// </summary>

        private string BuildCamelCaseFor(string fullCommandName)
        {
            if (String.IsNullOrWhiteSpace(fullCommandName)) { throw new Exception("fullCommandName cannot be empty"); }
            var result = fullCommandName[0].ToString();
            var allButFirst = fullCommandName.Substring(1);
            if (String.IsNullOrWhiteSpace(allButFirst)) { return result; }
            result += String.Concat(allButFirst.Cast<Char>().Where(c => Char.IsUpper(c)));
            var final = result.ToLower().ToString();
            return final;
        }

        /// <summary>
        /// Exceutes the command associated with the given command struct.
        /// </summary>

        private List<string> ExecuteCommand(Command cmd, IEnumerable<string> tokens, IMarineLMSClient commandRecorder, string line)
        {
            List<string> results = cmd.Handler(cmd, tokens);
            if (!cmd.ReadOnly) { commandRecorder.RecordCommandExecution(line, false, results); }
            return results;
        }

        /// <summary>
        /// Get usage text from optional attribute of the handler. Returns null if the
        /// attribute does not exist for the handler.
        /// </summary>

        private string GetUsageAttribute(CommandHandler handler)
        {
            string usageText = null;
            var attrs = handler.Method.GetCustomAttributes(typeof(CommandUsage), false);
            if (attrs.Length == 1) { usageText = ((CommandUsage)attrs[0]).Text; }
            return usageText;
        }

        /// <summary>
        /// Get description text from optional attribute of the handler. Returns null if the
        /// attribute does not exist for the handler.
        /// </summary>

        private string GetCommandDescription(CommandHandler handler)
        {
            string usageText = null;
            var attrs = handler.Method.GetCustomAttributes(typeof(CommandDescription), false);
            if (attrs.Length == 1) { usageText = ((CommandDescription)attrs[0]).Description; }
            return usageText;
        }

        /// <summary>
        /// Displays information about potentially matching commands.
        /// </summary>

        private List<string> DisplayMatches(string cmd, CommandLookupResult lookupResult)
        {
            var result = new List<string>();

            if (lookupResult.MatchesByPrefix.Count() > 0)
            {
                result.Add(String.Format("Commands beginning with '{0}':", cmd));
                result.AddRange(lookupResult.MatchesByPrefix.Select(c => FormatCommandForDisplay(c)));
            }

            if (lookupResult.MatchesByCamelCase.Count() > 0)
            {
                result.Add(String.Format("Commands matching {0}' (by camel case):", cmd));
                result.AddRange(lookupResult.MatchesByCamelCase.Select(c => FormatCommandForDisplay(c)));
            }

            return result;
        }

        /// <summary>
        /// Display help for all commands that contain the given string.
        /// </summary>

        private List<string> DisplayMatchingCommands(string matchStr)
        {
            var result = new List<string>();
            matchStr = matchStr.ToLower();
            var commands = GetCommandsByStringMatch(matchStr);
            result.Add(String.Format("Commands containing '{0}':", matchStr));
            result.AddRange(commands.Select(c => FormatCommandForDisplay(c)));
            return result;
        }

        /// <summary>
        /// Help text to display when the user's command does not match any registered
        /// command.
        /// </summary>

        private List<string> DisplayAllCommands(string cmd)
        {
            var result = new List<string>();
            result.Add(String.Format("{0} does not match any commands.", cmd));
            result.AddRange(DisplayHelp());
            return result;
        }

        /// <summary>
        /// Displays a list of all commands, including the test comamnds.
        /// </summary>

        private List<string> DisplayHelpWithTestCommands()
        {
            var result = new List<string>();
            result.Add("Available commands (including test commands):");
            result.AddRange(GetCommands(true).Select(c => FormatCommandForDisplay(c)));
            return result;
        }

        /// <summary>
        /// Displays a lust of all commands.
        /// </summary>

        private List<string> DisplayHelp()
        {
            var result = new List<string>();
            result.Add("Available commands:");
            result.AddRange(GetCommands(false).Select(c => FormatCommandForDisplay(c)));
            return result;
        }

        /// <summary>
        /// Formats a command for display.
        /// </summary>

        private string FormatCommandForDisplay(Command c)
        {
            return String.Format("    {0}{1} -- {2}", c.CasePreservedName, c.Test ? " (TEST)" : "", c.UsageText);
        }

        /// <summary>
        /// Lookup is keyed by command name in lowercase (for case insensitivity). Locale issues
        /// associated with .ToLower() are ignored since this is for internal usage.
        /// </summary>

        public Dictionary<string, Command> CommandMap = new Dictionary<string, Command>();
    }

    /// <summary>
    /// Attribute for supplying command usage message.
    /// </summary>

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandUsage : Attribute
    {
        public string Text { get; set; }
        public CommandUsage(string text)
        {
            Text = text;
        }
    }

    /// <summary>
    /// Attribute for supplying command description
    /// </summary>

    [AttributeUsage(AttributeTargets.Method)]
    public class CommandDescription : Attribute
    {
        public string Description { get; set; }
        public CommandDescription(string description)
        {
            Description = description;
        }
    }

}
