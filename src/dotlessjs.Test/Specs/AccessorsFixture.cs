using NUnit.Framework;

namespace dotless.Tests.Specs
{
  public class AccessorsFixture : SpecFixtureBase
  {
    [Test]
    public void Accessors()
    {
      var input = @"
.magic-box {
  @trim: orange;
  content: ""gold"";
  width: 60cm;
  height: 40cm;
  #lock {
    color: silver;
  }
}

#accessors {
  content: .magic-box['content']; // ""gold""
  width: .magic-box['width']; // 60cm
}

.unlock {
  .magic-box;
  color: #lock['color']; // silver
  border-color: .magic-box[@trim]; // orange
}";

      var expected = @"
.magic-box {
  content: ""gold"";
  width: 60cm;
  height: 40cm;
}
.magic-box #lock, .unlock #lock {
  color: silver;
}
#accessors {
  content: ""gold"";
  width: 60cm;
}
.unlock {
  content: ""gold"";
  width: 60cm;
  height: 40cm;
  color: silver;
  border-color: orange;
}
";

      AssertLess(input, expected);
    }
  }
}