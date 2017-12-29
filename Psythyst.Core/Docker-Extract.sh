mkdir -p ./Publish

docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.dll > ./Publish/Psythyst.Core.dll
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.deps.json > ./Publish/Psythyst.Core.deps.json
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.pdb > ./Publish/Psythyst.Core.pdb