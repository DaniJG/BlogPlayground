using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogPlayground.Services
{
    public interface IRequestUserProvider
    {
        string GetUserId();
    }
}
