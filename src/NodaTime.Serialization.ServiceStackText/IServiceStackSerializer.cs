namespace NodaTime.Serialization.ServiceStackText
{
    /// <summary>
    /// A JSON serializer for types which can be represented by a single string value, parsed or formatted.
    /// </summary>
    /// <typeparam name="T">The type to convert to/from JSON.</typeparam>
    public interface IServiceStackSerializer<T>
    {
        /// <summary>
        /// Serializes an object of <typeparamref name="T"/> to JSON.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize(T value);
        /// <summary>
        /// Deserializes JSON to an object of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        T Deserialize(string text);
        /// <summary>
        /// When true, JsConfig RawSerializeFn and RawDeserializeFn are set.
        /// Otherwise SerializeFn and DeSerializeFn are set.
        /// </summary>
        bool UseRawSerializer { get; }
    }
}