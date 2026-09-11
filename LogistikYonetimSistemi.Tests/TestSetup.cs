using Xunit;

// Tüm test sınıflarının sıralı çalışmasını sağlar — Logger dosyasına
// paralel erişim çakışmasını önler.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
