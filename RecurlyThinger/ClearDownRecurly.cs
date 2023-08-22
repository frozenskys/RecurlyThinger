using Recurly;
using Recurly.Constants;
using Recurly.Resources;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RecurlyThinger
{
    public class ClearDownRecurly
    {

        // Removes everything below from Recurly - In most cases this translates into changing the state to one of Active/Expired.
        public void TotalWipe()
        {
            var sandboxApiKey = "71253a7d77da4149b498748b94a3996a";
            var options = new ClientOptions()
            {
                Region = ClientOptions.Regions.EU
            };
            var client = new Client(sandboxApiKey, options);

            #region Plans
            var plans = client.ListPlans();
            if (plans.Where(x => x.State != ActiveState.Inactive).Any())
            {
                foreach (Plan plan in plans.Where(x => x.State != ActiveState.Inactive))
                {
                    try
                    {
                        client.RemovePlan(plan.Id);
                        Console.WriteLine($"Removed plan {plan.Code}");
                    }
                    catch (Recurly.Errors.Validation ex)
                    {
                        Console.WriteLine($"Failed validation: {ex.Error.Message}");
                    }
                    catch (Recurly.Errors.ApiError ex)
                    {
                        Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No active plans");
            }
            #endregion Plans

            #region Subscriptions
            var subscriptions = client.ListSubscriptions();
            if (subscriptions.Where(x => x.State != SubscriptionState.Expired).Any())
            {
                foreach (Subscription subscription in subscriptions.Where(x => x.State != SubscriptionState.Expired))
                {
                    try
                    {
                        var optionalParams = new TerminateSubscriptionParams()
                        {
                            Refund = RefundType.None
                        };
                        client.TerminateSubscription(subscription.Id, optionalParams);
                        Console.WriteLine($"Terminated Subscription {subscription.Uuid}");
                    }
                    catch (Recurly.Errors.Validation ex)
                    {
                        Console.WriteLine($"Failed validation: {ex.Error.Message}");
                    }
                    catch (Recurly.Errors.ApiError ex)
                    {
                        Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No active subscriptions");
            }
            #endregion Subscriptions

            #region Customers
            var accounts = client.ListAccounts();
            if (accounts.Where(x => x.State != ActiveState.Inactive).Any())
            {
                foreach (Account account in accounts.Where(x => x.State != ActiveState.Inactive))
                {
                    try
                    {
                        client.DeactivateAccount(account.Id);
                        Console.WriteLine($"Deactivated account {account.Code}");
                    }
                    catch (Recurly.Errors.Validation ex)
                    {
                        Console.WriteLine($"Failed validation: {ex.Error.Message}");
                    }
                    catch (Recurly.Errors.ApiError ex)
                    {
                        Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No active Accounts");
            }
            #endregion Customers
        }
    }
}
