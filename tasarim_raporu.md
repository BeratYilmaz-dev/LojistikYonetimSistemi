# Nesneye Dayalı Analiz ve Tasarım — Dönem Sonu Projesi
## TASARIM RAPORU

**Proje Adı:** Akıllı Tedarik ve Lojistik Yönetim Sistemi  
**Öğrenci No:** B221210034  
**Ad Soyad:** Berat Yılmaz  
**Tarih:** Mayıs 2026

---

## İÇİNDEKİLER

1. Mimari Genel Bakış (MVC)
2. SOLID Prensipleri Uygulaması
3. Tasarım Deseni Seçim Tablosu
4. Tasarım Desenlerinin Teknik Açıklamaları
   - 4.1 Singleton — Logger
   - 4.2 Factory Method — Kargo ve Ödeme Fabrikaları
   - 4.3 Builder — Bileşik Ürün Oluşturma
   - 4.4 Adapter — Kargo Firması Entegrasyonu
   - 4.5 Decorator — Kargo Ücreti Hesaplama
   - 4.6 Facade — Sipariş Süreci
   - 4.7 Observer — Stok Bildirim Sistemi
   - 4.8 Strategy — Ödeme Yöntemi Seçimi
   - 4.9 State — Sipariş Durum Yönetimi
   - 4.10 Composite — Ürün Hiyerarşisi
   - 4.11 Protection Proxy — Servis Katmanı Yetkilendirmesi
   - 4.12 Factory Method (Kullanıcı) — UserFactory

---

## 1. Mimari Genel Bakış (MVC)

Uygulama, Model-View-Controller (MVC) mimarisi üzerine kurgulanmıştır. Katmanlar ve içerdikleri bileşenler aşağıdaki tabloda özetlenmiştir:

| Katman | Klasör | İçerik |
|---|---|---|
| **Model** | `Models/` | `Order`, `OrderItem`, `SimpleProduct`, `CompositeProduct`, `User` ve alt sınıfları, durum sınıfları |
| **View** | `Views/` | Konsol çıktısı, menü ekranları, bildirim mesajları |
| **Controller** | `Controllers/` | `OrderController`, `StockController`, `UserController` |
| **Service** | `Services/` | `OrderFacade`, `StockManager`, `AuthorizationService` |
| **Pattern** | `Patterns/` | Adaptörler, Dekoratörler, Strateji sınıfları |
| **Util** | `Utils/` | `Logger`, `CargoProviderFactory`, `PaymentStrategyFactory` |

Bu ayrım sayesinde iş mantığı (Model/Service), sunum katmanından (View) tamamen bağımsız hale gelir. Controller yalnızca kullanıcı girdisini alıp uygun servis metodunu çağırır; iş kurallarına karışmaz.

---

## 2. SOLID Prensipleri Uygulaması

### S — Single Responsibility Principle (Tek Sorumluluk)
Her sınıf yalnızca bir sorumluluğa sahiptir. `Order` sınıfı sipariş verilerini tutar; durum geçişlerini `IOrderState` implementasyonları yönetir. `Logger` yalnızca loglama yapar; `StockManager` yalnızca stok takibi yapar.

### O — Open/Closed Principle (Açık/Kapalı)
Yeni kargo firması eklemek için `ICargoProvider` arayüzünü gerçekleyen yeni bir adaptör sınıfı yazmak yeterlidir; mevcut kodda hiçbir değişiklik gerekmez. Aynı şekilde yeni ödeme yöntemi eklemek için `IPaymentStrategy` implementasyonu yazılır.

### L — Liskov Substitution Principle (Liskov'un Yerine Geçme)
`ICargoProvider` kullanan her yer, `ArasCargoAdapter`, `YurticiCargoAdapter` veya `GlobalExpresAdapter` nesnesiyle çalışabilir; davranış tutarlıdır. `IProduct` kullanan her yer, `SimpleProduct` veya `CompositeProduct` alabilir.

### I — Interface Segregation Principle (Arayüz Ayrımı)
`IOrderState` yalnızca sipariş geçişlerini tanımlar. `IStockObserver` yalnızca stok bildirimini tanımlar. `IFeeCalculator` yalnızca ücret hesabını tanımlar. Sınıflar ihtiyaç duymadıkları metotları implement etmek zorunda kalmaz.

