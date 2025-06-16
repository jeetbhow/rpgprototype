using Godot;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    public const string Path = "/root/SoundManager";

    [Export] public AudioStreamPlayer BgmPlayer { get; set; }
    [Export] public AudioStreamPlayer SfxPlayer { get; set; }

    public enum Sfx
    {
        Success,
        Fail,
        Confirm,
    }

    private readonly Dictionary<Sfx, AudioStream> _sfx = new()
    {
        { Sfx.Confirm, GD.Load<AudioStream>("res://Audio/SFX/confirm.wav") },
        { Sfx.Success, GD.Load<AudioStream>("res://Audio/SFX/success.wav") },
        { Sfx.Fail, GD.Load<AudioStream>("res://Audio/SFX/fail.wav") }
    };

    public void PlaySfx(Sfx sfxName)
    {
        SfxPlayer.Stream = _sfx[sfxName];
        SfxPlayer.Play();
    }

}
