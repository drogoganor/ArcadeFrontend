$imgMagick = "& magick.exe"
$env:Path = $env:Path + ';C:\Program Files\ImageMagick-7.0.9-Q16'

$fileNames = Get-ChildItem -Path $pwd -Recurse

$outPath = "out"

if (-not (Test-Path $outPath -PathType Container)) {
    New-Item -Name %outPath -ItemType Directory
}

foreach ($file in $fileNames)
{
    if ($file.Name.EndsWith("svg"))
    {
        Write-Host "Turning SVG $file into PNG"
        $outFile = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
        Remove-Item -Path out\$outFile.png
		Invoke-Expression "$imgMagick convert -transparent white -background None -density 1024 $file -resize 96x96 out\$outFile.png"
    }
}