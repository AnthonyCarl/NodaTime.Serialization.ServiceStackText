#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"
#tool "nuget:?package=xunit.runner.console"

var target = Argument("target", "Default");
var buildConfiguration = Argument("configuration", "Release");
var majorMinorVersion = "2.0";
var baseDate = new DateTime(2017,04,25,0,0,0,0,DateTimeKind.Utc);
var elapsedSinceBaseDate = DateTime.UtcNow - baseDate;
var days = (int)elapsedSinceBaseDate.TotalDays;
var revision = (int)(elapsedSinceBaseDate.Subtract(TimeSpan.FromDays(days)).TotalSeconds / 1.5);
var version = majorMinorVersion + "." + days + "." + revision;

Task("Default")
  .IsDependentOn("Pack")
  .Does(() =>
{
});

Task("SetVersion")
  .Does(() =>
{
  TeamCity.SetBuildNumber(version);
  XmlPoke(
    "./src/NodaTime.Serialization.ServiceStackText/NodaTime.Serialization.ServiceStackText.csproj", 
    "/Project/PropertyGroup/Version",
    version
  );
});

Task("Restore")
  .IsDependentOn("SetVersion")
  .Does(() =>
{
  var restoreSettings = new DotNetCoreRestoreSettings
     {
         Sources = new[] {"https://api.nuget.org/v3/index.json"}
     };
  DotNetCoreRestore("./src", restoreSettings);
});

Task("Build")
  .IsDependentOn("Restore")
  .Does(() => 
{
  var buildSettings = new DotNetCoreBuildSettings
     {
         Configuration = buildConfiguration
     };
  DotNetCoreBuild("./src", buildSettings);
});

Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
  CopyDirectory("./src/NodaTime.Serialization.ServiceStackText.UnitTests/bin/" + buildConfiguration +"/net452", "./SsV4Test");
  DeleteFile("./SsV4Test/ServiceStack.Text.dll");
  CopyFile("./ServiceStack.Text.4/lib/net40/ServiceStack.Text.dll", "./SsV4Test/ServiceStack.Text.dll");

  Action<ICakeContext, string> runTests = (ctx, framework) => { 
    ctx.DotNetCoreTest("./src/NodaTime.Serialization.ServiceStackText.UnitTests/NodaTime.Serialization.ServiceStackText.UnitTests.csproj", new DotNetCoreTestSettings 
        {
          Configuration = buildConfiguration,
          Framework = framework,
          NoBuild = true
        });
  };

  var coverSettings = new DotCoverCoverSettings()
    .WithFilter("-:*Tests")
    .WithFilter("-:ServiceStack*");

  var coverageResultSsV4 = new FilePath("./dotcover/dotcoverSsV4.data");
  DotCoverCover(
    ctx => ctx.XUnit2("./SsV4Test/NodaTime.Serialization.ServiceStackText.UnitTests.dll"), 
    coverageResultSsV4, 
    coverSettings);

  var coverageResult452 = new FilePath("./dotcover/dotcover452.data");
  DotCoverCover(
    ctx => runTests(ctx, "net452"), 
    coverageResult452, 
    coverSettings);

  var coverageResultCoreApp = new FilePath("./dotcover/dotcoverCoreApp.data");
  DotCoverCover(
    ctx => runTests(ctx, "netcoreapp1.1"), 
    coverageResultCoreApp, 
    coverSettings);

  var mergedData = new FilePath("./dotcover/dotcoverMerged.data");
  DotCoverMerge(
    new []{
      coverageResultSsV4, 
      coverageResult452, 
      coverageResultCoreApp
    }, 
    mergedData, 
    new DotCoverMergeSettings());

  if(TeamCity.IsRunningOnTeamCity) {
    TeamCity.ImportDotCoverCoverage(
      mergedData, 
      MakeAbsolute(Directory("./tools/JetBrains.dotCover.CommandLineTools/tools"))
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
         OutputDirectory = "./ReleasePackages/"
     };
  DotNetCorePack("./src/NodaTime.Serialization.ServiceStackText/NodaTime.Serialization.ServiceStackText.csproj", packSettings);
});

RunTarget(target);
