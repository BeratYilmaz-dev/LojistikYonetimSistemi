# Test Raporu — Birim Testleri (Unit Tests)

**Proje:** Logistik Yönetim Sistemi (`LogistikYonetimSistemi`)  
**Test projesi:** `LogistikYonetimSistemi.Tests`  
**Çerçeve:** xUnit  


---

## 1. Özet

Kritik iş kuralları ve tasarım desenleri doğrulanmak üzere birim testleri yazılmıştır. Kapsam; sipariş yaşam döngüsü, stok ve gözlemci bildirimleri, Factory üretimi, Builder ile bileşik ürün, State geçişleri ve Singleton Logger davranışını içerir.

| Metrik | Değer |
|--------|--------|
| Toplam test | 55 |
| Başarılı | 55 |
| Başarısız | 0 |
| Atlanan | 0 |
| Çalıştırma süresi (örnek) | ~130–140 ms |

---

## 2. Kritik metotlar ve test eşlemesi

| Bileşen / desen | Ana metotlar / davranış | Test sınıfı |
|-----------------|-------------------------|-------------|
| **OrderFacade** | `PlaceOrder`, `ApproveOrder`, `CancelOrder`, `FindOrder`, `InitiateReturn`, `GetAllOrders` | `OrderFacadeTests` |
| **StockManager** | `AddProduct`, `GetProduct`, `RemoveProduct`, `UpdateStock`, gözlemci tetikleme | `StockManagerTests` |
| **Factory Method** | `PaymentStrategyFactory.Create`, `CargoProviderFactory.Create` | `FactoryTests` |
| **Builder + Composite** | `CompositeProductBuilder` doğrulama, fiyat/stok kuralları | `CompositeProductBuilderTests` |
| **State** | `Order` üzerinden `Approve`, `Prepare`, `ShipOut`, `Deliver`, `Cancel`, `StartReturn` | `OrderStateTests` |
| **Singleton** | `Logger.Instance`, dosyaya yazım | `LoggerTests` |

**Notlar:**

- `OrderFacade` doğrudan test edilir; **yetkilendirme** `AuthorizedOrderFacade` katmanındadır. Bu yüzden `CancelOrder` / `InitiateReturn` çağrılarında testlerde `callerUsername` ve `callerRole` parametreleri kullanılır; asıl yetki senaryoları için ayrıcı entegrasyon testi tanımlanmamıştır.
- MVC Controller ve Razor View birim test kapsamı dışındadır.

---

## 3. Test sınıfları ve senaryolar

### 3.1. `OrderFacadeTests` (12 test)

| Test | Doğrulanan çıktı |
|------|------------------|
| `PlaceOrder_ReturnsOrder_InPendingState` | Sipariş oluşur; `StateName == "Beklemede"`. |
| `PlaceOrder_DeductsStockFromProduct` | Sipariş sonrası stok kaleme göre azalır. |
| `PlaceOrder_AssignsTrackingNumber` | Takip numarası doludur. |
| `PlaceOrder_OrderAppearsInGetAllOrders` | `GetAllOrders()` ilgili siparişi içerir. |
| `ApproveOrder_TransitionsToApproved` | Durum `"Onaylandı"`. |
| `ApproveOrder_AlreadyApproved_Throws` | İkinci onayda `InvalidOperationException`. |
| `CancelOrder_FromPending_TransitionsToCancelled` | Beklemedeyken iptal → `"İptal"`. |
| `CancelOrder_FromInCargo_Throws` | Kargodayken iptal → `InvalidOperationException`. |
| `FindOrder_NonExistent_ThrowsKeyNotFound` | Olmayan ID → `KeyNotFoundException`. |
| `InitiateReturn_FromInCargo_RestoresStock` | İade sonrası stok geri yüklenir. |
| `InitiateReturn_FromInCargo_SetsReturnState` | Durum `"İade"`. |
| `InitiateReturn_FromPending_Throws` | Beklemede iade → `InvalidOperationException`. |

### 3.2. `StockManagerTests` (10 test)

