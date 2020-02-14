using System;
 
namespace iText.Signatures {
    public class TimestampConstants {
        /// <summary>The timestamp which is returned in case the signed document doesn't contain timestamp.</summary>
        /// <remarks>
        /// The timestamp which is returned in case the signed document doesn't contain timestamp.
        /// The constant's value is different in Java and .NET.
        /// </remarks>
        public static readonly DateTime UNDEFINED_TIMESTAMP_DATE = DateTime.MaxValue;
    }
}