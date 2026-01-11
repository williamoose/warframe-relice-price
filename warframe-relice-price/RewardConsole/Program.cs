using Rewards.Examples;
// See https://aka.ms/new-console-template for more information
using FuzzySharp;

int score = Fuzz.TokenSetRatio("ash prime blueprint", "atlas prime blueprint");
Console.WriteLine($"Fuzzy match score: {score}");

await RewardCountingExample.Run(); // call it here for now


