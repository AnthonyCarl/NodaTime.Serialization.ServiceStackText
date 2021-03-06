#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2019.2.3"
#tool "nuget:?package=xunit.runner.console&version=2.4.1"
#addin nuget:?package=Cake.GitVersioning&version=3.0.28

var target = Argument("target", "Default");
var buildConfiguration = Argument("configuration", "Release");

Task("Default")
  .IsDependentOn("Pack")
  .Does(() =>
{
});

Task("SetVersion")
  .Does(() =>
{
  TeamCity.SetBuildNumber(GitVersioningGetVersion().SemVer2);
});

Task("Restore")
  .IsDependentOn("SetVersion")
  .Does(() =>
{
  var restoreSettings = new DotNetCoreRestoreSettings
     {
         Sources = new[] {"https://api.nuget.org/v3/index.json"}
     };
  DotNetCoreRestore("./src/NodaTime.Serialization.ServiceStackText.sln", restoreSettings);
});

Task("Build")
  .IsDependentOn("Restore")
  .Does(() => 
{
  var buildSettings = new DotNetCoreBuildSettings
     {
         Configuration = buildConfiguration
     };
  DotNetCoreBuild("./src/NodaTime.Serialization.ServiceStackText.sln", buildSettings);
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
  Action<ICakeContext, string> runTests = (ctx, framework) => { 
    ctx.DotNetCoreTest("./src/NodaTime.Serialization.ServiceStackText.UnitTests/NodaTime.Serialization.ServiceStackText.UnitTests.csproj", new DotNetCoreTestSettings 
        {
          Configuration = buildConfiguration,
          Framework = framework,
          NoBuild = true
        });
  };

  var coverSettings = new DotCoverCoverSettings()
    .WithFilter("-:xunit*")
    .WithFilter("-:NuGet*")
    .WithFilter("-:MSBuild*")
    .WithFilter("-:*Tests")
    .WithFilter("-:ServiceStack*")
    .WithFilter("-:NodaTime")
    .WithFilter("-:NodaTime.Testing");

  var coverageResult452 = new FilePath("./dotcover/dotcover452.data");
  DotCoverCover(
    ctx => runTests(ctx, "net452"), 
    coverageResult452, 
    coverSettings);

  var coverageResultCoreApp20 = new FilePath("./dotcover/dotcoverCoreApp20.data");
  DotCoverCover(
    ctx => runTests(ctx, "netcoreapp2.0"), 
    coverageResultCoreApp20, 
    coverSettings);

  var mergedData = new FilePath("./dotcover/dotcoverMerged.data");
  DotCoverMerge(
    new []{
      coverageResult452, 
      coverageResultCoreApp20
    }, 
    mergedData, 
    new DotCoverMergeSettings());

  if(TeamCity.IsRunningOnTeamCity) {
    TeamCity.ImportDotCoverCoverage(
      mergedData, 
      MakeAbsolute(Directory("./tools/JetBrains.dotCover.CommandLineTools.2019.2.3/tools"))
    );
  }
  else {
    var htmlReportFile = new FilePath("./dotcover/dotcover.html");
    var reportSettings = new DotCoverReportSettings { ReportType = DotCoverReportType.HTML};
    DotCoverReport(mergedData, htmlReportFile, reportSettings);
    StartProcess("powershell", "start file:///" + MakeAbsolute(htmlReportFile));
  }
});

Task("Pack")
  .IsDependentOn("Test")
  .Does(() =>
{
  var packSettings = new DotNetCorePackSettings
     {
         Configuration = buildConfiguration,
         OutputDirectory = "./ReleasePackages/",
         ArgumentCustomization = args => args.Append("--include-symbols")
                                             .Append("--include-source")
     };
  DotNetCorePack("./src/NodaTime.Serialization.ServiceStackText/NodaTime.Serialization.ServiceStackText.csproj", packSettings);
});

RunTarget(target);
