namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// Provides the ServiceStack.Text JSON serializers to use for NodaTime. 
    /// </summary>
    public interface INodaSerializerSettings
    {
        /// <summary>
        /// The <see cref="DateTimeZone"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<DateTimeZone> DateTimeZoneSerializer { get; set; }
        /// <summary>
        /// The <see cref="Duration"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<Duration> DurationSerializer { get; set; }
        /// <summary>
        /// The <see cref="Instant"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<Instant> InstantSerializer { get; set; }
        /// <summary>
        /// The <see cref="Interval"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<Interval> IntervalSerializer { get; set; }
        /// <summary>
        /// The <see cref="LocalDate"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<LocalDate> LocalDateSerializer { get; set; }
        /// <summary>
        /// The <see cref="LocalDateTime"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<LocalDateTime> LocalDateTimeSerializer { get; set; }
        /// <summary>
        /// The <see cref="LocalTime"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<LocalTime> LocalTimeSerializer { get; set; }
        /// <summary>
        /// The <see cref="Offset"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<Offset> OffsetSerializer { get; set; }
        /// <summary>
        /// The <see cref="OffsetDateTime"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<OffsetDateTime> OffsetDateTimeSerializer { get; set; }
        /// <summary>
        /// The <see cref="Period"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<Period> PeriodSerializer { get; set; }
        /// <summary>
        /// The <see cref="ZonedDateTime"/> ServiceStack.Text JSON serializer to use.
        /// </summary>
        IServiceStackSerializer<ZonedDateTime> ZonedDateTimeSerializer { get; set; }
        /// <summary>
        /// The <see cref="IDateTimeZoneProvider"/> in use.
        /// </summary>
        IDateTimeZoneProvider Provider { get; set; }
    }
}