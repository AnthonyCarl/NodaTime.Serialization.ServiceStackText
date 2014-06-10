using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime.Utility;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class ComplexJsonIntervalSerializerTests
    {
        [Fact]
        public void Serialize()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var json = NodaSerializerDefinitions.ComplexIntervalSerializer.Serialize(interval);

            string expectedJson = "{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize()
        {
            string json = "{\"Start\":\"2012-01-02T03:04:05Z\",\"End\":\"2013-06-07T08:09:10Z\"}";

            var interval = NodaSerializerDefinitions.ComplexIntervalSerializer.Deserialize(json);

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.Equal(expectedInterval, interval);
        }

        [Fact]
        public void Deserialize_MissingEnd_Throws()
        {
            string json = "{\"Start\":\"2012-01-02T03:04:05Z\"}";
            Assert.Throws<InvalidNodaDataException>(
                () => NodaSerializerDefinitions.ComplexIntervalSerializer.Deserialize(json));
        }

        [Fact]
        public void Deserialize_MissingStart_Throws()
        {
            string json = "{\"End\":\"2012-01-02T03:04:05Z\"}";

            Assert.Throws<InvalidNodaDataException>(
                () => NodaSerializerDefinitions.ComplexIntervalSerializer.Deserialize(json));
        }

        [Fact]
        public void UseRawSerializer_Default_True()
        {
            Assert.True(NodaSerializerDefinitions.ComplexIntervalSerializer.UseRawSerializer);
        }

        [Fact]
        public void Constructor_NullInstantSerializer_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ComplexJsonIntervalSerializer(null));
        }

        [Fact]
        public void Deserialize_NullText_Throws()
        {
            Assert.Throws<InvalidNodaDataException>(
                () => NodaSerializerDefinitions.ComplexIntervalSerializer.Deserialize(null));
        }
    }
}
