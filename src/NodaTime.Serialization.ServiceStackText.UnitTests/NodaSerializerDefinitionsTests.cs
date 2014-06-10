using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime.TimeZones;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class NodaSerializerDefinitionsTests
    {
        [Fact]
        public void CreateZonedDateTimeSerializer_NullPattern_Throws()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NodaSerializerDefinitions.CreateZonedDateTimeSerializer(
                    new DateTimeZoneCache(TzdbDateTimeZoneSource.Default),
                    null));
        }
    }
}