using System;
using System.Linq;
using NodaTime.TimeZones;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// ServiceStack.Text JSON serializer for <see cref="DateTimeZone"/> for the given <see cref="IDateTimeZoneProvider"/>.
    /// Deserialization is case insensitive.
    /// </summary> 
    public class DateTimeZoneSerializer : IServiceStackSerializer<DateTimeZone>
    {
        private readonly IDateTimeZoneProvider _provider;

        /// <summary>
        /// The <see cref="DateTimeZoneSerializer"/> does not use the raw serializer.
        /// </summary>
        public bool UseRawSerializer { get { return false; } }

        /// <summary>
        /// Creates an instance of the <see cref="DateTimeZone"/> serializer for the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to use.</param>
        public DateTimeZoneSerializer(IDateTimeZoneProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this._provider = provider;
        }

        /// <summary>
        /// Serializes the provided <see cref="DateTimeZone"/>.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeZone"/> to serialize.</param>
        /// <returns>The serialized representation.</returns>
        public string Serialize(DateTimeZone value)
        {
            return value == null ? null : value.Id;
        }

        /// <summary>
        /// Deserializes the given JSON.
        /// </summary>
        /// <param name="text">The JSON to parse.</param>
        /// <returns>The deserialized <see cref="DateTimeZone"/>.</returns>
        public DateTimeZone Deserialize(string text)
        {
            var id = _provider.Ids.FirstOrDefault(s => String.Equals(text, s, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(id))
            {
                throw new DateTimeZoneNotFoundException("Time zone " + text + " is unknown.");
            }
            return _provider[id];
        }
    }
}