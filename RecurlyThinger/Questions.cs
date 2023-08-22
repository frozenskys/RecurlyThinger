namespace RecurlyThinger
{
    public static class Questions
    {
        public static List<Question> All => new List<Question>
        {
            new Question{ Key = "fname", Text = "What's your first name?"},
            new Question{ Key = "sname", Text = "What's your last name?"},
            new Question{ Key = "bikereg", Text = "What's your bike's registration?"},
            new Question{ Key = "bikemiles", Text = "How many miles a year do you ride?"},
            new Question{ Key = "favcol", Text = "What's your favourite colour?"}
        };
    }
}
