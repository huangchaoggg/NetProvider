namespace NetProvider.Channels
{
    public interface IServiceChannel
    {
        object Invok(Parameters parameters);
    }
    public interface ISocketServiceChannel : IServiceChannel
    {
        void StartClient();
        void Close();
    }
}
