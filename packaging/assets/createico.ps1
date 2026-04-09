$imgMagick = "& magick.exe"
$env:Path = $env:Path + ';C:\Program Files\ImageMagick-7.0.9-Q16'
$image = "icon.svg"

Invoke-Expression "$imgMagick convert -transparent white -background None -density 384 $image -define icon:auto-resize icon.ico"
Invoke-Expression "$imgMagick convert -transparent white -background None -density 384 editor.svg -define icon:auto-resize icon-editor.ico"
