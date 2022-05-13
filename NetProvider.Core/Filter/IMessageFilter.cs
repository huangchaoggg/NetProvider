namespace NetProvider.Core.Filter
{
    public interface IMessageFilter:IFilter<MessageFilterContext>
    {
    }
    public interface IFilter<T> where T:FilterContext
    {
        void Filter(T context);
    }
}
