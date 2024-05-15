using System.Collections;
using System.Collections.Generic;

namespace Forwarder.Tests.GeneratorTestData;

public abstract class BaseGeneratorTestData : IEnumerable<object[]>
{
    public abstract string[] GetOriginSource();
    public abstract string[] GetExpectedSource();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [GetOriginSource(), GetExpectedSource()];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}