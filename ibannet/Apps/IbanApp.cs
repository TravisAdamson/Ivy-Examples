using System;
using System.Linq;
using System.Threading.Tasks;
using IbanNet; // Core IBAN validation and parsing
using Ivy;     // Ivy UI framework

namespace IbanApp.Apps;

// Ivy app declaration with icon and title
[App(icon: Icons.Globe, title: "IBAN Demo App")]
public class IbanModel : ViewBase
{
    public override object? Build()
    {
        // IbanNet core validator and parser
        var validator = new IbanValidator();         // Validates IBAN structure, length, checksum
        var parser = new IbanParser(validator);      // Parses IBAN into components

        // Ivy state hooks for UI interactivity
        var selectedCountry = UseState<string?>(default(string)); // Tracks selected country
        var ibanInput = UseState(() => "");                       // Tracks IBAN input
        var result = UseState(() => (string?)null);               // Stores validation result
        var breakdown = UseState(() => "");                       // Stores parsed IBAN details

        // Ivy async select input: searchable country dropdown
        Task<Option<string>[]> QueryCountries(string query)
        {
            var countries = new[] { "PK", "GB", "DE", "FR", "NL", "CH", "IT", "ES", "SE", "AE" };
            return Task.FromResult(countries
                .Where(c => c.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Select(c => new Option<string>(c))
                .ToArray());
        }

        // Ivy async select input: resolves selected country
        Task<Option<string>?> LookupCountry(string country)
        {
            if (string.IsNullOrEmpty(country)) return Task.FromResult<Option<string>?>(null);
            return Task.FromResult<Option<string>?>(new Option<string>(country));
        }

        // Generates a synthetic IBAN for the selected country
        // This is for demo/testing purposes and not guaranteed to be real
        void GenerateSampleIban()
        {
            var country = selectedCountry.Value ?? "GB";
            var random = new Random();

            // Country-specific bank codes (hardcoded for demo)
            string bankCode = country switch
            {
                "PK" => "SCBL",
                "GB" => "WEST",
                "DE" => "37040044",
                "FR" => "20041010050500013M02606",
                "NL" => "ABNA",
                "CH" => "00762",
                "IT" => "X0542811101000000123456",
                "ES" => "21000418450200051332",
                "SE" => "50000000058398257466",
                "AE" => "0331234567890123456",
                _ => "0000"
            };

            // Country-specific IBAN format (simplified)
            string sample = country switch
            {
                "PK" => $"PK36{bankCode}{random.Next(10000000, 99999999)}{random.Next(1000, 9999)}",
                "GB" => $"GB82{bankCode}{random.Next(10000000, 99999999)}{random.Next(1000, 9999)}",
                "DE" => $"DE89{bankCode}{random.Next(10000000, 99999999)}",
                "FR" => $"FR14{bankCode}",
                "NL" => $"NL91{bankCode}{random.Next(10000000, 99999999)}",
                "CH" => $"CH93{bankCode}{random.Next(10000000, 99999999)}",
                "IT" => $"IT60{bankCode}",
                "ES" => $"ES91{bankCode}",
                "SE" => $"SE45{bankCode}",
                "AE" => $"AE07{bankCode}",
                _ => ""
            };

            // Update UI state
            ibanInput.Value = sample;
            result.Value = null;
            breakdown.Value = "";
        }

        // Validates the IBAN using IbanNet
        void ValidateIban()
        {
            var validation = validator.Validate(ibanInput.Value);

            // If invalid, show error
            if (!validation.IsValid)
            {
                result.Value = "❌ Invalid IBAN";
                breakdown.Value = "";
                return;
            }

            // If valid, parse and show details
            var iban = parser.Parse(ibanInput.Value);
            result.Value = $"✅ Valid IBAN";
            breakdown.Value =
                $"Country: {iban.Country.TwoLetterISORegionName}\n" +
                $"Bank ID: {iban.BankIdentifier}\n" +
                $"Branch ID: {iban.BranchIdentifier}\n" +
                $"Obfuscated: {iban.ToString(IbanFormat.Obfuscated)}";
        }

        // Simulates copying the IBAN to clipboard
        var copyMessage = UseState(() => "");
        void CopyIban() => copyMessage.Value = $"📋 Copied: {ibanInput.Value}";

        // Ivy UI layout
        return Layout.Vertical().Gap(12).Padding(12)

            | Text.H2("🌍 IBAN Explorer")

            // Country selector
            | Text.Label("Select a country:")
            | selectedCountry.ToAsyncSelectInput(QueryCountries, LookupCountry, placeholder: "Search countries...")
            | Text.Small($"Selected: {selectedCountry.Value ?? "None"}")

            // IBAN generator
            | new Button("Generate Sample IBAN", GenerateSampleIban)

            // Manual IBAN input
            | Text.Label("Enter or edit IBAN:")
            | new TextInput(ibanInput).Placeholder("Enter IBAN here...")

            // Validate and copy actions
            | Layout.Horizontal().Gap(8)
                | new Button("Validate IBAN", ValidateIban)
                | new Button("Copy IBAN", CopyIban)

            // Result panel
            | (result.Value != null ? Text.Block(result.Value) : null)
            | (breakdown.Value != "" ? Text.Block(breakdown.Value) : null)
            | (copyMessage.Value != "" ? Text.Small(copyMessage.Value) : null);
    }
}