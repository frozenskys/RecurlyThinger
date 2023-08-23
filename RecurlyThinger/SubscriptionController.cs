using Recurly;
using Recurly.Constants;
using Recurly.Errors;
using Recurly.Resources;
using RecurlyThinger;
using System.Numerics;
using System.Security.Principal;
using System.Xml.Linq;

public class SubscriptionController
{
    public Recurly.Client _client;
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
        _client = new Recurly.Client(sandboxApiKey, options);
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
        foreach (Subscription subscription in subscriptions.Where(x => x.State == SubscriptionState.Active))
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


        // We will have to add our own hosted pages
        /* 
         * Can we create a subscription based on a plan and
         * then add addons to that subscription as subscription.addOns before we bill?
         * 
         * Give it a go in here and create sub without hitting the sandbox
        */
        var purchaseUrl = $"{_sandboxUrl}subscribe/benplan";
        System.Diagnostics.Process.Start("explorer", purchaseUrl);
    }

    public void CreateSub(List<Product> selectedProducts)
    {

        var plan = _plansController.GetPlanByCode("benplan");
        try
        {

            //Billing info address: Name on account can't be blank,
            //Billing info: Account number is too long (maximum is 8 characters),
            //Billing info: Account number is invalid,
            //Billing info: Sort code can't be blank'

            var subReq = new SubscriptionCreate()
            {
                Currency = "GBP",
                Account = new AccountCreate()
                {
                    FirstName = "Tony",
                    LastName = "TheTiger",
                    Email = "imseankeenan+generated@gmail.com",
                    Code = "imseankeenan+generated@gmail.com",
                    BillingInfo= new BillingInfoCreate()
                    {
                        Type = AchType.Bacs,
                        RoutingNumber = "262654",
                        AccountNumber = "12345678",
                        SortCode = "262654",
                        Cvv = "123",
                        Month = "01", Year = "25",
                        NameOnAccount = "Tony Tiger",
                        Address = new Address()
                        {
                            City = "Pickwell",
                            Country = "GB",
                            PostalCode = "CE43AL",
                            Phone = "0123456789",
                            Street1 = "6 The Paddock",
                        },
                    }
                },
                PlanCode = plan.Code,
                AddOns = new List<SubscriptionAddOnCreate>()
            };

            #region dump
            var existingAddons = _plansController.GetPlanAddons(plan.Id);

            var newAddonsList = new List<SubscriptionAddOnCreate>() {
                new SubscriptionAddOnCreate()
                {
                    Code = "bsmug", 
                    // item / addon
                    AddOnSource = AddOnSource.Item,
                    Quantity = 1,
                    UnitAmount = 25m
                },
                new SubscriptionAddOnCreate()
                {
                    Code = "ins", 
                    // item / addon
                    AddOnSource = AddOnSource.Item,
                    Quantity = 1,
                    UnitAmount = 150m
                },
                new SubscriptionAddOnCreate()
                {
                    Code = "breakdown", 
                    // item / addon
                    AddOnSource = AddOnSource.Item,
                    Quantity = 1,
                    UnitAmount = 17m
                },

            };
            #endregion

            subReq.AddOns.AddRange(newAddonsList);

            Subscription subscription = _client.CreateSubscription(subReq);
            Console.WriteLine($"Created Subscription with Id: {subscription.Uuid}");
        }
        catch (Recurly.Errors.Validation ex)
        {
            // If the request was not valid, you may want to tell your user
            // why. You can find the invalid params and reasons in ex.Error.Params
            Console.WriteLine($"Failed validation: {ex.Error.Message}");
        }
        catch (Recurly.Errors.ApiError ex)
        {
            // Use ApiError to catch a generic error from the API
            Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
        }
    }
}