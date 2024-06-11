// This code will not compile until you build the project with the Source Generators

using System;

namespace Forwarder.Samples.PublicAndInternalForward;

public partial class ClassA
{
    [Forward(AccessModifier.Internal | AccessModifier.Public)] private readonly ClassB _b = new();
}

public class ClassB
{
    private int _integer;
    public int GetStored() => _integer;
    public void Store(int value) => _integer = value;
    internal void Increment() => _integer++;
}

static class ExampleResult
{
    static void Example()
    {
        var a = new ClassA();
        a.Store(5);
        a.Increment();
        Console.Write(a.GetStored());
    }
}