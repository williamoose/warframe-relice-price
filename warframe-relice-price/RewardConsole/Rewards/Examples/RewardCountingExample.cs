using Rewards.Processing;
using Rewards.Services;

namespace Rewards.Examples;

public static class RewardCountingExample
{
    public static async Task Run()
    {
        string ocrText =
            "Atlas Prime Neuroptics Blueprint Scourge Prime Blade Burston Prime Stock";

        var items = RewardCounter.Count(ocrText);
        var market = new WarframeMarketClient();

        foreach (var item in items)
        {
            string urlName = WarframeMarketNaming.ToUrlName(item.CanonicalName);

            int? price =
                await market.GetLowestPriceAsync(urlName);

            Console.WriteLine(
                $"{item.CanonicalName}: {(price is null ? "No listings" : price + "p")}"
            );
        }
    }
}
