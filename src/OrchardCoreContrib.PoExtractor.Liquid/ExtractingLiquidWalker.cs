using Fluid.Ast;
using System;
using System.Collections.Generic;

namespace OrchardCoreContrib.PoExtractor.Liquid;

/// <summary>
/// Traverses Fluid AST and extracts localizable strings using provided collection of <see cref="IStringExtractor{T}"/>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExtractingLiquidWalker"/> class
/// </remarks>
/// <param name="extractors">the collection of extractors to use</param>
/// <param name="localizableStrings">the <see cref="LocalizableStringCollection"/> where the results are saved</param>
public class ExtractingLiquidWalker(IEnumerable<IStringExtractor<LiquidExpressionContext>> extractors, LocalizableStringCollection localizableStrings)
{
    private string _filePath;

    private readonly LocalizableStringCollection _localizableStrings = localizableStrings ?? throw new ArgumentNullException(nameof(localizableStrings));
    private readonly IEnumerable<IStringExtractor<LiquidExpressionContext>> _extractors = extractors ?? throw new ArgumentNullException(nameof(extractors));

    /// <summary>
    /// Visits liquid statement.
    /// </summary>
    /// <param name="statementContext">The statement context.</param>
    public void Visit(LiquidStatementContext statementContext)
    {
        ArgumentNullException.ThrowIfNull(statementContext);

        _filePath = statementContext.FilePath;

        Visit(statementContext.Statement);
    }

    private void Visit(Statement node)
    {
        switch (node)
        {
            case AssignStatement assign:
                Visit(assign.Value);
                break;
            case CaseStatement @case:
                Visit(@case.Statements);
                Visit(@case.Whens);
                Visit(@case.Else);
                Visit(@case.Expression);
                break;
            case CycleStatement cycle:
                Visit(cycle.Group);
                Visit(cycle.Values);
                break;
            case ElseIfStatement elseIf:
                Visit(elseIf.Condition);
                Visit(elseIf.Statements);
                break;
            case IfStatement @if:
                Visit(@if.Condition);
                Visit(@if.Statements);
                Visit(@if.ElseIfs);
                Visit(@if.Else);
                break;
            case OutputStatement output:
                Visit(output.Expression);
                Visit(output.Filters);
                break;
            case UnlessStatement unless:
                Visit(unless.Condition);
                Visit(unless.Statements);
                break;
            case WhenStatement @when:
                Visit(when.Options);
                Visit(when.Statements);
                break;
            case TagStatement tag:
                if (tag.Statements != null)
                {
                    foreach (var item in tag.Statements)
                    {
                        Visit(item);
                    }
                }

                break;
        }
    }

    private void Visit(IEnumerable<Statement> statements)
    {
        if (statements == null)
        {
            return;
        }

        foreach (var statement in statements)
        {
            Visit(statement);
        }
    }
    private void Visit(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binary:
                Visit(binary.Left);
                Visit(binary.Right);
                break;
            case FilterExpression filter:
                ProcessFilterExpression(filter);
                break;

        }
    }

    private void Visit(IEnumerable<Expression> expressions)
    {
        if (expressions == null)
        {
            return;
        }

        foreach (var expression in expressions)
        {
            Visit(expression);
        }
    }

    private void ProcessFilterExpression(FilterExpression filter)
    {
        foreach (var extractor in _extractors)
        {
            if (extractor.TryExtract(new LiquidExpressionContext() { Expression = filter, FilePath = _filePath }, out var result))
            {
                _localizableStrings.Add(result);
            }
        }

        Visit(filter.Input);
    }
}
