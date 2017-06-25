# GFT - .NET Developer Practicum

The objective of this project is allow the evaluation of the .net developer practicum

## Getting Started

### Prerequisites

Powershell to run the build.ps1

```
PS C:\Path\To\The\Project> build.ps1 --Target=Compile
```

### Running the code

After the build process described before

```
cd .\src\GFT.DeveloperPracticum.ConsoleApp\bin\Release\netcoreapp1.1
& dotnet GFT.DeveloperPracticum.ConsoleApp.dll "morning, 1, 2, 3"
eggs, toast, coffee
```
## Running the tests

Running the build.ps1 with Test target, the packages will be restored, the project will be compiled and the tests will run.

```
PS C:\Path\To\The\Project> build.ps1 --Target=Test
```

## Built With

* [DryIoc](https://bitbucket.org/dadhi/dryioc) - Dependency injection Container
* [ElemarJR.FunctionalCSharp](https://github.com/ElemarJR/ElemarJR.FunctionalCSharp) - Library to enable functional language features on C#
* [XUnit](http://xunit.github.io/docs/getting-started-dotnet-core) - Library for Unit test
* [FakeItEasy](https://fakeiteasy.github.io/) - Library for mocking
* [FluentAssertions](http://fluentassertions.com/) - Library for assert
