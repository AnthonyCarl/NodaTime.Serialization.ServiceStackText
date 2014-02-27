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

        public string Serialize(DateTimeZone value)
        {
            return value == null ? null : value.Id;
        }

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