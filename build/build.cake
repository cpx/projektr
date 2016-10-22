#tool "nuget:?package=xunit.runner.console"

const string NuGetRestoreTask = "NuGet restore";
const string BuildTask = "Build";
const string UnitTestTask = "Unit test";
const string NuGetPackTask = "NuGet pack";
const string NuGetPushTask = "NuGet push";

var buildConfiguration = Argument("buildConfiguration", "Release");
var target = Argument("target", "Default");
var version = Argument("version", "0.1.0");
var nuGetApiFeed = Argument("nuGetApiFeed", "");
var nuGetApiKey = Argument("nuGetApiKey", "");

var solutionFile = "../Projektr.sln";
//var nuspec = new[]{ }; //,"../src/Projektr.WebApi/Projektr.WebApi.nuspec"};

Task(NuGetRestoreTask)
	.Does(() => {
		NuGetRestore(solutionFile);
	});
Task(BuildTask)
	.IsDependentOn(NuGetRestoreTask)
	.Does(() => {
		MSBuild(solutionFile, configurator =>
			configurator.SetConfiguration(buildConfiguration)
			);
	});
Task(UnitTestTask)
	.IsDependentOn(BuildTask)
	.Does(() => {
		XUnit2("../tests/Projektr.Tests/bin/Projektr.Tests.dll");
	});
Task(NuGetPackTask)
	.IsDependentOn(BuildTask)
	.Does(() => {
		CreateDirectory("../out/");
		NuGetPack("../src/Projektr/Projektr.nuspec", new NuGetPackSettings{
			OutputDirectory = "../out/",
			Version = version
		});
		NuGetPack("../src/Projektr.WebApi/Projektr.WebApi.nuspec", new NuGetPackSettings{
			OutputDirectory = "../out/",
			Version = version
		});
	});
Task(NuGetPushTask)
	.IsDependentOn(NuGetPackTask)
	.Does(() => {
		var packages = GetFiles("../out/*.nupkg");
		NuGetPush(packages, new NuGetPushSettings {
			Source = nuGetApiFeed,
			ApiKey = nuGetApiKey
		});
	});

Task("Default")
	.IsDependentOn(BuildTask)
	.IsDependentOn(UnitTestTask);
Task("Release")
	.IsDependentOn(NuGetPushTask);
RunTarget(target);
