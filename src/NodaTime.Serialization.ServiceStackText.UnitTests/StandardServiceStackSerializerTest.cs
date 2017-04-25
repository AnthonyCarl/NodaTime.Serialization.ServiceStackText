using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime.Text;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class StandardServiceStackSerializerTests
    {
        [Fact]
        public void Constructor_NullPattern_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StandardServiceStackSerializer<int>(null));
        }

        [Fact]
        public void OffsetConverter()
        {
            var value = Offset.FromHoursAndMinutes(5, 30);
            string json = "+05:30";
            AssertConversions(value, json, NodaSerializerDefinitions.OffsetSerializer);
        }

        [Fact]
        public void InstantConverter()
        {
            var value = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            string json = "2012-01-02T03:04:05Z";
            AssertConversions(value, json,NodaSerializerDefinitions.InstantSerializer);
        }
        
        [Fact]
        public void LocalDateConverter()
        {
            var value = new LocalDate(2012, 1, 2, CalendarSystem.Iso);
            string json = "2012-01-02";
            AssertConversions(value, json, NodaSerializerDefinitions.LocalDateSerializer);
        }

        [Fact]
        public void LocalDateConverter_SerializeNonIso_Throws()
        {
            var localDate = new LocalDate(2012, 1, 2, CalendarSystem.Coptic);//.GetCopticCalendar(4));

            Assert.Throws<ArgumentException>(() => NodaSerializerDefinitions.LocalDateSerializer.Serialize(localDate));
        }

        [Fact]
        public void LocalDateTimeConverter()
        {
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6, CalendarSystem.Iso);
            var json = "2012-01-02T03:04:05.006";
            AssertConversions(value, json,NodaSerializerDefinitions.LocalDateTimeSerializer);
        }

        [Fact]
        public void Deserialize_NoFallback_Throws()
        {
            var serviceStackSerializer = new StandardServiceStackSerializer<LocalDate>(LocalDatePattern.Iso);
            Assert.Throws<UnparsableValueException>(() => serviceStackSerializer.Deserialize("Invalid"));
        }

        [Fact]
        public void Deserialize_PoorlyFormedText_Deserialized()
        {
            var localDate = NodaSerializerDefinitions.LocalDateSerializer.Deserialize("2014/1/2");
            var expectedLocalDate = new LocalDate(2014, 1, 2);
            Assert.Equal(expectedLocalDate, localDate);
        }

        [Fact]
        public void Deserialize_NoFallbackPoorlyFormedText_Throws()
        {
            var serializer = new StandardServiceStackSerializer<LocalDate>(LocalDatePattern.Iso);
            Assert.Throws<UnparsableValueException>(() => serializer.Deserialize("2014/1/2"));
        }

        [Fact]
        public void UseRawSerializer_StandardSerializer_False()
        {
            var serializer = new StandardServiceStackSerializer<LocalDate>(LocalDatePattern.Iso);
            Assert.False(serializer.UseRawSerializer);
        }

        [Fact]
        public void LocalDateTimeConverter_SerializeNonIso_Throws()
        {
            var localDateTime = new LocalDateTime(2012, 1, 2, 3, 4, 5, CalendarSystem.Coptic);//.GetCopticCalendar(4));

            Assert.Throws<ArgumentException>(
                () => NodaSerializerDefinitions.LocalDateTimeSerializer.Serialize(localDateTime));
        }

        [Fact]
        public void LocalTimeConverter()
        {
            var value = new LocalTime(1, 2, 3, 4);
            var json = "01:02:03.004";
            AssertConversions(value, json,NodaSerializerDefinitions.LocalTimeSerializer);
        }

        [Fact]
        public void RoundtripPeriodConverter()
        {
            var value = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);
            string json = "P2DT3H90M";
            AssertConversions(value, json,NodaSerializerDefinitions.RoundtripPeriodSerializer);
        }

        [Fact]
        public void NormalizingIsoPeriodConverter_RequiresNormalization()
        {
            // Can't use AssertConversions here, as it doesn't round-trip
            var period = Period.FromDays(2) + Period.FromHours(3) + Period.FromMinutes(90);

            var json = NodaSerializerDefinitions.NormalizingIsoPeriodSerializer.Serialize(period);
            string expectedJson = "P2DT4H30M";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void NormalizingIsoPeriodConverter_AlreadyNormalized()
        {
            // This time we're okay as it's already a normalized value.
            var value = Period.FromDays(2) + Period.FromHours(4) + Period.FromMinutes(30);
            string json = "P2DT4H30M";
            AssertConversions(value, json, NodaSerializerDefinitions.NormalizingIsoPeriodSerializer);
        }

        [Fact]
        public void ZonedDateTimeConverter()
        {
            // Deliberately give it an ambiguous local time, in both ways.
            var zone = DateTimeZoneProviders.Tzdb["Europe/London"];
            var earlierValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(1));
            var laterValue = new ZonedDateTime(new LocalDateTime(2012, 10, 28, 1, 30), zone, Offset.FromHours(0));
            string earlierJson = "2012-10-28T01:30:00+01 Europe/London";
            string laterJson = "2012-10-28T01:30:00Z Europe/London";
            var converter = NodaSerializerDefinitions.CreateZonedDateTimeSerializer(DateTimeZoneProviders.Tzdb);

            AssertConversions(earlierValue, earlierJson, converter);
            AssertConversions(laterValue, laterJson, converter);
        }

        [Fact]
        public void OffsetDateTimeConverter()
        {
            var value = new LocalDateTime(2012, 1, 2, 3, 4, 5, 6).WithOffset(Offset.FromHoursAndMinutes(-1, -30) + Offset.FromMilliseconds(-1234));
            string json = "2012-01-02T03:04:05.006-01:30:01";
            AssertConversions(value, json, NodaSerializerDefinitions.OffsetDateTimeSerializer);
        }

        [Fact]
        public void Duration_WholeSeconds()
        {
            AssertConversions(Duration.FromHours(48), "48:00:00", NodaSerializerDefinitions.DurationSerializer);
        }

        [Fact]
        public void Duration_FractionalSeconds()
        {
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234567), "48:00:03.1234567", NodaSerializerDefinitions.DurationSerializer);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1230000), "48:00:03.123", NodaSerializerDefinitions.DurationSerializer);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(1234000), "48:00:03.1234", NodaSerializerDefinitions.DurationSerializer);
            AssertConversions(Duration.FromHours(48) + Duration.FromSeconds(3) + Duration.FromTicks(12345), "48:00:03.0012345", NodaSerializerDefinitions.DurationSerializer);
        }

        [Fact]
        public void Duration_MinAndMaxValues()
        {
            AssertConversions(Duration.FromTicks(long.MaxValue), "256204778:48:05.4775807", NodaSerializerDefinitions.DurationSerializer);
            AssertConversions(Duration.FromTicks(long.MinValue), "-256204778:48:05.4775808", NodaSerializerDefinitions.DurationSerializer);
        }

        [Fact]
        public void Duration_ParsePartialFractionalSecondsWithTrailingZeroes()
        {
            var parsed = NodaSerializerDefinitions.DurationSerializer.Deserialize("25:10:00.1234000");
            Assert.Equal(Duration.FromHours(25) + Duration.FromMinutes(10) + Duration.FromTicks(1234000), parsed);
        }

        private static void AssertConversions<T>(T value, string expectedJson, IServiceStackSerializer<T> serializer)
        {
            var actualJson = serializer.Serialize(value);
            Assert.Equal(expectedJson, actualJson);

            var deserializedValue = serializer.Deserialize(expectedJson);
            Assert.Equal(value, deserializedValue);
        }
    }
}
