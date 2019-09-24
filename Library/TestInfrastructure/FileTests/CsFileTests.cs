// (c) Euphemism Inc. All right reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Coconut.Library.TestInfrastructure.FileTests
{
    /// <summary>
    /// Test for all *.cs files
    /// </summary>
    [TestClass]
    public sealed class CsFileTests : FileTestsBase
    {
        private IReadOnlyList<string> _csFiles = null;

        /// <summary>
        /// Initializes the tests.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            _csFiles = GetFiles("*.cs");
        }

        /// <summary>
        /// Tests if all cs files contain the copyright header.
        /// </summary>
        [TestMethod]
        public void CsFile_ContainsCopyrightHeader()
        {
            // Arrange
            const string headerText = "// (c) Euphemism Inc. All right reserved.";
            var failedFiles = new List<string>();

            // Act
            foreach (string file in _csFiles)
            {
                var firstLine = File.ReadLines(file).First();

                // Assert
                if (firstLine.IndexOf(headerText) == -1)
                {
                    failedFiles.Add(file);
                }
            }

            if (failedFiles.Count > 0) {
                Assert.Fail("Header text not found in {0} file(s):\r\n{1}", failedFiles.Count, string.Join(Environment.NewLine, failedFiles));
            }
        }
    }
}
