using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using TM;
using TMDefinition;

class Program
{
    private const int PromptInterval = 50;
    private static int _stepsSinceLastPrompt = 0;

    static void Main()
    {
        string jsonPath = "machine.json";
        MachineList machineListContainer;

        try
        {
            string jsonContent = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            machineListContainer = JsonSerializer.Deserialize<MachineList>(jsonContent, options);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Erro ao carregar máquinas: {ex.Message}");
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        if (machineListContainer?.Machines == null || !machineListContainer.Machines.Any())
        {
            Console.WriteLine("Nenhuma máquina encontrada no arquivo.");
            Console.ReadKey();
            return;
        }

        // Machine selection
        Console.WriteLine("Máquinas disponíveis:");
        for (int i = 0; i < machineListContainer.Machines.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {machineListContainer.Machines[i].Name} - {machineListContainer.Machines[i].Description}");
        }

        TuringMachineDefinition selectedDefinition = null;
        while (selectedDefinition == null)
        {
            Console.Write($"Escolha (1-{machineListContainer.Machines.Count}): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= machineListContainer.Machines.Count)
            {
                selectedDefinition = machineListContainer.Machines[choice - 1].Definition;
            }
        }

        // Input word
        Console.Write("Digite a palavra: ");
        string input = Console.ReadLine() ?? "";

        // Initialize machine
        var tm = new TuringMachine();
        tm.Configure(selectedDefinition);
        tm.InitializeTape(input);

        Console.WriteLine("\nConfiguração Inicial:");
        tm.PrintConfiguration();

        Console.WriteLine("\nControles:");
        Console.WriteLine("H      - Passo único");
        Console.WriteLine("S      - 10 passos");
        Console.WriteLine("F      - Executar até final");
        Console.WriteLine("Q      - Sair");

        bool running = true;
        int stepCount = 0;
        bool shouldPrompt = false;

        while (running && !tm.IsHalted() && !tm.IsRejected())
        {
            if (shouldPrompt)
            {
                Console.Write($"\nMáquina não parou após {stepCount} passos. Continuar? (s/n): ");
                var response = Console.ReadLine();
                if (response?.ToLower() != "s")
                {
                    running = false;
                    continue;
                }
                _stepsSinceLastPrompt = 0;
                shouldPrompt = false;
            }

            if (!Console.KeyAvailable && _stepsSinceLastPrompt < PromptInterval)
            {
                continue;
            }

            var key = Console.ReadKey(true).Key;
            int steps = 1;

            switch (key)
            {
                case ConsoleKey.Enter:
                case ConsoleKey.H:
                    steps = 1;
                    break;
                case ConsoleKey.S:
                    steps = 10;
                    break;
                case ConsoleKey.F:
                    steps = int.MaxValue;
                    break;
                case ConsoleKey.Q:
                    running = false;
                    continue;
                default:
                    continue;
            }

            for (int i = 0; i < steps && running && !tm.IsHalted() && !tm.IsRejected(); i++)
            {
                Console.WriteLine($"\nPasso {stepCount + 1}:");
                if (!tm.Step())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("REJEITADO - Sem transição válida!");
                    Console.ResetColor();
                    running = false;
                    break;
                }
                tm.PrintConfiguration();
                stepCount++;
                _stepsSinceLastPrompt++;

                if (_stepsSinceLastPrompt >= PromptInterval)
                {
                    shouldPrompt = true;
                    break;
                }
            }
        }

        // Final result
        Console.WriteLine("\n--- Resultado Final ---");
        if (tm.IsHalted())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ACEITA");
        }
        else if (tm.IsRejected())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("REJEITADA");
        }
        else
        {
            Console.WriteLine("INTERROMPIDA");
        }
        Console.ResetColor();
        tm.PrintConfiguration();
        Console.ReadKey();
    }
}