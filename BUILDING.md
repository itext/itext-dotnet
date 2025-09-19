
# Building and testing **iText Community**

To build **iText Community**, you need to build `itextsharp/itextcore/iTextCore.sln`.
To run tests, [Ghostscript][1] and [Imagemagick][2] must be installed.  
Some of the tests compare generated PDF files with template files that show the correct results, and these tools are used to
visually compare PDF files. Ghostscript is required to render PDF files into images and Imagemagick is used to compare image data. 
There are two options for running tests:
1. Pass Ghostscript and Imagemagick compare commands as ITEXT_GS_EXEC and ITEXT_MAGICK_COMPARE_EXEC environment variables, respectively 
(Ghostscript and Imagemagick folders must be added to PATH environment variable). This option is suitable for both Windows and Unix systems. 
The syntax of commands depends on installed Ghostscript and Imagemagick versions (`gs`, `gswin64c`, `magick compare`).
2. Pass the paths to Ghostscript and Imagemagick compare execution files as ITEXT_GS_EXEC and ITEXT_MAGICK_COMPARE_EXEC 
environment variables. Examples of paths on Windows:
- `C:\Program Files\gs\gs9.26\bin\gswin64c.exe`
- `C:\Program Files\ImageMagick-7.0.9-Q16\compare.exe`

If you have a new version of ImageMagick, then there is no compare.exe utility there, wrap the path to magick.exe in quotes and call compare command:
ITEXT_MAGICK_COMPARE_EXEC=`"C:\Program Files\ImageMagick-7.0.9-Q16\magick.exe" compare`


# Deploying iText

When using **iText Community** in a project and want to deploy it you have to consider a few things for different deployments.

- **FrameworkDependend**: No additional parameters are required.
- **SelfContained**: No additional parameters are required.
- **PublishSingleFile**: When using `-p:PublishSingleFile=true` you will also need to add `-p:IncludeAllContentForSelfExtract=true`. This is important when using `hyph` or `font-asian` modules.
- **AssemblyTrimming**: Using `-p:PublishTrimmed=true` is currently not supported.


# Building AOT

Aot is a quite complex feature, iText library is a non trivial project, meaning it does require some  fixes to get it working.


1. iText relies on `SignedXml`  for the `LotlValidator` functionality currently there is an open issue: https://github.com/dotnet/runtime/issues/97274.
To fix usage within iText you need some additional annotations so the algorithms don't get trimmed away. 
2. One way to fix this is add following annotations to your code to make sure trimming does not happen.
```csharp

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.SHA1Managed",
        "System.Security.Cryptography.Algorithms")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.SHA256Managed",
        "System.Security.Cryptography.Algorithms")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.SHA384Managed",
        "System.Security.Cryptography.Algorithms")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.SHA512Managed",
        "System.Security.Cryptography.Algorithms")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.RSAPKCS1SignatureFormatter",
        "System.Security.Cryptography.Algorithms")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, "System.Security.Cryptography.RSAPKCS1SignatureDeformatter",
        "System.Security.Cryptography.Algorithms")]
    void LotlValidationExample() {
        ValidatorChainBuilder chainBuilder = new ValidatorChainBuilder();
        chainBuilder.TrustEuropeanLotl(true);
        ValidationReport report;
        using (LotlService lotlService =
               new LotlService(new LotlFetchingProperties(new RemoveOnFailingCountryData()).SetCountryNames("AT"))) {
            lotlService.InitializeCache();


            chainBuilder.WithLotlService(() => lotlService);
            LotlValidator lotlValidator = new LotlValidator(lotlService);
            lotlService.WithLotlValidator(() => lotlValidator);
            lotlService.InitializeCache();
            report = lotlValidator.Validate();
        }

    }
```
2. Some default modules rely on specific classes that initialize default implementations might be trimmed away. To avoid this we recommend adding following entries to the `ILLink descriptors file`:
```xml
<linker>
  <assembly fullname="itext.kernel">
    <type fullname="iText.Kernel.Utils.RegisterDefaultDiContainer" preserve="all" />
  </assembly>
  <assembly fullname="itext.forms">
    <type fullname="iText.Forms.Util.RegisterDefaultDiContainer" preserve="all" />
  </assembly>
</linker>
```
If you didn't set up an `ILLink descriptors file` already for your project you can do it by adding a file called `ILLink.Descriptors.xml` and adding the required configuration to you `project.csrpoj` file.
```xml
<ItemGroup>
		<EmbeddedResource Include="ILLink.Descriptors.xml">
			<LogicalName>ILLink.Descriptors.xml</LogicalName>
		</EmbeddedResource>
</ItemGroup>
```

3. If you are using the `font-asian`, or `hyph` module you should call the initializers on the startup of you application.
```
new iText.FontAsian.FontAsianDummyInitializer(); // Font asian
new iText.Hyph.HyphDummyInitializer(); // Hyph module
```




[1]: https://www.ghostscript.com/
[2]: https://www.imagemagick.org/
