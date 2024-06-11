namespace Forwarder.Tests.GeneratorTestData;

public class InternalForward : BaseGeneratorTestData
{
    public override string[] GetOriginSource() =>
        ["""
         using Forwarder;

         namespace Forwarder.Samples.InternalForward;
         
         public partial class ClassA
         {
             [Forward(AccessModifier.Internal)] private readonly ClassB _b = new();
         }
         
         public class ClassB
         {
             private int _integer;
             public int GetStored() => _integer;
             public void Store(int value) => _integer = value;
         
             internal void Increment() => _integer++;
         }
         """];

    public override string[] GetExpectedSource() => 
        ["""
         namespace Forwarder.Samples.InternalForward;
         
         public partial class ClassA
         {
             internal void Increment() => _b.Increment();
         }

         """];
}