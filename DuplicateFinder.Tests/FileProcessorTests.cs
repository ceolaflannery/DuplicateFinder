using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DuplicateFinder.FileProcessing;
using Moq;
using Xunit;

namespace DuplicateFinder.Tests
{
    public class FileProcessorTests
    {
        private readonly string TestDirectory = "test/directory";
        private readonly Mock<IIOHelper> _iOHelper = new Mock<IIOHelper>();
        private StringBuilder ErrorsStub = new StringBuilder();
        private FileProcessor sut;

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldThrowArgumentExceptionAndReturnError_WhenDirectoryDoesNotExist()
        {
            SetupDirectoryDoesNotExist();

            var messageToPrint = Assert.Throws<ArgumentException>(() => sut.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out ErrorsStub));

            Assert.Equal($"The directory '{TestDirectory}' does not exist.", messageToPrint.Message);
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldGroup_WhenTwoFilesAreEquivilant()
        {
            string[] testFiles = Setup2EquivalentFilesInRootDirectory();

            var filesGroupedWithDuplicates = sut.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out ErrorsStub);

            AssertGrouped(2, testFiles, filesGroupedWithDuplicates);
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldGroup_WhenThreeFilesAreEquivilant()
        {
            string[] testFiles = Setup3EquivalentFilesInRootDirectory();

            var filesGroupedWithDuplicates = sut.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out ErrorsStub);

            AssertGrouped(3, testFiles, filesGroupedWithDuplicates);
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldNotGroup_WhenFilesAreNotTheSame()
        {
            string[] testFiles = SetupDifferentFiles();

            var filesGroupedWithDuplicates = sut.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out ErrorsStub);

            AssertNotGrouped(testFiles, filesGroupedWithDuplicates);
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldNotGroup_WhenTwoFilesAreNamedTheSameButContentsAreDifferent()
        {
            string[] testFiles = SetupSameFileNameDifferentContents();

            var filesGroupedWithDuplicates = sut.GroupAllFilesInDirectoryWithAnyDuplicates(TestDirectory, out ErrorsStub);

            AssertNotGrouped(testFiles, filesGroupedWithDuplicates);
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldGroup_WhenFileInSubDirectoryIsEqiuvalentToFileInRoot()
        {
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldGroup_WhenDirectoryHasEquivalentFilesWithSameName()
        {
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldReturnError_WhenFileBeingComparedHasBeenDeleted()
        {
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldReturnError_WhenUnauthorisedToAccessFile()
        {
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldReturnError_WhenIOExceptionOccursWhenAccessingFile()
        {
        }

        [Fact]
        public void GroupAllFilesInDirectoryWithAnyDuplicates_ShouldReturnEmpty_WhenDirectoryHasNoFiles()
        {
        }

        #region Verification Helpers
        private void AssertGrouped(int numberOfDuplicates, string[] testFiles, ConcurrentDictionary<string, List<string>> filesGroupedWithDuplicates)
        {
            _iOHelper.Verify(i => i.GetFilesInDirectory(TestDirectory), Times.Once);

            for (var i = 0; i < numberOfDuplicates - 1; i++)
            {
                _iOHelper.Verify(h => h.ReadBytesForFile(testFiles[i]), Times.Exactly(1));
            }

            Assert.Single(filesGroupedWithDuplicates);
            Assert.Equal(numberOfDuplicates, filesGroupedWithDuplicates.First().Value.Count);
        }

        private void AssertNotGrouped(string[] testFiles, ConcurrentDictionary<string, List<string>> filesGroupedWithDuplicates)
        {
            _iOHelper.Verify(i => i.GetFilesInDirectory(TestDirectory), Times.Once);
            _iOHelper.Verify(i => i.ReadBytesForFile(testFiles[0]), Times.Exactly(2));
            Assert.Equal(2, filesGroupedWithDuplicates.Count);
            Assert.Single(filesGroupedWithDuplicates.First().Value);
            Assert.Single(filesGroupedWithDuplicates.ElementAt(1).Value);
        }
        #endregion Verification Helpers

        #region Setup Helpers
        private void SetupDirectoryDoesNotExist()
        {
            sut = new FileProcessor(_iOHelper.Object);
            _iOHelper.Setup(i => i.DirectoryExists(TestDirectory)).Returns(false);
        }

        private string[] SetupSameFileNameDifferentContents()
        {
            SetupSubjectUnderTest();

            SetupForNoSubDirectories();
            var testFiles = SetupAndReturn2FileNamesThatAreTheSame();
            ReturnDifferentByteArrays(testFiles);

            return testFiles;
        }

        private string[] SetupDifferentFiles()
        {
            SetupSubjectUnderTest();

            SetupForNoSubDirectories();
            var testFiles = SetupAndReturn2FileNamesThatAreTheSame();
            ReturnDifferentByteArrays(testFiles);
            
            return testFiles;
        }

        private string[] SetupAndReturn2FileNamesThatAreTheSame()
        {
            var testFiles = FilesOfSameName();
            _iOHelper.Setup(i => i.GetFilesInDirectory(TestDirectory)).Returns(testFiles);
            return testFiles;
        }

        private void ReturnDifferentByteArrays(string[] testFiles)
        {
            var testByteArrays = Get2TestByteArrays();
            _iOHelper.SetupSequence(i => i.ReadBytesForFile(testFiles[0])).Returns(testByteArrays[0]).Returns(testByteArrays[1]);
        }

        private string[] Setup3EquivalentFilesInRootDirectory()
        {
            SetupSubjectUnderTest();

            SetupForNoSubDirectories();
            AlwaysReturnSameByteArray();

            return SetupAndReturnTestFileNames(3);
        }

        private void SetupSubjectUnderTest()
        {
            sut = new FileProcessor(_iOHelper.Object);
        }

        private string[] Setup2EquivalentFilesInRootDirectory()
        {
            SetupSubjectUnderTest();

            SetupForNoSubDirectories();
            AlwaysReturnSameByteArray();
            return SetupAndReturnTestFileNames(2);
        }

        private string[] SetupAndReturnTestFileNames(int numberOfTestFileNames)
        {
            var testFiles = FileNames(numberOfTestFileNames);
            _iOHelper.Setup(i => i.GetFilesInDirectory(TestDirectory)).Returns(testFiles);
            return testFiles;
        }

        private void AlwaysReturnSameByteArray()
        {
            var testByteArray = GetTestByteArray();
            _iOHelper.Setup(i => i.ReadBytesForFile(It.IsAny<string>())).Returns(testByteArray);
        }

        private void SetupForNoSubDirectories()
        {
            _iOHelper.Setup(i => i.DirectoryExists(TestDirectory)).Returns(true);
            _iOHelper.Setup(i => i.GetSubDirectories(TestDirectory)).Returns(new string[0]);
        }
        #endregion

        #region Setup Stubs
        private string[] FilesOfSameName()
        {
            var fileName = "TestFile";
            return new string[2] { fileName, fileName };
        }

        private string[] FileNames(int numberOfFileNames)
        {
            string[] fileNames = new string[numberOfFileNames];

            for (var i = 0; i < numberOfFileNames; i++)
            {
                fileNames[i] = $"TestFile{i}";
            }
            return fileNames;
        }

        private static byte[] GetTestByteArray()
        {
            return new byte[] { 0x00, 0x01, 0x02, 0x03 };
        }

        private static byte[][] Get2TestByteArrays()
        {
            return new byte[][]
            {
                new byte[] { 0x00, 0x01, 0x02, 0x03 },
                new byte[] { 0x00, 0x04, 0x02, 0x03 }
            };
        }
        #endregion
    }
}
