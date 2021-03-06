﻿namespace Owin
{
    using System;
    using System.Collections.Generic;

    public interface IAppBuilder
    {
        IDictionary<string, object> Properties { get; }

        IAppBuilder Use(object middleware, params object[] args);

        object Build(Type returnType);

        IAppBuilder New();
    }
}
