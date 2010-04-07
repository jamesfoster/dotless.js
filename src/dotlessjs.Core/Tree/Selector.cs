﻿using System.Linq;
using dotless.Infrastructure;
using dotless.Utils;

namespace dotless.Tree
{
  public class Selector : Node
  {
    private string _css;
    public NodeList<Element> Elements { get; set; }

    public Selector(NodeList<Element> elements)
    {
      Elements = elements;

      if (Elements[0].Combinator.Value == "")
        Elements[0].Combinator.Value = " ";
    }

    public bool Match(Selector other)
    {
      return Elements[0].Value == other.Elements[0].Value;
    }

    public override string ToCSS(Env env)
    {
      if (!string.IsNullOrEmpty(_css)) 
        return _css;

      return _css = Elements.Select(e => e.ToCSS(env)).JoinStrings("");
    }
  }
}