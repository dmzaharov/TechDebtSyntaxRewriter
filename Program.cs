using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace TechDebtSyntaxRewriter
{
    class Program
    {

        public static void processList()
        {
            var list = GetListFromSomewhere("HELLO");
            Operation1(list);
            Operation2(list);
            // process list
            // ...

            var list2 = GetListFromSomewhere("BONJOUR");
            Operation1(list2);
            Operation3(list2);
            // process list
            // ...
        }

        public static List<string> GetListFromSomewhere(string keyword)
        {
            return new List<string>();
        }

        public static void processListOptimized()
        {
            var list = GetListCached("HELLO", OperationEnum.Operation1 | OperationEnum.Operation2);
            var list2 = GetListCached("BONJOUR", OperationEnum.Operation1 | OperationEnum.Operation3);
        }

        public static void Operation1(List<string> list)
        {
            // ...
        }

        public static void Operation2(List<string> list)
        {
            // ...
        }

        public static void Operation3(List<string> list)
        {
            // ...
        }

        public static List<String> GetListCached(string keyword, OperationEnum transform)
        {
            // to be done
            return new List<string>();
        }

        public class SimpleNodeRewriter : CSharpSyntaxRewriter
        {
            private Dictionary<SyntaxNode, SyntaxNode> nodeRewrites;
            public SimpleNodeRewriter(Dictionary<SyntaxNode, SyntaxNode> nodeRewrites)
            {
                this.nodeRewrites = nodeRewrites;
            }

            public override SyntaxNode Visit(SyntaxNode node)
            {
                SyntaxNode newNode = null;
                if (node != null && nodeRewrites.TryGetValue(node, out newNode)) return newNode;
                return base.Visit(node);
            }

        }

        public static void Main(string[] args)
        {

            //A syntax tree with an unnecessary semicolon on its own line
            var tree = CSharpSyntaxTree.ParseText(@"
    public class Sample
    {
       public void ShowExample()
       {
          {
            var list = GetListFromSomewhere(""HELLO"");
            Operation1(list);
            Operation2(list);
            // process list
            // ...

            var list2 = GetListFromSomewhere(""BONJOUR"" + ObtainSuffix(""PARIS""));
            Operation1(list2);
            Operation3(list2);
            // process list
            // ...
        }
    }
    }");

            // analyze tree and detect facts of intention
            var syntaxRoot = tree.GetRoot();
            var detectedFacts = CodeAnalyzer.GatherFacts(syntaxRoot);
            
            // prepare node rewrites and removals 
            Dictionary<SyntaxNode, SyntaxNode> nodeRewrites = new Dictionary<SyntaxNode, SyntaxNode>();
            foreach (var intention in detectedFacts) 
            {
                // replace key node with rewritten node
                nodeRewrites.Add(intention.KeyNode, intention.GetRewrittenNode());
                foreach (var nodeToRemove in intention.OtherNodes)
                {
                    // remove other nodes linked to fact of intention
                    nodeRewrites.Add(nodeToRemove, null);
                }
            }

            var rewriter = new SimpleNodeRewriter(nodeRewrites);
            var transformedSyntaxTree = rewriter.Visit(syntaxRoot);
            Console.WriteLine(transformedSyntaxTree.ToFullString());

        }


    }
}
