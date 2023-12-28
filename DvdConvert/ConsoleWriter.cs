namespace DvdConvert
{
    public class ConsoleWriter
    {
        private string _lastPartialWrite = "";

        public ConsoleWriter() { }

        public void WriteLine(string line, bool clearConsole = true)
        {
            string fullLine = clearConsole
                ? $"{new string('\b', _lastPartialWrite.Length)}{line}"
                : line;
            _lastPartialWrite = "";
            Console.WriteLine(fullLine);
        }

        public void Write(string line, bool overwritable = true)
        {
            string fullLine = $"{new string('\b', _lastPartialWrite.Length)}{line}";
            
            _lastPartialWrite = overwritable
                ? line
                : "";

            Console.Write(fullLine);
        }
    }
}