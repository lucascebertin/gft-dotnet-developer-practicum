// Usings
using System.Text.RegularExpressions;

// Arguments
var target = Argument<string>("target", "Default");

// Variables
var configuration = "Release";
var netCoreTarget = "netcoreapp1.1";

// Directories
var output = Directory("build");
var outputBinaries = output + Directory("binaries");
var outputPackages = output + Directory("packages");
var outputNuGet = output + Directory("nuget");

/*
/ TASK DEFINITIONS
*/

Task("Default")
    .IsDependentOn("Test");

Task("Compile")
    .Description("Builds all the projects in the solution")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var projects =
            GetFiles("./src/**/*.csproj");

        if (projects.Count == 0)
        {
            throw new CakeException("Unable to find any projects to build.");
        }

        foreach(var project in projects)
        {
            var content =
                System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                ArgumentCustomization = args => {
                    if (IsRunningOnUnix())
                    {
                        args.Append(string.Concat("-f ", netCoreTarget));
                    }

                    return args;
                },
                Configuration = configuration
            });
        }
    });

Task("Restore-NuGet-Packages")
    .Description("Restores NuGet packages for all projects")
    .Does(() =>
    {
        DotNetCoreRestore("./src/GFT.DeveloperPracticum.ConsoleApp.sln", new DotNetCoreRestoreSettings {
            ArgumentCustomization = args => {
                args.Append("--verbosity minimal");
                return args;
            }
        });
    });

Task("Test")
    .Description("Executes unit tests for all projects")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        var projects =
            GetFiles("./src/GFT.DeveloperPracticum.UnitTests/GFT.DeveloperPracticum.UnitTests.csproj");

        if (projects.Count == 0)
        {
            throw new CakeException("Unable to find any projects to test.");
        }

        foreach(var project in projects)
        {
            var content =
                System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            var settings = new ProcessSettings {
                Arguments = string.Concat("test --configuration ", configuration, " --no-build"),
                WorkingDirectory = project.GetDirectory()
            };

            if (IsRunningOnUnix())
            {
                settings.Arguments.Append(string.Concat("-framework ", netCoreTarget));
            }

            Information("Executing tests for " + project.GetFilename() + " with arguments: " + settings.Arguments.Render());

            if (StartProcess("dotnet", settings) != 0)
            {
                throw new CakeException("One or more tests failed during execution of: " + project.GetFilename());
            }
        }
    });

/*
/ RUN BUILD TARGET
*/

RunTarget(target);