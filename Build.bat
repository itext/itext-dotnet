del Packages\unofficialitext7.*
rmdir /s /q %userprofile%\.nuget\Packages\unofficialitext7
del Packages\unofficialitext7.commons.*
rmdir /s /q %userprofile%\.nuget\Packages\unofficialitext7.commons
nuget.exe restore iTextCore.sln
MSBuild.exe iTextCore.sln /property:Configuration=Release
cd NuSpecs
nuget pack -OutputDirectory ..\Packages BaseUnitTest.PieroViano.nuspec
nuget pack -OutputDirectory ..\Packages ConfigurationLibrary.Crypt.PieroViano.nuspec
nuget pack -OutputDirectory ..\Packages ConfigurationLibrary.PieroViano.nuspec
nuget pack -OutputDirectory ..\Packages RemoteLoggerLib.PieroViano.nuspec
cd ..
git add -A
git commit -a --allow-empty-message -m ''
git push