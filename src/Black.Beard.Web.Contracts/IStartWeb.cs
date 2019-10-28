using System;
using System.Collections.Generic;

namespace Black.Beard.Web.Contracts
{
    public interface IStartWeb
    {

        void ConfigureServices(List<object> instance);

        void Configure(List<object> instance);

    }
}
