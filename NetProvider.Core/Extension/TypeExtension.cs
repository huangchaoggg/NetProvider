using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetProvider.Core.Extension
{
    public static class TypeExtension
    {
        public static bool IsTask(this Type type)
        {
            if (type == typeof(Task) || type.GetInterfaces().Count(s => s == typeof(IAsyncResult)) > 0)
            {
                return true;
            }
            return false;
        }
    }
}
