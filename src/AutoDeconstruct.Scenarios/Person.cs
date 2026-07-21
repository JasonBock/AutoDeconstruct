namespace AutoDeconstruct.Scenarios;

[AutoDeconstruct]
public sealed class Person
{
	public Person(string name, uint age, Guid id) =>
		(this.Name, this.Age, this.Id) = 
			(name, age, id);

	public uint Age { get; }
	public Guid Id { get; }
	public string Name { get; }

	//public static void Print(Person person)
	//{
	//	(var age, var id, var name) = person;
	//	Console.WriteLine($"{name}, {age}, {id}");
	//}
}