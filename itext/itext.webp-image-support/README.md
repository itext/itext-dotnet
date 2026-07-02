# Webp image support for iText

## Overview

This module extends iText functionality by adding support for WebP image format, 
enabling seamless integration of WebP images into PDF documents.

## Features

- Loads and embeds WebP images into PDF documents
- Works with modern WebP encodings (lossy & lossless)
- Easy integration with existing iText workflows

## Installation

The easiest way to get started is to use NuGet, execute the following install command in the folder of your
project:

```shell
dotnet add package webp-image-support --version <REPLACE_WITH_DESIRED_WEBP_VERSION>
```
Also RuntimeIdentifier could be required to load native libraries for your system:

```xml
  <!-- RuntimeIdentifier is required for webp-image-support to load native libraries for your system. -->
<PropertyGroup Condition="'$(RuntimeIdentifier)' == '' And $([MSBuild]::IsOsPlatform('Windows'))">
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'AMD64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'X64'">win-x64</RuntimeIdentifier>
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'x86' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'X86'">win-x86</RuntimeIdentifier>
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'ARM64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'Arm64'">win-arm64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="'$(RuntimeIdentifier)' == '' And $([MSBuild]::IsOsPlatform('OSX'))">
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'AMD64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'X64'">osx-x64</RuntimeIdentifier>
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'ARM64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'Arm64'">osx-arm64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup Condition="'$(RuntimeIdentifier)' == '' And $([MSBuild]::IsOsPlatform('Linux'))">
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'AMD64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'X64'">linux-x64</RuntimeIdentifier>
    <RuntimeIdentifier Condition="'$(PROCESSOR_ARCHITECTURE)' == 'ARM64' Or '$([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture)' == 'Arm64'">linux-arm64</RuntimeIdentifier>
</PropertyGroup>
<PropertyGroup>
    <!-- Default fallback -->
    <RuntimeIdentifier Condition="'$(RuntimeIdentifier)' == ''">win-x64</RuntimeIdentifier>
    <AppendRuntimeIdentifierToOutputPath>false</AppendRuntimeIdentifierToOutputPath>
</PropertyGroup>
```


## Usage

To add WebP image to your document use ImageDataFactory as with any other image type:

```csharp
// Create a pdf document with WebP image
PdfDocument pdfDocument = new PdfDocument(new PdfWriter(resultFileLocation));
byte[] imageBytes;
using (Stream fis = FileUtil.GetInputStreamForFile(webpFileLocation)) {
    imageBytes = StreamUtil.InputStreamToArray(fis);
}
PdfPage page = pdfDocument.AddNewPage();
PdfCanvas canvas = new PdfCanvas(page);
ImageData img = ImageDataFactory.Create(imageBytes);
canvas.AddImageAt(img, 0, 0, false);
canvas.Release();

pdfDocument.Close();
// Done!
```