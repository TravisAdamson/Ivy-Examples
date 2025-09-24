# ShopifySharp Demo 

Web application created using [Ivy](https://github.com/Ivy-Interactive/Ivy). 

Ivy is a web framework for building interactive web applications using C# and .NET.

## Setup

Before running, set the required Shopify environment variables:

```powershell
$env:SHOPIFY_SHOP_DOMAIN="your-store.myshopify.com"
$env:SHOPIFY_ACCESS_TOKEN="your-access-token"
```

## Run

```
dotnet watch
```

## Deploy

```
ivy deploy
```