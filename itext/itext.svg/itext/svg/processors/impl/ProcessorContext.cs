using System;
using iText.IO.Font;
using iText.Layout.Font;
using iText.StyledXmlParser.Css.Media;
using iText.StyledXmlParser.Resolver.Font;
using iText.StyledXmlParser.Resolver.Resource;
using iText.Svg.Processors;

namespace iText.Svg.Processors.Impl {
    /// <summary>Context class with accessors to properties/objects used in processing Svg documents</summary>
    public class ProcessorContext {
        /// <summary>The font provider.</summary>
        private FontProvider fontProvider;

        /// <summary>Temporary set of fonts used in the PDF.</summary>
        private FontSet tempFonts;

        private ResourceResolver resourceResolver;

        /// <summary>The device description.</summary>
        private MediaDeviceDescription deviceDescription;

        /// <summary>The base URI.</summary>
        private String baseUri;

        /// <summary>
        /// Instantiates a new
        /// <see cref="ProcessorContext"/>
        /// instance.
        /// </summary>
        /// <param name="converterProperties">
        /// a
        /// <see cref="iText.Svg.Processors.ISvgConverterProperties"/>
        /// instance
        /// </param>
        public ProcessorContext(ISvgConverterProperties converterProperties) {
            deviceDescription = converterProperties.GetMediaDeviceDescription();
            if (deviceDescription == null) {
                deviceDescription = MediaDeviceDescription.GetDefault();
            }
            fontProvider = converterProperties.GetFontProvider();
            if (fontProvider == null) {
                fontProvider = new BasicFontProvider();
            }
            baseUri = converterProperties.GetBaseUri();
            if (baseUri == null) {
                baseUri = "";
            }
            resourceResolver = new ResourceResolver(baseUri);
        }

        /// <summary>Gets the font provider.</summary>
        /// <returns>the font provider</returns>
        public virtual FontProvider GetFontProvider() {
            return fontProvider;
        }

        /// <summary>Gets the resource resolver.</summary>
        /// <returns>the resource resolver</returns>
        public virtual ResourceResolver GetResourceResolver() {
            return resourceResolver;
        }

        /// <summary>Gets the device description.</summary>
        /// <returns>the device description</returns>
        public virtual MediaDeviceDescription GetDeviceDescription() {
            return deviceDescription;
        }

        /// <summary>Gets the temporary set of fonts.</summary>
        /// <returns>the set of fonts</returns>
        public virtual FontSet GetTempFonts() {
            return tempFonts;
        }

        /// <summary>Add temporary font from @font-face.</summary>
        /// <param name="fontProgram">the font program</param>
        /// <param name="encoding">the encoding</param>
        /// <param name="alias">the alias</param>
        public virtual void AddTemporaryFont(FontProgram fontProgram, String encoding, String alias) {
            if (tempFonts == null) {
                tempFonts = new FontSet();
            }
            tempFonts.AddFont(fontProgram, encoding, alias);
        }

        /// <summary>Add temporary font from @font-face.</summary>
        /// <param name="fontInfo">the font info</param>
        /// <param name="alias">the alias</param>
        public virtual void AddTemporaryFont(FontInfo fontInfo, String alias) {
            if (tempFonts == null) {
                tempFonts = new FontSet();
            }
            tempFonts.AddFont(fontInfo, alias);
        }
    }
}
