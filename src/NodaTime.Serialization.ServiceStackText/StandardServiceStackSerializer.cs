using System;
using System.Runtime.Serialization;
using NodaTime.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// A JSON serializer for types which can be represented by a single string value, parsed or formatted
    /// from an <see cref="IPattern{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public class StandardServiceStackSerializer<T> : IServiceStackSerializer<T>
    {
        private readonly IPattern<T> _pattern;

        private readonly Func<string, T> _serviceStackFallbackDeSerializer;

        private readonly Action<T> _serializationValidator;

        public bool UseRawSerializer
        {
            get { return false; }
        }

        /// <summary>
        /// Creates a new instance with a pattern and an optional validator and/or fallback deserializer. 
        /// The validator will be called before each value is written, and may throw an exception to indicate 
        /// that the value cannot be serialized. The fallback serializer will be called when parsing using the 
        /// pattern fails.
        /// </summary>
        /// <param name="pattern">The pattern to use for parsing and formatting.</param>
        /// <param name="serviceStackFallbackDeSerializer"></param>
        /// <param name="serializationValidator">The validator to call before writing values. May be null, indicating that no validation is required.</param>
        /// <exception cref="ArgumentNullException"><paramref name="pattern"/> is null.</exception>
        public StandardServiceStackSerializer(
            IPattern<T> pattern,
            Func<string, T> serviceStackFallbackDeSerializer = null,
            Action<T> serializationValidator = null)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }
            this._pattern = pattern;
            this._serializationValidator = serializationValidator;
            this._serviceStackFallbackDeSerializer = serviceStackFallbackDeSerializer;
        }

        public string Serialize(T value)
        {
            if (this._serializationValidator != null)
            {
                this._serializationValidator(value);
            }
            
            return _pattern.Format(value);
        }

        public T Deserialize(string text)
        {
            var parsedResult = _pattern.Parse(text);

            if (parsedResult.Success)
            {
                return parsedResult.Value;
            }

            if (_serviceStackFallbackDeSerializer == null)
            {
                throw parsedResult.Exception;
            }

            T fallbackObj;

            try
            {
                fallbackObj = _serviceStackFallbackDeSerializer(text);
            }
            catch (SerializationException)
            {
                //If fallback fails, throw original exception.
                throw parsedResult.Exception;
            }

            return fallbackObj;
        }
    }
}