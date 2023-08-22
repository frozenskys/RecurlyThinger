using Recurly.Resources;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecurlyThinger
{
    public class Journey
    {
        public ProductsController productsController;
        public Journey() 
        { 
            productsController = new ProductsController();
        }

        public List<Product> JourneyStart()
        {
            var products = productsController.GetAll();
            Console.WriteLine("Bennetts Journey Start");
            Console.WriteLine("[X] Subscription");
            Console.WriteLine();

            var selectedProducts = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Product>()
                    .Title("What else would you like to add?")
                    .AddChoices(products)
                    .UseConverter(x => $"{x.Name}")
            );
            foreach (var product in selectedProducts)
            {
                AnsiConsole.WriteLine($"[X] {product.Name}");
            }

            Console.WriteLine();

            return selectedProducts;
        }

        public void SelectionBasedQuestions(List<Product> selectedProducts)
        {
            decimal? totalPrice = 0m;
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

            // Manually add sub price for now
            totalPrice += 5.99m;

            foreach (var product in selectedProducts)
            {
                product.Price = PricingEngine.CalculatePrice(product, answers);
                AnsiConsole.WriteLine($"{product.Name} will cost £{product.Price} per month");
                totalPrice += product.Price;
            }


            AnsiConsole.WriteLine($"Your total price comes to {totalPrice}");

            var happyToContinue = AnsiConsole.Confirm("Are you happy to continue?");

            if (!happyToContinue)
            {
                AnsiConsole.WriteLine("Okay, bye!");
                Environment.Exit(0);
            }
        }
    }
}
