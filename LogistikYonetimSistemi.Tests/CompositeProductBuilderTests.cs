using LogistikYonetimSistemi.Models.Products;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

/// <summary>
/// Builder deseninin doğrulama kurallarını ve Composite ürün hesaplamalarını test eder.
/// </summary>
public class CompositeProductBuilderTests
{
    private static SimpleProduct Cpu()  => new("CPU-T1", "Intel i9", 18_000, 5, 2);
    private static SimpleProduct Ram()  => new("RAM-T1", "Corsair 32GB", 4_500, 10, 3);
    private static SimpleProduct Ssd()  => new("SSD-T1", "Samsung 2TB", 5_500, 8, 2);

    // ── Geçerli build ────────────────────────────────────────────────────────
    [Fact]
    public void Build_WithValidData_ReturnsCompositeProduct()
    {
        var pc = new CompositeProductBuilder()
            .SetId("PC-T1")
            .SetName("Test PC")
            .AddComponent(Cpu())
            .AddComponent(Ram())
            .Build();

        Assert.NotNull(pc);
        Assert.IsType<CompositeProduct>(pc);
    }

    [Fact]
    public void Build_Price_EqualsSumOfComponents()
    {
        var cpu = Cpu();
        var ram = Ram();
        var pc  = new CompositeProductBuilder()
            .SetId("PC-T2").SetName("Test PC")
            .AddComponent(cpu).AddComponent(ram)
            .Build();

        Assert.Equal(cpu.Price + ram.Price, pc.Price);
    }

    [Fact]
    public void Build_Stock_EqualsMinComponentStock()
    {
        var cpu = new SimpleProduct("CPU-T2", "i9", 18000, 3, 1); // stok: 3
        var ram = new SimpleProduct("RAM-T2", "32GB", 4500, 7, 1); // stok: 7

        var pc = new CompositeProductBuilder()
            .SetId("PC-T3").SetName("Test PC")
            .AddComponent(cpu).AddComponent(ram)
            .Build();

        Assert.Equal(3, pc.Stock); // CPU daha az stok → kısıtlayıcı
    }

    [Fact]
    public void Build_WithThreeComponents_CorrectPrice()
    {
        var cpu = Cpu(); var ram = Ram(); var ssd = Ssd();
        double expected = cpu.Price + ram.Price + ssd.Price;

        var pc = new CompositeProductBuilder()
            .SetId("PC-T4").SetName("Full PC")
            .AddComponent(cpu).AddComponent(ram).AddComponent(ssd)
            .Build();

        Assert.Equal(expected, pc.Price);
    }

    // ── Doğrulama hataları ────────────────────────────────────────────────────
    [Fact]
    public void Build_WithoutId_ThrowsInvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new CompositeProductBuilder()
                .SetName("PC")
                .AddComponent(Cpu())
                .Build());
    }

    [Fact]
    public void Build_WithoutName_ThrowsInvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new CompositeProductBuilder()
                .SetId("PC-X")
                .AddComponent(Cpu())
                .Build());
    }

    [Fact]
    public void Build_WithNoComponents_ThrowsInvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() =>
            new CompositeProductBuilder()
                .SetId("PC-X")
                .SetName("Empty PC")
                .Build());
    }
}
