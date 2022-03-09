using System;
using System.Threading;
using iText.Commons.Actions.Contexts;
using iText.Layout.Renderer;
using iText.Test;

namespace iText.Forms.Fields {
    public class FormsMetaInfoStaticContainerTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void UseMetaInfoDuringTheActionOneThreadTest() {
            MetaInfoContainer metaInfo1 = new MetaInfoContainer(new _IMetaInfo_16());
            MetaInfoContainer metaInfo2 = new MetaInfoContainer(new _IMetaInfo_17());
            FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfo1, () => {
                NUnit.Framework.Assert.AreSame(metaInfo1, FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
                FormsMetaInfoStaticContainer.UseMetaInfoDuringTheAction(metaInfo2, () => NUnit.Framework.Assert.AreSame(metaInfo2
                    , FormsMetaInfoStaticContainer.GetMetaInfoForLayout()));
                NUnit.Framework.Assert.IsNull(FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
            }
            );
            NUnit.Framework.Assert.IsNull(FormsMetaInfoStaticContainer.GetMetaInfoForLayout());
        }

        private sealed class _IMetaInfo_16 : IMetaInfo {
            public _IMetaInfo_16() {
            }
        }

        private sealed class _IMetaInfo_17 : IMetaInfo {
            public _IMetaInfo_17() {
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
                MetaInfoContainer metaInfo = new MetaInfoContainer(new _IMetaInfo_56());
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

            private sealed class _IMetaInfo_56 : IMetaInfo {
                public _IMetaInfo_56() {
                }
            }

            public virtual bool IsCheckFailed() {
                return checkFailed;
            }
        }
    }
}
