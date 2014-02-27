using System;
using ServiceStack.Text;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    public class ExtensionsTest
    {
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
            DateTimeZoneProviders.Tzdb.CreateDefaultSerializersForNodaTime().ConfigureSerializersForNodaTime();
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