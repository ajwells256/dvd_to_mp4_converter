using Fclp;
using MediaToolkit;
using MediaToolkit.Model;

namespace DvdConvert;

class Program
{
    static void Main(string[] args)
    {
        var settings = ParseArguments(args);
        ValidateArguments(settings);

        Console.WriteLine($"Starting Conversion from {settings.InputPath} to {settings.OutputPath}");

        Converter converter = new Converter(settings);
        converter.RunConversion();
    }

    static void ValidateArguments(Settings settings)
    {
        if (settings.SingleFileConversion)
        {
            if (!settings.OutputPath.ToLower().EndsWith(".mp4"))
            {
                Console.WriteLine($"Adding mp4 extension to output path {settings.OutputPath}");
                settings.OutputPath = $"{settings.OutputPath}.mp4"; 
            }

            if (!settings.InputPath.ToLower().EndsWith(".vob"))
            {
                throw new ArgumentException($"Expected DVD (.VOB) input path, got {settings.InputPath}");
            }
        }
        else
        {
            if (!Directory.Exists(settings.InputPath) || File.Exists(settings.InputPath))
            {
                throw new ArgumentException($"Expected {settings.InputPath} to be a directory");
            }
            Directory.CreateDirectory(settings.OutputPath);
        }
    }

    static Settings ParseArguments(string[] args)
    {
        var parser = new FluentCommandLineParser<Settings>();

        parser.Setup(arg => arg.SingleFileConversion)
            .As('f', "single_file")
            .SetDefault(false);

        parser.Setup(arg => arg.InputPath)
            .As('i', "input")
            .Required();
        
        parser.Setup(arg => arg.OutputPath)
            .As('o', "output")
            .SetDefault(Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString()));

        var parseResult = parser.Parse(args);
        if (parseResult.HasErrors)
        {
            throw new ArgumentException(parseResult.ErrorText);
        }

        return parser.Object;
    }
}
