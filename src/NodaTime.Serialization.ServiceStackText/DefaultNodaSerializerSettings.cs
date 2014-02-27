using System;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// A collection of typical ServiceStack.Text JSON serializers for use with NodaTime.
    /// </summary>
    public class DefaultNodaSerializerSettings : INodaSerializerSettings
    {
        public IServiceStackSerializer<DateTimeZone> DateTimeZoneSerializer { get; set; }

        public IServiceStackSerializer<Duration> DurationSerializer { get; set; }

        public IServiceStackSerializer<Instant> InstantSerializer { get; set; }

        public IServiceStackSerializer<Interval> IntervalSerializer { get;set; }

        public IServiceStackSerializer<LocalDate> LocalDateSerializer { get; set; }

        public IServiceStackSerializer<LocalDateTime> LocalDateTimeSerializer { get; set; }

        public IServiceStackSerializer<LocalTime> LocalTimeSerializer { get; set; }

        public IServiceStackSerializer<Offset> OffsetSerializer { get; set; }

        public IServiceStackSerializer<OffsetDateTime> OffsetDateTimeSerializer { get; set; }

        public IServiceStackSerializer<Period> PeriodSerializer { get; set; }

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