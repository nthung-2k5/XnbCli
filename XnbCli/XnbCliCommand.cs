using System.Collections;
using System.Diagnostics.CodeAnalysis;
using DotMake.CommandLine;
using Serilog;
using XnbReader;
using XnbReader.StardewValley;

namespace XnbCli;

[CliCommand]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public class XnbCliCommand
{
    [CliCommand(Description = "Used to unpack XNB files")]
    public class UnpackCommand : ActionCommand
    {
        private HashSet<string> readers = [];
        protected override void ProcessFile(string input, string output)
        {
            try
            {
                // ensure that the input file has the right extension
                if (!Path.GetExtension(input).Equals(".xnb", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                using var fileStream = File.OpenRead(input);
                using var stream = new XnbStream(fileStream);
                //using var reader = new ContentReader(stream);
                //reader.LoadObject();
                // load the XNB and get the object from it
                var xnb = stream.File;

                //ExportAction.ExportFile(output, xnb, SourceGenerationContext.Default.XnbFile);
                readers.Add(xnb.Readers[0].Type);
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

        public override void Run()
        {
            base.Run();
            foreach (string reader in readers)
            {
                Console.WriteLine(TypeResolver.SimplifyType(reader));
            }
        }
    }

    [CliCommand(Description = "Used to pack XNB files")]
    public class PackCommand : ActionCommand
    {
        protected override void ProcessFile(string input, string output)
        {
            throw new NotSupportedException();
        }
    }
}
