using Godot;
using System.Threading.Tasks;

public partial class BattlePlayerAttackMenu : StateNode
{
    private const int BaseDmg = 2;

    private int _index = 0;
    private bool _isAttacking = false;

    [Export] public PackedScene ChoiceContentScene { get; set; }
    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerTurn { get; set; }

    public override async void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.IsPressed() || _isAttacking)
        {
            return;
        }

        int prevIndex = _index;
        int numChoices = Battle.UI.CommandTextbox.Choices.GetChildCount();
        int prevApCost = GetAPCost(Battle.CurrFighter, Battle.Enemies[prevIndex]);
        int apCost = GetAPCost(Battle.CurrFighter, Battle.Enemies[_index]);
        int pnlIndex = Battle.Party.IndexOf((Ally)Battle.CurrFighter);
        PartyInfoPanel panel = Battle.UI.GetPartyInfoPanel(pnlIndex);

        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                _index = (_index + 1) % numChoices;
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                _index = (_index - 1 + numChoices) % numChoices;
                break;
            case InputEventKey k when k.IsActionPressed("Accept"):
                if (!_isAttacking && Battle.CurrFighter.AP >= apCost)
                {
                    Battle.UnhighlightEnemy(_index);
                    await Attack(Battle.Enemies[_index]);
                    EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                Battle.UnhighlightEnemy(_index);
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
        }

        if (prevIndex != _index && Battle.CurrFighter is not Player)
        {

        }
    }

    public async Task Attack(Fighter enemy)
    {
        _isAttacking = true;

        await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} attacks {enemy.Name}.");
        await Task.Delay(500);

        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);

        Battle.DamageEnemyHP(Battle.Enemies.IndexOf((Enemy)enemy), BaseDmg);
        Battle.UpdateAP(Battle.CurrFighter, GetAPCost(Battle.CurrFighter, enemy));
        await Task.Delay(500);

        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {BaseDmg} damage.");
    }

    public override async Task Enter()
    {
        _index = 0;
        _isAttacking = false;

        Battle.HighlightEnemy(_index);
        Battle.UI.CommandTextbox.Choices.Clear();

        foreach (var enemy in Battle.Enemies)
        {
            int apCost = GetAPCost(Battle.CurrFighter, enemy);
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]";
            choice.Enabled = Battle.CurrFighter.AP >= apCost;
            Battle.UI.CommandTextbox.Choices.AddChoice(choice);
        }
                
        Battle.UI.CommandTextbox.Choices.ShowArrow(_index);

        int index = Battle.Party.IndexOf((Ally)Battle.CurrFighter);
        PartyInfoPanel panel = Battle.UI.GetPartyInfoPanel(index);
        panel.AnimationPlayer.Play("Blink");
    }
    
    
    /// <summary>
    /// Get the AP cost for an attack by an attacker against a defender.
    /// </summary>
    /// <param name="attacker">The fighter that's attacking.</param>
    /// <param name="defender">The fighter that's defending.</param>
    /// <returns>The amount of AP used for the attack.</returns>
    public int GetAPCost(Fighter attacker, Fighter defender)
    {
        // For now, I'll use a fixed AP cost for attacks.
        // In the future, this could depend on the attack type, weapon, enemy, etc
        int baseCost = 2;
        return baseCost;
    }
}
