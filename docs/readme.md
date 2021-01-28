# Overview


# Ideas
1. All analyzers are executed in parallel
2. All reporters are executed in parallel. Source code and results of analysis are provided in read-only format.
3. `busybox` image can be used to execute trivial commands for mounted containers (copy\delete files).
4. File server (FTP? HTTP?) can be used to upload files inside container. This option shoudl be Kubernetes-friendly ant it's not bound to local computer. Access credentials can be passed as parameters when container is started. Volumes which needs to be initialized are attached to this container. Here is [example](https://hub.docker.com/r/delfer/alpine-ftp-server) or (example2)[https://hub.docker.com/r/bogem/ftp] of possible option.


# Commands

Installing git in Alpine docker image

```bash
apk update
apk add git
```

Cloning repository without git prompt:

# Workflow

1. Create source code volume
2. Run source code provider
3. For each analyzer do the following:
   1. Copy source code from source code volume
   2. Create artifacts volume
   3. Run analyzer
   4. Remove source code volume
4. Create analysis result volume
5. For each analyzer do the following
   1. Copy artifacts from analyzer volume to results volume
   2. Remove artifacts volume
3. For each report builder do the following:
   1. Mount sources as read-only
   2. Mount results as read-only
   3. Create report result volume
   4. Run report container
4. Create report result volume
5. For each reporter
   1. Copy reports to reports result volume
   2. Remove report volume
6. Mount source code
7. Mount analysis results
8. Mount report results
9. Run result submitter container
10. Remove all containers

# Links

* Command line docker tool https://github.com/estesp/mquery/blob/master/mtool-query-function/Dockerfile


# ReSharper Categories

* Potential Code Quality Issues (155 diagnostics)
* Spelling issues (3 diagnostics)
* Compiler Warnings (125 inspections)
  * Async \ await issues
  * new keyword

# SonarQube 

* Vulnerability
* Bug
* Security Hotspot

# Code Cracker

Almost all analyzers