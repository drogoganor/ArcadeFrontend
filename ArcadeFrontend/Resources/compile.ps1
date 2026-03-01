$fileNames = Get-ChildItem -Path $pwd -Recurse

$compiledDirectory = "Compiled"

cls

if (-not (Test-Path -PathType Container -Path $compiledDirectory)) 
{
    Write-Host "Creating compile directory '$compiledDirectory'"
    New-Item -ItemType Directory -Path $compiledDirectory
}

$vertexSuffix = ".vert";
$fragmentSuffix = ".frag";

foreach ($file in $fileNames)
{
    $isVertex = $file.Name.EndsWith($vertexSuffix);
    $isFragment = $file.Name.EndsWith($fragmentSuffix);
    
    if ($isVertex -Or $isFragment)
    {
        Write-Host "Compiling $file"
        
        $baseFileName = "$file".Replace("$vertexSuffix", "").Replace("$fragmentSuffix", "");

        glslangvalidator -V "$file" -o "$compiledDirectory/$file.spv"
        #glslangvalidator -V "$file" -e "fragment_main" -o "Compiled/$file.frag"

        #shadercross "$file" -e "vertex_main" -t vertex -s GLSL -o "Compiled/$filename_test.msl"

        $target = "fragment";
        if ($isVertex)
        {
            $target = "vertex";
        }
        
        #shadercross "$compiledDirectory/$file.spv" -e "main" -t $target -s SPIRV -o "$compiledDirectory/$file.msl"
    }
}