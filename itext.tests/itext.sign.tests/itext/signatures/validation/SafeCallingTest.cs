/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Kernel.Exceptions;
using iText.Signatures.Validation.Report;
using iText.Test;

namespace iText.Signatures.Validation {
    [NUnit.Framework.Category("IntegrationTest")]
    public class SafeCallingTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void SafeCallingAvoidExceptionThrowsException() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SafeCallingAvoidantException), () => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new SafeCallingAvoidantException("Test exception");
                }
                , new ValidationReport(), (reportItem) => new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO)
                    );
            }
            );
            NUnit.Framework.Assert.AreEqual("Test exception", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SafeCallingWithCreatorAvoidExceptionThrowsException() {
            Exception e = NUnit.Framework.Assert.Catch(typeof(SafeCallingAvoidantException), () => {
                SafeCalling.OnExceptionLog(() => {
                    throw new SafeCallingAvoidantException("Test exception");
                }
                , new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO), new ValidationReport(), (reportItem) =>
                     new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO));
            }
            );
            NUnit.Framework.Assert.AreEqual("Test exception", e.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SafeCallingAvoidExceptionDoesNotThrowException() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(Exception), () => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new SafeCallingAvoidantException("Test exception");
                }
                , new ValidationReport(), (reportItem) => new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO)
                    );
            }
            );
            NUnit.Framework.Assert.AreEqual("Test exception", exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SafeCallingWithFuncAvoidExceptionDoesNotThrowException() {
            Exception exception = NUnit.Framework.Assert.Catch(typeof(Exception), () => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new SafeCallingAvoidantException("Test exception");
                }
                , new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO), new ValidationReport(), (reportItem) =>
                     new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO));
            }
            );
            NUnit.Framework.Assert.AreEqual("Test exception", exception.Message);
        }

        [NUnit.Framework.Test]
        public virtual void SafCallingWithPdfExceptionDoesNotThrowException() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new PdfException("Test exception");
                }
                , new ValidationReport(), (reportItem) => new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void NpeExceptionDoesNotThrowException() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new NullReferenceException("Test exception");
                }
                , new ValidationReport(), (reportItem) => new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO)
                    );
            }
            );
        }

        [NUnit.Framework.Test]
        public virtual void IllegalArgumentExceptionDoesNotThrowException() {
            NUnit.Framework.Assert.DoesNotThrow(() => {
                SafeCalling.OnRuntimeExceptionLog(() => {
                    throw new ArgumentException("Test exception");
                }
                , new ValidationReport(), (reportItem) => new ReportItem("test", "test", ReportItem.ReportItemStatus.INFO)
                    );
            }
            );
        }
    }
}
