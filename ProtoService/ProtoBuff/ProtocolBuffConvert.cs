using Google.Protobuf;

namespace ProtoService.ProtoBuff
{
    class ProtocolBuffConvert
    {
        public static byte[] Serialize<T>(T obj) where T : IMessage
        {
            byte[] data = obj.ToByteArray();
            return data;
        }

        public static T Deserialize<T>(byte[] data) where T : class, IMessage, new()
        {
            T obj = new T();
            IMessage message = obj.Descriptor.Parser.ParseFrom(data);
            return message as T;
        }
    }
}