### D — Dependency Inversion Principle (Bağımlılığın Tersine Çevrilmesi)
`OrderFacade`, `ArasCargoAdapter`'a değil `ICargoProvider` arayüzüne bağımlıdır. `Order`, `CreditCardPayment`'a değil `IPaymentStrategy`'e bağımlıdır. Tüm üst seviye modüller soyutlamalara bağlıdır; somut sınıflara değil.

---

## 3. Tasarım Deseni Seçim Tablosu

| Süreç | Karşılaşılan Problem | Seçilen Tasarım Deseni | Neden Bu Desen? |
|---|---|---|---|
| Sipariş aşamaları | Durum geçişlerinin karmaşıklığı; her durumda farklı işlemlere izin verilmesi. `if (durum == KARGO) { iptal etme! }` gibi if-else zincirleri kodu kırılgan hale getirir. | **State** | Her durumu ayrı bir sınıfa bölerek, izin verilen ve verilmeyen işlemler o sınıfın kendi metodunda belirlenir. Yeni durum eklemek mevcut kodu bozmaz. |
| Farklı kargo firmalarının farklı API'leri | Aras, Yurtiçi ve GlobalExpres'in her birinin farklı metot imzaları var. Bunları doğrudan çağırmak sistemi dış API değişimlerine karşı kırılgan yapar. | **Adapter** | Her firma için bir adaptör sınıfı, dış API'yi `ICargoProvider` arayüzüne çevirir. Sistem yalnızca bu arayüzü bilir; firma değişiminden etkilenmez. |
| Ödeme yönteminin çalışma zamanında seçilmesi | Ödeme yöntemi sipariş sırasında müşteri tarafından belirlenir; sabit kodlamak OCP'yi ihlal eder. | **Strategy** | `IPaymentStrategy` arayüzü üzerinden ödeme nesnesi çalışma zamanında enjekte edilir. Kripto ödeme gibi yeni yöntemler mevcut kodu değiştirmeden eklenir. |
| Kargo ücreti + ek hizmetler | Sigortalı gönderim, kırılgan koruma vb. ek hizmetler birbirinden bağımsız, kombinlenebilir şekilde ücrete yansıtılmalıdır. | **Decorator** | Temel ücret üzerine dekoratörler zincirlenerek eklenir: `new InsuranceDecorator(new FragileProtectionDecorator(new BaseCargoFee()))`. Kalıtım yerine kompozisyon kullanılır. |
| Stok bildirimleri | Stok düşünce hem e-posta hem sistem bildirimi gitmeli; ileride SMS de eklenebilir olmalıdır. | **Observer** | `StockManager` gözlemcileri bilmeden `NotifyObservers()` çağırır. Yeni kanal için yalnızca `IStockObserver` implementasyonu yazılır; `StockManager` değişmez. |
| Loglama — tek nesne | Loglama nesnesi birden fazla üretilirse log dosyasına eşzamanlı ve tutarsız yazım gerçekleşebilir. | **Singleton** | `Logger.Instance` her çağrıda aynı nesneyi döndürür; uygulama genelinde tek bir log akışı garanti edilir. |
| Kargo/ödeme nesnesi üretimi | Hangi kargo firması veya ödeme yöntemi kullanılacağı çalışma zamanında belirlenir; `new ArasCargoAdapter()` şeklinde doğrudan üretim OCP'yi ihlal eder. | **Factory Method** | `CargoProviderFactory.Create(CargoType.Aras)` çağrısıyla nesne üretimi merkezi hale gelir. Yeni tip eklemek fabrikada tek bir `case` eklemek demektir. |
| Bileşik ürün oluşturma | Bilgisayar kasası gibi ürünlerin birçok bileşeni (RAM, CPU, SSD) vardır; bu bileşenlerin adım adım yapılandırılması gerekir. | **Builder** | `CompositeProductBuilder` aracılığıyla bileşenler `AddComponent()` ile eklenir ve `Build()` çağrısıyla tamamlanmış ürün döndürülür. |
| Ürün hiyerarşisi | Basit ürün ile bileşik ürün aynı `IProduct` arayüzüyle kullanılabilmeli; fiyat/stok hesaplama bileşik ürünlerde alt bileşenlere delege edilmelidir. | **Composite** | `CompositeProduct.Price` kendi bileşenlerinin fiyatlarını toplayarak döndürür. İstemci kod, ürünün basit mi yoksa bileşik mi olduğunu bilmek zorunda kalmaz. |
| Sipariş sürecinin karmaşıklığı | Sipariş oluşturma; ödeme, stok, kargo, loglama gibi birçok alt sistemi koordine etmeyi gerektirir. İstemci kodu bu alt sistemleri tek tek çağırmak zorunda kalırsa bağımlılıklar artar. | **Facade** | `OrderFacade.PlaceOrder()` tüm alt sistemi içeride koordine eder; istemci kodu tek bir metot çağrısıyla süreci başlatır. |

