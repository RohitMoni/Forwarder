namespace Forwarder.Tests.GeneratorTestData;

public class CircularForward : BaseGeneratorTestData
{
    public override string[] GetOriginSource() =>
        ["""
         using Forwarder;

         namespace Forwarder.Samples.CircularForward;

         public partial class ClassA
         {
             [Forward]
             private ClassB _b = new();

             internal string InternalFromA() => "A";
         }

         public partial class ClassB
         {
             [Forward(AccessModifier.Internal)]
             private ClassA _a = new();

             public string FromB() => "B";
         }
         """];

    public override string[] GetExpectedSource() =>
    [
        """
        namespace Forwarder.Samples.CircularForward;

        public partial class ClassA
        {
            public string FromB() => _b.FromB();
        }

        """,
        """
        namespace Forwarder.Samples.CircularForward;

        public partial class ClassB
        {
            internal string InternalFromA() => _a.InternalFromA();
        }

        """,
    ];
}
