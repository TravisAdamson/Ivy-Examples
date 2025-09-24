namespace ShopifySharpDemo.Apps;

[App(icon: Icons.ShoppingBag, title: "Shopify Products", path: ["Apps"])]
public class ProductsApp : ViewBase
{
    public override object? Build()
    {
        var ShopDomain = Environment.GetEnvironmentVariable("SHOPIFY_SHOP_DOMAIN") ?? "";
        var AccessToken = Environment.GetEnvironmentVariable("SHOPIFY_ACCESS_TOKEN") ?? "";
        var productService = new ProductService(ShopDomain, AccessToken);
        var products = this.UseState<Product[]?>(() => null);
        var isLoading = this.UseState(true);
        var error = this.UseState<string?>(() => null);

        this.UseEffect(async () =>
        {
            try
            {
                isLoading.Value = true;
                var result = await productService.ListAsync();
                products.Value = result?.Items?.ToArray() ?? Array.Empty<Product>();
            }
            catch (ShopifyException ex)
            {
                error.Value = $"Shopify error: {ex.Message}";
            }
            catch (Exception ex)
            {
                error.Value = ex.Message;
            }
            finally
            {
                isLoading.Value = false;
            }
        }, []);

        object productCard(Product p)
        {
            var imageUrl = p.Images?.FirstOrDefault()?.Src ?? "";
            var price = p.Variants?.FirstOrDefault()?.Price ?? 0m;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return new Card(
                    Layout.Vertical().Gap(3)
                    | Text.H4(p.Title ?? "Untitled")
                    | Text.Block(price > 0 ? $"{price:C}" : "")
                );
            }

            return new Card(
                Layout.Vertical().Gap(3)
                | Text.H4(p.Title ?? "Untitled")
                | new Html($"<img src=\"{imageUrl}\" style=\"width:100%;height:200px;object-fit:cover;border-radius:8px\" />")
                | Text.Block(price > 0 ? $"{price:C}" : "")
            );
        }

        var header = Layout.Horizontal().Align(Align.Right)
            | Text.Block("Domain: " + "example.myshopify.com");

        object body;
        if (error.Value != null)
        {
            body = Text.Block("Error: " + error.Value);
        }
        else if (isLoading.Value)
        {
            body = Layout.Center() | Text.Block("Loading products...");
        }
        else if (products.Value?.Length == 0)
        {
            body = Layout.Center() | Text.Block("No products found.");
        }
        else
        {
            body = Layout.Grid().Columns(4) | products.Value!.Select(productCard).ToArray();
        }

        return Layout.Horizontal().Align(Align.Center)
               | new HeaderLayout(header, Layout.Vertical().Gap(4)
                    | Text.H2("Products")
                    | body
                 ).Width(Size.Full().Max(300));
    }
}