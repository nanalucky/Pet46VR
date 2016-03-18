//#define USE_WORD_MIN_SCORE

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle word detection
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Word : MonoBehaviour
{
	public SpectrumMicrophone Mic = null;
	public bool NormalizeWave = false;
	public bool RemoveSpectrumNoise = false;
	public float RecordWordToNoiseInterval = 0.2f;
	public float RecordScoreThreshhold = 250.0f; 
	public float DetectScoreThreshhold = 100.0f;
	public int DetectThreshold = 60;

	public List<WordDetails> Words = new List<WordDetails>();
	public List<string> WordsToIgnore = new List<string>();
	

	public const string WORD_NOISE = "Noise";
	public const string WORD_SITDOWN = "SitDown";
	public const string WORD_FALLDOWN = "FallDown";
	public const string WORD_STANDUP = "StandUp";
	public const string WORD_RIGHTRAWUP = "RightRawUp";
	public const string WORD_LEFTRAWUP = "LeftRawUp";
	public const string WORD_INTERACT = "Interact";

	[HideInInspector]
	public bool interact = false;

	/// <summary>
	/// Args for a detected word event
	/// </summary>
	public class WordEventArgs : EventArgs
	{
		/// <summary>
		/// The details about the detected word
		/// </summary>
		public WordDetails Details = null;
		public bool result = false;
	}

	protected virtual void Start()
	{
		if (null == Mic)
		{
			Debug.LogError("Missing meta references");
			return;
		}
		
		// prepopulate words
		Words.Add(new WordDetails() { Label = WORD_NOISE });
		Words.Add(new WordDetails() { Label = WORD_SITDOWN });
		Words.Add(new WordDetails() { Label = WORD_FALLDOWN });
		Words.Add(new WordDetails() { Label = WORD_RIGHTRAWUP });
		Words.Add(new WordDetails() { Label = WORD_LEFTRAWUP });
		Words.Add(new WordDetails() { Label = WORD_INTERACT });
		Words.Add(new WordDetails() { Label = WORD_STANDUP });
		GetComponent<WordDetect>().WordDetectEvent += WordDetectHandler;
		GetComponent<WordRecord> ().WordRecordEvent += WordRecordHandler;

		foreach (string device in Microphone.devices)
		{
			if (string.IsNullOrEmpty(device))
			{
				continue;
			}
			
			Mic.DeviceName = device;
			break;
		}

		ProfileLoad ();
	}


	public WordDetails GetWord(string label)
	{
		foreach (WordDetails details in Words)
		{
			if (null == details)
			{
				continue;
			}
			if (details.Label.Equals(label))
			{
				return details;
			}
		}
		
		return null;
	}

	public bool LoadProfiles(FileInfo fi)
	{
		try
		{
			using (FileStream fs = File.Open(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					LoadProfiles(br);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Failed to load profiles exception={0}", ex));
			return false;
		}
	}
	
	bool LoadProfilesPrefs(string key)
	{
		try
		{
			if (!PlayerPrefs.HasKey(key))
			{
				Debug.LogError("Player prefs missing key");
				return false;
			}
			
			Words.Clear();
			
			int wordCount = PlayerPrefs.GetInt(key);
			
			for (int wordIndex = 0; wordIndex < wordCount; ++wordIndex)
			{
				string wordKey = string.Format("{0}_{1}", key, wordIndex);
				if (PlayerPrefs.HasKey(wordKey))
				{
					string base64 = PlayerPrefs.GetString(wordKey);
					byte[] buffer = System.Convert.FromBase64String(base64);
					using (MemoryStream ms = new MemoryStream(buffer))
					{
						using (BinaryReader br = new BinaryReader(ms))
						{
							WordDetails details = new WordDetails();
							Words.Add(details);
							LoadWord(br, details);
							
							Debug.Log(string.Format("Key={0} size={1} label={2}", wordKey, base64.Length, details.Label));
						}
					}
				}
			}
			
			return true;
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Failed to load profiles exception={0}", ex));
			return false;
		}
	}
	
	void LoadProfiles(BinaryReader br)
	{
		Words.Clear();
		
		int wordCount = br.ReadInt32();
		for (int wordIndex = 0; wordIndex < wordCount; ++wordIndex)
		{
			WordDetails details = new WordDetails();
			Words.Add(details);
			
			LoadWord(br, details);
		}
	}
	
	void LoadWord(BinaryReader br, WordDetails details)
	{
		details.Label = br.ReadString();
		int size = br.ReadInt32();
		details.Wave = new float[size];
		for (int index = 0; index < size; ++index)
		{
			details.Wave[index] = br.ReadSingle();
		}
	}
	
	void SaveProfiles(FileInfo fi)
	{
		try
		{
			using (FileStream fs = File.Open(fi.FullName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(fs))
				{
					SaveProfiles(bw);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Failed to save profiles exception={0}", ex));
		}
	}
	
	void SaveProfilesPrefs(string key)
	{
		try
		{
			int count = 0;
			for (int wordIndex = 0; wordIndex < Words.Count; ++wordIndex)
			{
				WordDetails details = Words[wordIndex];
				if (null == details)
				{
					continue;
				}
				++count;
			}
			PlayerPrefs.SetInt(key, count);
			
			Debug.Log(string.Format("Saving profiles count={0}", count));
			
			int index = 0;
			for (int wordIndex = 0; wordIndex < Words.Count; ++wordIndex)
			{
				WordDetails details = Words[wordIndex];
				if (null == details)
				{
					continue;
				}
				
				string wordKey = string.Format("{0}_{1}", key, index);
				
				using (MemoryStream ms = new MemoryStream())
				{
					using (BinaryWriter bw = new BinaryWriter(ms))
					{
						SaveWord(bw, details);
						bw.Flush();
						
						ms.Position = 0;
						byte[] buffer = ms.GetBuffer();
						string base64 = System.Convert.ToBase64String(buffer);
						PlayerPrefs.SetString(wordKey, base64);
						
						Debug.Log(string.Format("Key={0} size={1} label={2}", wordKey, base64.Length, details.Label));
					}
				}
				
				++index;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("Failed to save profiles exception={0}", ex));
		}
	}
	
	void SaveProfiles(BinaryWriter bw)
	{
		int count = 0;
		for (int index = 0; index < Words.Count; ++index)
		{
			WordDetails details = Words[index];
			if (null == details)
			{
				continue;
			}
			++count;
		}
		bw.Write(count);
		
		for (int index = 0; index < Words.Count; ++index)
		{
			WordDetails details = Words[index];
			if (null == details)
			{
				continue;
			}
			SaveWord(bw, details);
		}
	}
	
	void SaveWord(BinaryWriter bw, WordDetails details)
	{
		bw.Write(details.Label);
		if (null == details.Wave)
		{
			bw.Write(0);
		}
		else
		{
			bw.Write(details.Wave.Length);
			foreach (float f in details.Wave)
			{
				bw.Write(f);
			}
		}
	}

	/// <summary>
	/// Refilter all the samples, removing noise
	/// </summary>
	void Refilter()
	{
		for (int index = 1; index < Words.Count; ++index)
		{
			WordDetails details = Words[index];
			Mic.RemoveSpectrumNoise(GetWord(WORD_NOISE).SpectrumReal, details.SpectrumReal);
		}
	}


	public void SetupWordProfile(WordDetails details, bool isNoise)
	{
		if (null == Mic ||
		    string.IsNullOrEmpty(Mic.DeviceName))
		{
			return;
		}
		
		int size = details.Wave.Length;
		int halfSize = size/2;
		
		//allocate profile spectrum, real
		if (null == details.SpectrumReal ||
		    details.SpectrumReal.Length != halfSize)
		{
			details.SpectrumReal = new float[halfSize];
		}
		
		//allocate profile spectrum, imaginary
		if (null == details.SpectrumImag ||
		    details.SpectrumImag.Length != halfSize)
		{
			details.SpectrumImag = new float[halfSize];
		}
		
		//get the spectrum for the trimmed word
		if (null != details.Wave &&
		    details.Wave.Length > 0)
		{
			Mic.GetSpectrumData(details.Wave, details.SpectrumReal, details.SpectrumImag, FFTWindow.Rectangular);
		}
		
		//filter noise
		if (RemoveSpectrumNoise)
		{
			if (isNoise)
			{
				Refilter();
			}
			else
			{
				Mic.RemoveSpectrumNoise(GetWord(WORD_NOISE).SpectrumReal, details.SpectrumReal);
			}
		}
	}

	private const string FILE_PROFILES = "VerbalCommand_Example4.profiles";

	public void ProfileLoad()
	{
		if (LoadProfilesPrefs(FILE_PROFILES))
		{
			for (int wordIndex = 0; wordIndex < Words.Count; ++wordIndex)
			{
				WordDetails details = Words[wordIndex];
				
				if (null != details.Wave &&
				    details.Wave.Length > 0)
				{
					if (null == details.Audio)
					{
						details.Audio = AudioClip.Create(string.Empty, details.Wave.Length, 1, Mic.SampleRate, false,
						                                 false);
					}
					details.Audio.SetData(details.Wave, 0);
				}
				
				SetupWordProfile(details, false);
			}
		}
	}
	
	
	public void ProfileSave()
	{
		SaveProfilesPrefs(FILE_PROFILES);
	}

	public void PlayProfile(string wordLabel)
	{
		WordDetails details = GetWord (wordLabel);
		if (null != details.Audio)
		{
			if (NormalizeWave)
			{
				GetComponent<AudioSource>().PlayOneShot(details.Audio, 0.1f);
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot(details.Audio);
			}
		}
	}
	
	public bool HasProfile(string wordLabel)
	{
		WordDetails details = GetWord (wordLabel);
		if (null != details && null != details.Audio)
			return true;
		
		return false;
	}

	
	public void EnableDetect(bool enable)
	{
		GetComponent<WordDetect> ().Enable (enable);
	}
	
	public bool IsEnableDetect()
	{
		return GetComponent<WordDetect>().EnableDetect;
	}

	public float Score(float[] spectrumReal1, float[] spectrumReal2)
	{
		int halfSize = spectrumReal1.Length;
		float score = 0;
		for (int index = 0; index < halfSize;)
		{
			float sumSpectrum = 0f;
			float sumProfile = 0f;
			int nextIndex = index + DetectThreshold;
			for (; index < nextIndex && index < halfSize; ++index)
			{
				sumSpectrum += Mathf.Abs(spectrumReal1[index]);
				sumProfile += Mathf.Abs(spectrumReal2[index]);
			}
			sumProfile = sumProfile/(float) DetectThreshold;
			sumSpectrum = sumSpectrum/(float) DetectThreshold;
			float val = Mathf.Abs(sumSpectrum - sumProfile);
			score += Mathf.Abs(val);
		}
		return score;
	}

	public void WordDetectHandler(object sender, WordEventArgs args)
	{
		if (string.IsNullOrEmpty(args.Details.Label))
		{
			return;
		}
		
		Debug.Log(string.Format("Detected: {0}", args.Details.Label));
		
		if(interact)
		{
			if (string.Compare(args.Details.Label, WORD_INTERACT) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().ToInteract();
			}
		}
		else
		{
			if (string.Compare(args.Details.Label, WORD_SITDOWN) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().SitDown();
			} else if (string.Compare(args.Details.Label, WORD_FALLDOWN) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().FallDown();
			}else if (string.Compare(args.Details.Label, WORD_STANDUP) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().StandUp();
			}else if (string.Compare(args.Details.Label, WORD_RIGHTRAWUP) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().RightRawUp();
			}else if (string.Compare(args.Details.Label, WORD_LEFTRAWUP) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().LeftRawUp();
			}else if (string.Compare(args.Details.Label, WORD_INTERACT) == 0) {
				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().ToInteract2();
			}
		}
	}

	public void WordRecordHandler(object sender, WordEventArgs args)
	{
		if(args.result == false)
		{
			//Debug.LogError(string.Format("Record {0} FALSE", args.Details.Label));
			GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().ShowQuestion();
		}
		else
		{
			//Debug.LogError(string.Format("Record {0} TRUE", args.Details.Label));
			WordDetectHandler(null, args);
		}
	}

	

}