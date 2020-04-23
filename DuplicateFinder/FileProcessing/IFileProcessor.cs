using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DuplicateFinder.FileProcessing
{
    public interface IFileProcessor
    {
        ConcurrentDictionary<string, List<string>> GroupAllFilesInDirectoryWithAnyDuplicates(string rootDirectory, out StringBuilder errors);
    }
}