---

## 4. Tasarım Desenlerinin Teknik Açıklamaları

---

### 4.1 Singleton — Logger

**Problem:** Loglama nesnesi sistem genelinde yalnızca bir adet var olmalıdır. Birden fazla `Logger` örneği oluşturulması, log dosyasına eşzamanlı ve tutarsız yazıma yol açar.

**Çözüm:** `Logger` sınıfının yapıcısı `private` tanımlanmıştır. Tek erişim noktası `Logger.Instance` özelliğidir. Thread-safety için `lock` bloğu kullanılmıştır.

```csharp
public sealed class Logger
{
    private static Logger _instance;
    private static readonly object _lock = new object();

    private Logger() { }

    public static Logger Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Logger();
                }
            }
            return _instance;
        }
    }

    public void Log(string message, LogLevel level) { ... }
    public void LogStockChange(string productId, int from, int to) { ... }
    public void LogPayment(string orderId, string method, double amount) { ... }
    public void LogStateChange(string orderId, string from, string to) { ... }
}
```

**SOLID Katkısı:** SRP — loglama tek bir sınıfta toplandı. İstenirse `ILogger` arayüzü eklenerek DIP de sağlanabilir.

---

### 4.2 Factory Method — Kargo ve Ödeme Fabrikaları

**Problem:** Sipariş oluşturulurken hangi kargo firması veya ödeme yönteminin seçildiği çalışma zamanında belirlenir. `new ArasCargoAdapter()` gibi doğrudan nesne üretimi, üst seviye sınıfları somut sınıflara bağlar ve OCP'yi ihlal eder.

**Çözüm:** Fabrika sınıfları, tip parametresine göre doğru nesneyi üretip `ICargoProvider` / `IPaymentStrategy` arayüzü olarak döndürür. C# 8+ switch expression kullanılmıştır.

```csharp
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

public static class PaymentStrategyFactory
{
    public static IPaymentStrategy Create(PaymentType type) => type switch
    {
        PaymentType.CreditCard    => new CreditCardPayment(),
        PaymentType.WireTransfer  => new WireTransferPayment(),
        PaymentType.Crypto        => new CryptoPayment(),
        _ => throw new ArgumentException($"Bilinmeyen ödeme tipi: {type}")
    };
}
```

**SOLID Katkısı:** OCP — yeni kargo firması için yalnızca yeni adaptör sınıfı ve fabrikaya yeni `case` eklenir. DIP — `OrderFacade` somut tiplere değil, soyut arayüzlere bağımlıdır.

---

### 4.3 Builder — Bileşik Ürün Oluşturma

**Problem:** Bilgisayar kasası gibi bileşik ürünler birçok alt parçadan oluşur. Çok parametreli yapıcılar okunaksız ve hata eğilimlidir.

**Çözüm:** `CompositeProductBuilder` sınıfı, zincirleme metot çağrılarıyla bileşenleri toplar ve `Build()` ile tamamlanmış nesneyi döndürür.

```csharp
IProduct bilgisayar = new CompositeProductBuilder()
    .SetId("PC-001")
    .SetName("Oyuncu Bilgisayarı")
    .AddComponent(new SimpleProduct("RAM-001", "16 GB RAM", 1200, 50, 10))
    .AddComponent(new SimpleProduct("CPU-001", "Intel i7",  8500, 30,  5))
    .AddComponent(new SimpleProduct("SSD-001", "1 TB SSD",  3000, 40,  8))
    .Build();
```

