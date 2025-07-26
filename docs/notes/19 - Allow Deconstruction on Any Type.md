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