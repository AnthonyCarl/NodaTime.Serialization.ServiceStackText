using System;
using ServiceStack.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// Useful extensions used to configure the ServiceStack.Text JSON serializer.
    /// </summary>
    public static class Extensions
    {
        private static readonly object Mutex = new object();

        /// <summary>
        ///     Used for fluent setting of serializerSettings. Nothing is done if the serializer or config action is null.
        /// </summary>
        /// <param name="serializerSettings">The serializer to perform the action on.</param>
        /// <param name="configAction">The configuration action to perform on the serializerSettings.</param>
        /// <returns>The original <param ref="serializerSettings"/> value, for further chaining.</returns>
        public static INodaSerializerSettings SetSerializer(
            this INodaSerializerSettings serializerSettings,
            Action<INodaSerializerSettings> configAction)
        {
            if (serializerSettings == null || configAction == null)
            {
                //nothing to do
                return serializerSettings;
            }
            configAction(serializerSettings);
            return serializerSettings;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaSerializerDefinitions.ExtendedIsoIntervalSerializer"/>.
        /// Any other converters which can convert <see cref="Interval"/> are removed from the serializer.
        /// </summary>
        /// <param name="serializerSettings">The existing serializer settings to add Noda Time serializerSettings to.</param>
        /// <returns>The original <param ref="serializerSettings"/> value, for further chaining.</returns>
        public static INodaSerializerSettings WithIsoIntervalSerializer(this INodaSerializerSettings serializerSettings)
        {
            serializerSettings.SetSerializer(
                x => x.IntervalSerializer = NodaSerializerDefinitions.ExtendedIsoIntervalSerializer);

            return serializerSettings;
        }

        /// <summary>
        /// Configures the given serializer settings to use <see cref="NodaSerializerDefinitions.NormalizingIsoPeriodSerializer"/>.
        /// Any other converters which can convert <see cref="Period"/> are removed from the serializer.
        /// </summary>
        /// <param name="serializerSettings">The existing serializer settings to add Noda Time serializerSettings to.</param>
        /// <returns>The original <param ref="serializerSettings"/> value, for further chaining.</returns>
        public static INodaSerializerSettings WithNormalizingIsoPeriodSerializer(this INodaSerializerSettings serializerSettings)
        {
            serializerSettings.SetSerializer(
                x => x.PeriodSerializer = NodaSerializerDefinitions.NormalizingIsoPeriodSerializer);

            return serializerSettings;
        }

        /// <summary>
        /// Configuration for ServiceStack.Text with everything required to properly serialize and deserialize NodaTime data types to and from json.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>A new Noda serializer settings.</returns>
        public static INodaSerializerSettings CreateDefaultSerializersForNodaTime(this IDateTimeZoneProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            return new DefaultNodaSerializerSettings(provider);
        }

        /// <summary>
        /// Configures the ServiceStack.Text json serializer.
        /// </summary>
        /// <param name="nodaSerializerSettings">The serializer settings to use.</param>
        public static void ConfigureSerializersForNodaTime(this INodaSerializerSettings nodaSerializerSettings)
        {
            if (nodaSerializerSettings == null)
            {
                //nothing to do
                return;
            }
            nodaSerializerSettings.DateTimeZoneSerializer.ConfigureSerializer();
            nodaSerializerSettings.DurationSerializer.ConfigureSerializer();
            nodaSerializerSettings.InstantSerializer.ConfigureSerializer();
            nodaSerializerSettings.IntervalSerializer.ConfigureSerializer();
            nodaSerializerSettings.LocalDateSerializer.ConfigureSerializer();
            nodaSerializerSettings.LocalDateTimeSerializer.ConfigureSerializer();
            nodaSerializerSettings.LocalTimeSerializer.ConfigureSerializer();
            nodaSerializerSettings.OffsetDateTimeSerializer.ConfigureSerializer();
            nodaSerializerSettings.OffsetSerializer.ConfigureSerializer();
            nodaSerializerSettings.PeriodSerializer.ConfigureSerializer();
            nodaSerializerSettings.ZonedDateTimeSerializer.ConfigureSerializer();
        }

        /// <summary>
        /// Configures the ServiceStack.Text json serializer.
        /// </summary>
        /// <param name="serializer">The individual serializer to configure.</param>
        public static void ConfigureSerializer<T>(this IServiceStackSerializer<T> serializer)
        {
            if (serializer == null)
            {
                //nothing to do
                return;
            }

            //JsConfig is not thread safe.
            lock (Mutex)
            {
                if (serializer.UseRawSerializer)
                {
                    JsConfig<T>.RawSerializeFn = serializer.Serialize;
                    JsConfig<T>.RawDeserializeFn = serializer.Deserialize;
                }
                else
                {
                    JsConfig<T>.SerializeFn = serializer.Serialize;
                    JsConfig<T>.DeSerializeFn = serializer.Deserialize;
                }
            }
        }
    }
}