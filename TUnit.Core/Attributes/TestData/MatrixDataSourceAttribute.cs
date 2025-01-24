﻿namespace TUnit.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class MatrixDataSourceAttribute : NonTypedDataSourceGeneratorAttribute
{
    public override IEnumerable<Func<object?[]?>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        var parameterInformation = dataGeneratorMetadata
            .MembersToGenerate
            .OfType<SourceGeneratedParameterInformation>()
            .ToArray();

        if (parameterInformation.Length != dataGeneratorMetadata.MembersToGenerate.Length
            || parameterInformation.Length is 0)
        {
            throw new Exception("[MatrixDataSource] only supports parameterised tests");
        }
        
        foreach (var row in GetMatrixValues(parameterInformation.Select(GetAllArguments)))
        {
            yield return () => row.ToArray();
        }
    }

    private IReadOnlyList<object?> GetAllArguments(SourceGeneratedParameterInformation sourceGeneratedParameterInformation)
    {
        var matrixAttribute = sourceGeneratedParameterInformation.Attributes.OfType<MatrixAttribute>().FirstOrDefault();

        if (matrixAttribute is null or { Objects.Length: 0 })
        {
            var type = sourceGeneratedParameterInformation.Type;
            
            if (type == typeof(bool))
            {
                return [true, false];
            }

            if (type.IsEnum)
            {
                return Enum.GetValues(type).Cast<object>().Except(matrixAttribute?.Excluding ?? []).ToArray();
            }
            
            throw new ArgumentNullException($"No MatrixAttribute found for parameter {sourceGeneratedParameterInformation.Name}");
        }

        return matrixAttribute.Objects;
    }
    
    private readonly IEnumerable<IEnumerable<object?>> _seed = [[]];

    private IEnumerable<IEnumerable<object?>> GetMatrixValues(IEnumerable<IReadOnlyList<object?>> elements)
    {
        return elements.Aggregate(_seed, (accumulator, enumerable)
            => accumulator.SelectMany(x => enumerable.Select(x.Append)));
    }
}