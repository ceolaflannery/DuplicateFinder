using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using DuplicateFinder.FileProcessing;
using Moq;
using Xunit;

namespace DuplicateFinder.Tests
{
    public class FileMultiplesServiceTests
    {
        private readonly string TestDirectory = "test/directory";
        private readonly string MultiplesFoundText = "Multiples found";
        private readonly string ErrorsFoundText = "The following errors were found when processing files:";

        private readonly Mock<IFileProcessor> _fileProcessor = new Mock<IFileProcessor>();
        private StringBuilder errorsProcessingFiles = new StringBuilder();

        private readonly FileMultiplesService sut;

        public FileMultiplesServiceTests()
        {
            sut = new FileMultiplesService(_fileProcessor.Object);
        }

        [Fact]
        public void GroupFilesByMultiples_ShouldReturnGrouping_WhenDuplicatesFoundAndNoErrors()
        {
            SetupFileProcessorToReturnFiles(GetFilesWithDuplicates(), withErrors: false);

            var messageToPrint = sut.GroupFilesByMultiples(TestDirectory);

            Assert.Contains(MultiplesFoundText, messageToPrint);
            Assert.DoesNotContain(ErrorsFoundText, messageToPrint);
        }

        [Fact]
        public void GroupFilesByMultiples_ShouldReturnUngrouped_WhenNoDuplicatesFoundAndNoErrors()
        {
            SetupFileProcessorToReturnFiles(GetFilesWithNoDuplicates(), withErrors: false);

            var messageToPrint = sut.GroupFilesByMultiples(TestDirectory);

            Assert.Empty(messageToPrint);
        }

        [Fact]
        public void GroupFilesByMultiples_ShouldReturnGroupingPlusErrors_WhenDuplicatesFoundAndErrorsFound()
        {
            SetupFileProcessorToReturnFiles(GetFilesWithDuplicates(), withErrors: true);

            var messageToPrint = sut.GroupFilesByMultiples(TestDirectory);

            Assert.Contains(MultiplesFoundText, messageToPrint);
            Assert.Contains(ErrorsFoundText, messageToPrint);
        }

        [Fact]
        public void GroupFilesByMultiples_ShouldReturnJustErrors_WhenErrorsOccurButNoDuplicatesFound()
        {
            SetupFileProcessorToReturnFiles(GetFilesWithNoDuplicates(), withErrors: true);

            var messageToPrint = sut.GroupFilesByMultiples(TestDirectory);

            Assert.DoesNotContain(MultiplesFoundText, messageToPrint);
            Assert.Contains(ErrorsFoundText, messageToPrint);
        }

        #region Setup Helplers/Stubs
        private void SetupFileProcessorToReturnFiles(ConcurrentDictionary<string, List<string>> files, bool withErrors)
        {
            if (withErrors) errorsProcessingFiles.Append("Error occured with one of the files");

            _fileProcessor.Setup(i => i.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out errorsProcessingFiles)).Returns(files);
        }

        private ConcurrentDictionary<string, List<string>> GetFilesWithDuplicates()
        {
            return new ConcurrentDictionary<string, List<string>>
            {
                ["aaaaaaa"] = new List<string>{ "file1", "file2" },
                ["xxxxxxx"] = new List<string>{ "file3" }
            };
        }
        private ConcurrentDictionary<string, List<string>> GetFilesWithNoDuplicates()
        {
            return new ConcurrentDictionary<string, List<string>>
            {
                ["aaaaaaa"] = new List<string> { "file1" },
                ["xxxxxxx"] = new List<string> { "file3" }
            };
        }
        #endregion
    }
}
