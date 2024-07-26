/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
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
using System.Threading;
using iText.Commons.Actions.Contexts;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Fields {
    [NUnit.Framework.Category("UnitTest")]
    public class FormsMetaInfoStaticContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UseMetaInfoDuringTheActionOneThreadTest() {
            MetaInfoContainer metaInfo1 = new MetaInfoContainer(new _IMetaInfo_37());
            MetaInfoContainer metaInfo2 = new MetaInfoContainer(new _IMetaInfo_38());
            FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfo1, () => {
                NUnit.Framework.Assert.AreSame(metaInfo1, FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
                FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfo2, () => NUnit.Framework.Assert.AreSame(metaInfo2
                    , FormsMetaInfoStaticContainer.GetMetaInfoForLayout()));
                NUnit.Framework.Assert.IsNull(FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
            }
            );
            NUnit.Framework.Assert.IsNull(FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
        }

        private sealed class _IMetaInfo_37 : IMetaInfo {
            public _IMetaInfo_37() {
            }
        }

        private sealed class _IMetaInfo_38 : IMetaInfo {
            public _IMetaInfo_38() {
            }
        }

        [NUnit.Framework.Test]
        public virtual void UseMetaInfoDuringTheActionSeveralThreadsTest() {
            FormsMetaInfoStaticContainerTest.MetaInfoCheckClass metaInfoCheckClass1 = new FormsMetaInfoStaticContainerTest.MetaInfoCheckClass
                (null);
            FormsMetaInfoStaticContainerTest.MetaInfoCheckClass metaInfoCheckClass2 = new FormsMetaInfoStaticContainerTest.MetaInfoCheckClass
                (metaInfoCheckClass1);
            FormsMetaInfoStaticContainerTest.MetaInfoCheckClass metaInfoCheckClass3 = new FormsMetaInfoStaticContainerTest.MetaInfoCheckClass
                (metaInfoCheckClass2);
            Thread thread = new Thread(() => metaInfoCheckClass3.CheckMetaInfo());
            thread.Start();
            thread.Join();
            NUnit.Framework.Assert.IsFalse(metaInfoCheckClass1.IsCheckFailed());
            NUnit.Framework.Assert.IsFalse(metaInfoCheckClass2.IsCheckFailed());
            NUnit.Framework.Assert.IsFalse(metaInfoCheckClass3.IsCheckFailed());
        }

        private class MetaInfoCheckClass {
            private FormsMetaInfoStaticContainerTest.MetaInfoCheckClass metaInfoCheckClass = null;

            private bool checkFailed = false;

            public MetaInfoCheckClass(FormsMetaInfoStaticContainerTest.MetaInfoCheckClass metaInfoCheckClass) {
                this.metaInfoCheckClass = metaInfoCheckClass;
            }

            public virtual void CheckMetaInfo() {
                MetaInfoContainer metaInfo = new MetaInfoContainer(new _IMetaInfo_77());
                FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfo, () => {
                    if (metaInfoCheckClass != null) {
                        Thread thread = new Thread(() => metaInfoCheckClass.CheckMetaInfo());
                        thread.Start();
                        try {
                            thread.Join();
                        }
                        catch (Exception) {
                            checkFailed = true;
                        }
                    }
                    checkFailed |= metaInfo != FormsMetaInfoStaticContainer.GetMetaInfoForLayout();
                }
                );
                checkFailed |= FormsMetaInfoStaticContainer.GetMetaInfoForLayout() != null;
            }

            private sealed class _IMetaInfo_77 : IMetaInfo {
                public _IMetaInfo_77() {
                }
            }

            public virtual bool IsCheckFailed() {
                return checkFailed;
            }
        }
    }
}
