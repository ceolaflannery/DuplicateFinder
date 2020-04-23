namespace DuplicateFinder.FileProcessing
{
    public interface IIOHelper
    {
        string[] GetSubDirectories(string currentDir);

        string[] GetFilesInDirectory(string currentDir);

        bool DirectoryExists(string directory);

        byte[] ReadBytesForFile(string file);
    }
}