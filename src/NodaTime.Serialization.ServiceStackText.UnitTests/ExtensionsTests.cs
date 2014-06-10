using System;
using System.Runtime.Serialization;
using NodaTime.Testing;
using ServiceStack.Text;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    public class ExtensionsTests
    {
        public ExtensionsTests()
        {
            //ServiceStack.Text Json serializer is static, need to setup the serializers for some of these tests.
            DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime().ConfigureSerializersForNodaTime();
        }

        [Fact]
        public void CreateDefaultSerializersForNodaTime_NullDateTimeZoneProvider_Throws()
        {
            IDateTimeZoneProvider provider = null;
            Assert.Throws<ArgumentNullException>(() => provider.CreateDefaultSerializersForNodaTime());
        }

        [Fact]
        public void CreateDefaultSerializersForNodaTime_ValidProvider_SettingsCreated()
        {
            Assert.NotNull(DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime());
        }

        [Fact]
        public void SetSerializer_NonNullSettingsNullConfig_NoException()
        {
            INodaSerializerSettings settings = DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime();
            Assert.NotNull(settings.SetSerializer(null));
        }

        [Fact]
        public void SetSerializer_NullSettingsAndConfig_ReturnsNull()
        {
            INodaSerializerSettings settings = null;
            Assert.Null(settings.SetSerializer(null));
        }

        [Fact]
        public void SetSerializer_NullSettingsNonNullConfig_ReturnNull()
        {
            INodaSerializerSettings settings = null;
            Assert.Null(settings.SetSerializer(s => { s.PeriodSerializer = null; }));
        }

        [Fact]
        public void WithIsoIntervalSerializer_ValidSettings_Verify()
        {
            INodaSerializerSettings settings =
                DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime().WithIsoIntervalSerializer();
            Assert.Same(NodaSerializerDefinitions.ExtendedIsoIntervalSerializer, settings.IntervalSerializer);
        }

        [Fact]
        public void WithNormalizingIsoPeriodSerializer_ValidSettings_Verify()
        {
            INodaSerializerSettings settings =
                DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime().WithNormalizingIsoPeriodSerializer();
            Assert.Same(NodaSerializerDefinitions.NormalizingIsoPeriodSerializer, settings.PeriodSerializer);
        }

        [Fact]
        public void ConfigureSerializersForNodaTime_NullSettings_NoException()
        {
            INodaSerializerSettings settings = null;
            Assert.DoesNotThrow(settings.ConfigureSerializersForNodaTime);
        }

        [Fact]
        public void ConfigureSerializer_Nullserializer_NoException()
        {
            IServiceStackSerializer<LocalDate> serializer = null;
            Assert.DoesNotThrow(serializer.ConfigureSerializer);
        }

        [Fact]
        public void ConfigureSerializersForNodaTime_Default_VerifyConfiguration()
        {
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.DurationSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.InstantSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.LocalDateSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.LocalDateTimeSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.LocalTimeSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.RoundtripPeriodSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.OffsetSerializer);
            AssertSerializeAndDeserializeFunctions(NodaSerializerDefinitions.OffsetDateTimeSerializer);
            AssertRawSerializeAndDeserializeFunctions(NodaSerializerDefinitions.ComplexIntervalSerializer);
        }

        [Fact]
        public void DeserializeFromString_ValidNullableLocalDateText_ReturnsValidNullableLocaldate()
        {
            var expectedLocalDate = new LocalDate(2014, 2, 21);
            var actualLocalDate = JsonSerializer.DeserializeFromString<LocalDate?>("2014-02-21");
            Assert.Equal(expectedLocalDate, actualLocalDate);
        }

        [Fact]
        public void DeserializeFromString_BadNullableLocalDateText_Throws()
        {
            Assert.Throws<FormatException>(() => JsonSerializer.DeserializeFromString<LocalDate?>("Wrong"));
        }

        [Fact]
        public void DeserializeFromString_NullNullableLocalDateText_ReturnsNull()
        {
            Assert.Null(JsonSerializer.DeserializeFromString<LocalDate?>(null));
        }

        [Fact]
        public void DeserializeFromString_EmptyNullableLocalDateText_ReturnsNull()
        {
            Assert.Null(JsonSerializer.DeserializeFromString<LocalDate?>(string.Empty));
        }

        [Fact]
        public void DeserializeFromString_WhitespaceNullableLocalDateText_Throws()
        {
            Assert.Throws<FormatException>(() => JsonSerializer.DeserializeFromString<LocalDate?>(" \r \n    "));
        }

        [Fact]
        public void Serialize_NullNullableLocalDate_NullText()
        {
            Assert.Null(JsonSerializer.SerializeToString<LocalDate?>(null));
        }

        [Fact]
        public void Serialize_ValidNullableLocalDate_ValidText()
        {
            var text = JsonSerializer.SerializeToString<LocalDate?>(new LocalDate(2014, 2, 21));
            Assert.Equal("\"2014-02-21\"", text);
        }

        [Fact]
        public void DeserializeFromString_ValidNullableIntervalText_ReturnsValidNullableInterval()
        {
            var start = Instant.FromUtc(2014, 2, 21, 2, 21);
            var end = Instant.FromUtc(2014, 2, 21, 21, 2);
            var expectedInterval = new Interval(start, end);
            var actualInterval =
                JsonSerializer.DeserializeFromString<Interval?>(
                    "{\"Start\":\"2014-02-21T02:21:00Z\",\"End\":\"2014-02-21T21:02:00Z\"}");
            Assert.Equal(expectedInterval, actualInterval);
        }

        [Fact]
        public void DeserializeFromString_BadNullableIntervalText_Throws()
        {
            Assert.Throws<SerializationException>(() => JsonSerializer.DeserializeFromString<Interval?>("Wrong"));
        }

        [Fact]
        public void DeserializeFromString_NullNullableIntervalText_ReturnsNull()
        {
            Assert.Null(JsonSerializer.DeserializeFromString<Interval?>(null));
        }

        [Fact]
        public void DeserializeFromString_EmptyNullableIntervalText_ReturnsNull()
        {
            Assert.Null(JsonSerializer.DeserializeFromString<Interval?>(string.Empty));
        }

        [Fact]
        public void DeserializeFromString_WhitespaceNullableIntervalText_Throws()
        {
            Assert.Throws<IndexOutOfRangeException>(() => JsonSerializer.DeserializeFromString<Interval?>("  \n    "));
        }

        [Fact]
        public void Serialize_NullNullableInterval_NullText()
        {
            Assert.Null(JsonSerializer.SerializeToString<Interval?>(null));
        }

        [Fact]
        public void Serialize_ValidNullableInterval_ValidText()
        {
            var start = Instant.FromUtc(2014, 2, 21, 2, 21);
            var end = Instant.FromUtc(2014, 2, 21, 21, 2);
            var text = JsonSerializer.SerializeToString<Interval?>(new Interval(start, end));
            Assert.Equal("{\"Start\":\"2014-02-21T02:21:00Z\",\"End\":\"2014-02-21T21:02:00Z\"}", text);
        }

        [Fact]
        public void WithGeneralIsoZonedDateTimeSerializer_Serialize()
        {
            var clock = FakeClock.FromUtc(2014, 05, 02, 10, 30, 45);
            var now = clock.Now.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
            var serialisers = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb)
                .WithGeneralIsoZonedDateTimeSerializer();

            var expected = now.ToString();
            var actual = serialisers.ZonedDateTimeSerializer.Serialize(now);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WithGeneralIsoZonedDateTimeSerializer_Deserialize()
        {
            var clock = FakeClock.FromUtc(2014, 05, 02, 10, 30, 45);
            var now = clock.Now.InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault());
            var serialisers = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb)
                .WithGeneralIsoZonedDateTimeSerializer();

            var expected = now;
            var actual = serialisers.ZonedDateTimeSerializer.Deserialize(now.ToString());

            Assert.Equal(expected, actual);
        }

        private void AssertSerializeAndDeserializeFunctions<T>(IServiceStackSerializer<T> serializer)
        {
            Func<T, string> serializationFunc = serializer.Serialize;
            Func<string, T> deserializationFunc = serializer.Deserialize;
            Assert.Same(JsConfig<T>.SerializeFn.Target, serializationFunc.Target);
            Assert.Same(JsConfig<T>.DeSerializeFn.Target, deserializationFunc.Target);
        }

        private void AssertRawSerializeAndDeserializeFunctions<T>(IServiceStackSerializer<T> serializer)
        {
            Func<T, string> serializationFunc = serializer.Serialize;
            Func<string, T> deserializationFunc = serializer.Deserialize;
            Assert.Same(JsConfig<T>.RawSerializeFn.Target, serializationFunc.Target);
            Assert.Same(JsConfig<T>.RawDeserializeFn.Target, deserializationFunc.Target);
        }
    }
}