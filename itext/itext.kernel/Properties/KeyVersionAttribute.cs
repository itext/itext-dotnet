using System;

namespace Versions.Attributes {
    [AttributeUsage(AttributeTargets.Assembly)]
    internal class KeyVersionAttribute : Attribute {
        internal string KeyVersion { get; }

        internal KeyVersionAttribute(string keyVersion) {
            this.KeyVersion = keyVersion;
        }
    }
}