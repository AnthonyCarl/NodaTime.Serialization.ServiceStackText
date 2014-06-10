using System;
using System.Diagnostics.CodeAnalysis;
using NodaTime.Text;
using NodaTime.Utility;
using Xunit;

namespace NodaTime.Serialization.ServiceStackText.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class ExtendedIsoIntervalSerializerTests
    {
        [Fact]
        public void Serialize()
        {
            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var interval = new Interval(startInstant, endInstant);

            var json = NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Serialize(interval);

            string expectedJson = "2012-01-02T03:04:05Z/2013-06-07T08:09:10Z";
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void Deserialize()
        {
            string json = "2012-01-02T03:04:05Z/2013-06-07T08:09:10Z";

            var interval = NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Deserialize(json);

            var startInstant = Instant.FromUtc(2012, 1, 2, 3, 4, 5);
            var endInstant = Instant.FromUtc(2013, 6, 7, 8, 9, 10);
            var expectedInterval = new Interval(startInstant, endInstant);
            Assert.Equal(expectedInterval, interval);
        }

        [Fact]
        public void Deserialize_MissingEnd_Throws()
        {
            string json = "2012-01-02T03:04:05Z/";
            Assert.Throws<UnparsableValueException>(
                () => NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Deserialize(json));
        }

        [Fact]
        public void Deserialize_MissingStart_Throws()
        {
            string json = "/2012-01-02T03:04:05Z";

            Assert.Throws<UnparsableValueException>(
                () => NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Deserialize(json));
        }

        [Fact]
        public void Deserialize_EmptyString_Throws()
        {
            Assert.Throws<InvalidNodaDataException>(
                () => NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Deserialize(string.Empty));
        }

        [Fact]
        public void Deserialize_NoSlash_Throws()
        {
            Assert.Throws<InvalidNodaDataException>(
                () => NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.Deserialize("NotGoingToWork"));
        }

        [Fact]
        public void UseRawSerializer_Default_False()
        {
            Assert.False(NodaSerializerDefinitions.ExtendedIsoIntervalSerializer.UseRawSerializer);
        }

        [Fact]
        public void Constructor_NullInstantSerializer_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new ExtendedIsoIntervalSerializer(null));
        }
    }
}
