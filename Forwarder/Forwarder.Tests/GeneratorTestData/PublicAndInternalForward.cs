namespace Forwarder.Tests.GeneratorTestData;

public class PublicAndInternalForward : BaseGeneratorTestData
{
    public override string[] GetOriginSource() =>
        ["""
         using Forwarder;

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
         """];

    public override string[] GetExpectedSource() => 
        ["""
         namespace Forwarder.Samples.PublicAndInternalForward;
         
         public partial class ClassA
         {
             public int GetStored() => _b.GetStored();
             public void Store(int value) => _b.Store(value);
             internal void Increment() => _b.Increment();
         }

         """];
}