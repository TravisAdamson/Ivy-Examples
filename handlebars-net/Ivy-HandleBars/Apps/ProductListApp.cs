using HandlebarsDotNet;

namespace Ivy_Handlebars_Demo.Apps;

// A simple model for our product data
public class Product
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}

[App(icon: Icons.FileText, title: "Product List with Handlebars")]
public class ProductListApp : ViewBase
{
    // The Handlebars template for the product cards. This is a constant string.
    private const string productsTemplate = @"

 {{#each products}}

     <div style='border: 1px solid #ccc; padding: 1rem; border-radius: 0.5rem; background-color: #f9f9f9;'>

         <h3 style='font-size: 1.25rem; font-weight: bold;'>{{Name}}</h3>

         <p style='color: #666;'>Category: {{Category}}</p>

         <p style='font-size: 1.1rem; color: #000;'>Price: ${{Price}}</p>

     </div>

 {{/each}}

 ";

    public override object? Build()
    {
        // Define the initial state for our list of products.
        var productsState = this.UseState(new List<Product>());

        // State for the product being added in the input fields.
        var productNameState = this.UseState<string>(string.Empty);
        var productPriceState = this.UseState<decimal>(0);
        var productCategoryState = this.UseState<string>(string.Empty);

        // Compile the Handlebars template once
        var template = Handlebars.Compile(productsTemplate);

        // Add a new product to the list
        ValueTask AddProduct(Event<Button> _)
        {
            var products = productsState.Value;
            products.Add(new Product { Name = productNameState.Value, Price = productPriceState.Value, Category = productCategoryState.Value });

            productsState.Value = new List<Product>(products); // Create a new list to trigger a state update

            // Clear the form
            productNameState.Value = string.Empty;
            productPriceState.Value = 0;
            productCategoryState.Value = string.Empty;

            return ValueTask.CompletedTask;
        }

        // Build the UI using Ivy components
        return new Card(
            Layout.Center(
                Layout.Vertical()
                | Layout.Vertical().Gap(10).Padding(2)
                | Text.H2("Product List Demo")
                | Text.Block("Add products below. The list is rendered using Handlebars.Net.")
                | new Separator()
                | Text.Block("New Product Details:")
                | new TextInput(productNameState) { Placeholder = "Product Name" }
                | new TextInput(productPriceState) { Placeholder = "Price" }
                | new TextInput(productCategoryState) { Placeholder = "Category" }
                | new Button("Add Product") { OnClick = AddProduct }
                | new Separator()
                | Text.H2("Products")
                | new Html(template(new { products = productsState.Value }))
            )
        );
    }
}
