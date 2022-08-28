#!/bin/bash

dotnet build src/UnityExplorer.sln -c Release_STANDALONE_Mono
if [ $? -ne 0 ]; then
	exit 128
fi
Path="Release/UnityExplorer.Standalone.Mono"
mono lib/ILRepack.exe /target:library /lib:lib/net35 /lib:$Path /internalize /out:$Path/UnityExplorer.Standalone.Mono.dll $Path/UnityExplorer.STANDALONE.Mono.dll $Path/mcs.dll $Path/Tomlet.dll
rm $Path/UnityExplorer.STANDALONE.Mono.dll
rm $Path/Tomlet.dll
rm $Path/mcs.dll
