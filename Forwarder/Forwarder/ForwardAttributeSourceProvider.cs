﻿namespace Forwarder;

internal static class ForwardAttributeSourceProvider
{
    public const string Namespace = "Forwarder";
    public const string AttributeName = "ForwardAttribute";
    public const int AccessModifierParameterIndex = 0;
    public const string AccessModifierParameterName = "access";
    public const AccessModifier AccessModifierDefault = AccessModifier.Public;

    public const string AttributeSourceCode = $@"// <auto-generated/>
namespace {Namespace}
{{
    /// <summary>
    /// Specifies the access modifier(s) to use when looking for members to forward.
    /// Multiple modifiers can be combined using the bitwise OR (|) operator
    /// </summary>
    [System.Flags]
    public enum AccessModifier
    {{
        Internal = 1,
        Public = 2,
    }}

    /// <summary>
    /// Generates members in the owning class that forward calls to the specified field instance.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class {AttributeName} : System.Attribute
    {{
        public AccessModifier Access;
        public {AttributeName} (AccessModifier {AccessModifierParameterName} = AccessModifier.Public)
        {{
            Access = access;
        }}
    }}
}}";
}