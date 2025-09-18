namespace AutoDeconstruct.Scenarios;

public sealed class Person
{
	public Person(string name, uint age, Guid id) =>
		(this.Name, this.Age, this.Id) = 
			(name, age, id);

	public uint Age { get; }
	public Guid Id { get; }
	public string Name { get; }
}