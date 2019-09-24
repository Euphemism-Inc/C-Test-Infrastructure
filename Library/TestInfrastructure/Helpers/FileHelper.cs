// (c) Euphemism Inc. All right reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coconut.Library.TestInfrastructure.Helpers
{
    internal static class FileHelper
    {
        /// <summary>
        /// Get the first matching pattern in a set of files.
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetFirstMatchingPattern(IEnumerable<string> fileNames, Regex pattern)
        {
            string matchedPattern = null;
            foreach (string fileName in fileNames)
            {
                matchedPattern = GetMatchingPattern(fileName, pattern);
                if (!String.IsNullOrEmpty(matchedPattern))
                {
                    break;
                }
            }
            return matchedPattern;
        }

        /// <summary>
        /// Get the first matching pattern from a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string GetMatchingPattern(string fileName, Regex pattern)
        {
            var contents = File.ReadAllText(fileName);

            Match match = pattern.Match(contents);
            if (match.Success)
            {
                return new String(contents.Skip(match.Index).Take(match.Length).ToArray());
            }
            return null;
        }

    }
}
