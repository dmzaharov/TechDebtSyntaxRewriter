using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDebtSyntaxRewriter
{
    class CodeAnalyzer
    {
        public static List<GenericFact> GatherFacts(SyntaxNode syntaxRoot)
        {
            var keyNodes = syntaxRoot.DescendantNodes().OfType<LocalDeclarationStatementSyntax>()
                .Where(m => ((IdentifierNameSyntax)
                            ((InvocationExpressionSyntax)m.Declaration.Variables[0].Initializer.Value)
                            .Expression).Identifier.Value.ToString() == "GetListFromSomewhere").ToArray();
            var detectedFacts = new List<GenericFact>();

            foreach (var keyNode in keyNodes)
            {
                Console.WriteLine($"Rewriting {keyNode.ToString()}");
                var intention = new ListRetrievalFact(keyNode);
                var vardecl = (VariableDeclaratorSyntax)(keyNode.Declaration.Variables[0]);
                intention.varName = vardecl.Identifier.ValueText;
                intention.creationArgument = (((InvocationExpressionSyntax)vardecl.Initializer.Value).ArgumentList.Arguments[0]);
                var block = ((BlockSyntax)keyNode.Parent).Statements;
                var indexOfKeyNode = block.IndexOf(keyNode);
                for (var nodeIndex = indexOfKeyNode + 1; nodeIndex < block.Count; nodeIndex++)
                {
                    var stmt = block[nodeIndex];
                    try
                    {
                        var methodName = ((IdentifierNameSyntax)
                            ((InvocationExpressionSyntax)
                            ((ExpressionStatementSyntax)stmt).Expression).Expression).Identifier.ValueText;
                        OperationEnum methodEnumOption;
                        if (Enum.TryParse<OperationEnum>(methodName, out methodEnumOption))
                        {
                            intention.transformOption = intention.transformOption | methodEnumOption;
                            // schedule removal of operation node 
                            intention.OtherNodes.Add(stmt);
                        }
                        else { break; }
                    }
                    catch { break; }
                }
                // schedule rewrite of key node
                detectedFacts.Add(intention);
            }

            return detectedFacts;

        }

    }
}
