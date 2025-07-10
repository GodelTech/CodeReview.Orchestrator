#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app
COPY ["CodeReview.Orchestrator/*.csproj", "CodeReview.Orchestrator/"]
RUN dotnet restore "CodeReview.Orchestrator/CodeReview.Orchestrator.csproj" -r linux-musl-x64 --force

COPY CodeReview.Orchestrator/. ./CodeReview.Orchestrator/
RUN dotnet publish ./CodeReview.Orchestrator/CodeReview.Orchestrator.csproj -c Release -r linux-musl-x64 -o ./CodeReview.Orchestrator/out /p:PublishTrimmed=true

FROM mcr.microsoft.com/dotnet/runtime-deps:5.0-alpine3.15 AS runtime

WORKDIR /app
COPY --from=build /app/CodeReview.Orchestrator/out .
ENTRYPOINT ["./CodeReview.Orchestrator"]
CMD ["--help"]