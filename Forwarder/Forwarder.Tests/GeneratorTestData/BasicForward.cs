namespace Forwarder.Tests.GeneratorTestData;

public class BasicForward : BaseGeneratorTestData
{
    public override string[] GetOriginSource() =>
        ["""
         using Forwarder;

         namespace Forwarder.Samples.BasicForward;

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
         """];

    public override string[] GetExpectedSource() => 
        ["""
         namespace Forwarder.Samples.BasicForward;

         public partial class ClassA
         {
             public int GetStored() => _b.GetStored();
             public void Store(int value) => _b.Store(value);
         }

         """];
}