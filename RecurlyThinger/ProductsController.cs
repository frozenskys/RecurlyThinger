using Recurly;
using Recurly.Resources;

namespace RecurlyThinger
{
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public List<string> QuestionKeys { get; set; }

    }
    public class ProductsController
    {
        public Client client;
        public ProductsController()
        {
            var sandboxApiKey = "71253a7d77da4149b498748b94a3996a";
            var options = new ClientOptions()
            {
                Region = ClientOptions.Regions.EU
            };
            client = new Client(sandboxApiKey, options);
        }


        // Get all items from Recurly and change to Products
        public List<Product> GetAll()
        {
            var items = client.ListItems().ToList();
            var products = new List<Product>();
            foreach (var item in items)
            {
                products.Add(new Product { 
                    Name = item.Name,
                    Description = item.Description,
                    Code = item.Code,
                    Price = item.Currencies.Select(x => x.UnitAmount).FirstOrDefault(),
                    QuestionKeys = new List<string>()
                });
            }

            // Configure the questionKeys 
            foreach (var product in products) {
                switch (product.Code)
                {
                    case "ins":
                        product.QuestionKeys.Add("bikereg");
                        product.QuestionKeys.Add("bikemiles");
                        break;
                    case "bsmug":
                        product.QuestionKeys.Add("favcol");
                        break;
                    case "breakdown":
                        product.QuestionKeys.Add("wkdcvr");
                        break;
                    default:
                        break;
                }
            }

            return products;
        }
    }
}