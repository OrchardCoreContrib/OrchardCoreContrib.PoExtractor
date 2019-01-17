using Fluid.Ast;
using PoExtractor.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PoExtractor {
    class LiquidVisitor {
        private string _filePath;

        public LocalizableStringCollection Strings { get; }
        public IEnumerable<IStringExtractor<LiquidExpressionContext>> Extractors { get; set; }

        public LiquidVisitor(IEnumerable<IStringExtractor<LiquidExpressionContext>> extractors, LocalizableStringCollection strings) {
            this.Extractors = extractors;
            this.Strings = strings;
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
                    this.Visit(cycle.Values);
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
            foreach (var extractor in this.Extractors) {
                this.Strings.Add(extractor.TryExtract(new LiquidExpressionContext() { Expression = filter, FilePath = _filePath }));
            }

            this.Visit(filter.Input);
        }
    }
}
