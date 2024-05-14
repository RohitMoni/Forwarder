// This code will not compile until you build the project with the Source Generators

using System;

namespace Forwarder.Sample;

public partial class ClassA
{
    [Forward]
    public ClassB _b = new();
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
        var a = new ClassA();
        a.Store(5);
        var stored = a.GetStored();
        Console.WriteLine(stored);
    }
}