using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class DefaultNodaSerializerSettingsTests
    {
        [Fact]
        public void Constructor_NullDateTimeZoneProvider_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultNodaSerializerSettings(null));
        }

        [Fact]
        public void DurationSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.DurationSerializer, serializerSettings.DurationSerializer);
        }

        [Fact]
        public void InstantSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.InstantSerializer, serializerSettings.InstantSerializer);
        }

        [Fact]
        public void IntervalSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.ComplexIntervalSerializer, serializerSettings.IntervalSerializer);
        }

        [Fact]
        public void LocalDateSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.LocalDateSerializer, serializerSettings.LocalDateSerializer);
        }

        [Fact]
        public void LocalDateTimeSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.LocalDateTimeSerializer, serializerSettings.LocalDateTimeSerializer);
        }

        [Fact]
        public void LocalTimeSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.LocalTimeSerializer, serializerSettings.LocalTimeSerializer);
        }

        [Fact]
        public void OffsetSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.OffsetSerializer, serializerSettings.OffsetSerializer);
        }

        [Fact]
        public void OffsetDateTimeSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.OffsetDateTimeSerializer, serializerSettings.OffsetDateTimeSerializer);
        }

        [Fact]
        public void PeriodSerializer_Default_Verify()
        {
            var serializerSettings = new DefaultNodaSerializerSettings(DateTimeZoneProviders.Tzdb);
            Assert.Same(NodaSerializerDefinitions.RoundtripPeriodSerializer, serializerSettings.PeriodSerializer);
        }
    }
}
