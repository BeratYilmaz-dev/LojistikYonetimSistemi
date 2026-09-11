# Nesneye Dayalı Analiz ve Tasarım — Dönem Sonu Projesi
## ANALİZ RAPORU

**Proje Adı:** Akıllı Tedarik ve Lojistik Yönetim Sistemi  
**Öğrenci No:** B221210034  
**Ad Soyad:**  Berat Yılmaz

---

## İÇİNDEKİLER

1. Proje Genel Bakış
2. Paydaş ve Aktör Analizi
3. Fonksiyonel Gereksinimler
4. Fonksiyonel Olmayan Gereksinimler
5. Use Case Diyagramı ve Açıklamaları
6. Sınıf Diyagramları
7. Durum (State) Diyagramı
8. Sıralama (Sequence) Diyagramları

---

## 1. Proje Genel Bakış

Bu proje, bir e-ticaret lojistik merkezinin temel iş süreçlerini dijitalleştiren ve yönetilen uçtan uca bir yazılım sistemidir. Sistem; ürün ve stok yönetimi, sipariş yaşam döngüsü, farklı ödeme yöntemleri, çoklu kargo firması entegrasyonu ve rol tabanlı yetkilendirme gibi bileşenleri kapsamaktadır.

Yazılım, Yazılım Geliştirme Yaşam Döngüsü (SDLC) aşamalarına uygun biçimde tasarlanmış olup OOAD prensiplerini, SOLID ilkelerini ve çeşitli Tasarım Desenlerini temel almaktadır.

### Sistemin Temel Hedefleri

- Depo stoklarını gerçek zamanlı olarak takip etmek ve kritik eşik aşıldığında ilgili birimleri otomatik olarak bilgilendirmek.
- Sipariş sürecini tanımlı durum geçişleriyle (State Machine) yönetmek; her durumda izin verilen işlemleri açıkça belirlemek.
- Farklı kargo firmalarını tek bir standart arayüz üzerinden sisteme entegre etmek.
- Rol tabanlı erişim kontrolüyle (Admin, Personel, Müşteri, Kurye) yetkilendirmeyi merkezi olarak yönetmek.
- Tüm kritik işlemleri tek bir loglama bileşeniyle kayıt altına almak.

---

## 2. Paydaş ve Aktör Analizi

| Aktör | Açıklama | Temel Sorumluluklar |
|---|---|---|
| **Admin (Yönetici)** | Sisteme tam erişim yetkisi olan kullanıcı | Kullanıcı yönetimi, sistem loglarını görüntüleme, ürün/stok düzenleme |
| **Personel (Depo/Satın Alma)** | Operasyonel görevler yapan çalışan | Stok güncelleme, sipariş hazırlama, düşük stok bildirimlerini alma |
| **Müşteri** | Sisteme sipariş veren son kullanıcı | Sipariş oluşturma, ödeme yapma, kargo takibi, iade başlatma |
| **Kurye** | Teslimatı gerçekleştiren taraf | Sipariş durumunu güncelleme (Kargoda → Teslim Edildi), kargo takibi |
| **Kargo Firması (Dış Sistem)** | Aras, Yurtiçi, GlobalExpres | Takip numarası üretme, fiyatlandırma API'si |

---

## 3. Fonksiyonel Gereksinimler

### 3.1 Ürün ve Stok Yönetimi (FR-01)

| Kod | Gereksinim |
|---|---|
| FR-01.1 | Sistem, basit ürünleri (tek parça) ve bileşik ürünleri (birden fazla alt bileşenden oluşan, ör: bilgisayar kasası) desteklemelidir. |
| FR-01.2 | Her ürünün bir stok miktarı ve bir kritik eşik değeri (threshold) olmalıdır. |
| FR-01.3 | Bir ürünün stoğu eşik değerin altına düştüğünde; Satın Alma birimine otomatik e-posta, Depo sorumlusuna sistem içi bildirim gönderilmelidir. |
| FR-01.4 | Stok her güncellendiğinde değişiklik loglanmalıdır. |

### 3.2 Sipariş Yönetimi (FR-02)

| Kod | Gereksinim |
|---|---|
| FR-02.1 | Bir sipariş şu aşamalardan sırasıyla geçmelidir: **Beklemede → Onaylandı → Hazırlanıyor → Kargoda → Teslim Edildi**. |
| FR-02.2 | Sipariş yalnızca Beklemede veya Onaylandı durumundayken iptal edilebilir. |
| FR-02.3 | Kargoda aşamasındaki sipariş iptal edilemez; yalnızca İade sürecine geçilebilir. |
| FR-02.4 | Teslim Edildi durumundaki sipariş belirli bir süre içinde iade edilebilir. |
| FR-02.5 | Durum geçişlerinin her biri loglanmalıdır. |

### 3.3 Ödeme Yönetimi (FR-03)

