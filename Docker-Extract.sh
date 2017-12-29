mkdir -p ./Publish

# TODO: Replace all those docker runs... (so slow) X_X

# Extract Psythyst
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst/Psythyst/Publish/Psythyst.dll > ./Publish/Psythyst.dll
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst/Psythyst/Publish/Psythyst.deps.json > ./Publish/Psythyst.deps.json
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst/Psythyst/Publish/Psythyst.pdb > ./Publish/Psythyst.pdb

# Extract Psythyst Core
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.dll > ./Publish/Psythyst.Core.dll
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.deps.json > ./Publish/Psythyst.Core.deps.json
docker run --rm -it psythyst/psythyst-core:latest cat /Psythyst.Core/Publish/Psythyst.Core.pdb > ./Publish/Psythyst.Core.pdb

