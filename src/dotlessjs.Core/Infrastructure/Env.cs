﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dotless.Tree;

namespace dotless.Infrastructure
{
  public class Env
  {
    private Dictionary<string, Type> _functionTypes;

    public Stack<Ruleset> Frames { get; set; }

    public Env()
    {
      Frames = new Stack<Ruleset>();
    }

    public Function GetFunction(string name)
    {
      if (_functionTypes == null)
      {
        var functionType = typeof (Function);

        _functionTypes = Assembly.GetExecutingAssembly()
          .GetTypes()
          .Where(t => functionType.IsAssignableFrom(t) && t != functionType)
          .ToDictionary(t => GetFunctionName(t));
      }

      name = name.ToLowerInvariant();

      if (_functionTypes.ContainsKey(name))
        return (Function) Activator.CreateInstance(_functionTypes[name]);

      return null;
    }

    private static string GetFunctionName(Type t)
    {
      var name = t.Name.ToLowerInvariant();
      return name.EndsWith("function") ? name.Substring(0, name.Length - 8) : name;
    }
  }
}