```csharp
public class CompositeProductBuilder
{
    private string _id;
    private string _name;
    private readonly List<IProduct> _components = new();

    public CompositeProductBuilder SetId(string id)       { _id = id; return this; }
    public CompositeProductBuilder SetName(string name)   { _name = name; return this; }
    public CompositeProductBuilder AddComponent(IProduct p) { _components.Add(p); return this; }

    public CompositeProduct Build()
    {
        if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_name))
            throw new InvalidOperationException("Id ve Name zorunludur.");
        return new CompositeProduct(_id, _name, _components);
    }
}
```

**SOLID Katkısı:** SRP — nesne inşa sorumluluğu Builder'a devredildi. OCP — yeni alan eklemek Builder sözleşmesini bozmaz.

---

### 4.4 Adapter — Kargo Firması Entegrasyonu

**Problem:** Her kargo firmasının farklı metot imzaları ve veri yapıları var. Sistemi bu farklılıklara göre şekillendirmek dış bağımlılığı artırır ve firmanın API'sini değiştirmesi tüm entegrasyon kodunu bozar.

**Çözüm:** Her kargo firması için bir adaptör sınıfı yazılır. Adaptör, dış API'yi `ICargoProvider` arayüzüne çevirir.

```csharp
public class ArasCargoAdapter : ICargoProvider
{
    private readonly ArasCargoAPI _arasAPI;

    public ArasCargoAdapter(ArasCargoAPI arasAPI)
    {
        _arasAPI = arasAPI;
    }

    public string GenerateTrackingNumber()
    {
        return "ARAS-" + _arasAPI.CreateShipment(new Dictionary<string, object>());
    }

    public double CalculateFee(double weight, double distance)
    {
        return _arasAPI.GetPrice(weight, distance);
    }

    public bool SendPackage(string orderId)
    {
        return _arasAPI.CreateShipment(new Dictionary<string, object>
        {
            { "orderId", orderId }
        }) != null;
    }
}
```

**SOLID Katkısı:** OCP — yeni firma için yalnızca yeni adaptör eklenir. DIP — `OrderFacade` somut `ArasCargoAPI`'yi değil, `ICargoProvider` arayüzünü kullanır.

---

### 4.5 Decorator — Kargo Ücreti Hesaplama

**Problem:** Kargo ücreti temel ücretin üzerine birden fazla ek hizmetle artırılabilir. Bu kombinasyonları kalıtımla çözmek kombinatoryal patlama yaratır.

**Çözüm:** `FeeDecorator` soyut sınıfı bir `IFeeCalculator`'ı sarar ve üzerine ek hesaplama ekler. Dekoratörler zincirlenerek istenen kombinasyon elde edilir.

```csharp
// Zinciri kur: Temel ücret + Sigorta + Kırılgan Koruma
IFeeCalculator hesaplayici = new BaseCargoFee();
hesaplayici = new InsuranceDecorator(hesaplayici);
hesaplayici = new FragileProtectionDecorator(hesaplayici);

double toplam = hesaplayici.Calculate(2.5, 150.0);
```

```csharp
public abstract class FeeDecorator : IFeeCalculator
{
    protected readonly IFeeCalculator _wrapped;

    protected FeeDecorator(IFeeCalculator wrapped)
    {
        _wrapped = wrapped;
    }

    public abstract double Calculate(double weight, double distance);
}

public class InsuranceDecorator : FeeDecorator
{
    private const double Rate = 0.05; // %5 sigorta ücreti

    public InsuranceDecorator(IFeeCalculator wrapped) : base(wrapped) { }

    public override double Calculate(double weight, double distance)
    {
        double baseAmount = _wrapped.Calculate(weight, distance);
        return baseAmount + (baseAmount * Rate);
    }
}

public class FragileProtectionDecorator : FeeDecorator
{
    private const double FixedFee = 25.0;

    public FragileProtectionDecorator(IFeeCalculator wrapped) : base(wrapped) { }

    public override double Calculate(double weight, double distance)
    {
        return _wrapped.Calculate(weight, distance) + FixedFee;
    }
}
```

