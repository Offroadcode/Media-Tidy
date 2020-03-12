# Media-Tidy
A package for Umbraco v7 that audits and tidies up the media section.

## Package Notes

This package requires a few projects - both C# libraries and Umbraco packages - to be installed to work.

### Umbraco Packages

* [Nexu](https://our.umbraco.com/packages/backoffice-extensions/nexu/)

### NuGet Packages/C# Libraries

* [CsvHelper v2.16.3](https://www.nuget.org/packages/CsvHelper/2.16.3)
* [CsvHelper.Excel v1.0.6](https://www.nuget.org/packages/CsvHelper.Excel/1.0.6)
* [ClosedXml v0.87.1](https://www.nuget.org/packages/ClosedXML/0.87.1)

## Development

### Install Dependencies

#### JS
*Requires Node.js to be installed and in your system path*

    npm install -g grunt-cli && npm install -g grunt
    npm install

### Build

#### Initial Build

Before you can do the initial build, you'll need to open the **Orc.MediaTidy.sln** file, restore the nuget packages, and build. This version of the Umbraco package builder does not use grunt's MSBuild due to errors, so the C# needs to be handled in Visual Studio.

#### Continued Build/Post Initial C# Build

    grunt

Builds the project to /dist/. These files can be dropped into an Umbraco 7 site, or you can build directly to a site using:

    grunt --target="D:\inetpub\mysite"

You can also watch for changes using:

    grunt watch
    grunt watch --target="D:\inetpub\mysite"

If you want to build the package file (into a pkg folder), use:
