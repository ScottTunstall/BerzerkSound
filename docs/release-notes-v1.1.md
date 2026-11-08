## BerzerkSound 1.1

Says words in the robot voice of Stern's 1980 arcade game Berzerk, through an emulation of its S14001A speech
chip and the game's own speech ROMs.

```console
BerzerkSound "INTRUDER ALERT INTRUDER ALERT"
BerzerkSound "DESTROY THE CHICKEN"
BerzerkSound "THE HUMANOID MUST NOT ESCAPE"
```

### What's in this release

- Speaks any of the 29 words in Berzerk's vocabulary, with the same gaps between words as the arcade game.
- `-p` sets the pitch (0-7) and `--volume` the volume (0-7), as the game's own speech routine does.
- **Save as Ogg Vorbis or WAV.** `-f name.ogg` writes Ogg Vorbis. Any other name writes WAV, and a name with
  no extension gets `.wav` added, so `-f escape` writes `escape.wav`.
- `--help` lists every word Berzerk can say, and `--list` shows them with their word numbers.
- Words you give that Berzerk can't say are all reported together, and nothing is played.

### Download and run

Download `BerzerkSound-1.1-win-x64.zip`, unzip it, and run `BerzerkSound.exe` from a Command Prompt. It needs
the **.NET 10 Runtime** (x64) for Windows, from https://dotnet.microsoft.com/download/dotnet/10.0, and Windows
to play sound.

### You need the speech ROMs

The speech ROMs are copyrighted firmware and are **not** included. Put MAME's `berzerk.zip` (or `frenzy.zip`)
next to `BerzerkSound.exe`, or point to it with `--rom`. Only the English ROMs work. See the
[README](https://github.com/ScottTunstall/BerzerkSound#the-speech-roms).
