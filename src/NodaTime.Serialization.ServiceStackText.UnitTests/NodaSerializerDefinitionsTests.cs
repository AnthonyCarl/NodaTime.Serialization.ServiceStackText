using System;
using NodaTime.TimeZones;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
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