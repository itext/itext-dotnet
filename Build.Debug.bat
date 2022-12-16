del Packages\unofficialitext7.*
rmdir /s /q %userprofile%\.nuget\Packages\unofficialitext7
del Packages\unofficialitext7.commons.*
rmdir /s /q %userprofile%\.nuget\Packages\unofficialitext7.commons
nuget.exe restore iTextCore.sln
MSBuild.exe iTextCore.sln -m /property:Configuration=Debug
cd NuSpecs.Debug
nuget pack -OutputDirectory ..\Packages unofficialitext7-commons.nuspec
nuget pack -OutputDirectory ..\Packages unofficialitext7.nuspec
cd ..
git add -A
git commit -a --allow-empty-message -m ''
git push