**SOLID Katkısı:** OCP — yeni ek hizmet için yalnızca yeni dekoratör sınıfı eklenir. SRP — her dekoratör yalnızca kendi ek ücretini hesaplar.

---

### 4.6 Facade — Sipariş Süreci

**Problem:** Sipariş oluşturma; ödeme, stok güncelleme, kargo nesnesi üretme ve loglama gibi birden fazla alt sistemi koordine eder. İstemci kodunun tüm bu alt sistemleri ayrı ayrı çağırması gereksiz bağımlılık yaratır.

**Çözüm:** `OrderFacade.PlaceOrder()` tüm koordinasyonu tek metotta üstlenir. İstemci kod yalnızca facade'ı bilir.

```csharp
public class OrderFacade
{
    private readonly StockManager _stockManager;
    private readonly Logger _logger = Logger.Instance;

    public OrderFacade(StockManager stockManager)
    {
        _stockManager = stockManager;
    }

    public Order PlaceOrder(string customerId, List<OrderItem> items,
                            PaymentType paymentType, CargoType cargoType)
    {
        var payment = PaymentStrategyFactory.Create(paymentType);
        var cargo   = CargoProviderFactory.Create(cargoType);
        var order   = new Order(customerId, items, payment, cargo);

        bool paid = payment.Pay(order.TotalAmount);
        if (!paid) throw new PaymentFailedException("Ödeme başarısız.");

        _logger.LogPayment(order.Id, paymentType.ToString(), order.TotalAmount);

        order.Approve();

        foreach (var item in items)
            _stockManager.UpdateStock(item.Product.Id, -item.Quantity);

        order.TrackingNumber = cargo.GenerateTrackingNumber();

        return order;
    }

    public bool CancelOrder(string orderId) { ... }
    public bool InitiateReturn(string orderId) { ... }
}
```

**SOLID Katkısı:** SRP — koordinasyon mantığı tek sınıfta toplandı. DIP — facade somut sınıflara değil, `IPaymentStrategy` ve `ICargoProvider` arayüzlerine bağımlıdır.

---

### 4.7 Observer — Stok Bildirim Sistemi

**Problem:** Stok eşik altına düştüğünde birden fazla kanala (e-posta, sistem bildirimi) mesaj gönderilmeli; ilerleyen sürümlerde SMS gibi yeni kanallar eklenebilmelidir. `StockManager`'ın bu kanalları doğrudan çağırması OCP'yi ihlal eder.

**Çözüm:** `StockManager`, `IStockObserver` listesi tutar ve eşik aşımında `NotifyObservers()` çağırır; kimin dinlediğini bilmez.

```csharp
public interface IStockObserver
{
    void OnStockLow(IProduct product, int currentStock);
}

public class StockManager
{
    private readonly List<IStockObserver> _observers = new();

    public void RegisterObserver(IStockObserver obs) => _observers.Add(obs);
    public void RemoveObserver(IStockObserver obs)   => _observers.Remove(obs);

    public void UpdateStock(string productId, int delta)
    {
        var product = _productRepository.FindById(productId);
        int newStock = product.Stock + delta;
        product.Stock = newStock;
        Logger.Instance.LogStockChange(productId, product.Stock, newStock);
        CheckThreshold(product);
    }

    private void CheckThreshold(IProduct product)
    {
        if (product.Stock < product.Threshold)
            _observers.ForEach(obs => obs.OnStockLow(product, product.Stock));
    }
}

public class EmailNotificationObserver : IStockObserver
{
    public void OnStockLow(IProduct product, int currentStock)
    {
        Console.WriteLine($"[E-POSTA] Satın Alma: '{product.Name}' stoğu kritik! Mevcut: {currentStock}");
    }
}

public class SystemNotificationObserver : IStockObserver
{
    private readonly string _targetUserId;

    public SystemNotificationObserver(string targetUserId)
    {
        _targetUserId = targetUserId;
    }

    public void OnStockLow(IProduct product, int currentStock)
    {
        Console.WriteLine($"[BİLDİRİM → {_targetUserId}] '{product.Name}' stoğu kritik! Mevcut: {currentStock}");
    }
}
```

