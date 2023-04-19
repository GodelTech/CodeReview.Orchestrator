# Commands to run analysis

```bash

dotnet new tool-manifest
dotnet tool install GodelTech.CodeReview.Orchestrator
dotnet codereview run -f {git-roslyn-converter.yaml / file_url}

```

# Docker Engines with Authentication
Example with Basic Authentication (engines.yaml)
```
engines:
  engine_name:
    os:
    url:
    workerImage:
    authType: Basic
    basicAuthCredentials:
        username:
        password:
```

Example with X509 Authentication (engines.yaml)
```
engines:
  engine_name:
    os:
    url:
    workerImage:
    authType: X509
    certificateCredentials:
        fileName:
        password:
```

Example without Authentication (engines.yaml)
```
engines:
  engine_name:
    os:
    url:
    workerImage:
    authType: None
```