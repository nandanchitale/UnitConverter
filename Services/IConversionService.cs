using Models;

namespace Services;

public interface IConversionService
{
    ConversionResponse Convert(string fromUnit, string toUnit, double value);
}
