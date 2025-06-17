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

    private const string _sfxBasePath = "res://Assets/Audio/SFX/";
    private readonly Dictionary<Sfx, AudioStream> _sfx = new()
    {
        { Sfx.Confirm, GD.Load<AudioStream>(_sfxBasePath + "confirm.wav") },
        { Sfx.Success, GD.Load<AudioStream>(_sfxBasePath + "success.wav") },
        { Sfx.Fail, GD.Load<AudioStream>(_sfxBasePath + "fail.wav") }
    };

    public void PlaySfx(Sfx sfxName)
    {
        SfxPlayer.Stream = _sfx[sfxName];
        SfxPlayer.Play();
    }

}
