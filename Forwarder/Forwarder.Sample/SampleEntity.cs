namespace Forwarder.Sample;

// This code will not compile until you build the project with the Source Generators

public partial class ClassA
{
    [Forward]
    private ClassB _b = new();
}

public class ClassB
{
    private int _integer;

    public int GetStored() => _integer;

    public void Store(int value) => _integer = value;
}

static class ExampleResult
{
    static void Example()
    {
        // var a_instance = new ClassA();
        // a_instance.Store(1);
        // Console.WriteLine(a_instance.GetStored());
    }
}