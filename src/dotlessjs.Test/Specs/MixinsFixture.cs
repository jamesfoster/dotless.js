using NUnit.Framework;

namespace dotless.Tests.Specs
{
  public class MixinsFixture : SpecFixtureBase
  {
    [Test]
    public void Mixins()
    {
      var input =
        @"
.mixin { border: 1px solid black; }
.mixout { border-color: orange; }
.borders { border-style: dashed; }

#namespace {
  .borders {
    border-style: dotted;
  }
  .biohazard {
    content: ""death"";
    .man {
      color: transparent;
    }
  }
}
#theme {
  > .mixin {
    background-color: grey;
  }
}
#container {
  color: black;
  .mixin;
  .mixout;
  #theme > .mixin;
}

#header {
  .milk {
    color: white;
//    .mixin, #theme > .mixin;   // comma separated mixins not yet supported.
    .mixin;
    #theme > .mixin;
  }
  #cookie {
    .chips {
      #namespace .borders;
      .calories {
        #container;
      }
    }
    .borders;
  }
}
.secure-zone { #namespace .biohazard .man; }
.direct {
  #namespace > .borders;
}

";

      var expected = @"
.mixin {
  border: 1px solid black;
}
.mixout {
  border-color: orange;
}
.borders {
  border-style: dashed;
}
#namespace .borders {
  border-style: dotted;
}
#namespace .biohazard {
  content: ""death"";
}
#namespace .biohazard .man {
  color: transparent;
}
#theme > .mixin {
  background-color: grey;
}
#container {
  color: black;
  border: 1px solid black;
  border-color: orange;
  background-color: grey;
}
#header .milk {
  color: white;
  border: 1px solid black;
  background-color: grey;
}
#header #cookie {
  border-style: dashed;
}
#header #cookie .chips {
  border-style: dotted;
}
#header #cookie .chips .calories {
  color: black;
  border: 1px solid black;
  border-color: orange;
  background-color: grey;
}
.secure-zone {
  color: transparent;
}
.direct {
  border-style: dotted;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void ChildSelector()
    {
      var input =
        @"
#bundle {
  p {
    padding: 20px;
    color: purple;
  }
}

#header {
  #bundle > p;
}
";

      var expected = @"
#bundle p {
  padding: 20px;
  color: purple;
}
#header {
  padding: 20px;
  color: purple;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void MixinNestedRules()
    {
      var input =
        @"
.bundle() {
  p {
    padding: 20px;
    color: purple;
    a { margin: 0; }
  }
}

#header {
  .bundle;
}
";

      var expected = @"
#header p {
  padding: 20px;
  color: purple;
}
#header p a {
  margin: 0;
}
";

      AssertLess(input, expected);
    }

    [Test]
    public void MultipleMixins()
    {
      var input = @"
.mixin{
	border:solid 1px red;
}
.mixin{
	color:blue;
}
.mix-me-in{
	.mixin;
}
";

      var expected = @"
.mixin {
  border: solid 1px red;
}
.mixin {
  color: blue;
}
.mix-me-in {
  border: solid 1px red;
  color: blue;
}
";

      AssertLess(input, expected);
    }


    [Test]
    public void MixinWithArgs()
    {
      var input =
@".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.mixin-arg {
  .mixin(4px, 21%);
}";

      var expected =
@".mixin-arg {
  width: 20px;
  height: 20%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void CanPassNamedArguments()
    {
      var input =
@".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.named-arg {
  color: blue;
  .mixin(@b: 100%);
}";

      var expected =
@".named-arg {
  color: blue;
  width: 5px;
  height: 99%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void CanPassVariablesAsPositionalArgs()
    {
      var input =
@".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.class {
  @var: 20px;
  .mixin(@var);
}";

      var expected =
