using System;
using NodaTime.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// Convenience class to expose preconfigured serializers for Noda Time types, and factory methods
    /// for creating those which require parameters.
    /// </summary>
    public static class NodaSerializerDefinitions
    {
        /// <summary>
        /// Converter for <see cref="Interval"/>. This uses the same <see cref="Instant"/> serializer as the default Instant Serializer.
        /// </summary
        public static IServiceStackSerializer<Interval> ComplexIntervalSerializer = new ComplexJsonIntervalSerializer(CreateInstantSerializer());

        /// <summary>
        /// Converter for <see cref="Interval"/>. This uses the same <see cref="Instant"/> serializer as the default Instant Serializer.
        /// </summary
        public static IServiceStackSerializer<Interval> ExtendedIsoIntervalSerializer = new ExtendedIsoIntervalSerializer(CreateInstantSerializer());

        /// <summary>
        /// Converter for <see cref="LocalTime"/>, using the ISO-8601 time pattern, extended as required to accommodate milliseconds and ticks.
        /// </summary>
        public static IServiceStackSerializer<LocalTime> LocalTimeSerializer =
            new StandardServiceStackSerializer<LocalTime>(
                LocalTimePattern.ExtendedIsoPattern,
                ServiceStackFallbackDeserializers.ToLocalTime);

        /// <summary>
        /// Converter for <see cref="LocalDate"/>, using the ISO-8601 date pattern.
        /// </summary>
        public static readonly IServiceStackSerializer<LocalDate> LocalDateSerializer =
            new StandardServiceStackSerializer<LocalDate>(
                LocalDatePattern.IsoPattern,
                ServiceStackFallbackDeserializers.ToLocalDate,
                CreateIsoValidator<LocalDate>(x => x.Calendar));

        /// <summary>
        /// Converter for <see cref="LocalDateTime"/>, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks.
        /// No time zone designator is applied.
        /// </summary>
        public static readonly IServiceStackSerializer<LocalDateTime> LocalDateTimeSerializer =
            new StandardServiceStackSerializer<LocalDateTime>(
                LocalDateTimePattern.ExtendedIsoPattern,
                ServiceStackFallbackDeserializers.ToLocalDateTime,
                CreateIsoValidator<LocalDateTime>(x => x.Calendar));

        /// <summary>
        /// Converter for <see cref="OffsetDateTime"/>.
        /// </summary>
        public static readonly IServiceStackSerializer<OffsetDateTime> OffsetDateTimeSerializer =
            new StandardServiceStackSerializer<OffsetDateTime>(
                OffsetDateTimePattern.ExtendedIsoPattern,
                ServiceStackFallbackDeserializers.ToOffsetDateTime,
                CreateIsoValidator<OffsetDateTime>(x => x.Calendar));

        /// <summary>
        /// Converter for <see cref="Instant"/>, using the ISO-8601 date/time pattern, extended as required to accommodate milliseconds and ticks, and
        /// specifying 'Z' at the end to show it's effectively in UTC.
        /// </summary>
        public static readonly IServiceStackSerializer<Instant> InstantSerializer = CreateInstantSerializer();

        /// <summary>
        /// Creates a serializer for zoned date/times, using the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to use when parsing.</param>
        /// <returns>A serializer to handle <see cref="ZonedDateTime"/>.</returns>
        public static IServiceStackSerializer<ZonedDateTime> CreateZonedDateTimeSerializer(IDateTimeZoneProvider provider)
        {
            return
                new StandardServiceStackSerializer<ZonedDateTime>(
                    ZonedDateTimePattern.CreateWithInvariantCulture(
                        "yyyy'-'MM'-'dd'T'HH':'mm':'ss;FFFFFFFo<G> z",
                        provider),
                    ServiceStackFallbackDeserializers.ToZonedDateTime,
                    CreateIsoValidator<ZonedDateTime>(x => x.Calendar));
        }

        /// <summary>
        /// Round-tripping converter for <see cref="Period"/>. Use this when you really want to preserve information,
        /// and don't need interoperability with systems expecting ISO.
        /// </summary>
        public static IServiceStackSerializer<Period> RoundtripPeriodSerializer =
            new StandardServiceStackSerializer<Period>(PeriodPattern.RoundtripPattern);

        /// <summary>
        /// Normalizing ISO converter for <see cref="Period"/>. Use this when you want compatibility with systems expecting
        /// ISO durations (~= Noda Time periods). However, note that Noda Time can have negative periods. Note that
        /// this converter losses information - after serialization and deserialization, "90 minutes" will become "an hour and 30 minutes".
        /// </summary>
        public static IServiceStackSerializer<Period> NormalizingIsoPeriodSerializer =
            new StandardServiceStackSerializer<Period>(PeriodPattern.NormalizingIsoPattern);

        /// <summary>
        /// Converter for <see cref="Duration"/>.
        /// </summary>
        public static IServiceStackSerializer<Duration> DurationSerializer =
            new StandardServiceStackSerializer<Duration>(DurationPattern.CreateWithInvariantCulture("-H:mm:ss.FFFFFFF"));

        /// <summary>
        /// Creates a serializer for time zones, using the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IDateTimeZoneProvider"/> to use when parsing.</param>
        /// <returns>A serializer to handle <see cref="DateTimeZone"/>.</returns>
        public static IServiceStackSerializer<DateTimeZone> CreateDateTimeZoneSerializer(IDateTimeZoneProvider provider)
        {
            return new DateTimeZoneSerializer(provider);
        }

        /// <summary>
        /// Converter for <see cref="Offset"/>.
        /// </summary>
        public static IServiceStackSerializer<Offset> OffsetSerializer =
            new StandardServiceStackSerializer<Offset>(OffsetPattern.GeneralInvariantPattern);
        
        private static IServiceStackSerializer<Instant> CreateInstantSerializer()
        {
            return new StandardServiceStackSerializer<Instant>(
                InstantPattern.ExtendedIsoPattern,
                ServiceStackFallbackDeserializers.ToInstant);
        }

        private static Action<T> CreateIsoValidator<T>(Func<T, CalendarSystem> calendarProjection)
        {
            return value =>
            {
                var calendar = calendarProjection(value);
                // We rely on CalendarSystem.Iso being a singleton here.
                if (calendar != CalendarSystem.Iso)
                {
                    throw new ArgumentException(
                        string.Format("Values of type {0} must (currently) use the ISO calendar in order to be serialized.",
                            typeof(T).Name));
                }
            };
        }
    }
}