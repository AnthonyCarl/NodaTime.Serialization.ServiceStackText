using System;
using ServiceStack.Text;

namespace NodaTime.Serialization.ServiceStackText
{
    public class JsConfigWrapper<T>
    {
        public static void SetDeserializerMember(Func<string, T> deserializeFunc)
        {
            JsConfig<T>.DeSerializeFn = deserializeFunc;
        }

        public static void SetRawDeserializerMember(Func<string, T> deserializeFunc)
        {
            JsConfig<T>.RawDeserializeFn = deserializeFunc;
        }
    }
}