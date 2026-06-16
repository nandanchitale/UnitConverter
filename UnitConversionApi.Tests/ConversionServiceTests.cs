using System;
using UnitConversionApi.Services;
using Xunit;

namespace UnitConversionApi.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _service = new ConversionService();

    [Theory]
    // 1. Direct Edge Conversions
    [InlineData("meters", "feet", 1, 3.2808)]
    [InlineData("celsius", "fahrenheit", 0, 32)]
    [InlineData("kilograms", "pounds", 1, 2.2046)]
    // 2. Transitive Graph Path Conversions (Meters -> Feet -> Centimeters)
    [InlineData("meters", "centimeters", 1, 100)]
    [InlineData("centimeters", "meters", 100, 1)]
    // 3. Multi-Step Kilometer Paths (Kilometers -> Meters -> Feet -> Centimeters)
    [InlineData("kilometers", "meters", 1, 1000)]
    [InlineData("meters", "kilometers", 1000, 1)]
    [InlineData("kilometers", "centimeters", 0.01, 1000)]
    public void Convert_ValidGraphPaths_ReturnsAccurateMath(string from, string to, double value, double expected)
    {
        var result = _service.Convert(from, to, value);

        // Asserts float equality within 4 decimal places matching Math.Round precision
        Assert.Equal(expected, result.ConvertedValue, 4);
    }

    [Theory]
    // Singular vs Plural Normalization
    [InlineData("meter", "kilometer", 10, 0.01)]
    [InlineData("kilometers", "meter", 1, 1000)]
    // Common Abbreviation Normalization
    [InlineData("m", "km", 500, 0.5)]
    [InlineData("cm", "m", 250, 2.5)]
    [InlineData("kg", "pounds", 5, 11.0231)]
    // Mixed Casing Edge Cases
    [InlineData("kM", "MeTeR", 2, 2000)]
    [InlineData("C", "F", 25, 77)]
    public void Convert_UnitNormalization_HandlesSingularPluralAndAbbreviations(string from, string to, double value, double expected)
    {
        var result = _service.Convert(from, to, value);

        Assert.Equal(expected, result.ConvertedValue, 4);
    }

    [Fact]
    public void Convert_SameUnitIdentityProperty_ReturnsOriginalValueImmediately()
    {
        // Even with divergent variations ("kilometer" vs "km"), normalization recognizes they are equivalent
        var result = _service.Convert("kilometer", "km", 45.78);

        Assert.Equal(45.78, result.ConvertedValue);
    }

    [Fact]
    public void Convert_CrossCategoryIslands_ThrowsArgumentExceptionForInvalidMath()
    {
        // Both exist in the graph engine, but belong to decoupled categorical islands
        var exception = Assert.Throws<ArgumentException>(() => _service.Convert("km", "celsius", 10));

        Assert.Contains("mathematically invalid", exception.Message);
    }

    [Fact]
    public void Convert_UnknownOrUnsupportedNodes_ThrowsArgumentException()
    {
        // Targets a node completely unrecognized by the configuration
        var exception = Assert.Throws<ArgumentException>(() => _service.Convert("meters", "lightyears", 5));

        Assert.Contains("not supported", exception.Message);
    }

    [Theory]
    [InlineData("", "meters")]
    [InlineData("meters", "   ")]
    [InlineData(null, "kilometers")]
    public void Convert_NullOrWhitespaceInputs_ThrowsArgumentException(string? from, string? to)
    {
        // Enforces parameter safety rules before any graph traversal occurs
        Assert.Throws<ArgumentException>(() => _service.Convert(from!, to!, 10));
    }
}