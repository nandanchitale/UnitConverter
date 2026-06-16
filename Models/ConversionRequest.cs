namespace Models;

public record ConversionRequest
(
    string FromUnit, 
    string ToUnit,
    double Value
);
