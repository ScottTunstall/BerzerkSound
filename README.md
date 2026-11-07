# BerzerkSound

A Windows command-line program that says words in the robot voice of Stern's 1980 arcade game
**Berzerk**. It runs the words through an emulation of the game's **TSI S14001A** speech chip, using
the game's own speech ROMs.

```console
BerzerkSound "Intruder alert! Intruder alert!"
```

The chip emulation is a C# port of MAME's
[`s14001a.cpp`](https://github.com/mamedev/mame/blob/master/src/devices/sound/s14001a.cpp). The way
words are fed to the chip (clock dividers, volume, and the start-on-the-next-NMI timing between words)
follows MAME's [`berzerk.cpp`](https://github.com/mamedev/mame/blob/master/src/mame/stern/berzerk.cpp)
and the [Berzerk disassembly](https://github.com/ScottTunstall/Berzerk/blob/main/berzerk.asm).

## The speech ROMs

The speech data is copyrighted, so it isn't included. You need the English Berzerk speech ROMs from a
MAME ROM set: `berzerk.zip` (or `frenzy.zip`, which holds the same speech data). The program uses
the first of these it finds:

1. the path given with `--rom`: a zip file, or a directory holding the zip or the two loose files
   (`berzerk_r_vo_1c.1c` and `berzerk_r_vo_2c.2c`);
2. the directory the program is in;
3. the current directory.

Both chips are checked against MAME's CRC32 values (1C `2cfe825d`, 2C `d2b6324e`). Inside a zip they
are found by CRC, so renamed files work too. The French, German and Spanish ROMs hold other words and
are rejected.

For local builds, put `berzerk.zip` in the repository's `roms` folder and it is copied next to the
executable.

## Usage

```text
BerzerkSound [options] [--] <words>
BerzerkSound --list

  -f, --file <path>      Write the speech to a WAV file instead of playing it.
  -p, --pitch <0-7>      Speech clock setting; higher is faster and higher pitched (default 3).
      --volume <0-7>     Volume; 0 is silent, as on the real board (default 7).
  -r, --rom <path>       Directory or MAME zip holding the English speech ROMs.
  -l, --list             List the words Berzerk can say.
  -h, --help             Show this help.
  --                     Treat everything after this as words.
```

You can only use words that are in the ROM's vocabulary. Case, punctuation and digits are ignored.
If any word is unknown, nothing is said, and every unknown word is reported (exit code 5).

| No. | Word | No. | Word | No. | Word |
|----:|------|----:|------|----:|------|
| 00 | help¹ | 0A | the | 14 | escape |
| 01 | kill | 0B | in | 15 | destroy |
| 02 | attack | 0C | it | 16 | must |
| 03 | charge | 0D | there / their | 17 | not |
| 04 | got | 0E | where | 18 | chicken |
| 05 | shoot | 0F | humanoid | 19 | fight |
| 06 | get | 10 | coins | 1A | like |
| 07 | is | 11 | pocket | 1B | a |
| 08 | alert | 12 | intruder | 1C | robot |
| 09 | detected | 13 | no | | |

¹ In the ROM, but never used by the game.

### Pitch and volume

These are the two 3-bit fields of the byte the game writes to the speech board (port 44h,
`01vvvppp`). Pitch selects a clock divider of 16 − pitch. That sets the chip clock, which is also the
WAV sample rate, from 19.5 kHz (pitch 0) to 34.7 kHz (pitch 7). The defaults, pitch 3 and volume 7,
are the game's settings for "Intruder alert! Intruder alert!" (7Bh). The game's random robot taunts
use a random pitch.

### Exit codes

| Code | Meaning |
|-----:|---------|
| 0 | Success |
| 2 | Invalid arguments |
| 3 | Speech ROMs missing or invalid |
| 4 | Speech could not be played or written |
| 5 | A word is not in Berzerk's vocabulary |

## Building

You need the .NET 10 SDK. The project targets .NET 10 only.

```console
dotnet build
dotnet test
```

Every compiler, .NET analyzer and Roslynator warning is a build error. The house rules are listed at
the top of [.editorconfig](.editorconfig).

## Licensing

See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md). The ported MAME code is BSD-3-Clause.
