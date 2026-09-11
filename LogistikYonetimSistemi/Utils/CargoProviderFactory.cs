using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Patterns.Cargo;
using LogistikYonetimSistemi.Patterns.Cargo.External;

namespace LogistikYonetimSistemi.Utils;

public static class CargoProviderFactory
{
    public static ICargoProvider Create(CargoType type) => type switch
    {
        CargoType.Aras      => new ArasCargoAdapter(new ArasCargoAPI()),
        CargoType.Yurtici   => new YurticiCargoAdapter(new YurticiAPI()),
        CargoType.GlobalExp => new GlobalExpresAdapter(new GlobalExpresAPI()),
        _ => throw new ArgumentException($"Bilinmeyen kargo tipi: {type}")
    };
}
