New-Item -Path ./artifacts -Force -ItemType directory

$artifacts = (Get-Item ./artifacts).FullName

dotnet pack -c Release -o $artifacts -p:Version=$(git describe --tags --dirty)