**SOLID Katkısı:** OCP — yeni bildirim kanalı için yalnızca yeni `IStockObserver` implementasyonu yazılır. DIP — `StockManager`, somut gözlemci sınıflarına değil, arayüze bağımlıdır.

---

### 4.8 Strategy — Ödeme Yöntemi Seçimi

**Problem:** Ödeme yöntemi sipariş esnasında müşteri tarafından belirlenir ve her yöntemin farklı işleme mantığı vardır. Tüm bu mantığı `Order` içinde if-else ile yönetmek OCP'yi ihlal eder.

**Çözüm:** `IPaymentStrategy` arayüzü, her ödeme yöntemini aynı `Pay()` çağrısıyla temsil eder. `Order`, hangi yöntemin kullanıldığını bilmeden ödemeyi gerçekleştirir.

```csharp
public interface IPaymentStrategy
{
    bool Pay(double amount);
    string MethodName { get; }
}

public class CreditCardPayment : IPaymentStrategy
{
    public string MethodName => "Kredi Kartı";

    public bool Pay(double amount)
    {
        // Kart doğrulama ve banka API çağrısı
        Console.WriteLine($"Kredi kartıyla {amount:C} ödendi.");
        return true;
    }
}

public class WireTransferPayment : IPaymentStrategy
{
    public string MethodName => "Havale/EFT";

    public bool Pay(double amount)
    {
        Console.WriteLine($"Havale ile {amount:C} ödendi.");
        return true;
    }
}

public class CryptoPayment : IPaymentStrategy
{
    public string MethodName => "Kripto";

    public bool Pay(double amount)
    {
        // Cüzdan adresi doğrulama ve blockchain işlemi
        Console.WriteLine($"Kripto ile {amount:C} ödendi.");
        return true;
    }
}
```

**SOLID Katkısı:** OCP — yeni ödeme tipi, `IPaymentStrategy`'yi gerçekleyen yeni sınıfla eklenir; mevcut kod değişmez. SRP — her sınıf yalnızca kendi ödeme mantığını barındırır.

---

### 4.9 State — Sipariş Durum Yönetimi

**Problem:** Sipariş her aşamada farklı işlemlere izin verir veya yasaklar. `Kargoda` durumunda iptal edilemez, yalnızca iade başlatılabilir. Bu kuralları if-else ile `Order` sınıfı içinde yönetmek sınıfı şişirir ve kırılgan kılar.

**Çözüm:** Her sipariş durumu ayrı bir sınıf olarak temsil edilir. İzin verilmeyen geçişler ilgili durum sınıfında `InvalidOperationException` fırlatarak engellenir.

```csharp
public interface IOrderState
{
    void Approve(Order order);
    void Prepare(Order order);
    void ShipOut(Order order);
    void Deliver(Order order);
    void StartReturn(Order order);
    void Cancel(Order order);
    string StateName { get; }
}

public class InCargoState : IOrderState
{
    public string StateName => "Kargoda";

    public void Deliver(Order order)
    {
        order.SetState(new DeliveredState());
        Logger.Instance.LogStateChange(order.Id, "Kargoda", "Teslim Edildi");
    }

    public void StartReturn(Order order)
    {
        order.SetState(new ReturnState());
        Logger.Instance.LogStateChange(order.Id, "Kargoda", "İade");
    }

    public void Cancel(Order order)
    {
        // Kargodaki sipariş iptal edilemez — iş kuralı burada zorlanıyor
        throw new InvalidOperationException("Kargodaki sipariş iptal edilemez.");
    }

    public void Approve(Order order)  => throw new InvalidOperationException();
    public void Prepare(Order order)  => throw new InvalidOperationException();
    public void ShipOut(Order order)  => throw new InvalidOperationException();
}

public class Order
{
    private IOrderState _currentState = new PendingState();

    public string Id { get; }
    public string TrackingNumber { get; set; }
    public double TotalAmount => Items.Sum(i => i.Subtotal);

    public void Approve()     => _currentState.Approve(this);
    public void Prepare()     => _currentState.Prepare(this);
    public void ShipOut()     => _currentState.ShipOut(this);
    public void Deliver()     => _currentState.Deliver(this);
    public void StartReturn() => _currentState.StartReturn(this);
    public void Cancel()      => _currentState.Cancel(this);

    public void SetState(IOrderState state) => _currentState = state;
    public string GetStateName()            => _currentState.StateName;
}
```

