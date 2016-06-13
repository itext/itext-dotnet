using System;

namespace iText.Layout.Renderer
{
    static class AbstractRendererExtensions
    {

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Property.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public static float? GetPropertyAsFloat(this AbstractRenderer renderer, int property)
        {
            Object value = renderer.GetProperty<Object>(property);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToSingle(value) : (float?)null;
        }

        /// <summary>Returns a property with a certain key, as a floating point value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Property.Property">enum value</see>
        /// </param>
        /// <param name="defaultValue">default value to be returned if property is not found</param>
        /// <returns>
        /// a
        /// <see cref="float?"/>
        /// </returns>
        public static float? GetPropertyAsFloat(this AbstractRenderer renderer, int property, float? defaultValue)
        {

            Object value = renderer.GetProperty<Object>(property, defaultValue);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToSingle(value) : (float?)null;
        }

        /// <summary>Returns a property with a certain key, as an integer value.</summary>
        /// <param name="property">
        /// an
        /// <see cref="iText.Layout.Property.Property">enum value</see>
        /// </param>
        /// <returns>
        /// a
        /// <see cref="int?"/>
        /// </returns>
        public static int? GetPropertyAsInteger(this AbstractRenderer renderer, int property)
        {
            Object value = renderer.GetProperty<Object>(property);
            return (value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal)
                ? Convert.ToInt32(value) : (int?)null;
        }
    }
}