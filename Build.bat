del Packages\Net4x.itext7.*
rmdir /s /q %userprofile%\.nuget\Packages\Net4x.itext7
del Packages\Net4x.itext7.commons.*
rmdir /s /q %userprofile%\.nuget\Packages\Net4x.itext7.commons
nuget.exe restore iTextCore.sln
MSBuild.exe iTextCore.sln -m /property:Configuration=Release
cd NuSpecs
nuget pack -OutputDirectory ..\Packages Net4x.itext7-commons.nuspec
nuget pack -OutputDirectory ..\Packages Net4x.itext7.nuspec
cd ..
git add -A
git commit -a --allow-empty-message -m ''
git push