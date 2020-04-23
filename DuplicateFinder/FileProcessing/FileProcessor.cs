using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DuplicateFinder.FileProcessing
{
    public class FileProcessor : IFileProcessor
    {
        private readonly IIOHelper _iOHelper;
        private readonly StringBuilder ErrorsProcessingFiles = new StringBuilder();
        private readonly ConcurrentDictionary<string, List<string>> filesGroupedWithDuplicates = new ConcurrentDictionary<string, List<string>>();


        public FileProcessor(IIOHelper iOHelper)
        {
            _iOHelper = iOHelper;
        }

        public ConcurrentDictionary<string, List<string>> GroupAllFilesInDirectoryWithAnyDuplicates(string rootDirectory, out StringBuilder errorsToReturn)
        {
            Stack<string> directoriesToBeProcessed = InitialiseCollectionOfDirectoriesToBeProcessed(rootDirectory);

            while (directoriesToBeProcessed.Count > 0)
            {
                string currentDir = directoriesToBeProcessed.Pop();

                AddSubDirectoriesToListOfDirectoriesToBeProcessed(currentDir, directoriesToBeProcessed);

                string[] files = _iOHelper.GetFilesInDirectory(currentDir);

                foreach(var filePath in files)
                {
                    try
                    {
                        AddFileToGroupedCollection(filePath);
                    }
                    catch (Exception ex)
                    {
                        ErrorsProcessingFiles.AppendLine($"Error occured processing file {filePath}: {ex.Message}");
                    }
                }
            }

            errorsToReturn = ErrorsProcessingFiles ?? null;

            return filesGroupedWithDuplicates;
        }

        private Stack<string> InitialiseCollectionOfDirectoriesToBeProcessed(string rootDirectory)
        {
            Stack<string> dirs = new Stack<string>();

            if (!_iOHelper.DirectoryExists(rootDirectory))
            {
                throw new ArgumentException($"The directory '{rootDirectory}' does not exist.");
            }
            dirs.Push(rootDirectory);
            return dirs;
        }

        private void AddSubDirectoriesToListOfDirectoriesToBeProcessed(string currentDir, Stack<string> directoriesToBeProcessed)
        {
            string[] subDirs = _iOHelper.GetSubDirectories(currentDir);

            foreach (string dir in subDirs)
                directoriesToBeProcessed.Push(dir);
        }

        private void AddFileToGroupedCollection(string filePath)
        {
            var fileHash = GetHashForFile(filePath);

            if (filesGroupedWithDuplicates.ContainsKey(fileHash))
                filesGroupedWithDuplicates[fileHash].Add(filePath);
            else
                filesGroupedWithDuplicates[fileHash] = new List<string> { filePath };
        }

        private string GetHashForFile(string file)
        {
            var fileAsBytes = _iOHelper.ReadBytesForFile(file);
            string hash;

            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                hash = Convert.ToBase64String(sha1.ComputeHash(fileAsBytes));
            }
            return hash;
        }
    }
}
