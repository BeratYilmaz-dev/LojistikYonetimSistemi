namespace LogistikYonetimSistemi.Models.Products;

public class CompositeProductBuilder
{
    private string?          _id;
    private string?          _name;
    private readonly List<IProduct> _components = new();

    public CompositeProductBuilder SetId(string id)          { _id = id;     return this; }
    public CompositeProductBuilder SetName(string name)      { _name = name; return this; }
    public CompositeProductBuilder AddComponent(IProduct p)  { _components.Add(p); return this; }

    public CompositeProduct Build()
    {
        if (string.IsNullOrWhiteSpace(_id))   throw new InvalidOperationException("Ürün Id boş olamaz.");
        if (string.IsNullOrWhiteSpace(_name)) throw new InvalidOperationException("Ürün Name boş olamaz.");
        if (_components.Count == 0)           throw new InvalidOperationException("Bileşik ürünün en az bir bileşeni olmalıdır.");

        return new CompositeProduct(_id, _name, new List<IProduct>(_components));
    }
}
