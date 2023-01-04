/*
This file is part of the iText (R) project.
Copyright (c) 1998-2023 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using iText.Commons.Utils;
using iText.IO.Util;
using iText.Test.Attributes;
using NUnit.Framework;

namespace iText.Test {
    public abstract class WrappedSamplesRunner {

        protected Type sampleClass;

        protected RunnerParams sampleClassParams;

        private String errorMessage;

        protected WrappedSamplesRunner(RunnerParams runnerParams) {
            this.sampleClassParams = runnerParams;
        }

        protected static ICollection<TestFixtureData> GenerateTestsList(Assembly assembly) {
            return GenerateTestsList(assembly, new RunnerSearchConfig().AddPackageToRunnerSearchPath(""));
        }

        protected static ICollection<TestFixtureData> GenerateTestsList(Assembly assembly, RunnerSearchConfig searchConfig) {
            IList<TestFixtureData> parametersList = new List<TestFixtureData>();
            foreach (Type type in assembly.GetTypes()) {
                WrappedSamplesRunner.RunnerParams runnerParams = CheckIfTestAndCreateParams(type, searchConfig);
                if (runnerParams != null && !type.IsNested) {
                    parametersList.Add(new TestFixtureData(runnerParams));
                }
            }

            return parametersList;
        }

        public virtual void RunSamples() {
            Assume.That(sampleClassParams.ignoreMessage == null, sampleClassParams.ignoreMessage);

            InitClass();
            System.Console.Out.WriteLine("Starting test " + sampleClassParams);
            
            string oldCurrentDir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(NUnit.Framework.TestContext.CurrentContext.TestDirectory);

            RunMain();

            String dest = GetDest();
            String cmp = GetCmpPdf(dest);
            if (String.IsNullOrEmpty(dest)) {
                throw new ArgumentException("Can't verify results, DEST field must not be empty!");
            }
            String outPath = GetOutPath(dest);
            FileUtil.CreateDirectories(outPath);
            System.Console.Out.WriteLine("Test executed successfully, comparing results...");
            ComparePdf(outPath, dest, cmp);

            Directory.SetCurrentDirectory(oldCurrentDir);

            if (errorMessage != null) {
                NUnit.Framework.Assert.Fail(errorMessage);
            }
            System.Console.Out.WriteLine("Test complete.");

        }
        
        protected internal virtual void InitClass() {
            sampleClass = sampleClassParams.sampleType;
        }
        
        /// <summary>Compares two PDF files using iText's CompareTool.</summary>
        /// <param name="outPath">The path to the working folder where comparison results and temp files will be created</param>
        /// <param name="dest">The PDF that resulted from the test</param>
        /// <param name="cmp">The reference PDF</param>
        protected internal abstract void ComparePdf(String outPath, String dest, String cmp);

        /// <summary>Gets the path to the resulting PDF from the sample class;</summary>
        /// <returns>a path to a resulting PDF</returns>
        protected internal virtual String GetDest() {
            return GetStringField(sampleClassParams.sampleType, "DEST");
        }

        protected internal virtual String GetCmpPdf(String dest) {
            if (dest == null) {
                return null;
            }
            int i = dest.LastIndexOf("/");
            int j = dest.IndexOf("/") + 1;
            return "../../../cmpfiles/" + dest.Substring(j, (i + 1) - j) + "cmp_" + dest.Substring(i + 1);
        }

        protected internal virtual String GetOutPath(String dest) {
            return Path.GetDirectoryName(dest);
        }

        /// <summary>
        /// Returns a string value that is stored as a static variable
        /// inside an example class.
        /// </summary>
        /// <param name="name">the name of the variable</param>
        /// <returns>the value of the variable</returns>
        protected internal static String GetStringField(Type c, String name) {
            try {
                FieldInfo field = c.GetField(name);
                if (field == null) {
                    return null;
                }
                Object obj = field.GetValue(null);
                if (obj == null || !(obj is String)) {
                    return null;
                }
                return (String)obj;
            }
            catch (Exception) {
                return null;
            }
        }

        /// <summary>Helper method to construct error messages.</summary>
        /// <param name="error">part of an error message.</param>
        protected internal virtual void AddError(String error) {
            if (!string.IsNullOrEmpty(error)) {
                if (errorMessage == null) {
                    errorMessage = "";
                }
                else {
                    errorMessage += "\n";
                }
                errorMessage += error;
            }
        }

        protected void RunMain() {
            MethodInfo mainMethod = GetMain(sampleClassParams.sampleType);
            if (mainMethod == null) {
                throw new ArgumentException("Class must have main method.");
            }
            mainMethod.Invoke(null, new Object[] { null });
        }

        private static MethodInfo GetMain(Type c) {
            try {
                return c.GetMethod("Main", new[]{ typeof(String[]) } );
            }
            catch (MissingMethodException) {
                return null;
            }
        }

        private static WrappedSamplesRunner.RunnerParams CheckIfTestAndCreateParams(Type classType, RunnerSearchConfig searchConfig) {
            if (!IsInSearchPath(classType.FullName, searchConfig)) {
                return null;
            }
            if (IsIgnoredClassOrPackage(classType, searchConfig)) {
                return null;
            }

            WrappedSamplesRunner.RunnerParams runnerParams = new WrappedSamplesRunner.RunnerParams();
            runnerParams.sampleType = classType;
            
            return runnerParams;
        }

        private static bool IsIgnoredClassOrPackage(Type type, RunnerSearchConfig searchConfig) {
            String fullName = type.FullName;
            foreach (String ignoredPath in searchConfig.GetIgnoredPaths()) {
                String filePath = Path.Combine(TestUtil.GetParentProjectDirectory(type.GetAssembly().Location), 
                    ignoredPath.Replace(".", "\\"));

                if ((Directory.Exists(filePath) && fullName.Contains(ignoredPath))
                    || (File.Exists(filePath + ".cs") && fullName.Equals(ignoredPath))) {
                    return true;
                }
            }
            return false;
        }

        private static bool IsInSearchPath(String fullName, RunnerSearchConfig searchConfig) {
            if (searchConfig.GetSearchClasses().Contains(fullName)) {
                return true;
            }
            foreach (String searchPackage in searchConfig.GetSearchPackages()) {
                if (fullName.StartsWith(searchPackage)) {
                    return true;
                }
            }
            return false;
        }

        public class RunnerParams {
            internal Type sampleType;

            internal String ignoreMessage;

            public override String ToString() {
                return sampleType.FullName;
            }
        }
    }
}
