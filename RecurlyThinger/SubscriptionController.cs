using Recurly;
using Recurly.Constants;
using Recurly.Resources;
using RecurlyThinger;

public class SubscriptionController
{
    public Client _client;
    public string _sandboxUrl;
    public PlansController _plansController;
	public SubscriptionController()
	{
        _sandboxUrl = "https://bnc2.eu.recurly.com/";
        var sandboxApiKey = "71253a7d77da4149b498748b94a3996a";
        var options = new ClientOptions()
        {
            Region = ClientOptions.Regions.EU
        };
        _client = new Client(sandboxApiKey, options);
        _plansController = new();
    }

    public Subscription GetSubscription(string subscriptionId)
    {
        return _client.GetSubscription(subscriptionId);
    }

    // Subs for account, returning the first one.
    public Subscription? GetAccountSubscription(string accountId)
    {
        var subscriptions = _client.ListAccountSubscriptions(accountId);
        var activeSubs = new List<Subscription>();
        // Active, future maybe others, paused?
        foreach (Subscription subscription in subscriptions.Where(x => x.State == Recurly.Constants.SubscriptionState.Active))
        {
            Console.WriteLine($"Subscription.Id {subscription.Id}");
            activeSubs.Add(subscription);
        }

        return activeSubs.FirstOrDefault();
    }

    // In this case, add an addon that exists on the plan but not the subscription
    public SubscriptionChange? ChangeSubscription(string subscriptionId)
	{
        // Get existing addons for sub
        var subscription = GetSubscription(subscriptionId);
        PlansController pc = new();
        var existingAddons = pc.GetPlanAddons(subscription.Plan.Id);


        // Add a new addon
        // MUST BE EXISTING ADDON/ITEM
        var newAddonsList = new List<SubscriptionAddOnUpdate>() {
            new SubscriptionAddOnUpdate()
            {
                Code = "bsmug", 
                // item / addon
                AddOnSource = AddOnSource.Item,
                Quantity= 1,
                RevenueScheduleType = RevenueScheduleType.AtRangeStart,
                UnitAmount = 1500             
            }
        };
        // Add existing subscriptionAddons (already part of the sub) so they don't get overwritten.
        foreach(var existingAddon in subscription.AddOns)
        {
            newAddonsList.Add(new SubscriptionAddOnUpdate()
            {
                Id = existingAddon.Id,
            });
        }

        try
        {
            var changeReq = new SubscriptionChangeCreate()
            {
                AddOns = newAddonsList,
                Timeframe = ChangeTimeframe.Now // choose "now" or "renewal"
            };
            SubscriptionChange change = _client.CreateSubscriptionChange(subscriptionId, changeReq);
            Console.WriteLine($"Created subscription change {change.Id}");

            return change;
        }
        catch (Recurly.Errors.Validation ex)
        {
            Console.WriteLine($"Failed validation: {ex.Error.Message}");
        }
        catch (Recurly.Errors.ApiError ex)
        {
            Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
        }

        return null;
    }

    public void CreateSubscription(List<Product> selectedProducts)
    {
        // Get the plan
        #region Does nothing
        var plan = _plansController.GetPlanByCode("benplan");
        var planAddons = _plansController.GetPlanAddons(plan.Id);

        foreach(var addon in planAddons)
        {
            addon.Currencies.Add(new AddOnPricing()
            {
                Currency = "GBP",
                UnitAmount = selectedProducts.Where(x => x.Code == addon.Code).Select(x => x.Price).SingleOrDefault()
            });
        }
        #endregion Does nothing

        var purchaseUrl = $"{_sandboxUrl}subscribe/benplan";
        System.Diagnostics.Process.Start("explorer", purchaseUrl);
    }
}