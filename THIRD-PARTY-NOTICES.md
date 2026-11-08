# Third-party notices

## MAME S14001A emulation and Berzerk sound hardware

The files in `src/BerzerkSound/Synthesis` port `src/devices/sound/s14001a.cpp` and `s14001a.h` to C#.
`src/BerzerkSound/Speech/BerzerkTiming.cs` and `src/BerzerkSound/Roms/KnownRomSets.cs` take their
values from `src/mame/stern/berzerk.cpp`. Both source files come from MAME
(https://github.com/mamedev/mame) and are distributed under the BSD 3-Clause license. Each ported file
keeps its original `license` and `copyright-holders` header.

- `s14001a.cpp`, `s14001a.h`: copyright Ed Bernard, Jonathan Gevaryahu, hap (thanks to Kevin Horton)
- `berzerk.cpp`: copyright Zsolt Vasvari, Aaron Giles, R. Belmont, Jonathan Gevaryahu

```
Copyright (c) the copyright holders listed above
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of
   conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of
   conditions and the following disclaimer in the documentation and/or other materials provided
   with the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to
   endorse or promote products derived from this software without specific prior written
   permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
```

## Berzerk vocabulary and speech timing

The word numbers, the default pitch and volume, and the next-NMI word sequencing come from the Berzerk
Z80 disassembly at https://github.com/ScottTunstall/Berzerk/blob/main/berzerk.asm. Word 00h ("help")
was identified from the ROM data by the *TSI S14001A Speech Synthesizer LSI Integrated Circuit Guide*.

## Berzerk speech ROMs

The speech ROMs (`berzerk_r_vo_1c.1c`, `berzerk_r_vo_2c.2c`) are **not** included. Supply your own
dumps, as you would for MAME.

## OggVorbisEncoder

Ogg Vorbis output uses the [OggVorbisEncoder](https://github.com/SteveLillis/.NET-Ogg-Vorbis-Encoder) NuGet
package (a managed port of the Xiph.Org libvorbis encoder), distributed under the MIT license. The tests
decode the files with [NVorbis](https://github.com/NVorbis/NVorbis), also MIT.
