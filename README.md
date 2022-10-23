# SatiRogue

## A turn-based dungeon-crawler roguelike RPG, made with Godot 3.x C# + RelEcs

*I've heard it said that you've 7 only rebirths remaining, from 
[entering the stream](https://en.wikipedia.org/wiki/Sot%C4%81panna), to the 
[final extinguishment](https://en.wikipedia.org/wiki/Arhat).*

*My conclusive 7 lifetimes, and with them the conclusion of my hard-earned equanimity, are threatened by the 
ever-looming danger of the hindrances: murky mental poisons that lurk in the cerebral depths of this self-proliferating prison,
this dungeon I now find myself in, in this turn-based, roguelike, dungeon-crawler RPG inspired by the
[Early Buddhist Texts](https://en.wikipedia.org/wiki/Early_Buddhist_texts).*

---

## Open source code - but no assets!

The C# source code and Godot resource structure of this project is open-source and MIT licensed - but the art, sound, 
fonts and other resources have been licensed from artists separately and can't be redistributed except in the 
published binary. You won't be able to build and run this project in Godot without replacing those assets, as they're
stored on a private LFS server (I'm considering making a script to automatically replace all textures with Godot's 
`icon.png`, but I'm concerned that it'll look *too good*).

Still, I hope the code is useful to someone, it demonstrates up-to-date "good enough" (I daren't say "best") practices 
in Godot C#, targets `netstandard2.1`, uses language version 10, makes thorough use of nullable support, and integrates
various open source libaries such as:

* [RelEcs](https://github.com/Byteron/RelEcs), a pure C# ECS library with extra Godot goodies, used extensively 
in this project,
* [GodotOnReady](https://github.com/31/GodotOnReady), source-generating annotations for doing and fetching things
more concisely when Nodes become ready.
* [go_dot_log](https://github.com/chickensoft-games/go_dot_log), to implement log severity levels and buffer log messages.
* [activelogic-cs](https://github.com/active-logic/activelogic-cs), a minimal, no-DSL behaviour tree library.

Along with many other generously open-source addons, snippets and resources from the Godot and C# communities, the
licenses  to which you can find in the codebase where appropriate.

## Confused? I can explain...

I'm writing a series of blog posts to explain some of the techniques and patterns used in this game. 
You can find the first part here:

https://lost-terminal.co.uk/satirogue-ecs-experiments-in-godot-3-x-c-with-relecs/
