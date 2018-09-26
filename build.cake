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
    "./src/NetCoreKit.Infrastructure/NetCoreKit.Infrastructure.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore/NetCoreKit.Infrastructure.AspNetCore.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.CleanArch/NetCoreKit.Infrastructure.AspNetCore.CleanArch.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.Miniservice/NetCoreKit.Infrastructure.AspNetCore.Miniservice.csproj",
    "./src/NetCoreKit.Infrastructure.AspNetCore.OpenApi/NetCoreKit.Infrastructure.AspNetCore.OpenApi.csproj",
    "./src/NetCoreKit.Infrastructure.EfCore/NetCoreKit.Infrastructure.EfCore.csproj",
    "./src/NetCoreKit.Infrastructure.EfCore.SqlServer/NetCoreKit.Infrastructure.EfCore.SqlServer.csproj",
    "./src/NetCoreKit.Infrastructure.EfCore.MySql/NetCoreKit.Infrastructure.EfCore.MySql.csproj",
    "./src/NetCoreKit.Infrastructure.Bus/NetCoreKit.Infrastructure.Bus.csproj",
    "./src/NetCoreKit.Infrastructure.Bus.Kafka/NetCoreKit.Infrastructure.Bus.Kafka.csproj",
    "./src/NetCoreKit.Infrastructure.Bus.Redis/NetCoreKit.Infrastructure.Bus.Redis.csproj"
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
        DotNetCoreRestore("./netcorekit.sln", new DotNetCoreRestoreSettings{
            Verbosity = DotNetCoreVerbosity.Minimal,
        });
    });

GitVersion versionInfo = null;
DotNetCoreMSBuildSettings msBuildSettings = null;

Task("UpdateVersionInfo")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var tag = AppVeyor.Environment.Repository.Tag.Name;
        AppVeyor.UpdateBuildVersion(tag);
    });

Task("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = false,
            OutputType = GitVersionOutput.BuildServer
        });

        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });

        Information(versionInfo);

        msBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", versionInfo.Major + "." + versionInfo.Minor + "." + versionInfo.BuildMetaData)
            .WithProperty("AssemblyVersion", versionInfo.AssemblySemVer)
            .WithProperty("FileVersion", versionInfo.AssemblySemVer);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Version")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild("./netcorekit.sln", new DotNetCoreBuildSettings()
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
                        Source = "https://www.nuget.org/api/v2/package"
                });
            }
        }
        
    });

Task("Default")
    .IsDependentOn("DeployNuget");

RunTarget(target);
