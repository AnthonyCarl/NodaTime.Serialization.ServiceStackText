using System;
using System.Runtime.Serialization;
using ServiceStack.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// Standard ServiceStack.Text JSON fallback deserializers for NodaTime. These are used by 
    /// <see cref="StandardServiceStackSerializer{T}"/> to make deserialization more resilient and forgiving to misbehaving consumers.
    /// </summary>
    public static class ServiceStackFallbackDeserializers
    {
        /// <summary>
        /// Attempts to generate a <see cref="AnnualDate"/> by deserializing to a <see cref="DateTime"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="AnnualDate"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="AnnualDate"/></exception>
        public static AnnualDate ToAnnualDate(string text)
        {
            var dateTime = DeserializeStruct<DateTime>(text);
            return new AnnualDate(dateTime.Month, dateTime.Day);
        }

        /// <summary>
        /// Attempts to generate a <see cref="Instant"/> by deserializing to a <see cref="DateTimeOffset"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="Instant"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="Instant"/></exception>
        public static Instant ToInstant(string text)
        {
            var dateTimeOffset = DeserializeStruct<DateTimeOffset>(text);
            var instant = Instant.FromDateTimeOffset(dateTimeOffset);
            return instant;
        }

        /// <summary>
        /// Attempts to generate a <see cref="LocalTime"/> by deserializing to a <see cref="DateTime"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="LocalTime"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="LocalTime"/></exception>
        public static LocalTime ToLocalTime(string text)
        {
            var dateTime = DeserializeStruct<DateTime>(text);
            var localTime = LocalTime.FromTicksSinceMidnight(dateTime.TimeOfDay.Ticks);
            return localTime;
        }

        /// <summary>
        /// Attempts to generate a <see cref="LocalDate"/> by deserializing to a <see cref="DateTimeOffset"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="LocalDate"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="LocalDate"/></exception>
        public static LocalDate ToLocalDate(string text)
        {
            var dateTimeOffset = DeserializeStruct<DateTimeOffset>(text);
            var localDate = OffsetDateTime.FromDateTimeOffset(dateTimeOffset).Date;
            return localDate;
        }

        /// <summary>
        /// Attempts to generate a <see cref="LocalDateTime"/> by deserializing to a <see cref="DateTimeOffset"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="LocalDateTime"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="LocalDateTime"/></exception>
        public static LocalDateTime ToLocalDateTime(string text)
        {
            var dateTimeOffset = DeserializeStruct<DateTimeOffset>(text);
            var localDateTime = OffsetDateTime.FromDateTimeOffset(dateTimeOffset).LocalDateTime;
            return localDateTime;
        }

        /// <summary>
        /// Attempts to generate a <see cref="OffsetDateTime"/> by deserializing to a <see cref="DateTimeOffset"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="OffsetDateTime"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="OffsetDateTime"/></exception>
        public static OffsetDateTime ToOffsetDateTime(string text)
        {
            var dateTimeOffset = DeserializeStruct<DateTimeOffset>(text);
            var offsetDateTime = OffsetDateTime.FromDateTimeOffset(dateTimeOffset);
            return offsetDateTime;
        }

        /// <summary>
        /// Attempts to generate a <see cref="ZonedDateTime"/> by deserializing to a <see cref="DateTimeOffset"/> first.
        /// </summary>
        /// <param name="text">The JSON to deserialize.</param>
        /// <returns>The deserialized <see cref="ZonedDateTime"/></returns>
        /// <exception cref="SerializationException">Failed to deserialize to a <see cref="ZonedDateTime"/></exception>
        public static ZonedDateTime ToZonedDateTime(string text)
        {
            var dateTimeOffset = DeserializeStruct<DateTimeOffset>(text);
            var zonedDateTime = ZonedDateTime.FromDateTimeOffset(dateTimeOffset);
            return zonedDateTime;
        }

        private static T DeserializeStruct<T>(string text) where T : struct
        {
            var deserializedType = JsonSerializer.DeserializeFromString<T?>(text);

            if (deserializedType == null)
            {
                throw new SerializationException(string.Format("Unable to deserialize to {0}.", typeof (T).Name));
            }

            return deserializedType.Value;
        }
    }
}