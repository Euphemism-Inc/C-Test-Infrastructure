// (c) Euphemism Inc. All right reserved.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Coconut.Library.TestInfrastructure.Attributes
{
    /// <summary>
    /// Attribute to add multiple test categories at once.
    /// </summary>
    /// <seealso cref="Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryBaseAttribute" />
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestCategoriesAttribute : TestCategoryBaseAttribute
    {
        /// <summary>
        /// Gets the test categories that has been applied to the test.
        /// </summary>
        public override IList<string> TestCategories { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCategoriesAttribute"/> class.
        /// </summary>
        /// <param name="testCategories">The test categories.</param>
        public TestCategoriesAttribute(params string[] testCategories)
        {
            TestCategories = testCategories;
        }
    }
}
