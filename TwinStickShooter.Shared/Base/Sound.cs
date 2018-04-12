using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace TwinStickShooter
{
	static class Sound
	{
		static readonly Random rand = new Random ();

		public static Song Music { get; private set; }

		static SoundEffect [] explosions;
		public static SoundEffect Explosion { get { return explosions [rand.Next (explosions.Length)]; } }

		static SoundEffect [] shots;
		public static SoundEffect Shot { get { return shots [rand.Next (shots.Length)]; } }

		static SoundEffect [] spawns;
		public static SoundEffect Spawn { get { return spawns [rand.Next (spawns.Length)]; } }

		public static void Load(ContentManager content)
		{
			// load the music file
			Music = content.Load<Song> ("Audio/Music");

			// load sound file for the respective sound types
			// linq method alternate of a loop. 
			explosions = Enumerable.Range (1, 8).Select (x => content.Load<SoundEffect> ("Audio/explosion-0" + x)).ToArray ();
			shots = Enumerable.Range (1, 4).Select (x => content.Load<SoundEffect> ("Audio/shoot-0" + x)).ToArray ();
			spawns = Enumerable.Range (1, 8).Select (x => content.Load<SoundEffect> ("Audio/spawn-0" + x)).ToArray ();
		}
	}
}
