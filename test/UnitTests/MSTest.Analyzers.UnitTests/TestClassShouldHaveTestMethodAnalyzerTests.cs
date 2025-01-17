﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Testing.Framework;
using Microsoft.Testing.TestInfrastructure;

using VerifyCS = MSTest.Analyzers.Test.CSharpCodeFixVerifier<
    MSTest.Analyzers.TestClassShouldHaveTestMethodAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace MSTest.Analyzers.Test;

[TestGroup]
public sealed class TestClassShouldHaveTestMethodAnalyzerTests(ITestExecutionContext testExecutionContext) : TestBase(testExecutionContext)
{
    public async Task WhenTestClassHasTestMethod_NoDiagnostic()
    {
        var code = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public class MyTestClass
            {
                [TestMethod]
                public void MyTestMethod()
                {
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    public async Task WhenStaticTestClassWithAssemblyCleanup_DoesNotHaveTestMethod_NoDiagnostic()
    {
        var code = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public static class MyTestClass
            {
                [AssemblyCleanup]
                public static void AssemblyCleanup()
                {
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    public async Task WhenStaticTestClassWithAssemblyInitialization_DoesNotHaveTestMethod_NoDiagnostic()
    {
        var code = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public static class MyTestClass
            {
                [AssemblyInitialize]
                public static void AssemblyInitialize(TestContext context)
                {
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(code);
    }

    public async Task WhenTestClassDoesNotHaveTestMethod_Diagnostic()
    {
        var code = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public class {|#0:MyTestClass|}
            {
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            code,
            VerifyCS.Diagnostic(TestClassShouldHaveTestMethodAnalyzer.TestClassShouldHaveTestMethodRule)
                .WithLocation(0)
                .WithArguments("MyTestClass"));
    }

    public async Task WhenStaticTestClassWithoutAssemblyAttributes_DoesNotHaveTestMethod_Diagnostic()
    {
        var code = """
            using Microsoft.VisualStudio.TestTools.UnitTesting;

            [TestClass]
            public static class {|#0:MyTestClass|}
            {
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(
            code,
            VerifyCS.Diagnostic(TestClassShouldHaveTestMethodAnalyzer.TestClassShouldHaveTestMethodRule)
                .WithLocation(0)
                .WithArguments("MyTestClass"));
    }
}
