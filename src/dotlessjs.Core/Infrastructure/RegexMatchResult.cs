﻿using System.Text.RegularExpressions;

namespace dotless.Infrastructure
{
  public class RegexMatchResult : TextNode
  {
    public Match Match { get; set; }

    public RegexMatchResult(Match match) : base(match.Value)
    {
      Match = match;
    }

    public string this[int index]
    {
      get
      {
        var value = Match.Groups[index].Value;
        return string.IsNullOrEmpty(value) ? null : value;
      }
    }
  }
}