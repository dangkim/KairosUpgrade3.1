param(
    [parameter(Mandatory=$true)][string]$project
)

function GetScriptDirectory {
    Split-Path $script:MyInvocation.MyCommand.Path    
}

function CreateFolderIfNotExists {
    param (
        [parameter(Mandatory=$true)]$target
    )
    If (Test-Path -Path $target -PathType Container)
        {Write-Host "$target already exists" -ForegroundColor Red}
    Else
        {New-Item -ItemType Directory -Force -Path $target}
}

function GetArchiveFileName {
    param (
        [parameter(Mandatory=$true)]$path
    )
    $baseName = (Get-ChildItem $project).BaseName
    $i = 0
    do {
        $date = Get-Date -Format "yyyy-MM-dd"
        $file = "$baseName.$date.$i.zip"
        $exists = Test-Path -Path "$path/$file"
        $i = $i + 1
    } while($exists)
    $file
}

$currentFolder = GetScriptDirectory
$outputFolder = "$currentFolder/out"
$archivesFolder = "$currentFolder/archives"
$archiveFileName = GetArchiveFileName $archivesFolder


CreateFolderIfNotExists $outputFolder
CreateFolderIfNotExists $archivesFolder

dotnet publish $project -c Release -r win8-x64 -o $outputFolder

Compress-Archive -Path $outputFolder/* -DestinationPath $archivesFolder/$archiveFileName

Remove-Item $outputFolder -Recurse -Force
