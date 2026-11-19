using Domain.Core.Models;

namespace Domain.Core.Comparers;

public class ProductNameComparer : IComparer<Product>
{
    public int Compare(Product? first, Product? second)
    {
        if (ReferenceEquals(first, second))
            return 0;
        if (first is null)
            return -1;
        if (second is null)
            return 1;

        return string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase);
    }
}
