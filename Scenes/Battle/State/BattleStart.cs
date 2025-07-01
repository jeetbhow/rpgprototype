using System.Threading.Tasks;
using Godot;

public partial class BattleStart : StateNode
{
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattleStandby { get; set; }

    public override async Task Enter()
    {
        Battle.Init();
        await InitEnemies();
        EmitSignal(SignalName.StateUpdate, BattleStandby.Name);
    }

    public override Task Exit()
    {
        return Task.CompletedTask;
    }

    public async Task InitEnemies()
    {
        foreach (var enemy in Battle.EnemyNodes.GetChildren())
        {
            if (enemy is Enemy e)
            {
                GD.Print($"Adding enemy: {e.Data.Name}");
                Battle.Enemies.Add(new BattleParticipant(
                     e.Data.Name,
                     BattleParticipant.BattleParticipantType.Enemy,
                     e.Data.HP,
                     e.Data.AP,
                     e.Data.Strength,
                     e.Data.Endurance,
                     e.Data.Athletics
                 ));

                await e.FadeIn();

                string introduction = e.Data.Introduction;
                await Battle.UI.LogTextbox.ProcessAndWriteText(introduction);
            }
        }
    }
}
