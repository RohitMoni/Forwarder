// This code will not compile until you build the project with the Source Generators

using System;

namespace Forwarder.Samples.InterfaceForward;

public interface IStorage
{
    public int GetStored();
    public void Store(int value);
}

public partial class ClassA : IStorage
{
    [Forward] private readonly ClassB _b = new();
}

public class ClassB : IStorage
{
    private int _integer;
    public int GetStored() => _integer;
    public void Store(int value) => _integer = value;
}

static class ExampleResult
{
    static void StoreAndPrint(IStorage storage)
    {
        storage.Store(5);
        var stored = storage.GetStored();
        Console.WriteLine(stored);
    }
    
    static void Example()
    {
        var a = new ClassA();
        StoreAndPrint(a);
    }
}