**SOLID Katkısı:** SRP — her durum sınıfı yalnızca kendi geçiş mantığını yönetir. OCP — yeni durum eklemek yalnızca yeni sınıf yazmayı gerektirir; `Order` değişmez.

---

### 4.10 Composite — Ürün Hiyerarşisi

**Problem:** Sistemde hem tek parça ürünler hem de bileşik ürünler vardır. Her iki tür için fiyat, stok gibi operasyonların aynı arayüzle çağrılabilmesi gerekir.

**Çözüm:** `IProduct` arayüzü her iki tür için ortak sözleşmeyi tanımlar. `CompositeProduct`, alt bileşenlerinin fiyatlarını LINQ ile toplar. İstemci kod ürünün türünü bilmek zorunda kalmaz.

```csharp
public interface IProduct
{
    string Id   { get; }
    string Name { get; }
    double Price { get; }
    int Stock    { get; set; }
    int Threshold { get; }
}

public class SimpleProduct : IProduct
{
    public string Id        { get; }
    public string Name      { get; }
    public double Price     { get; }
    public int Stock        { get; set; }
    public int Threshold    { get; }

    public SimpleProduct(string id, string name, double price, int stock, int threshold)
    {
        Id = id; Name = name; Price = price; Stock = stock; Threshold = threshold;
    }
}

public class CompositeProduct : IProduct
{
    private readonly List<IProduct> _components;

    public string Id    { get; }
    public string Name  { get; }
    public int Threshold => 1;

    // Fiyat: tüm bileşenlerin toplamı
    public double Price => _components.Sum(p => p.Price);

    // Stok: en az stoğa sahip bileşen kısıtlayıcıdır
    public int Stock
    {
        get => _components.Min(p => p.Stock);
        set => _components.ForEach(p => p.Stock = value);
    }

    public CompositeProduct(string id, string name, List<IProduct> components)
    {
        Id = id; Name = name; _components = components;
    }

    public void AddComponent(IProduct p)    => _components.Add(p);
    public void RemoveComponent(IProduct p) => _components.Remove(p);
}
```

**SOLID Katkısı:** LSP — `CompositeProduct`, `IProduct`'ın yerine her yerde kullanılabilir. OCP — yeni bileşen tipi eklenmesi mevcut kodu etkilemez.

---

### 4.11 Protection Proxy — Servis Katmanı Yetkilendirmesi

**Problem:** `CancelOrder` ve `InitiateReturn` işlemleri yalnızca siparişin sahibi veya Admin tarafından çağrılabilmelidir. Bu kuralı her Controller metoduna yazmak DRY ve SRP'yi ihlal eder; birisinin kuralı atlama riski artar.

**Çözüm:** `AuthorizedOrderFacade`, gerçek `OrderFacade`'i sarar ve `IOrderFacade` arayüzünü uygular. Yetki gerektiren her çağrıdan önce `IsAllowed()` metodunu çalıştırır. Controller kodu sadece arayüzü bilir; Proxy mi yoksa gerçek Facade mi olduğunu bilmez.

```csharp
public class AuthorizedOrderFacade : IOrderFacade
{
    private readonly OrderFacade _inner;

    private static readonly HashSet<string> PrivilegedRoles =
        new(StringComparer.OrdinalIgnoreCase) { "Admin" };

    public AuthorizedOrderFacade(OrderFacade inner) => _inner = inner;

    public void CancelOrder(string orderId, string callerUsername, string callerRole)
    {
        if (!IsAllowed(orderId, callerUsername, callerRole))
            throw new UnauthorizedAccessException("Bu siparişi iptal etme yetkiniz yok.");
        _inner.CancelOrder(orderId, callerUsername, callerRole);
    }

    private bool IsAllowed(string orderId, string username, string role)
    {
        if (PrivilegedRoles.Contains(role)) return true;
        var order = _inner.FindOrder(orderId);
        return order.Customer.Username.Equals(username, StringComparison.OrdinalIgnoreCase);
    }
}
```

