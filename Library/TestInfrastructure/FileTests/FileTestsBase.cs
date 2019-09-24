// (c) Euphemism Inc. All right reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coconut.Library.TestInfrastructure.Helpers;

namespace Coconut.Library.TestInfrastructure.FileTests
{
    /// <summary>
    /// Base class for file tests.
    /// </summary>
    public abstract class FileTestsBase
    {
        /// <summary>
        /// Gets files relative from the root (<see cref="ConfigHelper.GetRoot"/>), found by the <paramref name="searchPattern"/>.
        /// </summary>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">files</exception>
        protected IReadOnlyList<string> GetFiles(string searchPattern)
        {
            var relativePath = ConfigHelper.GetRoot();
            var basePath = Path.GetFullPath(relativePath);
            var files = Directory.GetFiles(basePath, searchPattern, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
            {
                var message = $"Something went wrong, no files were found.\r\nSearch Pattern: {searchPattern}\r\nBasePath: {relativePath}\r\nPath: {basePath}";
                throw new ArgumentNullException(nameof(files), message);
            }
            return files.Where(IsInSolutionDirectory).ToList();
        }

        /// <summary>
        /// Checks if the <paramref name="fullPath"/> is in a solution directory.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        private static bool IsInSolutionDirectory(string fullPath)
        {
            var directoriesToExclude = new List<string>()
            {
                @"\.git\",
                @"\.vs\",
                @"\debug\",
                @"\obj\",
                @".designer.cs"
            };

            return !directoriesToExclude.Any(x => CaseContains(fullPath, x, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// <see cref="string.Contains(string)"/>, but with <paramref name="comparisonMode"/>.
        /// </summary>
        /// <param name="baseString">The base string.</param>
        /// <param name="textToSearch">The text to search.</param>
        /// <param name="comparisonMode">The comparison mode.</param>
        /// <returns></returns>
        protected static bool CaseContains(string baseString, string textToSearch, StringComparison comparisonMode)
        {
            return baseString.IndexOf(textToSearch, comparisonMode) != -1;
        }
    }
}