| Kod | Gereksinim |
|---|---|
| FR-03.1 | Sistem en az şu ödeme yöntemlerini desteklemelidir: Kredi Kartı, Havale/EFT. |
| FR-03.2 | Sistem, ileride eklenebilecek yeni ödeme yöntemlerine (Kripto vb.) kapalı kodla destek verebilecek şekilde tasarlanmalıdır. |
| FR-03.3 | Ödeme gerçekleştikten sonra sipariş onaylanabilir duruma geçmelidir. |

### 3.4 Kargo ve Lojistik Yönetimi (FR-04)

| Kod | Gereksinim |
|---|---|
| FR-04.1 | Sistem; Aras, Yurtiçi ve GlobalExpres kargo firmalarıyla entegre çalışmalıdır. |
| FR-04.2 | Her kargo firmasının farklı API yapısı, sistemin içinden gizlenmelidir; tek bir standart arayüz kullanılmalıdır. |
| FR-04.3 | Kargo ücreti; ağırlık, mesafe ve ek hizmetlere (Sigortalı Gönderim, Kırılacak Eşya Koruması) göre hesaplanmalıdır. |
| FR-04.4 | Ek hizmetler birbirinden bağımsız olarak eklenip çıkarılabilmelidir. |

### 3.5 Kullanıcı Yetkilendirme (FR-05)

| Kod | Gereksinim |
|---|---|
| FR-05.1 | Sisteme giriş yapan kullanıcının rolüne göre erişebileceği menüler ve çalıştırabileceği metotlar kısıtlanmalıdır. |
| FR-05.2 | Roller: Admin, Personel, Müşteri, Kurye. |
| FR-05.3 | Admin kullanıcıları oluşturabilir, güncelleyebilir ve silebilir. |

### 3.6 Loglama (FR-06)

| Kod | Gereksinim |
|---|---|
| FR-06.1 | Sistemde kritik her işlem (stok değişimi, ödeme, durum geçişi) loglanmalıdır. |
| FR-06.2 | Loglama nesnesi, sistem genelinde yalnızca bir örnek olarak var olmalıdır. |
| FR-06.3 | Log kayıtları dosyaya veya veritabanına yazılabilmelidir. |

---

## 4. Fonksiyonel Olmayan Gereksinimler

| Kod | Kategori | Gereksinim |
|---|---|---|
| NFR-01 | **Genişletilebilirlik** | Yeni kargo firması veya ödeme yöntemi eklemek için mevcut sınıflar değiştirilmemelidir (OCP). |
| NFR-02 | **Bakım Kolaylığı** | Switch-case ve uzun if-else blokları kullanılmayacak; her varyasyon polimorfik olarak çözülecektir. |
| NFR-03 | **Katmanlı Mimari** | Uygulama MVC katmanlarına ayrılacaktır: Model, View, Controller. |
| NFR-04 | **Test Edilebilirlik** | Bağımlılıklar arayüzler üzerinden enjekte edilecek; birim testleri izole şekilde yazılabilecektir. |
| NFR-05 | **Güvenlik** | Kullanıcı şifreleri hash'lenerek saklanacaktır. Her endpoint rol kontrolüne tabi olacaktır. |
| NFR-06 | **Performans** | Loglama işlemi ana iş akışını bloklamayacaktır. |

---

## 5. Use Case Diyagramı ve Açıklamaları

```
Aktörler & Use Case'ler
═══════════════════════════════════════════════════════
 Admin ──────┬── Giriş Yap (UC-01)
             ├── Sipariş Durumu Güncelle (UC-03) [tüm adımlar]
             ├── Stok Güncelle (UC-04)
             ├── Kullanıcı Yönetimi (UC-09)
             └── Sistem Logları Görüntüle (UC-10)

 Depo Personeli ─┬── Giriş Yap (UC-01)
                 ├── Sipariş Onayla / Hazırla / Kargola (UC-03)
                 └── Stok Güncelle (UC-04)

 Satın Alma Personeli ─┬── Giriş Yap (UC-01)
                       └── Stok Güncelle (UC-04)

 Müşteri ────┬── Giriş Yap (UC-01)
             ├── Sipariş Oluştur (UC-02)
             │    ├── <<include>> Ödeme Yap (UC-07)
             │    └── <<include>> Kargo Firması Seç (UC-08)
             ├── İade Başlat (UC-05)
             └── Sipariş İptal (UC-06)

 Kurye ──────┬── Giriş Yap (UC-01)
             └── Sipariş Teslim Et (UC-03-deliver)

 Kargo Firması (Dış) ── Takip No Üret / Fiyat API (UC-11)
═══════════════════════════════════════════════════════
```

### Use Case Açıklamaları

#### UC-01: Giriş Yap
| Alan | Açıklama |
|---|---|
| Aktörler | Admin, Personel, Müşteri, Kurye |
| Önkoşul | Kullanıcı kayıtlı olmalıdır |
| Ana Akış | Kullanıcı kullanıcı adı ve şifresini girer → sistem doğrular → rol bilgisiyle oturum açılır |
| Alternatif Akış | Hatalı giriş → hata mesajı |

