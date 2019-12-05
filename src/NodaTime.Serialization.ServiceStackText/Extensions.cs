using System;
using System.Reflection;
using NodaTime.Text;
using ServiceStack.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    ///     Useful extensions used to configure the ServiceStack.Text JSON serializer.
    /// </summary>
    public static class Extensions
    {
        private static readonly object Mutex = new object();
        private static MethodInfo _nullableSerializerMethodInfo;

        private static MethodInfo NullableSerializerMethodInfo
        {
            get
            {
                return _nullableSerializerMethodInfo
                       ?? (_nullableSerializerMethodInfo =
                           typeof(Extensions).GetTypeInfo().GetDeclaredMethod(nameof(ConfigureNullableSerializer)));
            }
        }

        /// <summary>
        ///     Used for fluent setting of serializerSettings. Nothing is done if the serializer or config action is null.
        /// </summary>
        /// <param name="serializerSettings">The serializer to perform the action on.</param>
        /// <param name="configAction">The configuration action to perform on the serializerSettings.</param>
        /// <returns>The original <param ref="serializerSettings" /> value, for further chaining.</returns>
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
        ///     Configures the given serializer settings to use
        ///     <see cref="NodaSerializerDefinitions.ExtendedIsoIntervalSerializer" />.
        ///     Any other converters which can convert <see cref="Interval" /> are removed from the serializer.
        /// </summary>
        /// <param name="serializerSettings">The existing serializer settings to add Noda Time serializerSettings to.</param>
        /// <returns>The original <param ref="serializerSettings" /> value, for further chaining.</returns>
        public static INodaSerializerSettings WithIsoIntervalSerializer(this INodaSerializerSettings serializerSettings)
        {
            serializerSettings.SetSerializer(
                x => x.IntervalSerializer = NodaSerializerDefinitions.ExtendedIsoIntervalSerializer);

            return serializerSettings;
        }

        /// <summary>
        ///     Configures the given serializer settings to use
        ///     <see cref="NodaSerializerDefinitions.NormalizingIsoPeriodSerializer" />.
        ///     Any other converters which can convert <see cref="Period" /> are removed from the serializer.
        /// </summary>
        /// <param name="serializerSettings">The existing serializer settings to add Noda Time serializerSettings to.</param>
        /// <returns>The original <param ref="serializerSettings" /> value, for further chaining.</returns>
        public static INodaSerializerSettings WithNormalizingIsoPeriodSerializer(
            this INodaSerializerSettings serializerSettings)
        {
            serializerSettings.SetSerializer(
                x => x.PeriodSerializer = NodaSerializerDefinitions.NormalizingIsoPeriodSerializer);

            return serializerSettings;
        }

        /// <summary>
        ///     Configures the <see cref="ZonedDateTime" /> serializer to use a format compatible 
        ///     with the ToString method. 
        /// </summary>
        /// <param name="serializerSettings">The existing serializer settings to add Noda Time serializerSettings to.</param>
        /// <returns>The original <param ref="serializerSettings" /> value, for further chaining.</returns>
        public static INodaSerializerSettings WithGeneralIsoZonedDateTimeSerializer(
            this INodaSerializerSettings serializerSettings)
        {
            serializerSettings.SetSerializer(
                x => x.ZonedDateTimeSerializer =
                    NodaSerializerDefinitions.CreateZonedDateTimeSerializer(x.Provider,
                        ZonedDateTimePattern.GeneralFormatOnlyIso.PatternText));

            return serializerSettings;
        }

        /// <summary>
        ///     Configuration for ServiceStack.Text with everything required to properly serialize and deserialize NodaTime data
        ///     types to and from json.
        /// </summary>
        /// <param name="provider">The time zone provider to use when parsing time zones and zoned date/times.</param>
        /// <returns>A new Noda serializer settings.</returns>
        public static INodaSerializerSettings CreateDefaultSerializersForNodaTime(this IDateTimeZoneProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }
            return new DefaultNodaSerializerSettings(provider);
        }

        /// <summary>
        ///     Configures the ServiceStack.Text json serializer.
        /// </summary>
        /// <param name="nodaSerializerSettings">The serializer settings to use.</param>
        /// <exception cref="MemberAccessException">
        /// Unable to find the deserializer property or field member on the ServiceStack.Text serializer.
        /// </exception>
        public static void ConfigureSerializersForNodaTime(this INodaSerializerSettings nodaSerializerSettings)
        {
            if (nodaSerializerSettings == null)
            {
                //nothing to do
                return;
            }
            nodaSerializerSettings.AnnualDateSerializer.ConfigureSerializer();
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
        ///     Configures the ServiceStack.Text json serializer.
        /// </summary>
        /// <param name="serializer">The individual serializer to configure.</param>
        /// <exception cref="MemberAccessException">
        /// Unable to find the deserializer property or field member on the ServiceStack.Text serializer.
        /// </exception>
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
                    JsConfigWrapper<T>.SetRawDeserializerMember(serializer.Deserialize);
                }
                else
                {
                    JsConfig<T>.SerializeFn = serializer.Serialize;
                    JsConfigWrapper<T>.SetDeserializerMember(serializer.Deserialize);
                }

                var type = typeof (T);

                if (type.GetTypeInfo().IsValueType)
                {
                    //register nullable
                    var genericMethod = NullableSerializerMethodInfo.MakeGenericMethod(type);
                    genericMethod.Invoke(serializer, new object[] {serializer});
                }
            }
        }

        private static void ConfigureNullableSerializer<T>(this IServiceStackSerializer<T> serializer) where T : struct
        {
            //ServiceStack.Text will never use the serialize / deserialize fn if the value is null 
            //or the text is null or empty.
            if (serializer.UseRawSerializer)
            {
                JsConfig<T?>.RawSerializeFn = arg => serializer.Serialize(arg.Value);
                JsConfigWrapper<T?>.SetRawDeserializerMember(s => serializer.Deserialize(s));
            }
            else
            {
                JsConfig<T?>.SerializeFn = arg => serializer.Serialize(arg.Value);
                JsConfigWrapper<T?>.SetDeserializerMember(s => serializer.Deserialize(s));
            }
        }
    }
}
