using System;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// A collection of typical ServiceStack.Text JSON serializers for use with NodaTime.
    /// </summary>
    public class DefaultNodaSerializerSettings : INodaSerializerSettings
    {
        /// <summary>
        /// The <see cref="DateTimeZone"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<DateTimeZone> DateTimeZoneSerializer { get; set; }

        /// <summary>
        /// The <see cref="Duration"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<Duration> DurationSerializer { get; set; }

        /// <summary>
        /// The <see cref="Instant"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<Instant> InstantSerializer { get; set; }

        /// <summary>
        /// The <see cref="Interval"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<Interval> IntervalSerializer { get;set; }

        /// <summary>
        /// The <see cref="LocalDate"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<LocalDate> LocalDateSerializer { get; set; }

        /// <summary>
        /// The <see cref="LocalDateTime"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<LocalDateTime> LocalDateTimeSerializer { get; set; }

        /// <summary>
        /// The <see cref="LocalTime"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<LocalTime> LocalTimeSerializer { get; set; }

        /// <summary>
        /// The <see cref="Offset"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<Offset> OffsetSerializer { get; set; }

        /// <summary>
        /// The <see cref="OffsetDateTime"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<OffsetDateTime> OffsetDateTimeSerializer { get; set; }

        /// <summary>
        /// The <see cref="Period"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<Period> PeriodSerializer { get; set; }

        /// <summary>
        /// The <see cref="ZonedDateTime"/> serializer to use.
        /// </summary>
        public IServiceStackSerializer<ZonedDateTime> ZonedDateTimeSerializer { get; set; }

        /// <summary>
        /// Creates an instance of default serializers using the given <see cref="IDateTimeZoneProvider"/>.
        /// </summary>
        /// <param name="provider"></param>
        public DefaultNodaSerializerSettings(IDateTimeZoneProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            DurationSerializer = NodaSerializerDefinitions.DurationSerializer;
            DateTimeZoneSerializer = NodaSerializerDefinitions.CreateDateTimeZoneSerializer(provider);
            InstantSerializer = NodaSerializerDefinitions.InstantSerializer;
            IntervalSerializer = NodaSerializerDefinitions.ComplexIntervalSerializer;
            LocalDateSerializer = NodaSerializerDefinitions.LocalDateSerializer;
            LocalDateTimeSerializer = NodaSerializerDefinitions.LocalDateTimeSerializer;
            LocalTimeSerializer = NodaSerializerDefinitions.LocalTimeSerializer;
            OffsetSerializer = NodaSerializerDefinitions.OffsetSerializer;
            OffsetDateTimeSerializer = NodaSerializerDefinitions.OffsetDateTimeSerializer;
            PeriodSerializer = NodaSerializerDefinitions.RoundtripPeriodSerializer;
            ZonedDateTimeSerializer = NodaSerializerDefinitions.CreateZonedDateTimeSerializer(provider);
        }
    }
}