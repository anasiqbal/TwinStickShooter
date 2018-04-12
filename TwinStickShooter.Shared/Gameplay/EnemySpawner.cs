using Microsoft.Xna.Framework;
using System;

namespace TwinStickShooter
{
	static class EnemySpawner
	{
		static Random rand = new Random ();
		static float inverseSpawnChance = 60;
		static float inverseBlackHoleChance = 600;

		static float spawnFrequency = 0.1f; // seconds
		static float timeSinceLastSpawn;

		public static void Update(GameTime gameTime)
		{
			// if the player is alive and there is less than 200 enemies
			// spawn enemies every 0.1 seconds (100 milliseconds)
			if(!PlayerShip.Instance.IsDead && EntityManager.Count < 200 && timeSinceLastSpawn > spawnFrequency)
			{
				if(rand.Next((int)inverseSpawnChance) == 0)
					EntityManager.Add (Enemy.CreateSeeker (GetSpawnPosition (), gameTime));

				if (rand.Next ((int) inverseSpawnChance) == 0)
					EntityManager.Add (Enemy.CreateWanderer (GetSpawnPosition (), gameTime));

				if (EntityManager.BlackHoleCount < 4 && rand.Next ((int) inverseBlackHoleChance) == 0)
					EntityManager.Add (new BlackHole (GetSpawnPosition ()));
			}

			if (inverseSpawnChance > 20)
				inverseSpawnChance -= 0.5f * (float) gameTime.ElapsedGameTime.TotalSeconds;

			timeSinceLastSpawn += (float) gameTime.ElapsedGameTime.TotalMilliseconds;
		}

		static Vector2 GetSpawnPosition()
		{
			// get spawn position atleast 250 units away from player
			Vector2 pos;
			do
			{
				pos = new Vector2 (rand.Next ((int) GameRoot.ScreenSize.X), rand.Next ((int) GameRoot.ScreenSize.Y));
			} while (Vector2.DistanceSquared (pos, PlayerShip.Instance.position) < 250 * 250);

			return pos;
		}

		public static void Reset()
		{
			inverseSpawnChance = 60;
		}

	}
}
