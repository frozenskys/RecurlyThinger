namespace RecurlyThinger
{
    public static class DisplayProducts
    {
        public static List<DisplayProduct> All => new List<DisplayProduct>
        {
            new DisplayProduct{ Name = "hat", DisplayName = "Funny Hat", QuestionKeys = new List<string> { "favcol" } },
            new DisplayProduct{ Name = "insurance", DisplayName = "Bike Insurance", QuestionKeys = new List<string> { "bikereg", "bikemiles" } },
        };
    }
}
