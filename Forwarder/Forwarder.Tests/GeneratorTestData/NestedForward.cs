namespace Forwarder.Tests.GeneratorTestData;

public class NestedForward : BaseGeneratorTestData
{
    public override string[] GetOriginSource() =>
        ["""
         using Forwarder;

         namespace Forwarder.Samples.NestedForward;

         public partial class ClassA
         {
             [Forward] private readonly ClassB _b = new();
         }

         public partial class ClassB
         {
             [Forward] private readonly ClassC _c = new();
         }

         public class ClassC
         {
             private int _integer;
         
             public void GetStored(out int stored) => stored = _integer;
             public void Store(ref int value) => _integer = value;
         }
         """];

    public override string[] GetExpectedSource() =>
    [
        """
        namespace Forwarder.Samples.NestedForward;

        public partial class ClassA
        {
            public void GetStored(out int stored) => _b.GetStored(out stored);
            public void Store(ref int value) => _b.Store(ref value);
        }

        """,
        """
        namespace Forwarder.Samples.NestedForward;

        public partial class ClassB
        {
            public void GetStored(out int stored) => _c.GetStored(out stored);
            public void Store(ref int value) => _c.Store(ref value);
        }

        """
    ];
}