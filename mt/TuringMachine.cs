using System.Collections.Generic;
using TMDefinition;

namespace TM
{
    public class TuringMachine
    {
        private List<char> tape = new List<char>();
        private int head = 0;
        private string currentState;
        private TuringMachineDefinition definition;
        private Dictionary<(string, char), (string nextState, char writeSymbol, char direction)> transitionTable
            = new Dictionary<(string, char), (string, char, char)>();
        private bool wordRejected = false;

        public void Configure(TuringMachineDefinition def)
        {
            this.definition = def;
            this.currentState = this.definition.InitialState;
            this.transitionTable.Clear();

            foreach (var stateEntry in this.definition.Transitions)
            {
                string currentState = stateEntry.Key;
                foreach (var transitionRule in stateEntry.Value)
                {
                    char readSymbol = transitionRule.Key[0];
                    List<string> ruleDetails = transitionRule.Value;

                    var key = (currentState, readSymbol);
                   var value = (
                        nextState: ruleDetails[0],
                        writeSymbol: ruleDetails[1][0],
                        direction: ruleDetails[2][0]
                    );
                    this.transitionTable[key] = value;
                }
            }
        }

        public void InitializeTape(string word)
        {
            tape.Clear();
            tape.Add('>');
            tape.Add('u');
            foreach (char c in word) tape.Add(c);
            tape.Add('u');
            head = 1;
            currentState = definition.InitialState;
            wordRejected = false;
        }

        public bool Step()
        {
            if (IsHalted() || wordRejected)
                return false;

            char currentSymbol = tape[head];
            var transitionKey = (currentState, currentSymbol);

            if (!transitionTable.TryGetValue(transitionKey, out var action))
            {
                wordRejected = true;
                return false;
            }

            tape[head] = action.writeSymbol;
            currentState = action.nextState;

            if (action.direction == 'R')
            {
                head++;
                if (head >= tape.Count) tape.Add('u');
            }
            else if (action.direction == 'L')
            {
                head--;
                if (head < 0)
                {
                    tape.Insert(0, 'u');
                    head = 0;
                }
            }

            return true;
        }

        public bool IsHalted() => definition.FinalStates.Contains(currentState);
        public bool IsRejected() => wordRejected;

        public void PrintConfiguration()
        {
            Console.WriteLine($"Estado atual: {currentState}");
            Console.Write("Fita: ");
            for (int i = 0; i < tape.Count; i++)
            {
                if (i == head)
                    Console.Write($"[{tape[i]}] ");
                else
                    Console.Write($"{tape[i]} ");
            }
            Console.WriteLine();
        }
    }
}