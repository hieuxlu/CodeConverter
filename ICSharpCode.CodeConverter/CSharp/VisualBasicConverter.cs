using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
using VBasic = Microsoft.CodeAnalysis.VisualBasic;
using VBSyntax = Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace ICSharpCode.CodeConverter.CSharp
{
    internal class VisualBasicConverter
    {
        public static async Task<SyntaxNode> ConvertCompilationTree(Document document,
            CSharpCompilation csharpViewOfVbSymbols, Project csharpReferenceProject)
        {
            Project project = document.Project;
            var tree = await document.GetSyntaxTreeAsync();
            return await ConvertCompilationTree(project, tree, csharpReferenceProject, csharpViewOfVbSymbols);
        }

        public static async Task<SyntaxNode> ConvertCompilationTree(Project project, SyntaxTree tree,
            Project csharpReferenceProject, CSharpCompilation csharpViewOfVbSymbols)
        {
            var compilation = await project.GetCompilationAsync();
            var semanticModel = compilation.GetSemanticModel(tree, true);
            var root = (VBasic.VisualBasicSyntaxNode) await tree.GetRootAsync();
            var csSyntaxGenerator = SyntaxGenerator.GetGenerator(csharpReferenceProject);
            var visualBasicSyntaxVisitor = new
                DeclarationNodeVisitor(project, compilation, semanticModel, csharpViewOfVbSymbols, csSyntaxGenerator);
            return await root.Accept(visualBasicSyntaxVisitor.TriviaConvertingVisitor);
        }
    }
}
