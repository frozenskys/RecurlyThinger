using Recurly;
using Recurly.Resources;
using RecurlyThinger;
using Spectre.Console;

var products = DisplayProducts.All.ToArray();

var selectedProducts = AnsiConsole.Prompt(
    new MultiSelectionPrompt<DisplayProduct>()
    .Title("What are you buying?")
    .AddChoices(products)
    .UseConverter(x => $"{x.DisplayName}")
    );

var answers = new List<Answer>();

foreach (var product in selectedProducts)
{
    foreach (var questionKey in product.QuestionKeys)
    {
        if (!answers.Any(x => x.Key == questionKey))
        {
            var questionText = Questions.All.FirstOrDefault(x => x.Key == questionKey);
            Console.WriteLine(questionText.Text);
            var answerValue = Console.ReadLine();
            answers.Add(new Answer { Key = questionKey, Value = answerValue });
        }
    }
}

foreach (var product in selectedProducts)
{
    product.Price = PricingEngine.CalculatePrice(product, answers);
    AnsiConsole.WriteLine($"{product.DisplayName} will cost £{product.Price} per month");
}

var happyToContinue = AnsiConsole.Confirm("Are you happy to continue?");

if (!happyToContinue)
{
    AnsiConsole.WriteLine("Okay, bye!");
    Environment.Exit(0);
}

var apiKey = "2e638cc52d174f56bcc5afec28924a07";
var options = new ClientOptions()
{
    Region = ClientOptions.Regions.EU
};
var client = new Client(apiKey, options);


var pc = new PlanCreate();

var code = Guid.NewGuid().ToString();

pc.Name = "Subscription";
pc.PricingModel = Recurly.Constants.PricingModelType.Fixed;
pc.IntervalUnit = Recurly.Constants.IntervalUnit.Months;
pc.Code = code;
pc.Currencies = new List<PlanPricing> { new PlanPricing() { Currency = "GBP", SetupFee = 0m, UnitAmount = 5.99m } };

pc.AddOns = new List<AddOnCreate>();

foreach (var product in selectedProducts)
{
    var addonProduct = new AddOnCreate
    {
        Name = product.DisplayName,
        Code = $"{code}-{product.Name}",
        Currencies = new List<AddOnPricing>
        {
            new AddOnPricing
            {
                Currency = "GBP",
                UnitAmount = product.Price
            }
        }
        
    };
    pc.AddOns.Add(addonProduct);
}

var aaa = client.CreatePlan(pc);

var purchaseUrl = $"https://bntstest.eu.recurly.com/subscribe/{code}";
System.Diagnostics.Process.Start("explorer", purchaseUrl);
