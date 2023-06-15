using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace TicTacToePro
{
	public class LocalUser : MonoBehaviour
	{
		public string USER_DATA_NAME = "TTTSave";
		public static string PersistentDataPath;

		public LocalUserData SavedData;
		public SessionData SessionData;

		public static LocalUser shared;

		public bool Initialized = false;

		void Awake()
		{
			Initialized = false;

			PersistentDataPath = Application.persistentDataPath + "/SaveData/";

			Load();

			SessionData = new SessionData();
			SessionData.Init();

			if (shared == null)
				shared = this;

			Initialized = true;
		}

		public void Save()
		{
			Save(USER_DATA_NAME, SavedData);
		}

		public static void Save(string dataId, object obj)
		{
			string filePath = GetFilePathFromId(dataId);
			var data = SerializeData(obj);
			File.WriteAllText(filePath, data);
		}

		public static T Load<T>(string dataId)
		{
			var filePath = GetFilePathFromId(dataId);
			if (File.Exists(filePath) == false)
				return default(T);

			return LoadPath<T>(filePath);
		}

		private void Load()
		{
			SavedData = Load<LocalUserData>(USER_DATA_NAME);

			if (SavedData == null)
				SavedData = new LocalUserData();
			else
				SavedData.Load();

			if (SavedData.SettingsData.UserID == "")
			{
				SavedData.SettingsData.UserID = NameGenerator.GetUserName();
				Save();
			}
		}

		public static T LoadPath<T>(string filePath)
		{
			string data = File.ReadAllText(filePath);
			return DeserializeData<T>(data);
		}

		public static string GetFilePathFromId(string dataId)
		{
			return $"{PersistentDataPath}{dataId}.dat";
		}

		private static T DeserializeData<T>(string data)
		{
			if (string.IsNullOrEmpty(data))
				return default(T);

			// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
			if (typeof(T).IsPrimitive)
			{
				T res = (T)Convert.ChangeType(data, typeof(T));
				return res;
			}

			T result = JsonUtility.FromJson<T>(data);
			return result;
		}

		public static string SerializeData(object obj)
		{
			CheckDataPath();

			// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single
			string data = obj.GetType().IsPrimitive ? obj.ToString() : JsonUtility.ToJson(obj);
			return data;
		}

		private static void CheckDataPath()
		{
			if (Directory.Exists(PersistentDataPath) == false)
				Directory.CreateDirectory(PersistentDataPath);
		}
	}

	[Serializable]
	public class LocalUserData
	{
		public AnalyticsUserData AnalyticsData;
		public TutorialUserData TutorialData;
		public SettingsUserData SettingsData;
		public TTTUserData TTTData;
		public Game2048UserData Game2048Data;

		public LocalUserData()
		{
			AnalyticsData = new AnalyticsUserData();
			TutorialData = new TutorialUserData();
			SettingsData = new SettingsUserData();
			TTTData = new TTTUserData();
			Game2048Data = new Game2048UserData();
		}

		public void Load()
		{
			if (AnalyticsData == null)
				AnalyticsData = new AnalyticsUserData();
			if (TutorialData == null)
				TutorialData = new TutorialUserData();
			if (SettingsData == null)
				SettingsData = new SettingsUserData();
			if (TTTData == null)
				TTTData = new TTTUserData();
			if (Game2048Data == null)
				Game2048Data = new Game2048UserData();
		}
	}

	[Serializable]
	public class TTTUserData
	{
		public DifficultyLevel AIDifficulty = DifficultyLevel.Easy;
		public UserAIDifficultySettings UserAIDifficultyMod = new UserAIDifficultySettings();
		public List<int> Last20GamesPlayerResults = new List<int>();
		public int HintsAmount = 3;
		public int UndoAmount = 3;
	}

	[Serializable]
	public class Game2048UserData
	{
		public int MatchedBlocks = 0;
		public int MaxScore = 0;
	}

	[Serializable]
	public class SettingsUserData
	{
		public bool VibrationEnabled = true;
		public bool SFXEnabled = true;
		public bool MusicEnabled = true;
		public string Language = "en";
		[SerializeField] string _userID = "";


		public string UserID
		{
			get { return _userID; }
			set
			{
				_userID = value;
				OnUserIDChanged.Invoke();
			}
		}


		public UnityEvent OnUserIDChanged = new();
	}

	[Serializable]
	public class TutorialUserData
	{
		public bool TutorialComplete = false;
	}

	[Serializable]
	public class AnalyticsUserData
	{
		public bool AlreadyRated;
		public int TimesWon;
		public int TimesLost;
		public int TimesDraw;
		public int TimesShownRating;

		public int TimesPlayed => TimesWon + TimesLost + TimesDraw;
	}

	[Serializable]
	public class SessionData
	{
		public void Init()
		{
			StartTime = Time.realtimeSinceStartup;
		}

		public bool AlreadyRated;
		public int TimesWon;
		public int TimesLost;
		public int TimesDraw;
		public int TimesPlayed => TimesWon + TimesLost + TimesDraw;

		public float StartTime = 0f;
		public float PlayTime => Time.realtimeSinceStartup - StartTime;
	}
}