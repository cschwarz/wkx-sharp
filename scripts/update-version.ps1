$version = ""

if($env:APPVEYOR_REPO_TAG -eq 'True') {
	$version = $env:APPVEYOR_REPO_TAG_NAME   
}
else {
	$versionFile = "$env:APPVEYOR_BUILD_FOLDER\version.txt"
	$version = [IO.File]::ReadAllText($versionFile)
	$version = "$version-ci$env:APPVEYOR_BUILD_NUMBER"	
}

Add-AppveyorMessage -Message "Version: $version"
Update-AppveyorBuild -Version $version
