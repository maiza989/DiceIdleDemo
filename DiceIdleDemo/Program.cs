using System;
using System.Threading;
using System.Threading.Tasks;

class IdleDiceGame
{
    private static readonly Random random = new Random();
    private int diceCount = 1;
    private int money = 0;
    private int moneyPerRoll = 1;
    private bool autoRollUnlocked = false;
    private bool autoRolling = false;
    private bool gambleDiceUnlocked = false;
    private bool doubleMoneyPowerUp = false;
    private bool extraDicePowerUp = false;
    private bool luckyRollPowerUp = false;
    private bool tripleMoneyPowerUp = false;
    private bool megaDicePowerUp = false;
    private bool safeGamblePowerUp = false;
    private bool rollStreakPowerUp = false;
    private CancellationTokenSource autoRollCts;

    public void Start()
    {
        Console.WriteLine("Welcome to Idle Dice Game!");
        Console.WriteLine("Roll the dice manually to earn money and buy upgrades!\n");

        while (true)
        {
            DisplayMenu();
            string choice = Console.ReadLine();
            HandleInput(choice);
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine($"\nMoney: ${money}");
        Console.WriteLine("1) Roll Dice");
        Console.WriteLine("2) Visit Store");
        if (gambleDiceUnlocked)
            Console.WriteLine("3) Roll Gamble Dice");
        Console.WriteLine("4) Buy Power-ups");
        Console.WriteLine("5) Exit Game");
        Console.Write("Choose an option: ");
    }

    private void HandleInput(string choice)
    {
        switch (choice)
        {
            case "1": RollDice(); break;
            case "2": OpenStore(); break;
            case "3": if (gambleDiceUnlocked) GambleRoll(); break;
            case "4": BuyPowerUps(); break;
            case "5": ExitGame(); break;
            default: Console.WriteLine("Invalid choice, try again."); break;
        }
    }

    private void BuyPowerUps()
    {
        Console.WriteLine("\nPower-ups:");
        Console.WriteLine("1) Double Money for Next Roll (Cost: $20)");
        Console.WriteLine("2) Extra Dice for Next Roll (Cost: $15)");
        Console.WriteLine("3) Lucky Roll - Guarantees a 6 on one die (Cost: $30)");
        Console.WriteLine("4) Triple Money for Next Roll (Cost: $50)");
        Console.WriteLine("5) Mega Dice - Rolls 1-12 instead of 1-6 (Cost: $40)");
        Console.WriteLine("6) Safe Gamble - Lose only half on bad rolls (Cost: $25)");
        Console.WriteLine("7) Roll Streak - Auto rolls 3 times (Cost: $35)");
        Console.WriteLine("8) Exit Power-up Store");
        Console.Write("Choose an option: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1": ActivatePowerUp(ref doubleMoneyPowerUp, 20, "Double Money Power-up Activated!"); break;
            case "2": ActivatePowerUp(ref extraDicePowerUp, 15, "Extra Dice Power-up Activated!"); break;
            case "3": ActivatePowerUp(ref luckyRollPowerUp, 30, "Lucky Roll Activated!"); break;
            case "4": ActivatePowerUp(ref tripleMoneyPowerUp, 50, "Triple Money Power-up Activated!"); break;
            case "5": ActivatePowerUp(ref megaDicePowerUp, 40, "Mega Dice Activated!"); break;
            case "6": ActivatePowerUp(ref safeGamblePowerUp, 25, "Safe Gamble Activated!"); break;
            case "7": ActivatePowerUp(ref rollStreakPowerUp, 35, "Roll Streak Activated!"); break;
            case "8": return;
            default: Console.WriteLine("Invalid choice, try again."); break;
        }
    }

    private void ActivatePowerUp(ref bool powerUp, int cost, string message)
    {
        if (money >= cost)
        {
            money -= cost;
            powerUp = true;
            Console.WriteLine(message);
        }
        else
        {
            Console.WriteLine("Not enough money!");
        }
    }
    private void RollDice()
    {
        Console.Clear();
        int totalEarnings = 0;
        int actualDiceCount = diceCount + (extraDicePowerUp ? 1 : 0);

        Console.WriteLine("Rolling Dice...");

        for (int i = 0; i < actualDiceCount; i++)
        {
            int roll = luckyRollPowerUp ? 6 : (megaDicePowerUp ? random.Next(1, 13) : random.Next(1, 7));
            totalEarnings += roll * moneyPerRoll;
        }

        if (luckyRollPowerUp)
            Console.WriteLine("Lucky Roll Power-up applied! All dice rolled a 6!");

        if (megaDicePowerUp)
            Console.WriteLine("Mega Dice Power-up applied! Dice range increased to 1-12!");

        if (tripleMoneyPowerUp)
        {
            totalEarnings *= 3;
            Console.WriteLine("Triple Money Power-up applied! Earnings tripled!");
        }
        else if (doubleMoneyPowerUp)
        {
            totalEarnings *= 2;
            Console.WriteLine("Double Money Power-up applied! Earnings doubled!");
        }

        if (extraDicePowerUp)
            Console.WriteLine("Extra Dice Power-up applied! Rolled an additional dice!");

        money += totalEarnings;
        Console.WriteLine($"You rolled {actualDiceCount} dice and earned ${totalEarnings}!");

        ResetPowerUps();
    }

    private void ResetPowerUps()
    {
        doubleMoneyPowerUp = false;
        extraDicePowerUp = false;
        luckyRollPowerUp = false;
        tripleMoneyPowerUp = false;
        megaDicePowerUp = false;
    }

    private void OpenStore()
    {
        while (true)
        {
            Console.WriteLine("\nStore:");
            Console.WriteLine("1) Buy More Dice (Cost: $10)");
            Console.WriteLine("2) Increase Money Per Roll (Cost: $10)");
            Console.WriteLine(autoRollUnlocked ? "3) Toggle Auto-Roll" : "3) Unlock Auto-Roll (Cost: $20)");
            if (!gambleDiceUnlocked)
                Console.WriteLine("4) Unlock Gamble Dice (Cost: $15)");
            Console.WriteLine("5) Exit Store");
            Console.Write("Choose an option: ");

            string storeChoice = Console.ReadLine();
            if (storeChoice == "5") break;
            HandleStoreInput(storeChoice);
        }
    }

    private void HandleStoreInput(string choice)
    {
        switch (choice)
        {
            case "1": BuyUpgrade(ref diceCount, 10, "You bought another dice!"); break;
            case "2": BuyUpgrade(ref moneyPerRoll, 10, "Money per roll increased!"); break;
            case "3": HandleAutoRoll(); break;
            case "4": if (!gambleDiceUnlocked) HandleGambleDice(); break;
            default: Console.WriteLine("Invalid choice, try again."); break;
        }
    }

    private void BuyUpgrade(ref int stat, int cost, string successMessage)
    {
        if (money >= cost)
        {
            money -= cost;
            stat++;
            Console.WriteLine(successMessage);
        }
        else
        {
            Console.WriteLine("Not enough money!");
        }
    }

    private void HandleAutoRoll()
    {
        if (!autoRollUnlocked && money >= 20)
        {
            money -= 20;
            autoRollUnlocked = true;
            Console.WriteLine("Auto-roll unlocked! You can now enable it.");
        }
        else if (!autoRollUnlocked)
        {
            Console.WriteLine("Not enough money to unlock auto-roll!");
        }
        else
        {
            autoRolling = !autoRolling;
            Console.WriteLine(autoRolling ? "Auto-roll enabled!" : "Auto-roll disabled!");
            if (autoRolling) StartAutoRoll();
            else StopAutoRoll();
        }
    }

    private async void StartAutoRoll()
    {
        autoRollCts = new CancellationTokenSource();
        var token = autoRollCts.Token;
        await Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                RollDice();
                await Task.Delay(3000, token);
            }
        }, token);
    }

    private void StopAutoRoll()
    {
        autoRollCts?.Cancel();
    }

    private void HandleGambleDice()
    {
        if (!gambleDiceUnlocked && money >= 15)
        {
            money -= 15;
            gambleDiceUnlocked = true;
            Console.WriteLine("Gamble Dice unlocked! You can now gamble.");
        }
        else
        {
            Console.WriteLine("Not enough money to unlock Gamble Dice!");
        }
    }

    private void GambleRoll()
    {
        int gambleRoll = random.Next(1, 778);
        int amount = Math.Min(gambleRoll * 777, money);
        if (gambleRoll == 777)
        {
            money += amount;
            Console.WriteLine($"You rolled a {gambleRoll} and WON ${amount}!");
        }
        else
        {
            money = Math.Max(0, money - amount);
            Console.WriteLine($"You rolled a {gambleRoll} and LOST ${amount}...");
        }
    }

    private void ExitGame()
    {
        Console.WriteLine("Thanks for playing!");
        Environment.Exit(0);
    }

    static void Main()
    {
        new IdleDiceGame().Start();
    }
}