`App.cs` içinde `OrderFacade` yerine `AuthorizedOrderFacade` dışarıya açılır:

```csharp
private static readonly OrderFacade _realOrderFacade = new(StockManager);
public  static IOrderFacade OrderFacade { get; } = new AuthorizedOrderFacade(_realOrderFacade);
```

**SOLID Katkısı:** SRP — yetkilendirme mantığı Proxy'ye taşındı; Controller ve Facade kendi sorumlulukları dışına çıkmaz. OCP — yeni yetkilendirme kuralı eklemek için yalnızca Proxy değişir.

---

### 4.12 Factory Method — UserFactory (Kullanıcı Oluşturma)

**Problem:** `UserController.Create()` içinde switch-case ile hangi `User` alt sınıfının oluşturulacağına karar veriliyordu. Yeni rol eklendiğinde Controller sınıfını değiştirmek OCP'yi ihlal eder.

**Çözüm:** `UserFactory.Create(CreateUserInput, passwordHash)` rolü alıp doğru alt sınıfı döndürür. Controller somut sınıfları bilmez.

```csharp
public static class UserFactory
{
    public static User Create(CreateUserInput input, string passwordHash)
        => input.Role switch
        {
            UserRole.Admin    => new Admin(input.Username, passwordHash),
            UserRole.Staff    => new Staff(input.Username, passwordHash,
                                           input.Extra, input.Extra2),
            UserRole.Customer => new Customer(input.Username, passwordHash,
                                              input.Extra, input.Extra2),
            UserRole.Courier  => new Courier(input.Username, passwordHash, input.Extra),
            _ => throw new ArgumentException($"Bilinmeyen rol: {input.Role}")
        };
}
```

Controller kodu:

```csharp
[HttpPost]
public IActionResult Create(CreateUserInput input)
{
    var hash = User.HashPassword(input.Password);
    var user = UserFactory.Create(input, hash);   // somut sınıf bilgisi yok
    App.UserService.Add(user);
    return RedirectToAction(nameof(Index));
}
```

**SOLID Katkısı:** OCP — yeni rol için yalnızca `UserFactory`'e bir `case` eklenir; Controller değişmez. SRP — nesne yaratma sorumluluğu Controller'dan ayrıldı.

---

## Özet: Desen — Sorun — Çözüm Eşleşmesi

| Tasarım Deseni | Kategori | Çözdüğü Temel Sorun |
|---|---|---|
| **Singleton** | Yaratımsal | `Logger`'ın uygulama genelinde tek örnek olması |
| **Factory Method** (Kargo/Ödeme) | Yaratımsal | Kargo/ödeme nesnelerinin tip bazlı merkezi üretimi |
| **Factory Method** (UserFactory) | Yaratımsal | Farklı roldeki kullanıcıların Controller'dan bağımsız yaratılması |
| **Builder** | Yaratımsal | Bileşik ürünlerin adım adım, okunabilir biçimde inşa edilmesi |
| **Adapter** | Yapısal | Farklı kargo API'lerinin `ICargoProvider` arayüzüne bağlanması |
| **Decorator** | Yapısal | Kargo ücretine bağımsız ek hizmetlerin zincir şeklinde eklenmesi |
| **Facade** | Yapısal | Sipariş sürecinin tek giriş noktasına indirgenmesi |
| **Protection Proxy** | Yapısal | Servis katmanında sahiplik + rol tabanlı yetkilendirme |
| **Observer** | Davranışsal | Stok eşik aşımında ilgili tarafların otomatik bildirilmesi |
| **Strategy** | Davranışsal | Ödeme yönteminin çalışma zamanında değiştirilebilmesi |
| **State** | Davranışsal | Sipariş aşamalarındaki kural farklılıklarının if-else'siz yönetimi |
| **Composite** | Yapısal | Basit ve bileşik ürünlerin tek tip `IProduct` arayüzüyle işlenmesi |

---

*Rapor Sonu*
