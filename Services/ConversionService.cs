using Models;
using Services;
using System;
using System.Collections.Generic;

namespace UnitConversionApi.Services;

public class ConversionService : IConversionService
{
    private record ConversionEdge(string TargetUnit, Func<double, double> Formula);
    private static readonly Dictionary<string, List<ConversionEdge>> UnitGraph = new(StringComparer.OrdinalIgnoreCase);

    static ConversionService()
    {
        // --- LENGTH ---
        AddDirectionalEdge("meters", "feet", v => v * 3.28084);
        AddDirectionalEdge("feet", "meters", v => v / 3.28084);

        AddDirectionalEdge("feet", "centimeters", v => v * 30.48);
        AddDirectionalEdge("centimeters", "feet", v => v / 30.48);

        // Connect Meters <-> Kilometers
        AddDirectionalEdge("meters", "kilometers", v => v / 1000);
        AddDirectionalEdge("kilometers", "meters", v => v * 1000);

        // --- WEIGHT ---
        AddDirectionalEdge("kilograms", "pounds", v => v * 2.20462);
        AddDirectionalEdge("pounds", "kilograms", v => v / 2.20462);

        // --- TEMPERATURE ---
        AddDirectionalEdge("celsius", "fahrenheit", v => (v * 9 / 5) + 32);
        AddDirectionalEdge("fahrenheit", "celsius", v => (v - 32) * 5 / 9);
    }

    private static void AddDirectionalEdge(string from, string to, Func<double, double> formula)
    {
        if (!UnitGraph.ContainsKey(from)) UnitGraph[from] = new List<ConversionEdge>();
        UnitGraph[from].Add(new ConversionEdge(to, formula));
    }

    public ConversionResponse Convert(string fromUnit, string toUnit, double value)
    {
        if (string.IsNullOrWhiteSpace(fromUnit) || string.IsNullOrWhiteSpace(toUnit))
        {
            throw new ArgumentException("Unit names cannot be empty.");
        }

        // Use the normalization engine here!
        fromUnit = NormalizeUnit(fromUnit);
        toUnit = NormalizeUnit(toUnit);

        if (fromUnit == toUnit)
        {
            return new ConversionResponse(fromUnit, toUnit, value, value);
        }

        if (!UnitGraph.ContainsKey(fromUnit) || !UnitGraph.ContainsKey(toUnit))
        {
            throw new ArgumentException($"Conversion from '{fromUnit}' to '{toUnit}' is not supported.");
        }

        // BFS Pathfinding Engine
        var queue = new Queue<(string CurrentUnit, double CurrentValue)>();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        queue.Enqueue((fromUnit, value));
        visited.Add(fromUnit);

        while (queue.Count > 0)
        {
            var (currentUnit, currentValue) = queue.Dequeue();

            foreach (var edge in UnitGraph[currentUnit])
            {
                if (visited.Contains(edge.TargetUnit)) continue;

                double computedValue = edge.Formula(currentValue);

                if (edge.TargetUnit.Equals(toUnit, StringComparison.OrdinalIgnoreCase))
                {
                    return new ConversionResponse(fromUnit, toUnit, value, Math.Round(computedValue, 4));
                }

                visited.Add(edge.TargetUnit);
                queue.Enqueue((edge.TargetUnit, computedValue));
            }
        }

        throw new ArgumentException($"Conversion from '{fromUnit}' to '{toUnit}' is mathematically invalid.");
    }

    private string NormalizeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit)) return string.Empty;

        unit = unit.Trim().ToLowerInvariant();

        // Standardize common singular/plural variations and abbreviations
        return unit switch
        {
            "meter" or "meters" or "m" => "meters",
            "kilometer" or "kilometers" or "km" => "kilometers",
            "centimeter" or "centimeters" or "cm" => "centimeters",
            "foot" or "feet" or "ft" => "feet",
            "inch" or "inches" or "in" => "inches",
            "kilogram" or "kilograms" or "kg" => "kilograms",
            "pound" or "pounds" or "lb" or "lbs" => "pounds",
            "celsius" or "c" => "celsius",
            "fahrenheit" or "f" => "fahrenheit",
            _ => unit // Fallback to whatever was entered if no match
        };
    }
}