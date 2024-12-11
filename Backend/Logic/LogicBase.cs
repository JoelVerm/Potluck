﻿using Microsoft.Extensions.DependencyInjection;

namespace Logic;

public abstract class LogicBase(IPotluckDb db)
{
    protected readonly IPotluckDb db = db;

    public LogicBase(IServiceProvider sp) : this(sp.GetRequiredService<IPotluckDb>())
    {
    }
}