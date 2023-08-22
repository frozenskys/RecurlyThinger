using Recurly;
using Recurly.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RecurlyThinger
{
    public class PlansController
    {
        public Client client;

        public PlansController() 
        {
            var sandboxApiKey = "71253a7d77da4149b498748b94a3996a";
            var options = new ClientOptions()
            {
                Region = ClientOptions.Regions.EU
            };
            client = new Client(sandboxApiKey, options);
        }

        public void GetListPlanDetails()
        {
            var plans = client.ListPlans();
            foreach (Plan plan in plans.Where(x => x.State == Recurly.Constants.ActiveState.Active))
            {
                Console.WriteLine($"Plan.Id {plan.Id}, plan.Code {plan.Code}");

                var addOns = client.ListPlanAddOns(plan.Id);
                foreach (AddOn addOn in addOns.Where(x => x.State == Recurly.Constants.ActiveState.Active))
                {
                    Console.WriteLine($"Addon for plan {plan.Name} - {addOn.Code} {addOn.Name} {addOn.Id}");
                }
            }
        }

        public void GetPlanDetails(string planId)
        {
            var plans = client.ListPlans();
            var plan = plans.Where(x => x.State == Recurly.Constants.ActiveState.Active && x.Id == planId).SingleOrDefault();

            if (plan != null)
            {
                Console.WriteLine($"Plan.Id {plan.Id}");

                var addOns = client.ListPlanAddOns(plan.Id);
                foreach (AddOn addOn in addOns.Where(x => x.State == Recurly.Constants.ActiveState.Active))
                {
                    Console.WriteLine($"Addon for plan {plan.Name} - {addOn.Code} {addOn.Name} {addOn.Id}");
                }
            } 
        }

        public Plan GetPlan(string planId)
        {
            return client.GetPlan(planId);
        }

        public Plan GetPlanByCode(string code)
        {
            return client.ListPlans().Where(x=> x.Code == code).SingleOrDefault();
        }

        public Plan? UpdatePlan(string planId, string newPlanName)
        {
            Console.WriteLine("Attempting UpdatePlan()");
            try
            {
                var planReq = new PlanUpdate()
                {
                    Name = newPlanName,
                };
                Plan plan = client.UpdatePlan(planId, planReq);
                Console.WriteLine($"Updated plan {plan.Code}");

                return plan;
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

        public List<AddOn> GetPlanAddons(string planId) 
        {
            return client.ListPlanAddOns(planId).ToList();
        }

        public AddOn? AddAddonToPlan(string planId, string newAddonCode, string newAddonName, decimal newAddonPrice)
        {
            Console.WriteLine("Attempting AddAddonToPlan()");
            // Create an addon for plan
            try
            {
                var addOnReq = new AddOnCreate()
                {
                    Code = newAddonCode,
                    Name = newAddonName,
                    DefaultQuantity = 1,
                    Currencies = new List<AddOnPricing>() {
                        new AddOnPricing() {
                            Currency = "GBP",
                            UnitAmount = newAddonPrice
                        }
                    }
                };
                
                AddOn addOn = client.CreatePlanAddOn(planId, addOnReq);
                Console.WriteLine($"Created add-on {addOn.Code}");
                
                return addOn;
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

        public void RemoveAddonFromPlan(string planId, string planAddOnId)
        {
            Console.WriteLine("Attempting RemoveAddonFromPlan()");
            try
            {
                AddOn addOn = client.RemovePlanAddOn(planId, planAddOnId);
                Console.WriteLine($"Removed Plan Add-On: {addOn.Code}");
            }
            catch (Recurly.Errors.NotFound ex)
            {
                Console.WriteLine($"Resource Not Found: {ex.Error.Message}");
            }
            catch (Recurly.Errors.ApiError ex)
            {
                Console.WriteLine($"Unexpected Recurly Error: {ex.Error.Message}");
            }
        }
    }
}
