using System;
using System.Text;
using DuplicateFinder.FileProcessing;

namespace DuplicateFinder
{
    public class FileMultiplesService : IFileMultiplesService
    {
        private readonly IFileProcessor _fileProcessor;
        private StringBuilder errorsProcessingFiles = new StringBuilder();

        public FileMultiplesService(IFileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        public string GroupFilesByMultiples(string rootDirectory)
        {
            try
            {
                var filesGroupedWithDuplicates = _fileProcessor.GroupAllFilesInDirectoryWithAnyDuplicates(rootDirectory, out errorsProcessingFiles);

                return RenderHelper.RenderData(filesGroupedWithDuplicates, errorsProcessingFiles);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
