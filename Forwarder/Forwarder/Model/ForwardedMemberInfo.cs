using System.Collections.Generic;

namespace Forwarder.Model;

internal record struct ForwardedMemberInfo(
    string ContainingTypeNamespace,
    string ContainingTypeName,
    string MemberName,
    List<ApiInfo> ApiInfos
);