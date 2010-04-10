using NUnit.Framework;

namespace dotless.Tests.Specs
{
  public class OperationsFixture : SpecFixtureBase
  {
    [Test]
    public void Operations()
    {
      var input =
        @"
#operations {
  color: #110000 + #000011 + #001100; // #111111
  height: 10px / 2px + 6px - 1px * 2; // 9px
  width: 2 * 4 - 5em; // 3em
  .spacing {
    height: 10px / 2px+6px-1px*2;
    width: 2  * 4-5em;
  }
}

@x: 4;
@y: 12em;

.with-variables {
  height: @x + @y; // 16em
  width: 12 + @y; // 24em
  size: 5cm - @x; // 1cm
}

@z: -2;

.negative {
  height: 2px + @z; // 0px
  width: 2px - @z; // 4px
}

.shorthands {
  padding: -1px 2px 0 -4px; //
}

.colors {
  color: #123; // #112233
  border-color: #234 + #111111; // #334455
  background-color: #222222 - #fff; // black
  .other {
    color: 2 * #111; // #222222
    border-color: #333333 / 3 + #111; // #222222
  }
}
";

      var expected = @"
#operations {
  color: #111111;
  height: 9px;
  width: 3em;
}
#operations .spacing {
  height: 9px;
  width: 3em;
}
.with-variables {
  height: 16em;
  width: 24em;
  size: 1cm;
}
.negative {
  height: 0px;
  width: 4px;
}
.shorthands {
  padding: -1px 2px 0 -4px;
}
.colors {
  color: #123;
  border-color: #334455;
  background-color: black;
}
.colors .other {
  color: #222222;
  border-color: #222222;
}
";

      AssertLess(input, expected);
    }
  }
}