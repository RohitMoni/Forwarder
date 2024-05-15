namespace Forwarder.Tests.GeneratorTestData;

public class ParameterModifiers : BaseGeneratorTestData
{
    public override string GetOriginSource() =>
        """
        using Forwarder;

        namespace Forwarder.Samples;

        public partial class ClassA
        {
            [Forward]
            private ClassB _b = new();
        }

        public class ClassB
        {
            private int _integer;
        
            public void GetStored(out int stored) => stored = _integer;
            public void Store(ref int value) => _integer = value;
        }
        """;

    public override string GetExpectedSource() => 
        """
        namespace Forwarder.Samples;

        public partial class ClassA
        {
            public void GetStored(out int stored) => _b.GetStored(stored);
            public void Store(ref int value) => _b.Store(value);
        }

        """;
}