using System;
using NodaTime.Utility;
using ServiceStack.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// ServiceStack.Text JSON serializer for <see cref="Interval"/> using a compound representation. The start and
    /// end aspects of the interval are represented with separate properties, each parsed and formatted
    /// by the <see cref="Instant"/> serializer provided.
    /// </summary> 
    public class ComplexJsonIntervalSerializer : IServiceStackSerializer<Interval>
    {
        private readonly IServiceStackSerializer<Instant> _instantSerializer;

        /// <summary>
        /// <see cref="ComplexJsonIntervalSerializer"/> uses the ServiceStack.Text raw serializer.
        /// </summary>
        public bool UseRawSerializer
        {
            get { return true; }
        }

        /// <summary>
        /// Creates a new instance of an <see cref="Interval"/> serializer that uses a complex JSON representation.
        /// </summary>
        /// <param name="instantSerializer">The serializer to use to parse and format the start and 
        /// end <see cref="Instant"/>.</param>
        public ComplexJsonIntervalSerializer(IServiceStackSerializer<Instant> instantSerializer)
        {
            if (instantSerializer == null)
            {
                throw new ArgumentNullException(nameof(instantSerializer));
            }
            this._instantSerializer = instantSerializer;
        }

        /// <summary>
        /// Serializes the provided <see cref="Interval"/>.
        /// </summary>
        /// <param name="value">The <see cref="Interval"/> to to serialize.</param>
        /// <returns>The serialized representation.</returns>
        public string Serialize(Interval value)
        {
            var complexIntervalDto = new ComplexRawIntervalDto
            {
                Start = _instantSerializer.Serialize(value.Start),
                End = _instantSerializer.Serialize(value.End)
            };

            return JsonSerializer.SerializeToString(complexIntervalDto);
        }

        /// <summary>
        /// Deserializes the given JSON.
        /// </summary>
        /// <param name="text">The JSON to parse.</param>
        /// <returns>The deserialized <see cref="Interval"/>.</returns>
        public Interval Deserialize(string text)
        {
            var complexIntervalDto = JsonSerializer.DeserializeFromString<ComplexRawIntervalDto>(text);

            if (!IsValid(complexIntervalDto))
            {
                throw new InvalidNodaDataException("An Interval must contain Start and End properties.");
            }

            var start = _instantSerializer.Deserialize(complexIntervalDto.Start);
            var end = _instantSerializer.Deserialize(complexIntervalDto.End);

            var interval = new Interval(start, end);
            
            return interval;
        }

        private static bool IsValid(ComplexRawIntervalDto complexIntervalDto)
        {
            if (complexIntervalDto == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(complexIntervalDto.Start))
            {
                return false;
            }
            return !string.IsNullOrEmpty(complexIntervalDto.End);
        }
    }
}