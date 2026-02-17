
<p align="center">
    <img src="https://github.com/user-attachments/assets/b381a38a-9fa3-4622-ba3e-95d356525d01" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/SQLComp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/SQLComp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/SQLComp)
![Nuget](https://img.shields.io/nuget/dt/SQLComp)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/kris701/SQLComp/main)
![GitHub commit activity (branch)](https://img.shields.io/github/commit-activity/m/kris701/SQLComp)
![Static Badge](https://img.shields.io/badge/Platform-Windows-blue)
![Static Badge](https://img.shields.io/badge/Platform-Linux-blue)
![Static Badge](https://img.shields.io/badge/Framework-dotnet--10.0-green)

# SQLComp
This is a simple little dotnet tool to compare tables between two SQL Server databases.

```
sqlcomp [-t|--target <PATH>] [-o|--output] [-f|--force]
```
* `-t|--target` the target comparison file to use.
* `-o|--output` the output SQL patch file.
* `-f|--force` if the program should delete an existing patch file.

This can be found as a package on the [NuGet Package Manager](https://www.nuget.org/packages/SQLComp/) or be installed by the command:
```
dotnet tool install SQLComp
```