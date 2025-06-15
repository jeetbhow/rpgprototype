using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Godot;


public static class TreeBuilder
{
    public static DialogueTree CreateTree(string jsonPath)
    {
        var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new ArgumentException($"Cannot open file: {jsonPath}");
        }

        string jsonStr = file.GetAsText();
        file.Close();

        var root = JObject.Parse(jsonStr);
        var nodes = new Dictionary<string, DialogueNode[]>();

        foreach (var prop in root.Properties())
        {
            string key = prop.Name;
            var arr = (JArray)prop.Value;
            nodes[key] = new DialogueNode[arr.Count];

            for (int i = 0; i < arr.Count; i++)
            {
                JToken entry = arr[i];
                string type = entry["type"]?.Value<string>()
                              ?? throw new JsonException($"Missing type in entry {i} of '{key}'");

                DialogueNode node;
                switch (type)
                {
                    case "regular":
                        {
                            // Only the base fields
                            var data = entry.ToObject<SerializedDialogue>()!;
                            node = new DialogueNode
                            {
                                Key = key,
                                Type = data.Type,
                                Text = data.Text,
                                Name = data.Name,
                                Portrait = data.Portrait,
                                Next = null
                            };
                            break;
                        }
                    case "jump":
                        {
                            var data = entry.ToObject<SerializedDialogue>()!;
                            node = new JumpNode
                            {
                                Key = key,
                                Type = data.Type,
                                Text = data.Text,
                                Name = data.Name,
                                Portrait = data.Portrait,
                                NextId = data.Next,
                                Next = null
                            };
                            break;
                        }
                    case "choice":
                        {
                            node = CreateChoiceNode(entry, key);
                            break;
                        }
                    default:
                        throw new ArgumentException($"Unknown dialogue type: {type}");
                }

                node.Text = CleanText(node.Text);
                nodes[key][i] = node;
            }
        }

        // 2nd pass - Connect the connected components.
        foreach (var kv in nodes)
        {
            DialogueNode[] sequentialNodes = kv.Value;
            for (int i = 0; i < sequentialNodes.Length; i++)
            {
                DialogueNode currNode = sequentialNodes[i];
                switch (currNode.Type)
                {
                    case "regular":
                        if (i < sequentialNodes.Length - 1)
                        {
                            currNode.Next = sequentialNodes[i + 1];
                        }
                        break;
                    case "jump":
                        JumpNode jn = (JumpNode)currNode;
                        string key = jn.NextId;
                        currNode.Next = nodes[key][0];
                        break;
                    case "choice":
                        ChoiceNode cn = (ChoiceNode)currNode;
                        foreach (var choice in cn.ChoiceData)
                        {
                            switch (choice.Type)
                            {
                                case "regular":
                                    choice.Next = nodes[choice.NextId][0];
                                    break;
                                case "skill":
                                    SkillCheckData scd = (SkillCheckData)choice;
                                    scd.SuccessNext = nodes[scd.SuccessNextId][0];
                                    scd.FailNext = nodes[scd.FailNextId][0];
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        DialogueTree tree = new()
        {
            Root = nodes["root"][0]
        };
        return tree;
    }

    private static ChoiceNode CreateChoiceNode(JToken entry, String key)
    {
        var common = entry.ToObject<SerializedDialogue>()!;
        var jChoices = (JArray)entry["choices"]!;
        var choiceDataArray = new ChoiceData[jChoices.Count];

        for (int i = 0; i < jChoices.Count; i++)
        {
            var choiceToken = jChoices[i];
            string type = choiceToken["type"]!.Value<string>();

            ChoiceData data;
            switch (type)
            {
                case "regular":
                {
                  var sc = choiceToken.ToObject<SerializedChoice>();
                    data = new()
                    {
                        Type = sc.Type,
                        Text = sc.Text,
                        NextId = sc.Next,
                    };
                    break;
                }
                case "skill":
                {
                    var ssc = choiceToken.ToObject<SerializedSkillCheck>();
                    data = new SkillCheckData
                    {
                        Type = ssc.Type,
                        Text = ssc.Text,
                        SkillId = ssc.SkillId,
                        Difficulty = ssc.Difficulty,
                        SuccessNextId = ssc.Success,
                        FailNextId = ssc.Failure,
                    };
                    break;
                }
                case "exit":
                {
                    var sc = choiceToken.ToObject<SerializedChoice>();
                    data = new()
                    {
                        Type = sc.Type,
                        Text = sc.Text,
                    };
                    break;
                }
                default:
                    throw new JsonException($"Unsupported type {type}");
            }
            choiceDataArray[i] = data;
        }
        
        return new ChoiceNode {
            Key = key,
            Type = common.Type,
            Text = common.Text,
            Name = common.Name,
            Portrait = common.Portrait,
            ChoiceData = choiceDataArray
        };
    }

    private static string CleanText(string text)
    {
        return text
            .Replace("\n", " ")
            .Replace("\t", "")
            .Replace("\r", "");
    }
}
