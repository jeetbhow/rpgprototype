using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Godot;


public static class TreeBuilder
{
    public static Dictionary<string, DialogueEntry[]> ParseJson(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new ArgumentException($"Cannot open file: {path}");
        }

        string jsonStr = file.GetAsText();
        file.Close();

        var json = JsonConvert.DeserializeObject<Dictionary<string, DialogueEntry[]>>(jsonStr);
        return json;
    }

    public static DialogueTree CreateTree(string jsonPath)
    {
        Dictionary<string, DialogueEntry[]> dict = ParseJson(jsonPath);
        Dictionary<string, DialogueNode[]> nodes = [];

        // First Pass - Collect all of the sequential nodes together into a dictionary.
        foreach (var kv in dict)
        {
            string key = kv.Key;
            DialogueEntry[] entries = kv.Value;
            nodes[key] = new DialogueNode[entries.Length];

            for (int i = 0; i < entries.Length; i++)
            {
                var currEntry = entries[i];
                var node = new DialogueNode
                {
                    Key = key,
                    Text = CleanText(currEntry.Text),
                    Name = currEntry.Name,
                    Portrait = currEntry.Portrait
                };

                if (currEntry.Options != null)
                {
                    Option[] options = new Option[currEntry.Options.Length];
                    for (int j = 0; j < options.Length; j++)
                    {
                        Option option = new()
                        {
                            Text = currEntry.Options[j].Text,
                            NextId = currEntry.Options[j].Next,
                        };
                        options[j] = option;
                    }
                    node.Options = options;
                }

                node.NextId = currEntry.Next;
                nodes[key][i] = node;
            }
        }

        // 2nd Pass - Connect all of the nodes together.
        foreach (var kv in nodes)
        {
            DialogueNode[] linkedNodes = kv.Value;
            for (int i = 0; i < linkedNodes.Length; i++)
            {
                DialogueNode currNode = linkedNodes[i];
                if (currNode.Options != null)
                {
                    foreach (Option option in currNode.Options)
                    {
                        option.Next = option.NextId != null ? nodes[option.NextId][0] : null;
                    }
                }
                currNode.Next = currNode.NextId != null ? nodes[currNode.NextId][0] : null;
            }
        }

        DialogueTree tree = new()
        {
            Root = nodes["root"][0]
        };
        return tree;
    }

    private static string CleanText(string text)
    {
        return text
            .Replace("\n", " ")
            .Replace("\t", "")
            .Replace("\r", "");
    }
}
