using Microsoft.Xna.Framework;
using System.IO;

namespace TwinStickShooter
{
	static class PlayerStatus
	{
		private const float multiplierExpiryTime = 1f;
		private const int maxMultiplier = 20;
		private const string highScoreFileName = "highscore.txt";

		public static int Lives { get; private set; }
		public static int Score { get; private set; }
		public static int Multiplier { get; private set; }
		public static int HighScore { get; private set; }

		public static bool IsGameOver { get { return Lives == 0; } }

		static float multiplierTimeLeft;		// time until the current multiplier expires
		static int scoreForExtraLife;			// score required to gain an extra life



		static PlayerStatus()
		{
			HighScore = LoadHighScore ();
			Reset ();
		}

		// reset to default values
		public static void Reset()
		{
			Score = 0;
			Multiplier = 1;
			Lives = 4;
			scoreForExtraLife = 2000;
			multiplierTimeLeft = 0;
		}

		public static void Update(GameTime gameTime)
		{
			// if multiplier is greater than one than decrease the multiplier expiry time every frame
			// and if it reaches zero then reset the multiplier
			if(Multiplier > 1)
			{
				if((multiplierTimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds) <= 0)
				{
					multiplierTimeLeft = multiplierExpiryTime;
					ResetMultiplier ();
				}
			}
		}
		
		/// <summary>
		/// Call this if a gameover scenario is met.
		/// Saves persistent data
		/// </summary>
		public static void EndSession()
		{
			if (Score > HighScore)
				SaveHighScore (HighScore = Score);
		}

		/// <summary>
		/// Resets the multiplier value to 1
		/// </summary>
		public static void ResetMultiplier()
		{
			Multiplier = 1;
		}

		/// <summary>
		/// Reduces life by 1
		/// </summary>
		public static void RemoveLife()
		{
			Lives--;
		}

		/// <summary>
		/// Add base point to the total score.
		/// </summary>
		/// <param name="basePoints"></param>
		public static void AddPoints(int basePoints)
		{
			if (PlayerShip.Instance.IsDead)
				return;

			// mulitply base points with the multiplier value
			Score += basePoints * Multiplier;

			// if the user has scored enough points reward an extra life
			while (Score >= scoreForExtraLife)
			{
				scoreForExtraLife += 2000;
				Lives++;
			}
		}

		/// <summary>
		/// Increase the multiplier by 1 and reset the multiplier expiry time
		/// </summary>
		public static void IncreaseMultiplier()
		{
			if (PlayerShip.Instance.IsDead)
				return;

			// reset the multiplier expiry time
			multiplierTimeLeft = multiplierExpiryTime;

			// multiplier cannot be more than a limit
			if (Multiplier < maxMultiplier)
				Multiplier++;
		}

		/// <summary>
		/// returns the current saved highscore
		/// </summary>
		/// <returns></returns>
		static int LoadHighScore()
		{
			int score = 0;
			if(File.Exists(highScoreFileName))
			{
				int.TryParse (File.ReadAllText (highScoreFileName), out score);
			}

			return score;
		}

		/// <summary>
		/// Save the new highscore to a file
		/// </summary>
		/// <param name="score"></param>
		static void SaveHighScore(int score)
		{
			File.WriteAllText (highScoreFileName, score.ToString ());
		}
	}
}
