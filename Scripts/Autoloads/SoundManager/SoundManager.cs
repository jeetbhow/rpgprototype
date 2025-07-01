using Godot;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    public const string Path = "/root/SoundManager";

    [Export] public AudioStreamPlayer BgmPlayer { get; set; }
    [Export] public AudioStreamPlayer SfxPlayer { get; set; }

    public static SoundManager Instance { get; private set; }

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("SoundManager instance already exists. This should not happen.");
            return;
        }

        // Ensure the audio players are set.
        if (BgmPlayer == null || SfxPlayer == null)
        {
            GD.PrintErr("BgmPlayer or SfxPlayer is not set in SoundManager.");
        }

        Instance = this;
    }

    public enum Sfx
    {
        Success,
        Fail,
        Confirm,
        Poof,
    }

    private const string _sfxBasePath = "res://Assets/Audio/SFX/";
    private readonly Dictionary<Sfx, AudioStream> _sfx = new()
    {
        { Sfx.Confirm, GD.Load<AudioStream>(_sfxBasePath + "confirm.wav") },
        { Sfx.Success, GD.Load<AudioStream>(_sfxBasePath + "success.wav") },
        { Sfx.Fail, GD.Load<AudioStream>(_sfxBasePath + "fail.wav") },
        { Sfx.Poof, GD.Load<AudioStream>(_sfxBasePath + "poof.wav") }
    };

    public void PlaySfx(Sfx sfxName)
    {
        SfxPlayer.Stream = _sfx[sfxName];
        SfxPlayer.Play();
    }

}
