NodaTime.Serialization.ServiceStackText
=======================================

[`ServiceStack.Text`](https://github.com/ServiceStack/ServiceStack.Text) JSON serialization support for [`NodaTime`](http://nodatime.org/). This library will work with `v3.9.44` and later versions of `v3.9.x`. It _should_ work with `v4.0.x` 

##Typical Setup
```cs 
DateTimeZoneProviders.Tzdb
  .CreateDefaultSerializersForNodaTime()
  .ConfigureSerializersForNodaTime();
```

##Optional Setup
There are static extension methods to allow for optional fluent configuration.

###Use Iso Interval Serializer
```cs 
DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime()
  .WithIsoIntervalSerializer()
  .ConfigureSerializersForNodaTime();
```

###Use Iso Period Serializer
```cs
DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime()
  .WithNormalizingIsoPeriodSerializer()
  .ConfigureSerializersForNodaTime();
```

###One-Off Serializer Setup
```cs 
//You could create you own serializer by implementing IServiceStackSerializer<T>
NodaSerializerDefinitions.LocalTimeSerializer.ConfigureSerializer();
```

Alternatively, you can create your own implementation of `INodaSerializerSettings` to suit your specific needs.

##Default Serializers
The `SerivceStack.Text` serializer uses the same default serialization formats as those used in the [`Json.NET` serializers](http://nodatime.org/1.2.x/userguide/serialization.html). You can optionally change the `Interval` serializer to use the [`ISO 8601` Interval spec](http://en.wikipedia.org/wiki/ISO_8601#Time_intervals).



##Notes
- Serializer setup should only occur once in your application root.
- Serialization setup should occur before any serialization occurs with `ServiceStack.Text` or there may be undesirable behavior.
- For Value types, this will also setup the nullable serializer for that value type.

##Using the Code

* [Install the NuGet Package](https://nuget.org/packages/NodaTime.Serialization.ServiceStackText)
* [NuGet Packages from Latest Successful Build](http://teamcity.codebetter.com/viewLog.html?buildId=lastSuccessful&buildTypeId=bt1209&tab=artifacts)
* You can check out the code and run `build.bat`. It will create NuGet packages you can consume in `.\ReleasePackages` or you can directly use the resulting binaries. 
* Build requirements
  * .Net 3.5
  * Powershell 2.0
 
  
##Continuous Integration
[CodeBetter.com CI Build Status:](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1209) ![Build Status](http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt1209)/statusIcon)

![CodeBetter CI](http://www.jetbrains.com/img/banners/Codebetter.png)

Special Thanks to [JetBrains](http://www.jetbrains.com/teamcity) and [CodeBetter](http://codebetter.com/codebetter-ci/) for hosting this project!
