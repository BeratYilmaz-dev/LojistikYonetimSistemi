using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Patterns.Cargo;
using LogistikYonetimSistemi.Patterns.Payment;
using LogistikYonetimSistemi.Utils;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

/// <summary>
/// Factory Method deseninin doğru nesneleri ürettiğini test eder.
/// </summary>
public class FactoryTests
{
    // ── PaymentStrategyFactory ────────────────────────────────────────────────
    [Fact]
    public void PaymentFactory_CreditCard_ReturnsCreditCardPayment()
    {
        var payment = PaymentStrategyFactory.Create(PaymentType.CreditCard);
        Assert.IsType<CreditCardPayment>(payment);
        Assert.Equal("Kredi Kartı", payment.MethodName);
    }

    [Fact]
    public void PaymentFactory_WireTransfer_ReturnsWireTransferPayment()
    {
        var payment = PaymentStrategyFactory.Create(PaymentType.WireTransfer);
        Assert.IsType<WireTransferPayment>(payment);
        Assert.Equal("Havale/EFT", payment.MethodName);
    }

    [Fact]
    public void PaymentFactory_Crypto_ReturnsCryptoPayment()
    {
        var payment = PaymentStrategyFactory.Create(PaymentType.Crypto);
        Assert.IsType<CryptoPayment>(payment);
        Assert.Equal("Kripto", payment.MethodName);
    }

    [Fact]
    public void PaymentFactory_Pay_ReturnsTrue()
    {
        // Simüle ödeme her zaman başarılı döner
        var payment = PaymentStrategyFactory.Create(PaymentType.CreditCard);
        Assert.True(payment.Pay(1000));
    }

    // ── CargoProviderFactory ─────────────────────────────────────────────────
    [Fact]
    public void CargoFactory_Aras_ReturnsArasAdapter()
    {
        var cargo = CargoProviderFactory.Create(CargoType.Aras);
        Assert.IsType<ArasCargoAdapter>(cargo);
    }

    [Fact]
    public void CargoFactory_Yurtici_ReturnsYurticiAdapter()
    {
        var cargo = CargoProviderFactory.Create(CargoType.Yurtici);
        Assert.IsType<YurticiCargoAdapter>(cargo);
    }

    [Fact]
    public void CargoFactory_GlobalExpres_ReturnsGlobalExpresAdapter()
    {
        var cargo = CargoProviderFactory.Create(CargoType.GlobalExp);
        Assert.IsType<GlobalExpresAdapter>(cargo);
    }

    [Fact]
    public void CargoFactory_GenerateTrackingNumber_NotEmpty()
    {
        var cargo = CargoProviderFactory.Create(CargoType.Aras);
        var trackingNo = cargo.GenerateTrackingNumber();
        Assert.False(string.IsNullOrWhiteSpace(trackingNo));
    }

    [Fact]
    public void CargoFactory_SendPackage_ReturnsTrue()
    {
        var cargo = CargoProviderFactory.Create(CargoType.Yurtici);
        Assert.True(cargo.SendPackage("ORDER-001"));
    }
}
