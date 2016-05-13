//Copyright (c) 2006, Adobe Systems Incorporated
//All rights reserved.
//
//        Redistribution and use in source and binary forms, with or without
//        modification, are permitted provided that the following conditions are met:
//        1. Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//        2. Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//        3. All advertising materials mentioning features or use of this software
//        must display the following acknowledgement:
//        This product includes software developed by the Adobe Systems Incorporated.
//        4. Neither the name of the Adobe Systems Incorporated nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//        THIS SOFTWARE IS PROVIDED BY ADOBE SYSTEMS INCORPORATED ''AS IS'' AND ANY
//        EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//        DISCLAIMED. IN NO EVENT SHALL ADOBE SYSTEMS INCORPORATED BE LIABLE FOR ANY
//        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//        LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//        ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//        (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//        SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//        http://www.adobe.com/devnet/xmp/library/eula-xmp-library-java.html
using System;
using System.IO;
using iTextSharp.Kernel.Xmp.Impl;
using iTextSharp.Kernel.Xmp.Options;

namespace iTextSharp.Kernel.Xmp
{
	/// <summary>Creates <code>XMPMeta</code>-instances from an <code>InputStream</code></summary>
	/// <since>30.01.2006</since>
	public sealed class XMPMetaFactory
	{
		/// <summary>The singleton instance of the <code>XMPSchemaRegistry</code>.</summary>
		private static XMPSchemaRegistry schema = new XMPSchemaRegistryImpl();

		/// <summary>cache for version info</summary>
		private static XMPVersionInfo versionInfo = null;

		/// <summary>Hides public constructor</summary>
		private XMPMetaFactory()
		{
		}

		// EMPTY
		/// <returns>Returns the singleton instance of the <code>XMPSchemaRegistry</code>.</returns>
		public static XMPSchemaRegistry GetSchemaRegistry()
		{
			return schema;
		}

		/// <returns>Returns an empty <code>XMPMeta</code>-object.</returns>
		public static XMPMeta Create()
		{
			return new XMPMetaImpl();
		}

		/// <summary>Parsing with default options.</summary>
		/// <param name="in">an <code>InputStream</code></param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <seealso cref="Parse(System.IO.Stream, iTextSharp.Kernel.Xmp.Options.ParseOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta Parse(Stream @in)
		{
			return Parse(@in, null);
		}

		/// <summary>
		/// These functions support parsing serialized RDF into an XMP object, and serailizing an XMP
		/// object into RDF.
		/// </summary>
		/// <remarks>
		/// These functions support parsing serialized RDF into an XMP object, and serailizing an XMP
		/// object into RDF. The input for parsing may be any valid Unicode
		/// encoding. ISO Latin-1 is also recognized, but its use is strongly discouraged. Serialization
		/// is always as UTF-8.
		/// <p/>
		/// <code>parseFromBuffer()</code> parses RDF from an <code>InputStream</code>. The encoding
		/// is recognized automatically.
		/// </remarks>
		/// <param name="in">an <code>InputStream</code></param>
		/// <param name="options">
		/// Options controlling the parsing.<br />
		/// The available options are:
		/// <ul>
		/// <li> XMP_REQUIRE_XMPMETA - The &lt;x:xmpmeta&gt; XML element is required around
		/// <tt>&lt;rdf:RDF&gt;</tt>.
		/// <li> XMP_STRICT_ALIASING - Do not reconcile alias differences, throw an exception.
		/// </ul>
		/// <em>Note:</em>The XMP_STRICT_ALIASING option is not yet implemented.
		/// </param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta Parse(Stream @in, ParseOptions options)
		{
			return XMPMetaParser.Parse(@in, options);
		}

		/// <summary>Parsing with default options.</summary>
		/// <param name="packet">a String contain an XMP-file.</param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <seealso cref="Parse(System.IO.Stream)"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta ParseFromString(String packet)
		{
			return ParseFromString(packet, null);
		}

		/// <summary>Creates an <code>XMPMeta</code>-object from a string.</summary>
		/// <param name="packet">a String contain an XMP-file.</param>
		/// <param name="options">Options controlling the parsing.</param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <seealso cref="ParseFromString(System.String, iTextSharp.Kernel.Xmp.Options.ParseOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta ParseFromString(String packet, ParseOptions options)
		{
			return XMPMetaParser.Parse(packet, options);
		}

		/// <summary>Parsing with default options.</summary>
		/// <param name="buffer">a String contain an XMP-file.</param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <seealso cref="ParseFromBuffer(byte[], iTextSharp.Kernel.Xmp.Options.ParseOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta ParseFromBuffer(byte[] buffer)
		{
			return ParseFromBuffer(buffer, null);
		}

		/// <summary>Creates an <code>XMPMeta</code>-object from a byte-buffer.</summary>
		/// <param name="buffer">a String contain an XMP-file.</param>
		/// <param name="options">Options controlling the parsing.</param>
		/// <returns>Returns the <code>XMPMeta</code>-object created from the input.</returns>
		/// <exception cref="XMPException">If the file is not well-formed XML or if the parsing fails.
		/// 	</exception>
		/// <seealso cref="Parse(System.IO.Stream, iTextSharp.Kernel.Xmp.Options.ParseOptions)
		/// 	"/>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static XMPMeta ParseFromBuffer(byte[] buffer, ParseOptions options)
		{
			return XMPMetaParser.Parse(buffer, options);
		}

