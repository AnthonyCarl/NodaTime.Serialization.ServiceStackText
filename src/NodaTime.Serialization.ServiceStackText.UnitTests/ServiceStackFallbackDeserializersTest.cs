using System;
using System.Runtime.Serialization;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    public class ServiceStackFallbackDeserializersTest
    {
        [Fact]
        public void ToInstant_ValidText_Deserialize()
        {
            DeserializeAssert(
                ServiceStackFallbackDeserializers.ToInstant,
                "2014-2-21 19:02:13Z",
                Instant.FromDateTimeOffset(new DateTimeOffset(2014, 2, 21, 19, 2, 13, TimeSpan.Zero)));
        }

        [Fact]
        public void ToInstant_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToInstant);
        }

        [Fact]
        public void ToInstant_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToInstant);
        }

        [Fact]
        public void ToLocalTime_ValidText_Deserialize()
        {
            DeserializeAssert(
               ServiceStackFallbackDeserializers.ToLocalTime,
               "7:02:13",
               new LocalTime(7,2,13));
        }

        [Fact]
        public void ToLocalTime_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToLocalTime);
        }

        [Fact]
        public void ToLocalTime_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToLocalTime);
        }

        [Fact]
        public void ToLocalDate_ValidText_Deserialize()
        {
            DeserializeAssert(
               ServiceStackFallbackDeserializers.ToLocalDate,
               "2014-2-21",
               new LocalDate(2014,2,21));
        }

        [Fact]
        public void ToLocalDate_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToLocalDate);
        }

        [Fact]
        public void ToLocalDate_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToLocalDate);
        }

        [Fact]
        public void ToLocalDateTime_ValidText_Deserialize()
        {
            DeserializeAssert(
                ServiceStackFallbackDeserializers.ToLocalDateTime,
                "2014/2/21 19:02:13",
                new LocalDateTime(2014, 2, 21, 19, 2, 13));
        }

        [Fact]
        public void ToLocalDateTime_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToLocalDateTime);
        }

        [Fact]
        public void ToLocalDateTime_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToLocalDateTime);
        }

        [Fact]
        public void ToOffsetDateTime_ValidText_Deserialize()
        {
            DeserializeAssert(
                ServiceStackFallbackDeserializers.ToOffsetDateTime,
                "2014/2/21 19:02:13 +05:00",
                new OffsetDateTime(new LocalDateTime(2014, 2, 21, 19, 2, 13), Offset.FromHours(5)));
        }

        [Fact]
        public void ToOffsetDateTime_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToOffsetDateTime);
        }

        [Fact]
        public void ToOffsetDateTime_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToOffsetDateTime);
        }

        [Fact]
        public void ToZonedDateTime_ValidText_Deserialize()
        {
            DeserializeAssert(
                ServiceStackFallbackDeserializers.ToZonedDateTime,
                "2014/2/21 19:02:13 +05:00",
                ZonedDateTime.FromDateTimeOffset(new DateTimeOffset(2014, 2, 21, 19, 2, 13, TimeSpan.FromHours(5))));
        }

        [Fact]
        public void ToZonedDateTime_MalformedText_Throws()
        {
            MalformedTextAssert(ServiceStackFallbackDeserializers.ToZonedDateTime);
        }

        [Fact]
        public void ToZonedDateTime_NullText_Throws()
        {
            NullTextAssert(ServiceStackFallbackDeserializers.ToZonedDateTime);
        }

        private static void DeserializeAssert<T>(Func<string, T> fallbackSerializer, string text, T expected)
        {
            var actual = fallbackSerializer(text);
            Assert.Equal(expected, actual);
        }
        private static void MalformedTextAssert<T>(Func<string, T> fallbackSerializer)
        {
            Assert.Throws<FormatException>(() => fallbackSerializer("JustPlainWrong"));
        }

        private static void NullTextAssert<T>(Func<string, T> fallbackSerializer)
        {
            Assert.Throws<SerializationException>(() => fallbackSerializer(null));
        }
    }
}
