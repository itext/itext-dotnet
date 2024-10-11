
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



[1]: https://www.ghostscript.com/
[2]: https://www.imagemagick.org/
