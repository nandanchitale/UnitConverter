namespace Models;

public record ConversionResponse
(
    string FromUnit, 
    string ToUnit,
    double originalValue,
    double ConvertedValue
);