		/// <summary>
		/// Serializes an <code>XMPMeta</code>-object as RDF into an <code>OutputStream</code>
		/// with default options.
		/// </summary>
		/// <param name="xmp">a metadata object</param>
		/// <param name="out">an <code>OutputStream</code> to write the serialized RDF to.</param>
		/// <exception cref="XMPException">on serializsation errors.</exception>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static void Serialize(XMPMeta xmp, Stream @out)
		{
			Serialize(xmp, @out, null);
		}

		/// <summary>Serializes an <code>XMPMeta</code>-object as RDF into an <code>OutputStream</code>.
		/// 	</summary>
		/// <param name="xmp">a metadata object</param>
		/// <param name="options">
		/// Options to control the serialization (see
		/// <see cref="iTextSharp.Kernel.Xmp.Options.SerializeOptions"/>
		/// ).
		/// </param>
		/// <param name="out">an <code>OutputStream</code> to write the serialized RDF to.</param>
		/// <exception cref="XMPException">on serializsation errors.</exception>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static void Serialize(XMPMeta xmp, Stream @out, SerializeOptions options)
		{
			AssertImplementation(xmp);
			XMPSerializerHelper.Serialize((XMPMetaImpl)xmp, @out, options);
		}

		/// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a byte buffer.</summary>
		/// <param name="xmp">a metadata object</param>
		/// <param name="options">
		/// Options to control the serialization (see
		/// <see cref="iTextSharp.Kernel.Xmp.Options.SerializeOptions"/>
		/// ).
		/// </param>
		/// <returns>Returns a byte buffer containing the serialized RDF.</returns>
		/// <exception cref="XMPException">on serializsation errors.</exception>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static byte[] SerializeToBuffer(XMPMeta xmp, SerializeOptions options)
		{
			AssertImplementation(xmp);
			return XMPSerializerHelper.SerializeToBuffer((XMPMetaImpl)xmp, options);
		}

		/// <summary>Serializes an <code>XMPMeta</code>-object as RDF into a string.</summary>
		/// <remarks>
		/// Serializes an <code>XMPMeta</code>-object as RDF into a string. <em>Note:</em> Encoding
		/// is ignored when serializing to a string.
		/// </remarks>
		/// <param name="xmp">a metadata object</param>
		/// <param name="options">
		/// Options to control the serialization (see
		/// <see cref="iTextSharp.Kernel.Xmp.Options.SerializeOptions"/>
		/// ).
		/// </param>
		/// <returns>Returns a string containing the serialized RDF.</returns>
		/// <exception cref="XMPException">on serializsation errors.</exception>
		/// <exception cref="iTextSharp.Kernel.Xmp.XMPException"/>
		public static String SerializeToString(XMPMeta xmp, SerializeOptions options)
		{
			AssertImplementation(xmp);
			return XMPSerializerHelper.SerializeToString((XMPMetaImpl)xmp, options);
		}

		/// <param name="xmp">Asserts that xmp is compatible to <code>XMPMetaImpl</code>.s</param>
		private static void AssertImplementation(XMPMeta xmp)
		{
			if (!(xmp is XMPMetaImpl))
			{
				throw new NotSupportedException("The serializing service works only" + "with the XMPMeta implementation of this library"
					);
			}
		}

		/// <summary>Resets the schema registry to its original state (creates a new one).</summary>
		/// <remarks>
		/// Resets the schema registry to its original state (creates a new one).
		/// Be careful this might break all existing XMPMeta-objects and should be used
		/// only for testing purpurses.
		/// </remarks>
		public static void Reset()
		{
			schema = new XMPSchemaRegistryImpl();
		}

		/// <summary>Obtain version information.</summary>
		/// <remarks>
		/// Obtain version information. The XMPVersionInfo singleton is created the first time
		/// its requested.
		/// </remarks>
		/// <returns>Returns the version information.</returns>
		public static XMPVersionInfo GetVersionInfo()
		{
			lock (typeof(XMPMetaFactory))
			{
				if (versionInfo == null)
				{
					try
					{
						int major = 5;
						int minor = 1;
						int micro = 0;
						int engBuild = 3;
						bool debug = false;
						// Adobe XMP Core 5.0-jc001 DEBUG-<branch>.<changelist>, 2009 Jan 28 15:22:38-CET
						String message = "Adobe XMP Core 5.1.0-jc003";
						versionInfo = new _XMPVersionInfo_263(major, minor, micro, debug, engBuild, message
							);
					}
					catch (Exception e)
					{
						// EMTPY, severe error would be detected during the tests
						System.Console.Out.WriteLine(e);
					}
				}
				return versionInfo;
			}
		}

		private sealed class _XMPVersionInfo_263 : XMPVersionInfo
		{
			public _XMPVersionInfo_263(int major, int minor, int micro, bool debug, int engBuild
				, String message)
			{
				this.major = major;
				this.minor = minor;
				this.micro = micro;
				this.debug = debug;
				this.engBuild = engBuild;
				this.message = message;
			}

			public int GetMajor()
			{
				return major;
			}

			public int GetMinor()
			{
				return minor;
			}

			public int GetMicro()
			{
				return micro;
			}

			public bool IsDebug()
			{
				return debug;
			}

			public int GetBuild()
			{
				return engBuild;
			}

			public String GetMessage()
			{
				return message;
			}

			public override String ToString()
			{
				return message;
			}

			private readonly int major;

			private readonly int minor;

			private readonly int micro;

			private readonly bool debug;

			private readonly int engBuild;

			private readonly String message;
		}
	}
}
