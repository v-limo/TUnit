﻿using TUnit.Assertions.AssertConditions.Interfaces;
using TUnit.Assertions.AssertionBuilders;

namespace TUnit.Assertions.AssertConditions;

public class EnumerableSatisfiesAssertCondition<TActual, TInner, TExpected> : BaseAssertCondition<TActual>
    where TActual : IEnumerable<TInner?>
{
    private readonly Func<TInner?, Task<TExpected>?> _mapper;
    private readonly Func<IValueSource<TExpected?>, InvokableAssertionBuilder<TExpected?>> _assertionBuilder;
    private readonly string _assertionBuilderExpression;

    public EnumerableSatisfiesAssertCondition(Func<TInner?, Task<TExpected>?> mapper,
        Func<IValueSource<TExpected?>, InvokableAssertionBuilder<TExpected?>> assertionBuilder, string mapperExpression, string assertionBuilderExpression)
    {
        _mapper = mapper;
        _assertionBuilder = assertionBuilder;
        _assertionBuilderExpression = assertionBuilderExpression;
        
        SetSubject(mapperExpression);
    }

    protected override string GetExpectation()
        => $"to satisfy {_assertionBuilderExpression}";

    protected override async Task<AssertionResult> GetResult(TActual? actualValue, Exception? exception)
    {
        if (actualValue is null)
        {
            return AssertionResult.Fail("is null");
        }
        var mergedAsserts = AssertionResult.Passed;
        var i = 0;
        foreach (var itemValue in actualValue)
        {
            var currentIndex = i++;
            var assertionResult = await GetResult(itemValue, exception);
            if (assertionResult.IsPassed)
            {
                continue;
            }
            var failMessage = mergedAsserts.IsPassed ? "items not satisfying the condition were found:" : mergedAsserts.Message;
            failMessage += $"{Environment.NewLine}at [{currentIndex}] {assertionResult.Message}";
            mergedAsserts = AssertionResult.Fail(failMessage);
        }
        return mergedAsserts;
    }
    
    private async Task<AssertionResult> GetResult(TInner? itemValue, Exception? exception)
    {
        var innerItemTask = _mapper(itemValue);
        var innerItem = innerItemTask == null ? default : await innerItemTask;
        var innerAssertionBuilder = new ValueAssertionBuilder<TExpected?>(innerItem, "");
        var assertion = _assertionBuilder(innerAssertionBuilder);
        foreach (var baseAssertCondition in assertion.Assertions)
        {
            var result = await baseAssertCondition.Assert(innerItem, exception, "");
            if (!result.IsPassed)
            {
                return result;
            }
        }
        return AssertionResult.Passed;
    }
}