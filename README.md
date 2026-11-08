# BerzerkSound

Says words in the robot voice of Stern's 1980 arcade game **Berzerk**. It runs your text through an
emulation of the game's **TSI S14001A** speech chip, using the game's own speech ROMs.

```console
BerzerkSound "Intruder alert! Intruder alert!"
```

**Windows only.** It needs the .NET 10 SDK to build and Windows to run.

## Quick start

1. **Get the speech ROMs.** They're copyrighted, so they aren't in this repo. You need MAME's
   `berzerk.zip` (or `frenzy.zip`) from a MAME ROM set. Put it in the [roms](roms) folder.
2. **Build.**

   ```console
   dotnet build
   ```

   The build copies the ROMs next to `BerzerkSound.exe`.
3. **Run.**

   ```console
   dotnet run --project src/BerzerkSound -- "Chicken! Fight like a robot!"
   ```

   Or run `src\BerzerkSound\bin\Debug\net10.0\BerzerkSound.exe` directly.

## Usage

```text
BerzerkSound [options] [--] <words>
BerzerkSound --list

  -f, --file <path>      Write the speech to a file instead of playing it: Ogg Vorbis if the name
                         ends in .ogg, otherwise WAV.
  -p, --pitch <0-7>      Speech clock setting; higher is faster and higher pitched (default 3).
      --volume <0-7>     Volume; 0 is silent, as on the real board (default 7).
  -r, --rom <path>       Directory or MAME zip holding the English speech ROMs.
  -l, --list             List the words Berzerk can say.
  -h, --help             Show this help.
  --                     Treat everything after this as words.
```

Examples:

```console
BerzerkSound "INTRUDER ALERT INTRUDER ALERT"
BerzerkSound "DESTROY THE CHICKEN"
BerzerkSound "THE HUMANOID MUST NOT ESCAPE"
BerzerkSound "Chicken! Fight like a robot!"
BerzerkSound "Coins detected in pocket"
```

Words can be separate arguments or one quoted string, so these are the same:

```console
BerzerkSound "destroy the chicken"
BerzerkSound destroy the chicken
```

Change the voice with pitch (0-7) and volume (0-7), as the game does for its robots:

```console
BerzerkSound -p 6 "Get the humanoid"
BerzerkSound -p 1 "Kill the intruder"
```

Save to a file instead of playing. The format follows the extension: `.ogg` writes Ogg Vorbis, anything
else writes WAV.

```console
BerzerkSound -f escape.wav "The humanoid must not escape"
BerzerkSound -f escape.ogg "The humanoid must not escape"
```

Case, punctuation and digits are ignored. Anything Berzerk can't say is reported and nothing is played:

```console
> BerzerkSound "Hello robot"
BerzerkSound: Berzerk cannot say the word 'Hello'. Run with --list to see the words it knows.
```

### Supported words

Berzerk's ROM holds only 29 words, so you can only say those. If any word in your text is unknown,
nothing is said and every unknown word is reported.

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

The same list is in `--help`, and `--list` also shows the word numbers.

### Pitch and volume

These are the two 3-bit fields of the byte the game writes to the speech board (port 44h,
`01vvvppp`). Pitch selects a clock divider of 16 − pitch. That sets the chip clock, which is also the
WAV sample rate, from 19.5 kHz (pitch 0) to 34.7 kHz (pitch 7). Volume 0 is silent.

The defaults, pitch 3 and volume 7, are the game's settings for "Intruder alert! Intruder alert!". The
game picks a random pitch for its robot taunts, so try a few values with `-p`.

### Exit codes

| Code | Meaning |
|-----:|---------|
| 0 | Success |
| 2 | Invalid arguments |
| 3 | Speech ROMs missing or invalid |
| 4 | Speech could not be played or written |
| 5 | A word is not in Berzerk's vocabulary |

## The speech ROMs

Only the English Berzerk/Frenzy ROMs work. The French, German and Spanish sets hold different
words and are rejected. The program uses the first of these it finds:

1. the path given with `--rom`: a zip file, or a directory holding the zip or the two loose files
   (`berzerk_r_vo_1c.1c` and `berzerk_r_vo_2c.2c`);
2. the folder `BerzerkSound.exe` is in;
3. the current folder.

Both chips are checked against MAME's CRC32 values (1C `2cfe825d`, 2C `d2b6324e`). Inside a zip they are
found by CRC, so renamed files work too.

## How it works

Berzerk's voice board has a single S14001A chip. The chip reads a 4 KB ROM that holds a word table, a
syllable table and the sound itself, stored as 2-bit delta-modulated samples. To say a word, the game
puts a 6-bit word number on the chip's inputs and pulses its START pin. The chip then walks the ROM and
produces one 4-bit sample per clock until the word ends.

This program does the same in software:

- [S14001AChip.cs](src/BerzerkSound/Synthesis/S14001AChip.cs) is a C# port of MAME's
  [`s14001a.cpp`](https://github.com/mamedev/mame/blob/master/src/devices/sound/s14001a.cpp), clock
  for clock.
- [BerzerkSpeechRenderer.cs](src/BerzerkSound/Speech/BerzerkSpeechRenderer.cs) feeds it the way the game
  does. The game checks the chip about 477 times a second and sends the next word only when the chip is
  no longer busy, so the short gaps between words match the arcade machine.
- The pitch and volume settings, and the clock dividers, come from MAME's
  [`berzerk.cpp`](https://github.com/mamedev/mame/blob/master/src/mame/stern/berzerk.cpp). The word
  numbers and the game's own speech routine come from the
  [Berzerk disassembly](https://github.com/ScottTunstall/Berzerk/blob/main/berzerk.asm).

The output is a straight port, but it hasn't been compared sample for sample against a MAME recording.

## Development

```console
dotnet build
dotnet test
```

The project targets .NET 10 only. Every compiler, .NET analyzer and Roslynator warning is a build
error, and the house rules are listed at the top of [.editorconfig](.editorconfig). The tests build
their own synthetic ROM data, so they don't need the real ROMs.

## Licensing

The Ogg Vorbis files are written with
[OggVorbisEncoder](https://github.com/SteveLillis/.NET-Ogg-Vorbis-Encoder) (MIT).

This code is based on MAME's Berzerk code
([`berzerk.cpp`](https://github.com/mamedev/mame/blob/master/src/mame/stern/berzerk.cpp)) and its S14001A
speech chip emulation
([`s14001a.cpp`](https://github.com/mamedev/mame/blob/master/src/devices/sound/s14001a.cpp)). Both are
BSD-3-Clause licensed.

Credits to the original authors:

- **Berzerk driver (`berzerk.cpp`):** Zsolt Vasvari, Aaron Giles, R. Belmont and Jonathan Gevaryahu.
  Original sound driver by Alex Judd.
- **S14001A emulation (`s14001a.cpp`):** Ed Bernard, Jonathan Gevaryahu (Lord Nightmare) and hap,
  with thanks to Kevin Horton.

See [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for the full license text.
