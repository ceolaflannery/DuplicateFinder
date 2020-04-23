using System.IO;

namespace DuplicateFinder.FileProcessing
{
    public class IOHelper : IIOHelper
    {
        public string[] GetSubDirectories(string currentDir)
        {
            return Directory.GetDirectories(currentDir);
        }

        public string[] GetFilesInDirectory(string currentDir)
        {
            return Directory.GetFiles(currentDir);
        }

        public bool DirectoryExists(string directory)
        {
            return Directory.Exists(directory);
        }

        public byte[] ReadBytesForFile(string file)
        {
            return File.ReadAllBytes(file);
        }
    }
}
