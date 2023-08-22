namespace RecurlyThinger
{
    public static class PricingEngine
    {
        public static decimal CalculatePrice(Product product, List<Answer> answers)
        {
            switch (product.Code)
            {
                case "bsmug":
                    var mugColour = answers?.FirstOrDefault(x => x.Key == "favcol").Value;
                    return mugColour?.ToLower() switch
                    {
                        "red" => 3m,
                        "blue" => 5m,
                        "green" => 7m,
                        _ => 20m
                    };
                case "ins":
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
                case "breakdown":
                    var weekendCover = answers?.FirstOrDefault(x => x.Key == "wkdcvr").Value;
                    return weekendCover.ToLower() == "no" ? 4m : 5m;
                default:
                    return 0m;
            }
        }
    } 
}
