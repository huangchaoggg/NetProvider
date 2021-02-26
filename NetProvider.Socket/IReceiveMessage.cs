namespace NetProvider.Sock
{
    public interface IReceiveMessage<T> where T : class
    {
        void Receives(T value);
    }
}
