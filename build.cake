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
  Action<ICakeContext> runTests = ctx => { 
	ctx.XUnit2("./**/bin/" + buildConfiguration + "/*Tests.dll");
  };

  Action<DotCoverCoverageSettings> applySettings = settings => {
	settings        
	  .WithFilter("-:*Tests");
  };

  if(TeamCity.IsRunningOnTeamCity) {
    var coverageResult = new FilePath("./dotcover/dotcover.data");
    var coverSettings = new DotCoverCoverSettings();
    applySettings(coverSettings);
    DotCoverCover(runTests, coverageResult, coverSettings);
    TeamCity.ImportDotCoverCoverage(
      coverageResult, 
      MakeAbsolute(Directory("./tools/JetBrains.dotCover.CommandLineTools/tools"))
    );
  }
  else {
    var htmlReportFile = new FilePath("./dotcover/dotcover.html");
    var analyseSettings = new DotCoverAnalyseSettings { ReportType = DotCoverReportType.HTML};
    applySettings(analyseSettings);
    DotCoverAnalyse(runTests, htmlReportFile, analyseSettings);
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
