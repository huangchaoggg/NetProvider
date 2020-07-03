namespace NetProvider
{
    public interface IReceiveMessage<T> where T : class
    {
        void Receives(T value);
    }
}
