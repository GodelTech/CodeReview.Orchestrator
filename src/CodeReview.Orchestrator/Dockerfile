#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /app
COPY ["CodeReview.Orchestrator/*.csproj", "CodeReview.Orchestrator/"]
RUN dotnet restore "CodeReview.Orchestrator/CodeReview.Orchestrator.csproj" --force

COPY CodeReview.Orchestrator/. ./CodeReview.Orchestrator/
RUN dotnet publish ./CodeReview.Orchestrator/CodeReview.Orchestrator.csproj -c Release -o ./CodeReview.Orchestrator/out

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS runtime


WORKDIR /app
COPY --from=build /app/CodeReview.Orchestrator/out .
ENTRYPOINT ["dotnet", "CodeReview.Orchestrator.dll"]
CMD ["--help"]