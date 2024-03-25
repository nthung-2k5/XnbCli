using DotMake.CommandLine;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace XnbCli;

public abstract class ActionCommand
{
    [CliArgument(Description = "Input file or folder", ValidationRules = CliValidationRules.ExistingFileOrDirectory)]
    public string Input { get; set; }

    [CliArgument(Description = "Output file or folder", Required = false, ValidationRules = CliValidationRules.LegalPath)]
    public string? Output { get; set; }

    [CliOption(Description = "Enables debug verbose printing")]
    public bool Debug { get; set; }

    [CliOption(Description = "Only prints error messages")]
    public bool Errors { get; set; }

    protected int Failed { get; set; }
    protected int Success { get; set; }

    public void Run()
    {
        var themes = new AnsiConsoleTheme(new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[38;5;0015m",
            [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0007m",
            [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0008m",
            [ConsoleThemeStyle.Invalid] = "\x1b[38;5;0011m",
            [ConsoleThemeStyle.Null] = "\x1b[38;5;0027m",
            [ConsoleThemeStyle.Name] = "\x1b[38;5;0007m",
            [ConsoleThemeStyle.String] = "\x1b[38;5;0045m",
            [ConsoleThemeStyle.Number] = "\x1b[38;5;0200m",
            [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0027m",
            [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0085m",
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;0007m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[35;1m",
            [ConsoleThemeStyle.LevelInformation] = "\x1b[34;1m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;0011m",
            [ConsoleThemeStyle.LevelError] = "\x1b[31;1m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;1m"
        });

        var logConf = new LoggerConfiguration().WriteTo.Console(LogEventLevel.Information, "[{Level}] {Message}{NewLine}{Exception}", theme: themes);

        if (Debug)
        {
            logConf = logConf.MinimumLevel.Debug();
        }

        if (Errors)
        {
            logConf = logConf.MinimumLevel.Error();
        }

        Log.Logger = logConf.CreateLogger();
        ProcessFiles(Input, Output);
        Console.WriteLine("\x1b[32;1mSuccess\x1b[0m {0}", Success);
        Console.WriteLine("\x1b[31;1mFailed\x1b[0m {0}", Failed);
    }

    protected virtual void ProcessFiles(string input, string? output)
    {
        if (File.Exists(input))
        {
            // get the new extension
            string newExt = Path.GetExtension(input) == ".xnb" ? ".json" : ".xnb";

            // output is undefined or is a directory
            if (string.IsNullOrEmpty(output))
            {
                output = Path.ChangeExtension(input, newExt);
            }
            // output is a directory
            else if (Directory.Exists(output))
            {
                output = Path.Combine(output, Path.ChangeExtension(Path.GetFileName(input), newExt));
            }

            // call the function
            ProcessFile(input, output);
        }
        else
        {
            foreach (string file in Directory.EnumerateFiles(input, "*.*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith("json") || file.ToLower().EndsWith("xnb")))
            {
                // swap the input base directory with the base output directory for our target directory
                string newFolder = Path.GetDirectoryName(file)!.Replace(input, output);
                Directory.CreateDirectory(newFolder);
                ProcessFiles(file, newFolder);
            }
        }
    }

    protected abstract void ProcessFile(string input, string output);
}
