# Lojistik Yönetim Sistemi / Logistics Management System

Sipariş, kargo, ödeme ve stok yaşam döngüsünü yöneten ASP.NET Core MVC uygulaması.  
An ASP.NET Core MVC application managing order, shipping, payment, and inventory lifecycles.

---

## 🇹🇷 Türkçe

### Genel Bakış
Bu proje; e-ticaret ve lojistik süreçlerini (sipariş takibi, dinamik kargo hesaplama, çoklu ödeme yöntemleri ve kritik stok yönetimi) GoF tasarım desenleri ve nesne yönelimli programlama prensipleriyle modelleyen kurumsal bir web uygulamasıdır.

### Uygulanan Tasarım Desenleri
- **State:** Sipariş durum makinesi (`Beklemede`, `Onaylandı`, `Hazırlanıyor`, `Kargoda`, `Teslim Edildi`, `İptal`, `İade`). Geçersiz durum geçişleri engellenir.
- **Adapter:** Farklı arayüzlere sahip harici kargo servislerinin (Aras, Yurtiçi, Global Express) sisteme uyarlanması.
- **Strategy:** Değiştirilebilir ödeme algoritmaları (Kredi Kartı, Kripto, Havale/EFT).
- **Decorator:** Temel kargo ücretine kırılabilir ürün koruması ve kargo sigortası gibi ek maliyetlerin dinamik eklenmesi.
- **Builder & Composite:** Tekil ürünler ile paket/set ürünlerin hiyerarşik olarak oluşturulması ve fiyat/stok yönetimi.
- **Observer:** Stok kritik seviyenin altına düştüğünde çalışan e-posta ve sistem bildirim mekanizması.
- **Facade & Proxy:** Sipariş ve stok alt sistemlerini tek merkezden yöneten ve rol bazlı erişim kontrolü (RBAC) sağlayan arayüz (`AuthorizedOrderFacade`).
- **Factory Method:** Ödeme, kargo ve kullanıcı nesnelerinin çalışma zamanında üretimi.
- **Singleton:** Dosya tabanlı thread-safe loglama yöneticisi (`Logger`).

### Çalıştırma ve Test

```bash
# Çözümü derle
dotnet build

# Birim testlerini çalıştır (55 xUnit testi)
dotnet test

# Web uygulamasını başlat
dotnet run --project LogistikYonetimSistemi
```

### Proje Dokümanları
* [Tasarım Raporu](tasarim_raporu.md)
* [Analiz Raporu](analiz_raporu.md)
* [Test Raporu](TEST_RAPORU.md)
* [Mimari Diyagramlar](diyagramlar/)

---

## 🇬🇧 English

### Overview
An enterprise-grade logistics and order management platform built with ASP.NET Core MVC. It demonstrates practical implementations of GoF design patterns to manage order lifecycles, dynamic shipping calculations, multi-provider logistics, and real-time inventory notifications.

### Implemented Design Patterns
- **State:** Strict order state machine transitions (`Pending`, `Approved`, `Preparing`, `InCargo`, `Delivered`, `Cancelled`, `Return`).
- **Adapter:** Standardizes disparate third-party carrier APIs (Aras, Yurtici, Global Express).
- **Strategy:** Interchangeable payment processing strategies (Credit Card, Crypto, Bank Transfer).
- **Decorator:** Dynamic fee augmentation for shipping options (fragile protection, insurance).
- **Builder & Composite:** Hierarchical bundling for standalone and composite product structures.
- **Observer:** Event-driven low-stock alerts dispatched via email and in-app notifications.
- **Facade & Proxy:** Unified sub-system entry point combined with role-based access control (`AuthorizedOrderFacade`).
- **Factory Method:** Runtime instantiation for payments, carriers, and user roles.
- **Singleton:** Thread-safe, centralized file logger (`Logger`).

### Build & Test

```bash
# Build the solution
dotnet build

# Execute unit test suite (55 xUnit tests)
dotnet test

# Run the web application
dotnet run --project LogistikYonetimSistemi
```

### Documentation
* [Design Report (TR)](tasarim_raporu.md)
* [Analysis Report (TR)](analiz_raporu.md)
* [Test Report (TR)](TEST_RAPORU.md)
* [Architecture Diagrams](diyagramlar/)
