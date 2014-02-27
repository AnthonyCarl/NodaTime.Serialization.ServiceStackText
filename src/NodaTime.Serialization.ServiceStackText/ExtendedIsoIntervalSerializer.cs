using System;
using NodaTime.Utility;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// ServiceStack.Text JSON serializer for <see cref="Interval"/> using a ISO8601 Interval representation. The 
    /// start and end aspects of the interval are each parsed and formatted by the <see cref="Instant"/> serializer 
    /// provided.
    /// </summary> 
    public class ExtendedIsoIntervalSerializer : IServiceStackSerializer<Interval>
    {
        internal const char Iso8601TimeIntervalSeparator = '/';
        private readonly IServiceStackSerializer<Instant> _instantSerializer;

        /// <summary>
        /// <see cref="ExtendedIsoIntervalSerializer"/> does not use the ServiceStack.Text raw serializer.
        /// </summary>
        public bool UseRawSerializer
        {
            get { return false; }
        }

        /// <summary>
        /// Creates an instance of the <see cref="ExtendedIsoIntervalSerializer"/>
        /// </summary>
        /// <param name="instantSerializer">The <see cref="Instant"/> serializer to use.</param>
        public ExtendedIsoIntervalSerializer(IServiceStackSerializer<Instant> instantSerializer)
        {
            if (instantSerializer == null)
            {
                throw new ArgumentNullException("instantSerializer");
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
            return string.Format(
                "{0}{1}{2}",
                _instantSerializer.Serialize(value.Start),
                Iso8601TimeIntervalSeparator,
                _instantSerializer.Serialize(value.End)
                );
        }

        /// <summary>
        /// Deserializes the given JSON.
        /// </summary>
        /// <param name="text">The JSON to parse.</param>
        /// <returns>The deserialized <see cref="Interval"/>.</returns>
        public Interval Deserialize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new InvalidNodaDataException("No text to parse.");
            }

            var slash = text.IndexOf(Iso8601TimeIntervalSeparator);
            if (slash == -1)
            {
                throw new InvalidNodaDataException("Expected ISO-8601-formatted interval; slash was missing.");
            }
            var startText = text.Substring(0, slash);
            var endText = text.Substring(slash + 1);

            var start = _instantSerializer.Deserialize(startText);
            var end = _instantSerializer.Deserialize(endText);
            return new Interval(start, end);
        }
    }
}