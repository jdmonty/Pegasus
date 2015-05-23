﻿// -----------------------------------------------------------------------
// <copyright file="TutorialTests.cs" company="(none)">
//   Copyright © 2014 John Gietzen.  All Rights Reserved.
//   This source is subject to the MIT license.
//   Please see license.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Pegasus.Tests.Workbench
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Pegasus.Compiler;
    using Pegasus.Parser;
    using Pegasus.Workbench;

    [TestFixture]
    public class TutorialTests
    {
        private List<TestCaseData> Tutorials
        {
            get
            {
                return Tutorial.FindAll().ConvertAll(t => new TestCaseData(t));
            }
        }

        [TestCaseSource("Tutorials")]
        public void Compile_ForAllFoundTutorials_Succeeds(Tutorial tutorial)
        {
            var grammar = new PegParser().Parse(tutorial.GrammarText);

            var result = PegCompiler.Compile(grammar);

            Assert.That(result.Errors, Is.Empty);
        }

        [TestCaseSource("Tutorials")]
        public void Parse_ForAllFoundTutorials_Succeeds(Tutorial tutorial)
        {
            var grammar = new PegParser().Parse(tutorial.GrammarText);
            var compiled = PegCompiler.Compile(grammar);
            var parser = CodeCompiler.Compile<object>(compiled.Code);

            var result = parser.Parse(tutorial.TestText);

            Assert.That(result, Is.Not.Null);
        }
    }
}
