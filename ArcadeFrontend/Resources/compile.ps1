$fileNames = Get-ChildItem -Path $pwd -Recurse

foreach ($file in $fileNames)
{
    if ($file.Name.EndsWith("vert") -Or $file.Name.EndsWith("frag") -Or $file.Name.EndsWith("comp"))
    {
        Write-Host "Compiling $file"
        glslangvalidator -V $file -o $file".spv"
        $sourcePath = Join-Path -Path $pwd -ChildPath $file".spv"
        $destPath = Join-Path -Path $pwd -ChildPath "..\Content\shader\"
        $destFile = Join-Path -Path $destPath -ChildPath $file".spv"
        Write-Host "Deleting $destFile"
        Remove-Item -Path $destFile
        Write-Host "Moving $sourcePath to $destFile"
        Move-Item -Path $sourcePath -Destination $destFile
    }
}