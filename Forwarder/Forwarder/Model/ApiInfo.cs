using System.Collections.Generic;

namespace Forwarder.Model;

internal record struct ApiInfo(
    string FullSignature,
    string MethodName, 
    List<string> ParameterUsageStrings
);