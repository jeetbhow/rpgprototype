using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Godot;


public static class TreeBuilder
{

    public static string ReadFile(string path)
    {
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            throw new ArgumentException($"Cannot open file: {path}");
        }

        string jsonStr = file.GetAsText();
        file.Close();

        return jsonStr;
    }

    public static DialogueTree CreateTree(string jsonPath)
    {
        JObject rootObj = JObject.Parse(ReadFile(jsonPath));
        Dictionary<string, DialogueNode[]> nodes = [];

        foreach (var prop in rootObj.Properties())
        {
            string dialogueKey = prop.Name;
            var dialogueTokens = (JArray)prop.Value;
            nodes[dialogueKey] = new DialogueNode[dialogueTokens.Count];

            for (int i = 0; i < dialogueTokens.Count; i++)
            {
                JToken token = dialogueTokens[i];
                string type = token["type"]?.Value<string>()
                              ?? throw new JsonException($"Missing type in entry {i} of '{dialogueKey}'");

                DialogueNode node;
                switch (type)
                {
                    case "regular":
                        {
                            var dialog = token.ToObject<SerializedDialogue>()!;
                            node = new DialogueNode
                            {
                                Key = dialogueKey,
                                Type = dialog.Type,
                                Text = dialog.Text,
                                Name = dialog.Name,
                                Portrait = dialog.Portrait,
                                Next = null
                            };
                            break;
                        }
                    case "jump":
                        {
                            var dialogue = token.ToObject<SerializedDialogue>()!;
                            node = new JumpNode
                            {
                                Key = dialogueKey,
                                Type = dialogue.Type,
                                Text = dialogue.Text,
                                Name = dialogue.Name,
                                Portrait = dialogue.Portrait,
                                NextId = dialogue.Next,
                                Next = null
                            };
                            break;
                        }
                    case "choice":
                        {
                            node = CreateChoiceNode(token, dialogueKey);
                            break;
                        }
                    default:
                        throw new ArgumentException($"Unknown dialogue type: {type}");
                }
                node.Text = CleanText(node.Text);
                nodes[dialogueKey][i] = node;
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
                        foreach (var choice in cn.Choices)
                        {
                            switch (choice.Type)
                            {
                                case "regular":
                                    choice.Next = nodes[choice.NextId][0];
                                    break;
                                case "skill":
                                    SkillCheck scd = (SkillCheck)choice;
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

    private static ChoiceNode CreateChoiceNode(JToken token, String dialogueKey)
    {
        var dialogue = token.ToObject<SerializedDialogue>()!;
        var choices = (JArray)token["choices"]!;
        var choiceDataArray = new Choice[choices.Count];

        for (int i = 0; i < choices.Count; i++)
        {
            var choiceToken = choices[i];
            string type = choiceToken["type"]!.Value<string>();

            Choice data;
            switch (type)
            {
                case "regular":
                {
                  var serChoice = choiceToken.ToObject<SerializedChoice>();
                    data = new()
                    {
                        Type = serChoice.Type,
                        Text = serChoice.Text,
                        NextId = serChoice.Next,
                    };
                    break;
                }
                case "skill":
                {
                    var serSkillCheck = choiceToken.ToObject<SerializedSkillCheck>();
                    Skill skill = new(Skill.TypeFromString(serSkillCheck.SkillName), serSkillCheck.Difficulty);
                    data = new SkillCheck
                    {
                        Type = serSkillCheck.Type,
                        Text = serSkillCheck.Text,
                        Skill = skill,
                        SuccessNextId = serSkillCheck.Success,
                        FailNextId = serSkillCheck.Failure,
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
            Key = dialogueKey,
            Type = dialogue.Type,
            Text = dialogue.Text,
            Name = dialogue.Name,
            Portrait = dialogue.Portrait,
            Choices = choiceDataArray
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
