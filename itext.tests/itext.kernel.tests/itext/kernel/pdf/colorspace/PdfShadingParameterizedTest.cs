using System;
using System.Collections.Generic;
using System.Linq;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Test;

namespace iText.Kernel.Pdf.Colorspace {
    [NUnit.Framework.TestFixtureSource("ParametersTestFixtureData")]
    public class PdfShadingParameterizedTest : ExtendedITextTest {
        private String shadingName;

        private int shadingType;

        public PdfShadingParameterizedTest(Object name, Object type) {
            shadingName = (String)name;
            shadingType = (int)type;
        }

        public PdfShadingParameterizedTest(Object[] array)
            : this(array[0], array[1]) {
        }

        public static IEnumerable<Object[]> Parameters() {
            return JavaUtil.ArraysAsList(new Object[][] { new Object[] { "FreeFormGouraudShadedTriangleMesh", 4 }, new 
                Object[] { "LatticeFormGouraudShadedTriangleMesh", 5 }, new Object[] { "CoonsPatchMesh", 6 }, new Object
                [] { "TensorProductPatchMesh", 7 } });
        }

        public static ICollection<NUnit.Framework.TestFixtureData> ParametersTestFixtureData() {
            return Parameters().Select(array => new NUnit.Framework.TestFixtureData(array)).ToList();
        }

        [NUnit.Framework.Test]
        public virtual void AllAboveType3FromDictionaryShouldFailTest() {
            PdfDictionary dict = new PdfDictionary();
            dict.Put(PdfName.ShadingType, new PdfNumber(shadingType));
            dict.Put(PdfName.ColorSpace, PdfName.DeviceRGB);
            Exception e = NUnit.Framework.Assert.Catch(typeof(PdfException), () => PdfShading.MakeShading(dict), "Creating "
                 + shadingName + " should throw PdfException.");
            NUnit.Framework.Assert.AreEqual(KernelExceptionMessageConstant.UNEXPECTED_SHADING_TYPE, e.Message);
        }
    }
}
