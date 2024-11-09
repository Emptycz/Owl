using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Owl.Generators;

[Generator(LanguageNames.CSharp)]
internal class VariableResolverGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor NoClassesFoundDescriptor = new(
        "GEN001", "No Classes Found", "No classes with the attribute 'RegisterToVariableResolver' were found.", "Generator", DiagnosticSeverity.Warning, true);

    private static readonly DiagnosticDescriptor GeneratedCodeEmptyDescriptor = new(
        "GEN002", "Generated Code Empty", "The generated source code is empty.", "Generator", DiagnosticSeverity.Warning, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register syntax nodes to analyze for the "RegisterToVariableResolver" attribute
        var classesWithAttribute = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0,
                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
            .Where(static cls => cls.AttributeLists
                .Any(attrList => attrList.Attributes
                    .Any(attr => attr.Name.ToString() == "MapResolver")))
            .Collect();

        // Get the compilation
        var compilationProvider = context.CompilationProvider;

        // Register the main generation step
        context.RegisterSourceOutput(compilationProvider.Combine(classesWithAttribute), (spc, source) =>
        {
            var (compilation, classes) = source;

            if (classes.IsEmpty)
            {
                spc.ReportDiagnostic(Diagnostic.Create(NoClassesFoundDescriptor, Location.None));
                return;
            }

            var (switchStatement, constructorParams) = GenerateSwitchStatement(compilation, classes);
            var sourceCode = GenerateSourceCode(switchStatement, constructorParams);

            if (string.IsNullOrWhiteSpace(sourceCode))
            {
                spc.ReportDiagnostic(Diagnostic.Create(GeneratedCodeEmptyDescriptor, Location.None));
                return;
            }

            spc.AddSource("Owl.Services.VariableResolvers.VariableResolverFactory.g.cs", sourceCode);
        });
    }

    private static (string switchStatement, string constructorParams) GenerateSwitchStatement(Compilation compilation, IEnumerable<ClassDeclarationSyntax> classes)
    {
        StringBuilder methodBuilder = new();
        List<IParameterSymbol> paramList = [];
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
                .FirstOrDefault(attr => attr.AttributeClass?.Name == "MapResolverAttribute");

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

            methodBuilder.AppendLine($"            {className} {variableName} => new {resolverType.Name}({GenerateParams(variableName, paramString)}),");
        }

        // TODO: Handle duplicates or other necessary logic here
        string constructorParams = string.Join(", ", paramList.Select(p => $"{p.Type.Name} {p.Name}"));
        methodBuilder.AppendLine("             _ => throw new ArgumentException(\"Unknown variable type: \" + variable.GetType().Name)");

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
            methodBuilder.AppendLine($"public partial class VariableResolverFactory({constructorParams})");
        }
        else
        {
            methodBuilder.AppendLine("public partial class VariableResolverFactory()");
        }

        methodBuilder.AppendLine("""
                                 {
                                     private IVariableResolver _getResolver(IVariable variable)
                                     {
                                         return variable switch
                                         {
                                 """);
        methodBuilder.Append(switchStatement);
        methodBuilder.AppendLine("""
                                         };
                                     }
                                 }
                                 """);
        return methodBuilder.ToString();
    }

    private static string GenerateParams(string variableName, string paramList)
    {
        return string.IsNullOrWhiteSpace(paramList) ? variableName : $"{variableName}, {paramList}";
    }
}
