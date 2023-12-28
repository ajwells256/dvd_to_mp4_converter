namespace DvdConvert
{
    public class Settings
    {
        public bool SingleFileConversion { get; set; }

        public string InputPath { get; set; } = null!;

        public string OutputPath { get; set; } = null!;
    }
}