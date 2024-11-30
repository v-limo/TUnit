﻿using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using Vogen;

namespace TUnit.TestProject.Bugs._1304;

public class Tests
{
    // This test continues indefinitely without finishing (hangs)
// and, depending on the number of tests in the class, it causes other tests to wait
    [Test]
    [Arguments("\"2c48c152-7cb7-4f51-8f01-704454f36e60\"")]
    [Arguments("invalidNotAGui")]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments(null)]
    public async Task TryParse_InvalidString_ReturnsFailure(string? input, CancellationToken cancellationToken)
    {
        // Act
        bool success = AccountId.TryParse(input, out var id);

        // Assert
        await Assert.That(success).IsFalse();
        await Assert.That(id).IsNull();
    }

// This tests works fine
    [Test]
    [Arguments("\"2c48c152-7cb7-4f51-8f01-704454f36e60\"")]
    [Arguments("invalidNotAGui")]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments(null)]
    public async Task Parse_InvalidString_ThrowsException(string? input)
    {
        await Assert.That(() => AccountId.Parse(input)).ThrowsException();
    }

// This test works fine
    [Test]
    [Arguments("2c48c152-7cb7-4f51-8f01-704454f36e60")]
    [Arguments("00000000-0000-0000-0000-000000000000")]
    public async Task TryParse_ValidString_ReturnsAccountId(string input)
    {
        // Act
        bool success = AccountId.TryParse(input, out var id);

        // Assert
        //using var _ = Assert.Multiple();
        await Assert.That(success).IsTrue();
        await Assert.That(id.HasValue).IsTrue();
        await Assert.That(id.ToString()).IsEqualTo(input);
    }
}

// this is the account id's code
[ValueObject<Guid>]
public readonly partial record struct AccountId : IIdentifiable<AccountId>
{
    public bool HasValue => _value != Guid.Empty;

    public static AccountId Empty => From(Guid.Empty);

    /// <summary>
    /// Builds a new ID from a new <see cref="Guid"/> version 4
    /// </summary>
    public static AccountId New => From(Guid.NewGuid());
}

public interface IIdentifiable<out TId>
{
    bool HasValue { get; }
    static abstract TId Empty { get; }
    static abstract TId New { get; }
}