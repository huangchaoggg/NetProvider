using System;
using System.Collections.Generic;
using System.Text;

namespace NetProvider
{
    public interface IReceiveMessage<T> where T:class
    {
        void Receives(T value);
    }
}
