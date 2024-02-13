

namespace ZenithBeepData.ExceptionData
{
    [Serializable]
    public class DataObjectExists : Exception
    {
        public DataObjectExists(string message) : base(message) { }
    }

    public class NotFoundObjectData : Exception
    {
        public NotFoundObjectData(string message) : base(message) { }
    }
}
