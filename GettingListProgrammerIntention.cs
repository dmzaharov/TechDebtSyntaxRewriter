using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnalysisApp1
{

    [Flags]
    public enum OperationEnum
    {
        Operation1 = 1,
        Operation2 = 2,
        Operation3 = 4
    }

    class GettingListProgrammerIntention 
    {
        public string varName;
        public ArgumentSyntax creationArgument;
        public OperationEnum transformOption;

        public SyntaxNode GetRewrittenNode(SyntaxNode original)
        {
            // var list = GetListCached("HELLO", OperationEnum.Operation1 | OperationEnum.Operation2);
            var opt = from o in transformOption.ToString().Split(',') select "OperationEnum." + o.Trim();
            var syntax = $"var {varName} = GetListCached({creationArgument.ToString()}, {String.Join(" | ", opt)});";
            return SyntaxFactory.ParseStatement(syntax).NormalizeWhitespace().WithTriviaFrom(original); 
        }

    }
}
