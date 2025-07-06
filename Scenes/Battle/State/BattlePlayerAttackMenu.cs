using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class BattlePlayerAttackMenu : StateNode
{
    private const int BaseDmg = 2;

    private int _index = 0;
    private bool _isAttacking = false;
    private EnemyBattleSprite _target = null;

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
        int numChoices = Battle.UI.Commands.Choices.GetChildCount();
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
                    await Attack();
                    EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                }
                break;
            case InputEventKey k when k.IsActionPressed("Cancel"):
                _target.HideHP();
                EmitSignal(SignalName.StateUpdate, BattlePlayerTurn.Name);
                break;
        }

        if (prevIndex != _index && Battle.CurrFighter is not Player)
        {

        }
    }

    public async Task Attack()
    {
        _isAttacking = true;

        Enemy enemy = Battle.Enemies[_index];

        await Battle.UI.Log.AppendLine($"{Battle.CurrFighter.Name} attacks {enemy.Name}.");
        await Battle.Wait(500);

        SoundManager.Instance.PlaySfx(SoundManager.Sfx.Hurt, 8.0f);

        Battle.DamageEnemyHP(Battle.Enemies.IndexOf((Enemy)enemy), BaseDmg);
        Battle.UpdateAP(Battle.CurrFighter, GetAPCost(Battle.CurrFighter, enemy));

        await Battle.UI.Log.AppendLine($"{enemy.Name} takes {BaseDmg} damage.");
        await Battle.Wait(500);
        _target.HideHP();
        
        if (enemy.HP <= 0)
        {
            EnemyBattleSprite sprite = Battle.GetEnemySprite(_index);

            await sprite.Die();
            sprite.QueueFree();

            Battle.Enemies.RemoveAt(_index);
            GD.Print(Battle.TurnQueue.Peek().Name);
            Battle.TurnQueue = new Queue<Fighter>(Battle.TurnQueue.Where(fighter => fighter != enemy));
            Battle.UI.SetTurnQueue(Battle.TurnQueue);
            Battle.UI.TQPeek().Glow();

            await Battle.UI.Log.AppendLine($"{enemy.Name} has fallen!");
        }
    }

    public override async Task Enter()
    {
        _index = 0;
        _isAttacking = false;

        Battle.UI.Commands.Choices.Clear();

        foreach (var enemy in Battle.Enemies)
        {
            int apCost = GetAPCost(Battle.CurrFighter, enemy);
            ChoiceContent choice = (ChoiceContent)ChoiceContentScene.Instantiate();
            choice.Label.Text = enemy.Name + $" [color={Game.APColor.ToHtml()}](AP: {apCost})[/color]";
            choice.Enabled = Battle.CurrFighter.AP >= apCost;
            Battle.UI.Commands.Choices.AddChoice(choice);
        }

        Battle.UI.Commands.Choices.ShowArrow(_index);

        _target = Battle.GetEnemySprite(_index);
        _target.ShowHP();
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
