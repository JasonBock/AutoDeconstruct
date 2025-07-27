How do I handle closed generics correctly? For example:

```c#
[assembly: AutoDeconstruct(typeof(Test<string>))]

public class Test<T>
{ 
  public T Id { get; set; }
  public T Value { get; set; }
}
```

I would need to generate this:

```c#
public static class TestExtensions
{
  public static void Deconstruct(this global::TestSpace.Test<string> @self, out string @id, out string @value)
  {
    global::System.ArgumentNullException.ThrowIfNull(@self);
    (@id, @value) =
      (@self.Id, @self.Value);
  }
}
```

But then what if I do this?

```c#
[assembly: AutoDeconstruct(typeof(Test<string>))]

[AutoDeconstruct]
public class Test<T>
{ 
  public T Id { get; set; }
  public T Value { get; set; }
}
```

Would I generate this?:

```c#
public static class TestExtensions
{
  public static void Deconstruct<T>(this global::TestSpace.Test<T> @self, out T @id, out T @value)
  {
    global::System.ArgumentNullException.ThrowIfNull(@self);
    (@id, @value) =
      (@self.Id, @self.Value);
  }

  public static void Deconstruct(this global::TestSpace.Test<string> @self, out string @id, out string @value)
  {
    global::System.ArgumentNullException.ThrowIfNull(@self);
    (@id, @value) =
      (@self.Id, @self.Value);
  }
}
```

This would be really weird to run into. I may need to make the extension type `partial` so both are generated separately:

```c#
public static partial class TestExtensions
{
  public static void Deconstruct<T>(this global::TestSpace.Test<T> @self, out T @id, out T @value)
  {
    global::System.ArgumentNullException.ThrowIfNull(@self);
    (@id, @value) =
      (@self.Id, @self.Value);
  }
}

public static partial class TestExtensions
{
  public static void Deconstruct(this global::TestSpace.Test<string> @self, out string @id, out string @value)
  {
    global::System.ArgumentNullException.ThrowIfNull(@self);
    (@id, @value) =
      (@self.Id, @self.Value);
  }
}
```

Probably the right thing to do is to disallow closed generics, similar to Rocks. There's no reason to generate for `Test<string>` where `Test<T>` covers that and a lot more.

TODO:
* DONE - Make the extension type `partial`
* DONE - Only open generics, error if a closed one is given (look at Rocks for that validation).
* DONE - Consider moving `GetModel()` to `TypeSymbolModel`. Then this could be use to add an analyzer check.
* DONE - Remove first check for `NoAutoDeconstructAttribute` in `GetModel()`
* Lots of tests
* Create issue to build refactoring to define `[AutoDeconstruct]` at type or assembly level, also allow for project property definition similar to what I do in Rocks.
* (Potentially) create an issue in Transpire to encourage using named optional parameters.