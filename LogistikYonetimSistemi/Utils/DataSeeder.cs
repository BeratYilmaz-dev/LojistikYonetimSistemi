using LogistikYonetimSistemi.Models.Products;
using LogistikYonetimSistemi.Models.Users;

namespace LogistikYonetimSistemi.Utils;

public static class DataSeeder
{
    public static void Seed()
    {
        SeedUsers();
        SeedProducts();
    }

    private static void SeedUsers()
    {
        App.UserService.Add(new Admin   ("admin",        User.HashPassword("admin123")));
        App.UserService.Add(new Staff   ("ahmet.depo",   User.HashPassword("staff123"),    "ahmet@mail.com",    "Depo"));
        App.UserService.Add(new Staff   ("berat",   User.HashPassword("berat123"),    "beratyyilmaz@protonmail.com",   "Satın Alma"));
        App.UserService.Add(new Customer("ali.musteri",  User.HashPassword("customer123"), "ali@mail.com",  "İstanbul, Kadıköy"));
        App.UserService.Add(new Customer("fatma.uye",    User.HashPassword("customer123"), "fatma@mail.com","Ankara, Çankaya"));
        App.UserService.Add(new Courier ("kurye.mehmet", User.HashPassword("kurye123"),    "Motosiklet"));
        App.UserService.Add(new Courier ("kurye.osman", User.HashPassword("kurye123"),    "Araç"));
    }

    private static void SeedProducts()
    {
        // ── İşlemciler (CPU) — ~0.3 kg ──────────────────────────────────
        var cpu1 = new SimpleProduct("CPU-001", "Intel Core i9-14900K",   18_500, 25, 5,  weightKg: 0.3);
        var cpu2 = new SimpleProduct("CPU-002", "AMD Ryzen 9 7950X",      21_000, 15, 5,  weightKg: 0.3);
        var cpu3 = new SimpleProduct("CPU-003", "Intel Core i5-13600K",    9_500, 30, 8,  weightKg: 0.3);
        var cpu4 = new SimpleProduct("CPU-004", "AMD Ryzen 5 7600X",       7_200, 35, 8,  weightKg: 0.3);

        // ── Ekran Kartları (GPU) — 1.0–1.5 kg ───────────────────────────
        var gpu1 = new SimpleProduct("GPU-001", "NVIDIA GeForce RTX 4090 24GB", 65_000, 10, 3,  weightKg: 1.5);
        var gpu2 = new SimpleProduct("GPU-002", "AMD Radeon RX 7900 XTX 24GB",  45_000, 12, 3,  weightKg: 1.3);
        var gpu3 = new SimpleProduct("GPU-003", "NVIDIA GeForce RTX 4070 Ti 12GB", 28_000, 20, 5, weightKg: 1.1);
        var gpu4 = new SimpleProduct("GPU-004", "AMD Radeon RX 7800 XT 16GB",   18_500, 18, 5,  weightKg: 1.0);

        // ── Bellekler (RAM) — ~0.1 kg ────────────────────────────────────
        var ram1 = new SimpleProduct("RAM-001", "Corsair Vengeance 32GB DDR5-6000",    4_500, 40, 10, weightKg: 0.1);
        var ram2 = new SimpleProduct("RAM-002", "G.Skill Trident Z5 64GB DDR5-5600",   8_200, 25, 8,  weightKg: 0.1);
        var ram3 = new SimpleProduct("RAM-003", "Kingston Fury 16GB DDR4-3200",         1_800, 60, 15, weightKg: 0.1);

        // ── Depolama (SSD/HDD) — 0.1–0.6 kg ────────────────────────────
        var ssd1 = new SimpleProduct("SSD-001", "Samsung 990 Pro 2TB NVMe",    5_500, 35, 8,  weightKg: 0.1);
        var ssd2 = new SimpleProduct("SSD-002", "WD Black SN850X 1TB NVMe",    3_200, 40, 10, weightKg: 0.1);
        var ssd3 = new SimpleProduct("SSD-003", "Seagate Barracuda 4TB HDD",   1_600, 50, 12, weightKg: 0.6);

        // ── Anakartlar (MB) — ~1.5 kg ────────────────────────────────────
        var mb1  = new SimpleProduct("MB-001", "ASUS ROG Maximus Z790 Hero", 16_000, 12, 3, weightKg: 1.5);
        var mb2  = new SimpleProduct("MB-002", "MSI MEG X670E ACE",          14_000, 10, 3, weightKg: 1.5);
        var mb3  = new SimpleProduct("MB-003", "Gigabyte B650M DS3H",          5_200, 25, 6, weightKg: 1.2);

        // ── Güç Kaynakları (PSU) — ~2.0 kg ──────────────────────────────
        var psu1 = new SimpleProduct("PSU-001", "Corsair HX1000i 1000W Platinum", 7_500, 20, 5,  weightKg: 2.0);
        var psu2 = new SimpleProduct("PSU-002", "EVGA SuperNOVA 750W Gold",        3_800, 30, 8,  weightKg: 1.8);
        var psu3 = new SimpleProduct("PSU-003", "be quiet! Pure Power 12 650W",    2_800, 35, 10, weightKg: 1.6);

        // ── Kasalar (CASE) — 5.0–7.0 kg ─────────────────────────────────
        var case1 = new SimpleProduct("CASE-001", "NZXT H510 Flow Beyaz",        3_200, 20, 5, weightKg: 5.5);
        var case2 = new SimpleProduct("CASE-002", "Fractal Design Meshify 2",     4_100, 15, 4, weightKg: 7.0);
        var case3 = new SimpleProduct("CASE-003", "Corsair 4000D Airflow Siyah",  2_900, 25, 6, weightKg: 6.0);

        // ── Soğutucular (COOL) — 0.8–1.5 kg ─────────────────────────────
        var cool1 = new SimpleProduct("COOL-001", "Noctua NH-D15 Çift Kule",         3_500, 20, 5, weightKg: 1.3);
        var cool2 = new SimpleProduct("COOL-002", "Corsair H150i Elite AIO 360mm",   5_800, 15, 4, weightKg: 1.5);
        var cool3 = new SimpleProduct("COOL-003", "be quiet! Dark Rock Pro 4",       2_800, 22, 5, weightKg: 1.1);

        var all = new IProduct[] {
            cpu1,cpu2,cpu3,cpu4, gpu1,gpu2,gpu3,gpu4,
            ram1,ram2,ram3, ssd1,ssd2,ssd3,
            mb1,mb2,mb3, psu1,psu2,psu3,
            case1,case2,case3, cool1,cool2,cool3
        };
        foreach (var p in all) App.StockManager.AddProduct(p);

        // ── Montaj PC'ler (CompositeProduct + Builder) ────────────────────
        var gamingPc = new CompositeProductBuilder()
            .SetId("PC-001").SetName("Gaming PC — RTX 4090 i9 Paketi")
            .AddComponent(cpu1).AddComponent(gpu1).AddComponent(ram1)
            .AddComponent(ssd1).AddComponent(mb1).AddComponent(psu1)
            .AddComponent(case2).AddComponent(cool2)
            .Build();

        var budgetPc = new CompositeProductBuilder()
            .SetId("PC-002").SetName("Budget Gaming PC — RX 7800 XT Paketi")
            .AddComponent(cpu4).AddComponent(gpu4).AddComponent(ram3)
            .AddComponent(ssd2).AddComponent(mb3).AddComponent(psu3)
            .AddComponent(case3).AddComponent(cool3)
            .Build();

        App.StockManager.AddProduct(gamingPc);
        App.StockManager.AddProduct(budgetPc);
    }
}
