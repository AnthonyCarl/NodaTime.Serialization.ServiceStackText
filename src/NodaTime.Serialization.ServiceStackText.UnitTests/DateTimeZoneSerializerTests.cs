using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime.TimeZones;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class DateTimeZoneSerializerTests
    {
        private readonly IServiceStackSerializer<DateTimeZone> serializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(DateTimeZoneProviders.Tzdb);

        [Fact]
        public void Serialize()
        {
            var dateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var json = serializer.Serialize(dateTimeZone);
            string expectedJson = "America/Los_Angeles";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize()
        {
            string json = "America/Los_Angeles";
            var dateTimeZone = serializer.Deserialize(json);
            var expectedDateTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            Assert.Equal(expectedDateTimeZone, dateTimeZone);
        }

        [Fact]
        public void Deserialize_TimeZoneNotFound()
        {
            string json = "America/DOES_NOT_EXIST";
            Assert.Throws<DateTimeZoneNotFoundException>(() => serializer.Deserialize(json));
        }

        [Fact]
        public void Constructor_NullProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new DateTimeZoneSerializer(null));
        }

        [Fact]
        public void Serialize_NullDateTimeZone_NullString()
        {
            var serializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(DateTimeZoneProviders.Tzdb);
            Assert.Null(serializer.Serialize(null));
        }

        [Fact]
        public void Deserialize_WrongCaseForId_ReturnsCorrectDateTimeZone()
        {
            var serializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(DateTimeZoneProviders.Tzdb);
            var expectedTimeZone = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
            var actualTimeZone = serializer.Deserialize("amerICa/LOS_angELes");
            Assert.Equal(expectedTimeZone,actualTimeZone);
        }

        [Fact]
        public void Deserialize_EmptyId_Throws()
        {
            var serializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(DateTimeZoneProviders.Tzdb);
            Assert.Throws<DateTimeZoneNotFoundException>(() => serializer.Deserialize(string.Empty));
        }

        [Fact]
        public void UseRawSerializer_Default_False()
        {
            var serializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(DateTimeZoneProviders.Tzdb);
            Assert.False(serializer.UseRawSerializer);
        }
    }
}
