using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using iTextSharp.Forms;
using iTextSharp.Forms.Fields;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Utils;
using iTextSharp.Test;
using NUnit.Framework;

namespace iTextSharp.Samples {
    [LogListener]
    [TestFixture]
    public class GenericTest {


        protected bool compareRenders = false;

        /// <summary>
        /// An error message
        /// </summary>
        private string errorMessage;

        /** A prefix that is part of the error message. */
        protected String differenceImagePrefix = "difference";

        [SetUp]
        public virtual void BeforeTestMethodAction()
        {
        }

        [TearDown]
        public virtual void AfterTestMethodAction()
        {
        }

        [Test]
        [Timeout(120000)]
        public virtual void Test() {
            if (GetType() == typeof (GenericTest)) {
                return;
            }
            //LOGGER.info("Starting test " + getClass().getName() + ".");
            // Getting the destination PDF file (must be there!)
            String dest = GetDest();
            if (dest == null || dest.Length == 0)
                throw new ArgumentException("DEST cannot be empty!");
            // Compare the destination PDF with a reference PDF
            BeforeManipulatePdf();
            ManipulatePdf(dest);
            AfterManipulatePdf();
            //System.out.println(dest + "\n" + getCmpPdf());
            ComparePdf(dest, GetCmpPdf());
            //LOGGER.info("Test complete.");
        }

        protected virtual void ManipulatePdf(String dest) { 
        }

        protected virtual void BeforeManipulatePdf() {
        }

        protected virtual void AfterManipulatePdf() {
        }

        /// <summary>
        /// Gets the path to the resulting PDF from the sample class;
        /// this method also creates directories if necessary.
        /// </summary>
        /// <returns>a path to a resulting PDF</returns>
        protected string GetDest() {
            string dest = GetStringField("DEST");
            if (dest != null) {
                DirectoryInfo dir = new FileInfo(dest).Directory;
                if (dir != null)
                    dir.Create();
            }
            return dest;
        }

        /// <summary>
        /// Returns a string value that is stored as a static variable
        /// inside an example class.
        /// </summary>
        /// <param name="name">the name of the variable</param>
        /// <returns>the value of the variable</returns>
        protected string GetStringField(string name) {
            try {
                FieldInfo field = GetType().GetField(name);
                if (field == null)
                    return null;
                Object obj = field.GetValue(null);
                return obj as String;
            } catch (Exception e) {
                return null;
            }
        }

        /// <summary>
        /// Every test needs to know where to find its reference file.
        /// </summary>
        /// <returns></returns>
        protected string GetCmpPdf() {
            string tmp = GetDest();
            if (tmp == null)
                return null;
            int i = tmp.LastIndexOf("/");
            string path = "../../cmpfiles/" + tmp.Substring(8, (i + 1) - 8) + "cmp_" + tmp.Substring(i + 1);
            return path;
        }

        /// <summary>
        /// Compares two PDF files using iText's CompareTool.
        /// </summary>
        /// <param name="dest">the PDF that resulted from the test</param>
        /// <param name="cmp">the reference PDF</param>
        protected void ComparePdf(string dest, string cmp) {
            if (string.IsNullOrEmpty(cmp))
                return;
            CompareTool compareTool = new CompareTool();
            string outPath = "./target/" + new DirectoryInfo(dest).Parent;
            new DirectoryInfo(outPath).Create();
            if (compareRenders) {
                AddError(compareTool.CompareVisually(dest, cmp, outPath, differenceImagePrefix));
                AddError(compareTool.CompareLinkAnnotations(dest, cmp));
            } else {
                AddError(compareTool.CompareByContent(dest, cmp, outPath, differenceImagePrefix));
            }
            AddError(compareTool.CompareDocumentInfo(dest, cmp));


            if (errorMessage != null)
                Assert.Fail(errorMessage);
        }

        /// <summary>
        /// Helper method to construct error messages.
        /// </summary>
        /// <param name="error">part of an error message.</param>
        private void AddError(string error) {
            if (!string.IsNullOrEmpty(error)) {
                if (errorMessage == null)
                    errorMessage = "";
                else
                    errorMessage += "\n";

                errorMessage += error;
            }
        }
    }
}
