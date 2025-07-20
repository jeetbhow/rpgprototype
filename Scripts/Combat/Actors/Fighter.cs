using Godot;

using Signal;

namespace Combat.Actors;

public enum StatType
{
    HP,
    AP,
}


[GlobalClass]
public partial class Fighter : Resource
{
    public bool IsDead
    {
        get { return HP <= 0; }
    }

    [Export] public Texture2D Portrait { get; set; }
    [Export] public string Name { get; set; }
    [ExportGroup("General")]
    [Export] public int Level { get; set; }
    [Export] public int MaxHP { get; set; }
    [Export] public int MaxAP { get; set; }

    [ExportGroup("Body")]
    [Export] public Skill Strength { get; set; } = new Skill(SkillType.Strength, 0);
    [Export] public Skill Endurance { get; set; } = new Skill(SkillType.Endurance, 0);
    [Export] public Skill Athletics { get; set; } = new Skill(SkillType.Athletics, 0);

    [ExportGroup("Mind")]
    [Export] public Skill Rhetoric { get; set; } = new Skill(SkillType.Rhetoric, 0);
    [Export] public Skill Logic { get; set; } = new Skill(SkillType.Logic, 0);
    [Export] public Skill Knowledge { get; set; } = new Skill(SkillType.Knowledge, 0);

    [ExportGroup("Psyche")]
    [Export] public Skill Authority { get; set; } = new Skill(SkillType.Authority, 0);
    [Export] public Skill Empathy { get; set; } = new Skill(SkillType.Empathy, 0);
    [Export] public Skill Charisma { get; set; } = new Skill(SkillType.Charisma, 0);

    [ExportGroup("Sense")]
    [Export] public Skill Perception { get; set; } = new Skill(SkillType.Perception, 0);
    [Export] public Skill Reflexes { get; set; } = new Skill(SkillType.Reflexes, 0);
    [Export] public Skill Dexterity { get; set; } = new Skill(SkillType.Dexterity, 0);

    public int Initiative { get; set; }   // Set at runtime. 
    public int HP
    {
        get { return _hp; }
        set
        {
            _hp = Mathf.Max(0, value);
            SignalHub.Instance?.EmitSignal(SignalHub.SignalName.FighterStatChanged, this, (int)StatType.HP, _hp);
        }
    }
    public int AP
    {
        get { return _ap; }
        set
        {
            _ap = Mathf.Max(0, value);
            SignalHub.Instance?.EmitSignal(SignalHub.SignalName.FighterStatChanged, this, (int)StatType.AP, _ap);
        }
    }

    private int _ap, _hp;

    public void InitializeHPAndAP()
    {
        AP = MaxAP;
        HP = MaxHP;
    }

    public Skill GetSkillFromType(SkillType type)
    {
        return type switch
        {
            SkillType.Strength => Strength,
            SkillType.Endurance => Endurance,
            SkillType.Athletics => Athletics,
            SkillType.Rhetoric => Rhetoric,
            SkillType.Logic => Logic,
            SkillType.Knowledge => Knowledge,
            SkillType.Authority => Authority,
            SkillType.Empathy => Empathy,
            SkillType.Charisma => Charisma,
            SkillType.Reflexes => Reflexes,
            SkillType.Dexterity => Dexterity,
            _ => null
        };
    }
}
