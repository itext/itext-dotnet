using iText.Commons.Bouncycastle.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace iText.Commons.Bouncycastle.Cert
{
    public interface IX509CrlEntry
    {
        DateTime GetRevocationDate();
        IBigInteger GetSerialNumber();
        CRLReason GetReason();
    }
}
