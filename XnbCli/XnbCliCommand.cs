using System.Diagnostics.CodeAnalysis;
using DotMake.CommandLine;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Xnb;

namespace XnbCli;

[CliCommand]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class XnbCliCommand
{
    [CliCommand(Description = "Used to unpack XNB files")]
    public class UnpackCommand: ActionCommand
    {
        public override void ProcessFile(string input, string output)
        {
            try
            {
                // ensure that the input file has the right extension
                if (!Path.GetExtension(input).Equals(".xnb", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                // load the XNB and get the object from it
                var xnb = XnbProcessor.Load(input);

                FileAction.ExportFile(output, xnb);

                // log that the file was saved
                Log.Information("Output file saved: {output:l}", output);

                // increase success count
                Success++;
            }
            catch (Exception e)
            {
                // log out the error
                Log.Error("Filename: {input}\n{ex}\n", input, e.Message);
                // increase fail count
                Failed++;
            }
        }
    }
    
    [CliCommand(Description = "Used to pack XNB files")]
    public class PackCommand: ActionCommand
    {
        public override void ProcessFile(string input, string output)
        {
            throw new NotImplementedException();
        }
    }
}
