using Recurly;
using Recurly.Resources;
using RecurlyThinger;
using Spectre.Console;


#region Cleardown
bool performWipe = false;

if (performWipe) {
    ClearDownRecurly cdr = new();
    cdr.TotalWipe();
}
#endregion Cleardown

#region Start
SubscriptionController subscriptionController = new();
PlansController plansController = new();

Journey journey = new();
var selectedProducts = journey.JourneyStart();
journey.SelectionBasedQuestions(selectedProducts);

subscriptionController.CreateSubscription(selectedProducts);
#endregion Start

#region Subs & Plans
//SubscriptionController subscriptionController = new();
//PlansController plansController = new();

//var sub = subscriptionController.GetAccountSubscription("code-imseankeenan@gmail.com");

//if (sub != null)
//{

//    #region PlanBits
//    // Get sub plan
//    var plan = plansController.GetPlan(sub.Plan.Id);

//    // Update the plan name / other info
//    plansController.UpdatePlan(plan.Id, "The biggest plan ever");

//    //Create and add a new addon to the plan
//    var newAddon = plansController.AddAddonToPlan(plan.Id, "Membership", "BSMembership", 500);

//    // List the plan details
//    plansController.GetPlanDetails(plan.Id);

//    // If we managed to create a new addon, remove it
//    var wantToRemoveAddon = false;
//    if (newAddon != null && wantToRemoveAddon)
//    {
//        plansController.RemoveAddonFromPlan(plan.Id, newAddon.Id);
//    }

//    // List the plan details again
//    plansController.GetPlanDetails(plan.Id);
//    #endregion PlanBits

//    #region SubscriptionBits
//    // Write addons for the plan
//    //var plansAddOns = plansController.GetPlanAddons(plan.Id);
//    //foreach(var addOn in plansAddOns.Where(x => x.State == Recurly.Constants.ActiveState.Active))
//    //{
//    //    Console.WriteLine(addOn.Name);
//    //}

//    var change = subscriptionController.ChangeSubscription(sub.Id);
//    #endregion SubscriptionBits
//}

#endregion Subs & Plans
