using System;
using NodaTime.Testing;
using NodaTime.Text;
using NodaTime.Utility;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    public class ToStringSerializerTests
    {
        [Fact]
        public void Serialize()
        {
            var clock = FakeClock.FromUtc(2014, 05, 02, 10, 30, 45);
            var now = clock.Now.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
            var serialisers = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb)
                .WithToStringZonedDateTimeSerializer();

            var expected = now.ToString();
            var actual = serialisers.ZonedDateTimeSerializer.Serialize(now);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Deserialize()
        {
            var clock = FakeClock.FromUtc(2014, 05, 02, 10, 30, 45);
            var now = clock.Now.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
            var serialisers = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb)
                .WithToStringZonedDateTimeSerializer();

            var expected = now;
            var actual = serialisers.ZonedDateTimeSerializer.Deserialize(now.ToString());

            Assert.Equal(expected, actual);
        }
    }
}
