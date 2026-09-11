using LogistikYonetimSistemi.Models.Enums;
using LogistikYonetimSistemi.Patterns.Payment;

namespace LogistikYonetimSistemi.Utils;

public static class PaymentStrategyFactory
{
    public static IPaymentStrategy Create(PaymentType type) => type switch
    {
        PaymentType.CreditCard   => new CreditCardPayment(),
        PaymentType.WireTransfer => new WireTransferPayment(),
        PaymentType.Crypto       => new CryptoPayment(),
        _ => throw new ArgumentException($"Bilinmeyen ödeme tipi: {type}")
    };
}
