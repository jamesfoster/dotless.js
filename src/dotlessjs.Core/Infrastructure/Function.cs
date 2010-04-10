﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace dotless.Infrastructure
{
  public abstract class Function
  {
    public string Name { get; set; }
    protected List<Node> Arguments { get; set; }

    public Node Call(IEnumerable<Node> arguments)
    {
      Arguments = arguments.ToList();

      return Evaluate();
    }

    protected abstract Node Evaluate();
  }
}