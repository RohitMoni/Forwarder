// This code will not compile until you build the project with the Source Generators

using System;

namespace Forwarder.Samples.ParameterModifers;

public partial class ClassA
{
    [Forward] private readonly ClassB _b = new();
}

public class ClassB
{
    private int _integer;

    public void GetStored(out int stored) => stored = _integer;
    public void Store(ref int value) => _integer = value;
}

static class ExampleResult
{
    static void Example()
    {
        var a = new ClassA();
        var toStore = 5;
        a.Store(ref toStore);
        a.GetStored(out var stored);
        Console.WriteLine(stored);
    }
}