using NUnit.Framework;

namespace dotless.Tests.Specs
{
  public class SelectorsFixture : SpecFixtureBase
  {
    [Test]
    public void SelfSelector1()
    {
      var input =
        @"
h1, h2, h3 {
  a, p {
    &:hover {
      color: red;
    }
  }
}
";

      var expected = @"
h1 a:hover,
h2 a:hover,
h3 a:hover,
h1 p:hover,
h2 p:hover,
h3 p:hover {
  color: red;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void SelfSelector2()
    {
      var input =
        @"
a {
  color: red;

  &:hover { color: blue; }

  div & { color: green; }

  p & span { color: yellow; }
}
";

      var expected = @"
a {
  color: red;
}
a:hover {
  color: blue;
}
div a {
  color: green;
}
p a span {
  color: yellow;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void IdSelectors()
    {
      var input =
        @"
#all { color: blue; }
#the { color: blue; }
#same { color: blue; }
";

      var expected = @"
#all {
  color: blue;
}
#the {
  color: blue;
}
#same {
  color: blue;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void Tag()
    {
      var input = @"
td {
  margin: 0;
  padding: 0;
}
";

      var expected = @"
td {
  margin: 0;
  padding: 0;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void TwoTags()
    {
      var input = @"
td, input {
  line-height: 1em;
}
";

      var expected = @"
td, input {
  line-height: 1em;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void MultipleTags()
    {
      var input =
        @"
ul, li, div, q, blockquote, textarea {
  margin: 0;
}
";

      var expected = @"
ul,
li,
div,
q,
blockquote,
textarea {
  margin: 0;
}
";

      AssertLess(input, expected);
    }
  }
}