#### UC-02: Sipariş Oluştur
| Alan | Açıklama |
|---|---|
| Aktörler | Müşteri |
| Önkoşul | Müşteri giriş yapmış olmalı; ürünler stokta mevcut olmalı |
| Ana Akış | Ürün seç → sepete ekle → ödeme yöntemi seç → kargo firması seç → siparişi onayla |
| Dahil Edilen UC | UC-07 (Ödeme Yap), UC-08 (Kargo Firması Seç) |
| Sonkoşul | Sipariş "Beklemede" durumunda oluşturulur; stok güncellenir |

#### UC-03: Sipariş Durumu Güncelle
| Alan | Açıklama |
|---|---|
| Aktörler | **Admin** (Onayla, Hazırla, Kargola), **Kurye** (Hazırla, Kargola, Teslim Et) |
| Önkoşul | Sipariş var olmalı |
| Ana Akış | Sipariş seç → yeni durumu belirle → güncelle |
| İş Kuralı | Her durumda izin verilen geçişler sınırlıdır (bkz. State Diyagramı). Personel sipariş akışına müdahale edemez; yalnızca stok güncelleyebilir. |

#### UC-04: Stok Güncelle
| Alan | Açıklama |
|---|---|
| Aktörler | Personel, Admin |
| Sonkoşul | Stok güncellenir; eşik altına düşerse ilgili gözlemciler otomatik bildirim alır |

#### UC-05: İade Başlat
| Alan | Açıklama |
|---|---|
| Aktörler | Müşteri |
| Önkoşul | Sipariş "Kargoda" veya "Teslim Edildi" durumunda olmalı |
| İş Kuralı | Sadece iade geçişine izin verilir; başka duruma geçilemez |

---

## 6. Sınıf Diyagramları

### 6.1 Çekirdek Katman: Ürün, Sipariş, Kullanıcı

/diyagramlar/domain.png

**Temel Kararlar:**

- `IProduct` arayüzü; `SimpleProduct` ve `CompositeProduct` sınıfları tarafından gerçeklenir (**Composite Pattern**).
- `Order` sınıfı, `IOrderState` üzerinden durumunu tutar; geçiş mantığı durum sınıflarının içindedir (**State Pattern**).
- `User` soyut sınıfı; `Admin`, `Staff`, `Customer`, `Courier` alt sınıflarıyla genişletilir. `UserFactory` (Dictionary tabanlı) doğru alt sınıfı üretir (**Factory Method**).

### 6.2 Desen Katmanı: Kargo, Ödeme, Bildirim, Logger

/diyagramlar/pattern1.png
/diyagramlar/pattern1.png


**Temel Kararlar:**

- `ICargoProvider` adaptörleri dış API'leri standart arayüze çevirir (**Adapter**).
- `IFeeCalculator` dekoratörleri, temel ücret üzerine sigorta/kırılgan koruma zincirler (**Decorator**).
- `StockManager` gözlemciler listesi tutar; eşik aşımında `OnStockLow()` yayınlar (**Observer**).
- `AuthorizedOrderFacade`, `OrderFacade`'i sarar; iptal/iade için sahiplik + rol kontrolü yapar (**Protection Proxy**).
- `OrderPermissions` statik yardımcı sınıfı, Controller'daki if bloklarını merkezi hale getirir (**SRP/DRY**).

### 6.3 Controller Katmanı — MVC Tam Görünümü

/diyagramlar/classdiagram.png

**Notlar:**
- Controller'lar iş mantığı içermez; `App` statik erişim noktası üzerinden servislere yönlendirir.
- `OrderPermissions` statik sınıfı, Controller'ın yetki kontrollerini merkezi bir noktada toplar.
- `UserFactory` (Dictionary tabanlı), `UserController`'ın somut `User` alt sınıflarını bilmesini engeller (DIP).

---

## 7. Durum (State) Diyagramı — Sipariş Yaşam Döngüsü

/diyagramlar/state.png

### Durum Açıklamaları

| Durum | Açıklama | İzin Verilen Geçişler |
|---|---|---|
| **Beklemede** | Sipariş oluşturuldu, henüz işlem yapılmadı | → Onaylandı, → İptal |
| **Onaylandı** | Ödeme alındı, sipariş onaylandı | → Hazırlanıyor, → İptal |
| **Hazırlanıyor** | Ürün depoda hazırlanıyor | → Kargoda |
| **Kargoda** | Paket kargo firmasına teslim edildi | → Teslim Edildi, → İade |
| **Teslim Edildi** | Müşteri paketi teslim aldı | → İade (30 gün içinde) |
| **İade** | İade süreci başlatıldı | Son durum |
| **İptal** | Sipariş iptal edildi | Son durum |

**Kritik İş Kuralı:** Kargoda durumundaki sipariş **iptal edilemez**. Bu kural, `InCargoState.Cancel()` metodunun `InvalidOperationException` fırlatmasıyla kod düzeyinde zorlanır; herhangi bir if-else kontrolüne gerek kalmaz.

---

## 8. Sıralama (Sequence) Diyagramları

### 8.1 Sipariş Oluşturma Akışı

/diyagramlar/sequence.png

### 8.2 Stok Azaldığında Bildirim Akışı

/diyagramlar/notification.png

---

*Rapor Sonu*
