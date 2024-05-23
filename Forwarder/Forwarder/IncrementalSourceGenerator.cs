using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Forwarder.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Forwarder;

/// <summary>
/// A sample source generator that creates a custom report based on class properties. The target class should be annotated with the 'Generators.ReportAttribute' attribute.
/// When using the source code as a baseline, an incremental source generator is preferable because it reduces the performance overhead.
/// </summary>
[Generator]
public class IncrementalSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the attribute to the compilation.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{ForwardAttributeSourceProvider.AttributeName}.g.cs",
            SourceText.From(ForwardAttributeSourceProvider.AttributeSourceCode, Encoding.UTF8)));

        var forwardedMembers = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                $"{ForwardAttributeSourceProvider.Namespace}.{ForwardAttributeSourceProvider.AttributeName}",
                predicate: IsValidSyntaxNode,
                transform: TransformToForwardedMemberInfo
            )
            .Where(info => info != default);

        context.RegisterSourceOutput(forwardedMembers, (productionContext, info) =>
        {
            productionContext.AddSource(GenerateSourceHint(info), SourceText.From(GenerateSourceString(info), Encoding.UTF8));    
        });
    }

    /// <summary>
    /// Validating the syntax node:
    /// 1. Must be a Field
    /// 2. Must be a single field (ex: ignore multiple declarations on the same node)
    /// </summary>
    private static bool IsValidSyntaxNode(SyntaxNode node, CancellationToken _)
    {
        return node switch
        {
            VariableDeclaratorSyntax variableDeclaration => variableDeclaration.Ancestors().OfType<FieldDeclarationSyntax>().First().Declaration.Variables.Count == 1,
            FieldDeclarationSyntax fieldDeclaration => fieldDeclaration.Declaration.Variables.Count == 1,
            _ => false
        };
    }

    private static ForwardedMemberInfo TransformToForwardedMemberInfo(GeneratorAttributeSyntaxContext context, CancellationToken _)
    {
        var fieldSymbol = (IFieldSymbol)context.TargetSymbol;
        var apiList = BuildForwardedApiInfoListFor(fieldSymbol.Type);

        return new ForwardedMemberInfo(
            context.TargetSymbol.ContainingNamespace.ToString(),
            context.TargetSymbol.ContainingType.Name,
            context.TargetSymbol.Name,
            apiList
        );
    }

    private static List<ApiInfo> BuildForwardedApiInfoListFor(ITypeSymbol member)
    {
        var apiList = new List<ApiInfo>();
        var symbolsToScan = new Queue<ITypeSymbol>();
        symbolsToScan.Enqueue(member);

        while (symbolsToScan.Count > 0)
        {
            var toScan = symbolsToScan.Dequeue();

            // Iterate over all public members of the target field/property
            foreach (var memberSymbol in toScan.GetMembers())
            {
                // Check for fields with the same attribute for nested scanning
                // Todo: use the same conditional logic for validating if a symbol is valid everywhere (IsValidSyntaxNode)
                if (memberSymbol is IFieldSymbol fieldSymbol && 
                    fieldSymbol.GetAttributes().Any(attr => attr.AttributeClass?.Name == ForwardAttributeSourceProvider.AttributeName))
                    symbolsToScan.Enqueue(fieldSymbol.Type);

                // Only forwarding public methods
                if (memberSymbol.DeclaredAccessibility != Accessibility.Public) continue;
                if (memberSymbol is not IMethodSymbol methodSymbol) continue;
                if (methodSymbol.MethodKind is not MethodKind.Ordinary) continue;

                // Collect info
                var methodName = methodSymbol.Name;
                var parameterUsageString = methodSymbol.Parameters.Select(GetParameterUsageString).ToList();
                var fullSignature = GetMethodDeclarationString(methodSymbol);

                // Add method information to the API list
                apiList.Add(new ApiInfo(fullSignature, methodName, parameterUsageString));
            }
        }

        return apiList;

        string GetParameterUsageString(IParameterSymbol symbol)
        {
            if (symbol.RefKind == RefKind.Out)
                return $"out {symbol.Name}";
            else if (symbol.RefKind == RefKind.Ref)
                return $"ref {symbol.Name}";
            else
                return symbol.Name;
        }

        string GetMethodDeclarationString(IMethodSymbol symbol)
        {
            var syntaxNode = (MethodDeclarationSyntax)symbol.DeclaringSyntaxReferences.Single().GetSyntax();
            var declarationOnly = syntaxNode.WithExpressionBody(null).WithSemicolonToken(default);
            var ret = declarationOnly.ToString();
            return ret;
        }
    }

    private static string GenerateSourceHint(ForwardedMemberInfo info)
    {
        return $"{info.ContainingTypeNamespace}.{info.ContainingTypeName}.{info.MemberName}_Forwarded.g.cs";
    }

    private static string GenerateSourceString(ForwardedMemberInfo info)
    {
        var sb = new StringBuilder();
        sb.Append($$"""
                    namespace {{info.ContainingTypeNamespace}};
                    
                    public partial class {{info.ContainingTypeName}}
                    {

                    """);

        foreach (var api in info.ApiInfos)
        {
            sb.AppendLine($"    {api.FullSignature} => {info.MemberName}.{api.MethodName}({string.Join(", ", api.ParameterUsageStrings)});");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}