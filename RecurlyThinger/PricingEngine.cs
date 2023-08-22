namespace RecurlyThinger
{
    public static class PricingEngine
    {
        public static decimal CalculatePrice(DisplayProduct product, List<Answer> answers)
        {
            switch (product.Name)
            {
                case "hat":
                    var hatColour = answers?.FirstOrDefault(x => x.Key == "favcol").Value;
                    return hatColour?.ToLower() switch
                    {
                        "red" => 3m,
                        "blue" => 5m,
                        "green" => 7m,
                        _ => 20m
                    };
                case "insurance":
                    var bikeReg = answers?.FirstOrDefault(x => x.Key == "bikereg").Value;
                    var miles = Convert.ToInt32(answers?.FirstOrDefault(x => x.Key == "bikemiles").Value);
                    var milesMultiplier = miles < 1000 ? 1 : miles / 1000;
                    return bikeReg.ToLower().FirstOrDefault() switch
                    {
                        'a' => 5m * milesMultiplier,
                        'b' => 10m * milesMultiplier,
                        'c' => 15m * milesMultiplier,
                        _ => 100m * milesMultiplier
                    };
                default:
                    return 0m;
            }
        }
    }
}
