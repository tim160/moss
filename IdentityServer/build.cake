//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var configurations = new List<string>() {
    "Debug",
    "Release"
};

var localBuildNumber = 9.ToString();
var target = Argument("target", "Default");
var configuration = Argument("configuration", configurations[1]);
var buildNumber = Argument("buildNumber", localBuildNumber); 

var project = "IdentityServer.csproj";
var solution = "../EC.sln";
var pathToArtifacts = MakeAbsolute(Directory("../../")).Combine("Artifacts");
var artifactsDir = string.Format("{0}/{1}", pathToArtifacts, buildNumber);
var buildDir = Directory("./bin");
var objDir = Directory("./obj");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(objDir);
    CleanDirectory(artifactsDir);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{   
    NuGetRestore(solution, new NuGetRestoreSettings { NoCache = true });
});

Task("BuildAndPublish")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild(
        project, 
        new MSBuildSettings().
        SetVerbosity(Verbosity.Minimal).
        SetConfiguration(configuration).
        SetPlatformTarget(PlatformTarget.MSIL).
        WithTarget("Build").
        WithProperty("DeployOnBuild", "true").
        WithProperty("DeployDefaultTarget", "WebPublish").
        WithProperty("WebPublishMethod", "FileSystem").
        WithProperty("DeleteExistingFiles", "false").
        WithProperty("ProfileTransformWebConfigEnabled", "true").
        WithProperty("PublishUrl", artifactsDir));
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("BuildAndPublish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
