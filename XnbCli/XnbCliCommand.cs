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
        protected override void ProcessFile(string input, string output)
        {
            try
            {
                // ensure that the input file has the right extension
                if (!Path.GetExtension(input).Equals(".xnb", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                using var stream = new XnbStream(File.OpenRead(input));
                using var reader = new ContentReader(stream, new StardewValleyTypeResolver());
                _ = reader.LoadObject();
                // load the XNB and get the object from it
                var xnb = stream.File;

                ExportAction.ExportFile(output, xnb);
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

    [CliCommand(Description = "Used to pack XNB files (not supported)")]
    public class PackCommand : ActionCommand
    {
        protected override void ProcessFile(string input, string output)
        {
            throw new NotSupportedException();
        }
    }
    
    [CliCommand(Description = "Unpack XNB file and read the first reader only")]
    public class ListCommand: ActionCommand
    {
        protected override void ProcessFiles(string input, string? output)
        {
            if (File.Exists(input))
            {
                // call the function
                ProcessFile(input, string.Empty);
            }
            else
            {
                foreach (string file in Directory.EnumerateFiles(input, "*.xnb", SearchOption.AllDirectories))
                {
                    ProcessFile(file, string.Empty);
                }
            }

            var dir = Directory.CreateDirectory(output ?? ".");
            File.WriteAllLines(Path.Combine(dir.FullName, "readers.txt"), readers);
            Console.WriteLine("Readers written to {0}", Path.Combine(dir.FullName, "readers.txt"));
        }

        protected override void ProcessFile(string input, string output)
        {
            using var fileStream = File.OpenRead(input);
            using var stream = new XnbStream(fileStream);
            // load the XNB and get the reader from it
            var xnb = stream.File;
            readers.Add(xnb.Readers[0].Type);
            
            // log that the file was saved
            Log.Information("Reader for file {filename}: {output:l}", Path.GetFileName(input), output);
            Success++;
        }
        
        private readonly HashSet<string> readers = [];
    }
}
