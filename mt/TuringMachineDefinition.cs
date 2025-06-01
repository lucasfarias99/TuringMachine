using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMDefinition
{
    public class TuringMachineDefinition
    {
        [JsonPropertyName("states")]
        public List<string> States { get; set; }

        [JsonPropertyName("tape_alphabet")]
        public List<string> TapeAlphabet { get; set; }

        [JsonPropertyName("initial_state")]
        public string InitialState { get; set; }

        [JsonPropertyName("final_states")]
        public List<string> FinalStates { get; set; }

        [JsonPropertyName("transitions")]
        public Dictionary<string, Dictionary<string, List<string>>> Transitions { get; set; }
    }

    public class MachineDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("definition")]
        public TuringMachineDefinition Definition { get; set; }
    }

    public class MachineList
    {
        [JsonPropertyName("machines")]
        public List<MachineDefinition> Machines { get; set; }
    }
}