@".class {
  width: 100px;
  height: 49%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void CanPassVariablesAsNamedArgs()
    {
      var input =
@".mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
 
.class {
  @var: 20%;
  .mixin(@b: @var);
}";

      var expected =
@".class {
  width: 5px;
  height: 19%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void MixedPositionalAndNamedArguments()
    {
      var input =
@".mixin (@a: 1px, @b: 50%, @c: 50) {
  width: @a * 5;
  height: @b - 1%;
  color: #000000 + @c;
}
 
.mixed-args {
  .mixin(3px, @c: 100);
}";

      var expected =
@".mixed-args {
  width: 15px;
  height: 49%;
  color: #646464;
}";

      AssertLess(input, expected);
    }
    
    [Test]
    public void PositionalArgumentsMustAppearBeforeAllNamedArguments()
    {
      var input =
@".mixin (@a: 1px, @b: 50%, @c: 50) {
  width: @a * 5;
  height: @b - 1%;
  color: #000000 + @c;
}
 
.oops {
  .mixin(@c: 100, 3px);
}";

      AssertError("Positional arguments must appear before all named arguments. in '.mixin(@c: 100, 3px)'", input);
    }

    [Test]
    public void ThrowsIfArumentNotFound()
    {
      var input =
@".mixin (@a: 1px, @b: 50%) {
  width: @a * 3;
  height: @b - 1%;
}
 
.override-inner-var {
  .mixin(@var: 6);
}";

      AssertError("Argument '@var' not found. in '.mixin(@var: 6)'", input);
    }

    [Test]
    public void ThrowsIfTooManyArguments()
    {
      var input =
          @"
.mixin (@a: 5) {  width: @a * 5; }

.class { .mixin(1, 2, 3); }";

      AssertError("Expected at most 1 arguments in '.mixin(1, 2, 3)', found 3", input);
    }
    
    [Test]
    public void MixinWithArgsInsideNamespace()
    {
      var input =
@"#namespace {
  .mixin (@a: 1px, @b: 50%) {
    width: @a * 5;
    height: @b - 1%;
  }
}

.namespace-mixin {
  #namespace .mixin(5px);
}";

      var expected =
@".namespace-mixin {
  width: 25px;
  height: 49%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void NestedParameterizedMixins()
    {
      var input =
@".outer(@a: 5) {
  .inner (@b: 10) {
    width: @a + @b;
  }
}

.class1 {
  .outer;
}

.class2 {
  .outer;
  .inner;
}

.class3 {
  .outer .inner;
}

.class4 {
  .outer(1);
  .inner(2);
}

.class5 {
  .outer(2) .inner(4);
}";

      var expected =
@".class2, .class3 {
  width: 15;
}
.class4 {
  width: 3;
}
.class5 {
  width: 6;
}";

      AssertLess(input, expected);
    }


    [Test]
    public void CanUseVariablesAsDefaultArgumentValues()
    {
      var input =
@"@var: 5px;

.mixin (@a: @var, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

      var expected =
@".class {
  width: 25px;
  height: 49%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void ArgumentsOverridesVariableInSameScope()
    {
      var input =
@"@a: 10px;

.mixin (@a: 5px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

      var expected =
@".class {
  width: 25px;
  height: 49%;
}";

      AssertLess(input, expected);
    }

    [Test, Ignore("Infinite Loop - breaks tester")]
    public void CanUseArgumentsWithSameNameAsVariable()
    {
      var input =
@"@a: 5px;

.mixin (@a: @a, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}


.class {
  .mixin;
}";

      var expected =
@".class {
  width: 25px;
  height: 49%;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void CanNestParameterizedMixins()
    {
      var input = @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@width: 20px) {
  width: @width;
  .inner(10px);
}

.class {
 .outer(12px);
}";

      var expected = @"
.class {
  width: 12px;
  font-size: 10px;
}";

      AssertLess(input, expected);
    }

    [Test]
    public void CanNestParameterizedMixinsWithDefaults()
    {
      var input = @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@width: 20px) {
  width: @width;
  .inner();
}

.class {
 .outer();
}";

      var expected = @"
.class {
  width: 20px;
  font-size: 12px;
}";

      AssertLess(input, expected);
    }


    [Test]
    public void CanNestParameterizedMixinsWithSameParameterNames()
    {
      var input = @"
.inner(@size: 12px) {
  font-size: @size;
}

.outer(@size: 20px) {
  width: @size;
  .inner(14px);
}

.class {
 .outer(16px);
}";

      var expected = @"
.class {
  width: 16px;
  font-size: 14px;
}";

      AssertLess(input, expected);
    }
  }
}