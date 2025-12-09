using System.Collections.Generic;

public class MarkovChain
{
    private Dictionary<int, List<int>> transitions = new Dictionary<int, List<int>>();
    private System.Random random = new System.Random();

    public void Train(int[] sequence)
    {
        for (int i = 0; i < sequence.Length - 1; i++)
        {
            int current = sequence[i];
            int next = sequence[i + 1];

            if (!transitions.ContainsKey(current))
                transitions[current] = new List<int>();

            transitions[current].Add(next);
        }
    }

    public int Next(int current)
    {
        if (!transitions.ContainsKey(current))
            return current;

        List<int> possibilities = transitions[current];
        return possibilities[random.Next(possibilities.Count)];
    }

    public int[] Generate(int startNote, int length)
    {
        int[] result = new int[length];
        result[0] = startNote;

        for (int i = 1; i < length; i++)
        {
            result[i] = Next(result[i - 1]);
        }

        return result;
    }
}
