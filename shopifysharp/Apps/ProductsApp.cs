namespace ShopifySharpDemo.Apps;

[App(icon: Icons.ShoppingBag, title: "Shopify Products", path: ["Apps"])]
public class ProductsApp : ViewBase
{
    public override object? Build()
    {
        var shopDomain = this.UseState<string>(() => "");
        var accessToken = this.UseState<string>(() => "");
        var products = this.UseState<Product[]?>(() => null);
        var isLoading = this.UseState(false);
        var error = this.UseState<string?>(() => null);

        async Task LoadProducts()
        {
            if (string.IsNullOrWhiteSpace(shopDomain.Value) || string.IsNullOrWhiteSpace(accessToken.Value))
            {
                error.Value = "Please fill in both domain and access token fields";
                return;
            }

            try
            {
                isLoading.Value = true;
                error.Value = null;
                var productService = new ProductService(shopDomain.Value, accessToken.Value);
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
        }

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

        var header = Layout.Vertical().Gap(3)
            | Text.H3("Shopify Products")
            | shopDomain.ToTextInput().Placeholder("Domain (e.g.: example.myshopify.com)")
            | accessToken.ToTextInput().Placeholder("Access Token")
            | new Button("Get Products", onClick: async _ => await LoadProducts());

        object body;
        if (error.Value != null)
        {
            body = Text.Block("Error: " + error.Value);
        }
        else if (isLoading.Value)
        {
            body = Layout.Center() | Text.Block("Loading products...");
        }
        else if (products.Value == null)
        {
            body = Layout.Center() | Text.Block("Click the button above to view products");
        }
        else if (products.Value.Length == 0)
        {
            body = Layout.Center() | Text.Block("No products found.");
        }
        else
        {
            body = Layout.Grid().Columns(4) | products.Value.Select(productCard).ToArray();
        }

        return Layout.Vertical().Gap(4)
               | header
               | body;
    }
}