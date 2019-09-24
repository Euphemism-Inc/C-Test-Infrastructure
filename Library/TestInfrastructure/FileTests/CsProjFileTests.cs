// (c) Euphemism Inc. All right reserved.

using Coconut.Library.TestInfrastructure.Attributes;
using Coconut.Library.TestInfrastructure.Helpers;
using Coconut.Library.TestInfrastructure.Names;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coconut.Library.TestInfrastructure.FileTests
{
    /// <summary>
    /// Test for all *.csproj files
    /// </summary>
    [TestClass]
    public sealed class CsProjFileTests : FileTestsBase
    {
        private IReadOnlyList<string> _csProjFiles = null;
        private IReadOnlyList<string> _frontendCsProj = new List<string>()
        {
            "Coconut.Presentation.SomeProject.Console"
        };

        /// <summary>
        /// Initializes the tests.
        /// </summary>
        [TestInitialize]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void Initialize()
        {
            _csProjFiles = GetFiles("*.csproj");
        }


        /// <summary>
        /// Tests if all csproj files are named correctly.
        /// </summary>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_NamedCorrectly()
        {
            // Arrange
            var baseNamespace = nameof(Coconut);
            var failedFiles = new List<string>();

            // Act
            foreach (string file in _csProjFiles)
            {
                var foundNamespace = Path.GetFileNameWithoutExtension(file);

                var startsWithBaseNamespace = foundNamespace.StartsWith(baseNamespace + ".");

                var expectedDiretory = (startsWithBaseNamespace ?
                    foundNamespace.Remove(0, baseNamespace.Length + 1) :
                    "").Split(".");

                var expectedNamespace = string.Join('.',
                    Path.GetDirectoryName(file)
                        .Split(Path.DirectorySeparatorChar)
                        .Reverse()
                        .Take(expectedDiretory.Length)
                        .Reverse()
                );

                // Assert
                if (!startsWithBaseNamespace && !string.Equals(expectedNamespace, string.Join('.', expectedDiretory)))
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0)
            {
                Assert.Fail("{0} csproj file(s) with wrong file name:\r\n{1}", failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }

        /// <summary>
        /// Tests if all csproj files (but test and frontend projects) are targetting the correct framework.
        /// </summary>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_HasCorrectTargetFramework()
        {
            // Arrange
            var expectedTargetFramework = "<TargetFramework>netstandard2.0</TargetFramework>";
            var failedFiles = new List<string>();
            var projectWithoutTestsNorFrontend = _csProjFiles
                .Where(x =>
                    !CaseContains(x, ".tests", StringComparison.InvariantCultureIgnoreCase)
                 && !CaseContains(x, nameof(TestInfrastructure), StringComparison.InvariantCultureIgnoreCase)
                 && !_frontendCsProj.Any(y => CaseContains(x, y, StringComparison.InvariantCultureIgnoreCase))
            );

            // Act
            foreach (string file in projectWithoutTestsNorFrontend)
            {
                var contents = File.ReadAllText(file);

                // Assert
                if (contents.IndexOf(expectedTargetFramework) == -1)
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0)
            {
                Assert.Fail("{0} csproj file(s) with wrong target framework:\r\n{1}", failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }

        // Frontend projects

        /// <summary>
        /// Tests if all frontend csproj files are targetting the correct framework.
        /// </summary>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_FrontendProj_HasCorrectTargetFramework()
        {
            // Arrange
            var expectedTargetFramework = "<TargetFramework>netcoreapp2.0</TargetFramework>";
            var failedFiles = new List<string>();
            var frontendProjects = _csProjFiles
                .Where(x =>
                    _frontendCsProj.Any(y => CaseContains(x, y, StringComparison.InvariantCultureIgnoreCase))
            );

            // Act
            foreach (string file in frontendProjects)
            {
                var contents = File.ReadAllText(file);

                // Assert
                if (contents.IndexOf(expectedTargetFramework) == -1)
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0)
            {
                Assert.Fail("{0} csproj file(s) with wrong target framework:\r\n{1}", failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }

        // Test projects

        /// <summary>
        /// Tests if all test csproj files are targetting the correct framework.
        /// </summary>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_TestProj_HasCorrectTargetFramework()
        {
            // Arrange
            var expectedTargetFramework = "<TargetFramework>netcoreapp2.0</TargetFramework>";
            var failedFiles = new List<string>();
            var testProjects = _csProjFiles
                .Where(x =>
                    CaseContains(x, ".tests", StringComparison.InvariantCultureIgnoreCase)
                 || CaseContains(x, nameof(TestInfrastructure), StringComparison.InvariantCultureIgnoreCase)
            );

            // Act
            foreach (string file in testProjects)
            {
                var contents = File.ReadAllText(file);

                // Assert
                if (contents.IndexOf(expectedTargetFramework) == -1)
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0)
            {
                Assert.Fail("{0} csproj file(s) with wrong target framework:\r\n{1}", failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }

        /// <summary>
        /// Tests if all test csproj files have one and the same version for 'MSTest.TestAdapter' and 'MSTest.TestFramework'.
        /// </summary>
        /// <remarks>
        /// If there are multiple MSTest versions, test might not run.
        /// </remarks>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_TestProj_HaveOneMSTestAdapterVersion()
        {
            // Arrange
            var testProjects = _csProjFiles
                .Where(x =>
                    CaseContains(x, ".tests", StringComparison.InvariantCultureIgnoreCase)
                 || CaseContains(x, nameof(TestInfrastructure), StringComparison.InvariantCultureIgnoreCase)
            );

            var regexPattern = new Regex("<PackageReference Include=\"MSTest\\.TestAdapter\" Version=\"\\d+\\.\\d+\\.\\d+\" />", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Act & Assert
            TestFilesContainOneInstanceOfPattern(testProjects, regexPattern, "{0} csproj file(s) with wrong target MS Test Adapter:\r\n{1}");
        }

        /// <summary>
        /// Tests if all test csproj files have one and the same version for 'MSTest.TestAdapter' and 'MSTest.TestFramework'.
        /// </summary>
        /// <remarks>
        /// If there are multiple MSTest versions, test might not run.
        /// </remarks>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_TestProj_HaveOneMSTestFrameworkVersion()
        {
            // Arrange
            var testProjects = _csProjFiles
                .Where(x =>
                    CaseContains(x, ".tests", StringComparison.InvariantCultureIgnoreCase)
                 || CaseContains(x, nameof(TestInfrastructure), StringComparison.InvariantCultureIgnoreCase)
            );

            var regexPattern = new Regex("<PackageReference Include=\"MSTest\\.TestFramework\" Version=\"\\d+\\.\\d+\\.\\d+\" />", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Act & Assert
            TestFilesContainOneInstanceOfPattern(testProjects, regexPattern, "{0} csproj file(s) with wrong target MS Test Framework:\r\n{1}");
        }

        /// <summary>
        /// Tests if all test csproj files have one and the same version for 'Microsoft.NET.Test.Sdk'.
        /// </summary>
        /// <remarks>
        /// If there are multiple MSTest versions, test might not run.
        /// </remarks>
        [TestMethod]
        [TestCategories(TestCategories.UnitTest, TestCategories.Library, TestCategories.Environment)]
        public void CsProjFile_TestProj_HaveOneMSdotNetTestSdkVersion()
        {
            // Arrange
            var testProjects = _csProjFiles
                .Where(x =>
                    CaseContains(x, ".tests", StringComparison.InvariantCultureIgnoreCase)
                 || CaseContains(x, nameof(TestInfrastructure), StringComparison.InvariantCultureIgnoreCase)
            );

            var regexPattern = new Regex("<PackageReference Include=\"Microsoft\\.NET\\.Test\\.Sdk\" Version=\"\\d+\\.\\d+\\.\\d+\" />", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            // Act & Assert
            TestFilesContainOneInstanceOfPattern(testProjects, regexPattern, "{0} csproj file(s) with wrong target MS Test Sdk:\r\n{1}");
        }

        // Helper methods

        /// <summary>
        /// Check if a regex pattern exists in a file and is the same. Does not care about more occurence than the pattern.
        /// </summary>
        /// <param name="testFileNames"></param>
        /// <param name="regexPattern"></param>
        /// <param name="failMessage">Message to be shown if the assert fails. Will be formatted with {0} = failed file count and (1) = failed files.</param>
        private void TestFilesContainOneInstanceOfPattern(IEnumerable<string> testFileNames, Regex regexPattern, string failMessage)
        {
            // Arrange
            var failedFiles = new List<string>();
            var pattern = FileHelper.GetFirstMatchingPattern(testFileNames, regexPattern);

            // Act
            foreach (string file in testFileNames)
            {
                var contents = File.ReadAllText(file);

                // Assert
                if (contents.IndexOf(pattern) == -1)
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0)
            {
                Assert.Fail(failMessage, failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }
    }
}
