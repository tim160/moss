using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Interfaces
{
    public interface IMarineLMSClient
    {
        /// <summary>
        /// Will execute a command for one of the clients 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        List<string> ExecuteCommand(string line);

        /// <summary>
        /// In each application that uses the command table (e.g. MarineLMS.Client.CommandLine)
        /// an implementation of ICommandRecorder will need to be defined. 
        /// These implementations will be quite simple. 
        /// The implementation will have the admin service proxy injected as a dependency 
        /// and then will implement the RecordCommandExecution() method by calling into the admin service. 
        /// The 'source' parameter will be hard-coded into the ICommandRecorder implementation.
        /// </summary>
        /// <param name="command">Command that is executed</param>
        /// <param name="exceptionGenerated">Whether an exception was generated in executing the command</param>
        /// <param name="results">Result of the command</param>

        void RecordCommandExecution(string command, bool exceptionGenerated, List<string> results);
    }
}
