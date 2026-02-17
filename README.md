
<p align="center">
    <img src="https://github.com/user-attachments/assets/b381a38a-9fa3-4622-ba3e-95d356525d01" width="200" height="200" />
</p>

[![Build and Publish](https://github.com/kris701/SQLComp/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/kris701/SQLComp/actions/workflows/dotnet-desktop.yml)
![Nuget](https://img.shields.io/nuget/v/SQLComp.CLI)
![Nuget](https://img.shields.io/nuget/dt/SQLComp.CLI)
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
* `-c|--check` do the run with a `TOP(10)` addition to the query, so you can check if the input file is valid
* `-p|--patchreg` a set of regex replacements to perform to the SQL patch file.
    * It is in the format "MATCH;;;REPLACEMENT". Where the first regex is used for capturing data, and the other regex supports [substitutions](https://learn.microsoft.com/en-us/dotnet/standard/base-types/substitutions-in-regular-expressions).

This can be found as a package on the [NuGet Package Manager](https://www.nuget.org/packages/SQLComp.CLI/) or be installed by the command:
```
dotnet tool install SQLComp.CLI
```
