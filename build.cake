#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var outputDir = "./artifacts/";
var isAppVeyor = BuildSystem.IsRunningOnAppVeyor;
var isWindows = IsRunningOnWindows();

// libs come here
var libs = new List<string>{
    "./src/NetCoreKit.Utils/NetCoreKit.Utils.csproj",
    "./src/NetCoreKit.Domain/NetCoreKit.Domain.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore/NetCoreKit.Infrastructure.AspNetCore.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.CleanArch/NetCoreKit.Infrastructure.AspNetCore.CleanArch.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.Miniservice/NetCoreKit.Infrastructure.AspNetCore.Miniservice.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.OpenApi/NetCoreKit.Infrastructure.AspNetCore.OpenApi.csproj",
    "./src/NetCoreKit.Infrastructure.EfCore/NetCoreKit.Infrastructure.EfCore.csproj",
    "./src/NetCoreKit.Infrastructure.EfCore.SqlServer/NetCoreKit.Infrastructure.EfCore.SqlServer.csproj"
};

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
    });

Task("Restore")
    .Does(() => {
        DotNetCoreRestore("./netcore-kit.sln", new DotNetCoreRestoreSettings{
            Verbosity = DotNetCoreVerbosity.Minimal,
        });
    });

GitVersion versionInfo = null;
DotNetCoreMSBuildSettings msBuildSettings = null;

Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });

        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
        
        msBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", versionInfo.FullSemVer)
            .WithProperty("AssemblyVersion", versionInfo.FullSemVer)
            .WithProperty("FileVersion", versionInfo.FullSemVer);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild("./netcore-kit.sln", new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            MSBuildSettings = msBuildSettings
        });
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        foreach (var lib in libs)
            DotNetCorePack(lib, new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory = outputDir,
                MSBuildSettings = msBuildSettings
            });

        if (!isWindows) return;

        if (isAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("DeployNuget")
    .IsDependentOn("Package")
    .Does(() =>
    {
        if (isAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*.nupkg")) {
                Information(file.ToString());
                NuGetPush(
                    file,
                    new NuGetPushSettings {
                        ApiKey = EnvironmentVariable("NuGetApiKey"),
                        Source = "https://api.nuget.org/v3/index.json"
                });
            }
        }
        
    });

Task("Default")
    .IsDependentOn("DeployNuget");

RunTarget(target);
