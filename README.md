# CodeReview.Orchestrator
Code review orchestration tool

```bash

dotnet pack CodeReview.Orchestrator.sln
dotnet nuget push GodelTech.CodeReview.Orchestrator.0.0.1.nupkg -k XXXXX -s https://api.nuget.org/v3/index.json

```

Docker

```bash

docker build -t godeltech/codereview.orchestrator:0.0.1 -f src/CodeReview.Orchestrator/Dockerfile ./src
docker image tag godeltech/codereview.orchestrator:0.0.1 godeltech/codereview.orchestrator:latest
docker push godeltech/codereview.orchestrator:latest
docker push godeltech/codereview.orchestrator:0.0.1



docker run -v "/var/run/docker.sock:/var/run/docker.sock" -v "/mnt/d/_Tests/temp/manifests/manifest.yaml:/app/manifest.yaml" -v "/mnt/d/_Tests/temp/artifacts:/app/artifacts"   --rm godeltech/codereview.orchestrator run -f manifest.yaml

```