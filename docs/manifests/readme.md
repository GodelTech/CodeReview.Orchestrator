# Commands to run analysis

```bash

dotnet new tool-manifest
dotnet tool install GodelTech.CodeReview.Orchestrator
dotnet codereview run -f {git-roslyn-converter.yaml / file_url}

```