using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStickShooter
{
	static class Art
	{
		public static Texture2D Player { get; private set; }
		public static Texture2D Seeker { get; private set; }
		public static Texture2D Wanderer { get; private set; }
		public static Texture2D Bullet { get; private set; }
		public static Texture2D Pointer { get; private set; }
		public static Texture2D BlackHole { get; private set; }
		public static Texture2D LineParticle { get; private set; }
		public static Texture2D Glow { get; private set; }

		public static SpriteFont Font { get; private set; }

		public static void Load(ContentManager content)
		{
			Player = content.Load<Texture2D> ("Sprites/Player");
			Seeker = content.Load<Texture2D> ("Sprites/Seeker");
			Wanderer = content.Load<Texture2D> ("Sprites/Wanderer");
			Bullet = content.Load<Texture2D> ("Sprites/Bullet");
			Pointer = content.Load<Texture2D> ("Sprites/Pointer");
			BlackHole = content.Load<Texture2D> ("Sprites/Black Hole");
			LineParticle = content.Load<Texture2D> ("Sprites/Laser");
			Glow = content.Load<Texture2D> ("Sprites/Glow");

			Font = content.Load<SpriteFont> ("Font/Audiowide");
		}
	}
}
