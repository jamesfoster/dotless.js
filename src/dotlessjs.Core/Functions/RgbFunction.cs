﻿using dotless.Infrastructure;
using dotless.Tree;
using dotless.Utils;

namespace dotless.Functions
{
  public class RgbFunction : RgbaFunction
  {
    protected override Node Evaluate()
    {
      Guard.ExpectNumArguments(3, Arguments.Count, this);

      Arguments.Add(new Number(1d, ""));

      return base.Evaluate();
    }
  }
}