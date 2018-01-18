//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var Target = Argument("Target", "Build");
var Platform = Argument("Platform", "Linux");
var Configuration = Argument("Configuration", "Release");

var Project = "Psythyst.Core";
var Solution = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Configure-Solution").Does(() => 
{
    Solution = $"{Project}.{Platform}.sln";
});

Task("Restore-NuGet-Package")
    .IsDependentOn("Configure-Solution")
    .IsDependentOn("Generate-Protobuild-Project")
    .Does(() =>
{
    if(FileExists(Solution)) 
        NuGetRestore(Solution, new NuGetRestoreSettings { ToolPath = "./Tools/nuget.exe" });
});

Task("Copy-Output").Does(() => 
{
    Func<IFileSystemInfo, bool> Exclude_OutputDirectory =
     FileSystemInfo => !FileSystemInfo.Path.FullPath.EndsWith(
         "Bin", StringComparison.OrdinalIgnoreCase);

    var OutputDirectory = $"./Bin/{Platform}/AnyCPU/{Configuration}";
    var DllCollection = GetFiles($"./**/{Platform}/AnyCPU/{Configuration}/*.dll", Exclude_OutputDirectory);
    
    if (!DirectoryExists(OutputDirectory)) 
        CreateDirectory(OutputDirectory); 
    else 
        CleanDirectory(OutputDirectory);

    CopyFiles(DllCollection, OutputDirectory);
});

Task("Build")
    .IsDependentOn("Configure-Solution")
    .Does(() =>
{
    if(FileExists(Solution)) 
        MSBuild(Solution, Settings => Settings.SetConfiguration(Configuration));
});

Task("Clean-Project-Folder")
    .IsDependentOn("Configure-Solution")
    .Does(() => 
{
    var Settings = new DeleteDirectorySettings { Recursive = true, Force = true };

    var PublishCollection = GetDirectories("./**/Publish");
    var PackageCollection = GetDirectories("./**/Package");

    DeleteDirectories(PublishCollection, Settings);
    DeleteDirectories(PackageCollection, Settings);

    var ObjCollection = GetDirectories("./**/obj");
    var BinCollection = GetDirectories("./**/bin");
    
    DeleteDirectories(ObjCollection, Settings);
    DeleteDirectories(BinCollection, Settings);

    if (DirectoryExists($"./Bin/{Platform}"))
        DeleteDirectory($"./Bin/{Platform}", Settings);
});

Task("Clean-Project")
    .IsDependentOn("Clean-Protobuild-Project")
    .IsDependentOn("Clean-Project-Folder");

//////////////////////////////////////////////////////////////////////
// PROTOBUILD TASKS
//////////////////////////////////////////////////////////////////////

Task("Generate-Protobuild-Project")
    .IsDependentOn("Configure-Solution")
    .Does(() => 
{
    StartProcess("./Protobuild.exe", new ProcessSettings{ Arguments = $"--generate {Platform}" });
});

Task("Clean-Protobuild-Project")
    .IsDependentOn("Configure-Solution")
    .Does(() => 
{
    //StartProcess("./Protobuild.exe", new ProcessSettings{ Arguments = $"--clean {Platform}" });

    var ProjectCollection = GetFiles($"./**/*.{Platform}.csproj");
    var SolutionCollection = GetFiles($"./**/*.{Platform}.sln");
    var SpeccacheCollection = GetFiles($"*.{Platform}.speccache");

    DeleteFiles(ProjectCollection);
    DeleteFiles(SolutionCollection);
    DeleteFiles(SpeccacheCollection);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Generate-Protobuild-Project")
    .IsDependentOn("Restore-NuGet-Package")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Output");

Task("Quick-Build")
    .IsDependentOn("Build")
    .IsDependentOn("Copy-Output");

Task("Clean").IsDependentOn("Clean-Project");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(Target);