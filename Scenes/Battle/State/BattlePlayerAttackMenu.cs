using Godot;
using System.Threading.Tasks;

public partial class BattlePlayerAttackMenu : StateNode
{
    private const int BaseDmg = 2;
    
    private int _index = 0;
    private bool _isAttacking = false;

    [Export] public Battle Battle { get; set; }
    [Export] public StateNode BattlePlayerTurn { get; set; }

    public override async void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent || !keyEvent.IsPressed() || _isAttacking)
        {
            return;
        }

        int numChoices = Battle.UI.CommandTextbox.Choices.GetChildCount();
        switch (keyEvent)
        {
            case InputEventKey k when k.IsActionPressed("MoveDown"):
                _index = (_index + 1) % numChoices;
                break;
            case InputEventKey k when k.IsActionPressed("MoveUp"):
                _index = (_index - 1 + numChoices) % numChoices;
                break;
            case InputEventKey k when k.IsActionPressed("Accept"):
                if (!_isAttacking)
                {
                    await Attack(Battle.Enemies[_index]);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                Battle.UnhighlightEnemy(_index);
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
        }

        if (Battle.CurrFighter.Type == Fighter.FighterType.Player)
        {
        
        }
    }

    public async Task Attack(Fighter enemy)
    {
        _isAttacking = true;

        Battle.UnhighlightEnemy(_index);
        await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} attacks {enemy.Name}.");
        await Task.Delay(500);

        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);

        Battle.DamageEnemy(Battle.Enemies.IndexOf(enemy), BaseDmg);
        Battle.UpdateAP(Battle.CurrFighter, GetAPCost(Battle.CurrFighter, enemy));
        await Task.Delay(500);

        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {BaseDmg} damage.");
        EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
    }

    public override async Task Enter()
    {
        _index = 0;
        _isAttacking = false;

        Battle.HighlightEnemy(_index);
        Battle.UI.CommandTextbox.Choices.Clear();
        foreach (var enemy in Battle.Enemies)
        {
            // Compute the AP cost to attack the enemy and append it after the enemy's name.
            int apCost = GetAPCost(Battle.CurrFighter, enemy);
            Battle.UI.CommandTextbox.Choices.AddChoice(enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]");
        }

        Battle.UI.CommandTextbox.Choices.ShowArrow(_index);
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
