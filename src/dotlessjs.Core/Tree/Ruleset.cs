﻿using System.Collections.Generic;
using System.Linq;
using dotless.Infrastructure;
using dotless.Utils;

namespace dotless.Tree
{
  public class Ruleset : Node
  {
    public NodeList<Selector> Selectors { get; set; }
    public List<Node> Rules { get; set; }
    public bool Root { get; set; }

    private Dictionary<string, NodeList> _lookups;
    private Dictionary<string, Rule> _variables;


    public Ruleset(NodeList<Selector> selectors, List<Node> rules)
      : this()
    {
      Selectors = selectors;
      Rules = rules;
    }

    protected Ruleset()
    {
      _lookups = new Dictionary<string, NodeList>();
    }

    public Rule Variable(string name)
    {
      if (_variables == null)
      {
        _variables = new Dictionary<string, Rule>();

        var variables = Rules.Where(r => r is Rule).Cast<Rule>().Where(r => r.Variable);

        foreach (var variable in variables)
        {
          _variables[variable.Name] = variable;
        }
      }

      if(_variables.ContainsKey(name))
        return _variables[name];

      return null;
    }

    public List<Ruleset> Rulesets()
    {
      // Note: No noticable slow down by caching ruleset. Removing this allows dynamic parameterised mixins.
//      if (_rulesets != null)
//        return _rulesets;

      return Rules.Where(r => r is Ruleset).Cast<Ruleset>().ToList();
    }
    
    public NodeList Find(Selector selector, Ruleset self)
    {
      self = self ?? this;
      var rules = new NodeList();
      var key = selector.ToCSS(null);

      if (_lookups.ContainsKey(key))
        return _lookups[key];

      foreach (var rule in Rulesets().Where(rule => rule != self))
      {
        if (rule.Selectors && rule.Selectors.Any(selector.Match))
        {
          if (selector.Elements.Count > 1)
          {
            var remainingSelectors = new Selector(new NodeList<Element>(selector.Elements.Skip(1)));
            rules.AddRange(rule.Find(remainingSelectors, self));
          }
          else
            rules.Add(rule);
        }
      }
      return _lookups[key] = rules;
    }

    public virtual bool MatchArguements(NodeList<Expression> arguements, Env env)
    {
      return arguements == null || arguements.Count == 0;
    }

    //
    // Entry point for code generation
    //
    //     `context` holds an array of arrays.
    //
    public override string ToCSS(Env env)
    {
      return ToCSS(new List<IEnumerable<Selector>>(), env);
    }

    public virtual string ToCSS(List<IEnumerable<Selector>> context, Env env)
    {
      if(!Rules.Any())
        return "";

      var css = new List<string>();                   // The CSS output
      var rules = new List<string>();                 // node.Rule instances
      var rulesets = new List<string>();              // node.Ruleset instances
      var paths = new List<IEnumerable<Selector>>();  // Current selectors

      if (!Root)
      {
        if (context.Count == 0)
          paths = Selectors.Select(s => (IEnumerable<Selector>) new List<Selector> {s}).ToList();
        else
        {
          foreach (var sel in Selectors)
          {
            paths.AddRange(context.Select(t => t.Concat(new[] {sel})));
          }
        }
      }
      else
      {
        env = env ?? new Env();

        NodeHelper.ExpandNodes<Import>(env, this);
      }

      // push the current ruleset to the frames stack
      env.Frames.Push(this);

      // Evaluate mixins
      NodeHelper.ExpandNodes<Mixin.Call>(env, this);

      // Evaluate rules and rulesets
      foreach (var rule in Rules)
      {
        if (rule is Ruleset)
        {
          var ruleset = (Ruleset) rule;
          rulesets.Add(ruleset.ToCSS(paths, env));
        }
        else if (rule is Comment)
        {
          if (Root)
            rulesets.Add(rule.ToCSS(env));
          else
            rules.Add(rule.ToCSS(env));
        }
        else if (rule is Rule)
        {
          var r = (rule as Rule);

          if (!r.Variable)
            rules.Add(r.ToCSS(env));
        }
        else if(!(rule is TextNode))
        {
          rules.Add(rule.ToCSS(env));
        }
      }


      var rulesetsStr = rulesets.JoinStrings("");

      // If this is the root node, we don't render
      // a selector, or {}.
      // Otherwise, only output if this ruleset has rules.
      if (Root)
        css.Add(rules.JoinStrings("\n"));
      else
      {
        if (rules.Count > 0)
        {
          var selector = paths.Select(p => p.Select(s => s.ToCSS(null)).JoinStrings("").Trim()).
            JoinStrings(paths.Count > 3 ? ",\n" : ", "); // The fully rendered selector
          css.Add(selector);
          css.Add(" {\n  " + rules.JoinStrings("\n  ") + "\n}\n");
        }
      }
      css.Add(rulesetsStr);

      // Pop the stack
      env.Frames.Pop();

      return css.JoinStrings("");
    }

    public override string ToString()
    {
      var format = "{0}{{{1}}}";
      return Selectors != null && Selectors.Count > 0
               ? string.Format(format, Selectors.Select(s => s.ToCSS(null)).JoinStrings(""), Rules.Count)
               : string.Format(format, "*", Rules.Count);
    }
  }
}