| Test | Doğrulanan çıktı |
|------|------------------|
| `AddAndGet_Product_ReturnsCorrectProduct` | Ürün eklenir ve ID ile bulunur. |
| `GetProduct_NonExistent_ReturnsNull` | Bilinmeyen ID → `null`. |
| `RemoveProduct_RemovesFromDictionary` | Silme sonrası sorgu boş. |
| `UpdateStock_PositiveDelta_IncreasesStock` | Pozitif delta stoku artırır. |
| `UpdateStock_NegativeDelta_DecreasesStock` | Negatif delta stoku azaltır. |
| `UpdateStock_NonExistentProduct_ThrowsKeyNotFound` | Olmayan ürün güncellenemez. |
| `UpdateStock_BelowThreshold_NotifiesObserver` | Eşik ve altında bildirim tetiklenir. |
| `UpdateStock_AboveThreshold_DoesNotNotifyObserver` | Eşik üzerinde bildirim yok. |
| `MultipleObservers_AllNotified_WhenBelowThreshold` | Çoklu gözlemci çağrılır. |
| `RemoveObserver_NotNotifiedAfterRemoval` | Kayıttan çıkan gözlemci çağrılmaz. |

### 3.3. `FactoryTests` (9 test)

| Test | Doğrulanan çıktı |
|------|------------------|
| `PaymentFactory_CreditCard_ReturnsCreditCardPayment` | Tip + `"Kredi Kartı"`. |
| `PaymentFactory_WireTransfer_ReturnsWireTransferPayment` | Tip + `"Havale/EFT"`. |
| `PaymentFactory_Crypto_ReturnsCryptoPayment` | Tip + `"Kripto"`. |
| `PaymentFactory_Pay_ReturnsTrue` | Simüle ödeme `true`. |
| `CargoFactory_Aras_ReturnsArasAdapter` | `ArasCargoAdapter`. |
| `CargoFactory_Yurtici_ReturnsYurticiAdapter` | `YurticiCargoAdapter`. |
| `CargoFactory_GlobalExpres_ReturnsGlobalExpresAdapter` | `GlobalExpresAdapter`. |
| `CargoFactory_GenerateTrackingNumber_NotEmpty` | Takip kodu boş değil. |
| `CargoFactory_SendPackage_ReturnsTrue` | Simülasyon `true`. |

### 3.4. `CompositeProductBuilderTests` (7 test)

| Test | Doğrulanan çıktı |
|------|------------------|
| `Build_WithValidData_ReturnsCompositeProduct` | Geçerli bileşenlerle ürün oluşur. |
| `Build_Price_EqualsSumOfComponents` | Fiyat bileşen toplamı. |
| `Build_Stock_EqualsMinComponentStock` | Stok minimum bileşen stoğu. |
| `Build_WithThreeComponents_CorrectPrice` | Üç bileşenle toplam doğru. |
| `Build_WithoutId_ThrowsInvalidOperation` | ID yoksa hata. |
| `Build_WithoutName_ThrowsInvalidOperation` | Ad yoksa hata. |
| `Build_WithNoComponents_ThrowsInvalidOperation` | Bileşen yoksa hata. |

### 3.5. `OrderStateTests` (13 test)

| Test grubu | Doğrulanan çıktı |
|------------|------------------|
| Başlangıç | Yeni sipariş `"Beklemede"`. |
| Geçerli geçişler | Onay, iptal, hazırlama, kargo, teslim, iade adımları beklenen `StateName` değerlerine gider. |
| `FullHappyPath_Pending_To_Delivered` | Beklemede → Onaylandı → Hazırlanıyor → Kargoda → Teslim Edildi. |
| Geçersiz geçişler | İş kuralı ihlalinde `InvalidOperationException`. |
| Son durum | İptal sonrası ek işlem engellenir. |

### 3.6. `LoggerTests` (4 test)

| Test | Doğrulanan çıktı |
|------|------------------|
| `Instance_IsNotNull` | `Logger.Instance` null değil. |
| `Instance_ReturnsSameObject_Singleton` | Tek örnek (`Same` referans). |
| `Log_WritesToFile` | Log dosyası oluşur; benzersiz mesaj dosyada bulunur. |
| `LogStockChange_ContainsProductId` | Stok değişikliği satırında ürün kimliği vardır. |

---


