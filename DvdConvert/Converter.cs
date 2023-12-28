using System.Security.Cryptography.X509Certificates;
using MediaToolkit;
using MediaToolkit.Model;

namespace DvdConvert
{
    public class Converter
    {
        private readonly ConsoleWriter _consoleWriter = new ConsoleWriter();
        private readonly Settings _settings;
        private const int _PROGRESS_BAR_WIDTH = 80;

        public Converter(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public void RunConversion()
        {
            if (_settings.SingleFileConversion)
            {
                RunSingleFileConversion();
            }
            else
            {
                RunDirectoryConversion();
            }
        }

        private void RunSingleFileConversion()
        {
            var inputFile = new MediaFile { Filename = _settings.InputPath };
            var outputFile = new MediaFile { Filename = _settings.OutputPath };

            var fileName = Path.GetFileName(_settings.OutputPath!);

            using (var engine = new Engine())
            {
                engine.ConvertProgressEvent += ConvertProgressEvent;
                engine.ConversionCompleteEvent += (object sender, ConversionCompleteEventArgs e) => ConversionCompleteEvent(sender, e, fileName!);
                engine.Convert(inputFile, outputFile);
            }
        }

        private void RunDirectoryConversion()
        {
            IEnumerable<string> videoFiles = Directory.EnumerateFiles(_settings.InputPath, "*.vob");

            string outputFilePrefix = Guid.NewGuid().ToString().Substring(0, 8);

            int totalFiles = videoFiles.Count();
            int i = 1;

            foreach (string videoFile in videoFiles)
            {
                string originalFileName = Path.GetFileNameWithoutExtension(videoFile);
                string outputFilePath = Path.Combine(_settings.OutputPath, $"{originalFileName}.mp4");

                if (File.Exists(outputFilePath))
                {
                    Console.WriteLine($"Adding a prefix ({outputFilePrefix}) because a file with the same name already existed");
                    outputFilePath = Path.Combine(_settings.OutputPath, $"{outputFilePrefix}_{originalFileName}.mp4");
                }

                var inputFile = new MediaFile { Filename = videoFile };
                var outputFile = new MediaFile { Filename = outputFilePath };

                var fileName = Path.GetFileName(outputFilePath);

                using (var engine = new Engine())
                {
                    engine.ConvertProgressEvent += ConvertProgressEvent;
                    engine.ConversionCompleteEvent += (object sender, ConversionCompleteEventArgs e) => ConversionCompleteEvent(sender, e, fileName!);
                    _consoleWriter.Write($"({i}/{totalFiles}) ", false);
                    engine.Convert(inputFile, outputFile);
                }

                i++;
            }
        }

        private void ConvertProgressEvent(object sender, ConvertProgressEventArgs e)
        {
            try
            {
                double percent = e.ProcessedDuration.TotalMilliseconds / e.TotalDuration.TotalMilliseconds;
                int barWidth = Math.Min(Console.WindowWidth, _PROGRESS_BAR_WIDTH);
                int dotsCount = (int)(percent*barWidth);
                string dotString = new string('.', dotsCount);
                string emptyString = new string(' ', barWidth-dotsCount);
                _consoleWriter.Write($"[{dotString}{emptyString}]");
            }
            catch {}
        }

        private void ConversionCompleteEvent(object sender, ConversionCompleteEventArgs e, string fileName)
        {
            string doneString = $" - done ({fileName})";
            _consoleWriter.WriteLine(doneString, false);
        }
    }
}