using Fluid.Ast;
using PoExtractor.Core;
using PoExtractor.Core.Contracts;
using System.Collections.Generic;

namespace PoExtractor.Liquid {
    /// <summary>
    /// Traverses Fluid AST and extracts localizable strings using provided collection of <see cref="IStringExtractor{T}"/>
    /// </summary>
    class ExtractingLiquidWalker {
        private string _filePath;

        private readonly LocalizableStringCollection _strings;
        private readonly IEnumerable<IStringExtractor<LiquidExpressionContext>> _extractors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractingLiquidWalker"/> class
        /// </summary>
        /// <param name="extractors">the collection of extractors to use</param>
        /// <param name="strings">the <see cref="LocalizableStringCollection"/> where the results are saved</param>
        public ExtractingLiquidWalker(IEnumerable<IStringExtractor<LiquidExpressionContext>> extractors, LocalizableStringCollection strings) {
            _extractors = extractors;
            _strings = strings;
        }

        public void Visit(LiquidStatementContext statementContext) {
            _filePath = statementContext.FilePath;
            this.Visit(statementContext.Statement);
        }

        private void Visit(Statement node) {
            switch (node) {
                case AssignStatement assign: this.Visit(assign.Value); break;
                case CaseStatement @case:
                    this.Visit(@case.Statements);
                    this.Visit(@case.Whens);
                    this.Visit(@case.Else);
                    this.Visit(@case.Expression);
                    break;
                case CycleStatement cycle:
                    this.Visit(cycle.Group);
                    this.Visit(cycle.Values2);
                    break;
                case ElseIfStatement elseIf:
                    this.Visit(elseIf.Condition);
                    this.Visit(elseIf.Statements);
                    break;
                case IfStatement @if:
                    this.Visit(@if.Condition);
                    this.Visit(@if.Statements);
                    this.Visit(@if.ElseIfs);
                    this.Visit(@if.Else);
                    break;
                case OutputStatement output:
                    this.Visit(output.Expression);
                    this.Visit(output.Filters);
                    break;
                case UnlessStatement unless:
                    this.Visit(unless.Condition);
                    this.Visit(unless.Statements);
                    break;
                case WhenStatement @when:
                    this.Visit(when.Options);
                    this.Visit(when.Statements);
                    break;
                case TagStatement tag:
                    if (tag.Statements != null) {
                        foreach (var item in tag.Statements) {
                            this.Visit(item);
                        }
                    }
                    break;
            }
        }

        private void Visit(IEnumerable<Statement> statements) {
            if (statements == null) {
                return;
            }

            foreach (var statement in statements) {
                this.Visit(statement);
            }
        }
        private void Visit(Expression expression) {
            switch (expression) {
                case BinaryExpression binary:
                    this.Visit(binary.Left);
                    this.Visit(binary.Right);
                    break;
                case FilterExpression filter:
                    this.ProcessFilterExpression(filter);
                    break;

            }
        }

        private void Visit(IEnumerable<Expression> expressions) {
            if (expressions == null) {
                return;
            }

            foreach (var expression in expressions) {
                this.Visit(expression);
            }
        }

        private void ProcessFilterExpression(FilterExpression filter) {
            foreach (var extractor in _extractors) {
                if (extractor.TryExtract(new LiquidExpressionContext() { Expression = filter, FilePath = _filePath }, out var result)) {
                    _strings.Add(result);
                }
            }

            this.Visit(filter.Input);
        }
    }
}
