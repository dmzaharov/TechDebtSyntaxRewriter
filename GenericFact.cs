using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDebtSyntaxRewriter
{
    public abstract class GenericFact
    {
        public SyntaxNode KeyNode { get; set; }
        public List<SyntaxNode> OtherNodes = new List<SyntaxNode>();

        public abstract SyntaxNode GetRewrittenNode();

        public GenericFact(SyntaxNode keyNode)
        {
            this.KeyNode = keyNode;
        }

    }
}
