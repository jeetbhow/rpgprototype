using System.Collections.Generic;
using Godot;

public partial class Battle : Node2D
{
    [Signal] public delegate void ActionBarFullEventHandler(int id);

    [Export] public PackedScene PartyInfoPanelScene { get; set; }

    public Node2D EnemyNodes { get; private set; }
    public BattleUI UI { get; private set; }
    public BattleParticipant CurrActor { get; set; } = null;
    public List<BattleParticipant> Party { get; private set; } = [];
    public List<BattleParticipant> Enemies { get; private set; } = [];

    public void Init()
    {
        UI = GetNode<BattleUI>("BattleUI");
        EnemyNodes = GetNode<Node2D>("EnemyNodes");

        UI.Init();
        InitParty();
    }
    
    public void InitParty()
    {
        foreach (PartyMemberData partyMemberData in Game.Instance.Party)
        {
            Party.Add(new BattleParticipant(
                partyMemberData.Name,
                BattleParticipant.BattleParticipantType.PartyMember,
                partyMemberData.HP,
                partyMemberData.AP,
                partyMemberData.Strength,
                partyMemberData.Endurance,
                partyMemberData.Athletics
            ));

            PartyInfoPanel panel = PartyInfoPanelScene.Instantiate<PartyInfoPanel>();
            UI.PartyInfoHBox.AddChild(panel);

            panel.PartyMemberName = partyMemberData.Name;
            panel.HP = partyMemberData.HP;
            panel.MaxHP = partyMemberData.MaxHP;
            panel.AP = partyMemberData.AP;
            panel.MaxAP = partyMemberData.MaxAP;
        }
    }
}
