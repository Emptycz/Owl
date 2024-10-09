using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Owl.Generators.IncrementalGenerators;

[Generator]
public class VariableResolverGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register syntax nodes to analyze for the "RegisterToVariableResolver" attribute
        var classesWithAttribute = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static cls => cls.AttributeLists
                .Any(attrList => attrList.Attributes
                    .Any(attr => attr.Name.ToString() == "RegisterToVariableResolver")))
            .Collect();

        // Register the main generation step
        context.RegisterSourceOutput(classesWithAttribute, (spc, classes) =>
        {
            var (switchStatement, constructorParams) = GenerateSwitchStatement(spc.Compilation, classes);
            var sourceCode = GenerateSourceCode(switchStatement, constructorParams);
            spc.AddSource("Owl.Services.VariableResolvers.VariableResolverFactory.g.cs", sourceCode);
        });
    }

    private static (string switchStatement, string constructorParams) GenerateSwitchStatement(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes)
    {
        StringBuilder methodBuilder = new();
        List<IParameterSymbol> paramList = new();
        INamedTypeSymbol? ivariableSymbol = compilation.GetTypeByMetadataName("Owl.Models.Variables.IVariable");

        if (ivariableSymbol is null)
        {
            return (string.Empty, string.Empty); // Handle the error as needed
        }

        foreach (var cls in classes)
        {
            var semanticModel = compilation.GetSemanticModel(cls.SyntaxTree);
            var classSymbol = semanticModel.GetDeclaredSymbol(cls);
            var attributeData = classSymbol?.GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == "RegisterToVariableResolver");

            if (attributeData is null) continue;

            var resolverType = attributeData.ConstructorArguments.FirstOrDefault().Value as INamedTypeSymbol;

            if (resolverType is null) continue;

            // Check constructor parameters of the resolver
            var requiredParams = resolverType.Constructors
                .SelectMany(c => c.Parameters)
                .Where(p => p.Type.BaseType?.Interfaces.FirstOrDefault()?.Name != "IVariable")
                .ToArray();

            if (requiredParams.Length > 0)
            {
                paramList.AddRange(requiredParams);
            }

            string className = cls.Identifier.Text;
            string variableName = char.ToLowerInvariant(className[0]) + className[1..]; // variable name in camel case

            // Generate the case for each variable type
            string paramString = string.Join(", ", requiredParams.Select(p => $"{p.Name}"));

            methodBuilder.AppendLine($"        {className} {variableName} => new {resolverType.Name}({GenerateParams(variableName, paramString)}),");
        }

        // Handle duplicates or other necessary logic here
        string constructorParams = string.Join(", ", paramList.Select(p => $"{p.Type.Name} {p.Name}"));
        methodBuilder.AppendLine("        _ => throw new ArgumentException(\"Unknown variable type: \" + variable.GetType().Name)");

        return (methodBuilder.ToString(), constructorParams);
    }

    private static string GenerateSourceCode(string switchStatement, string constructorParams)
    {
        var methodBuilder = new StringBuilder();

        // Generate the constructor for VariableResolverFactory
        methodBuilder.AppendLine("""
                             // <auto-generated/>
                             #pragma warning disable
                             #nullable enable

                             using System;
                             using Owl.Models.Variables;
                             using Owl.States;
                             using Owl.Repositories.RequestNode;

                             namespace Owl.Services.VariableResolvers;
                             """);

        if (!string.IsNullOrEmpty(constructorParams))
        {
            methodBuilder.AppendLine($"public partial class VariableResolverFactory({constructorParams}) : IVariableResolverFactory");
        }
        else
        {
            methodBuilder.AppendLine("public partial class VariableResolverFactory() : IVariableResolverFactory");
        }

        methodBuilder.AppendLine("{");
        methodBuilder.AppendLine("private IVariableResolver SourceGenMapping(IVariable variable)");
        methodBuilder.AppendLine("{");
        methodBuilder.AppendLine("return variable switch");
        methodBuilder.AppendLine("{");
        methodBuilder.Append(switchStatement);
        methodBuilder.AppendLine("};");
        methodBuilder.AppendLine("}");
        methodBuilder.AppendLine("}");

        return methodBuilder.ToString();
    }

    private static string GenerateParams(string variableName, string paramList)
    {
        return string.IsNullOrWhiteSpace(paramList) ? variableName : $"{variableName}, {paramList}";
    }
}
