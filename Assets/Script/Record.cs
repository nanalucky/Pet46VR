using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example of detecting audio words
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Record : MonoBehaviour
{
	/// <summary>
	/// Reference to the spectrum microphone
	/// </summary>
	public SpectrumMicrophone Mic = null;
	
	/// <summary>
	/// Reference to word detection
	/// </summary>
	public WordDetection AudioWordDetection = null;
	
	/// <summary>
	/// Flag to normalize wave samples
	/// </summary>
	public bool NormalizeWave = false;
	
	/// <summary>
	/// Flag to remove spectrum noise
	/// </summary>
	public bool RemoveSpectrumNoise = false;
	
	/// <summary>
	/// Indicates a profile is being recorded
	/// </summary>
	private string m_RecordingProfile = string.Empty;
	
	/// <summary>
	/// Start position for recording
	/// </summary>
	protected int m_startPosition = 0;
	
	/// <summary>
	/// Hold to talk timer
	/// </summary>
	protected DateTime m_timerStart = DateTime.MinValue;

	protected DateTime m_timerStartNoise = DateTime.MinValue;
	
	public const string WORD_NOISE = "Noise";
	public const string WORD_SITDOWN = "SitDown";
	public const string WORD_FALLDOWN = "FallDown";
	public const string WORD_STANDUP = "StandUp";
	public const string WORD_RIGHTRAWUP = "RightRawUp";
	public const string WORD_LEFTRAWUP = "LeftRawUp";
	public const string WORD_INTERACT = "Interact";


	public Button btnNoiseRecord;
	public Button btnSitDownRecord;
	public Button btnFallDownRecord;
	public Button btnStandUpRecord;
	public Button btnRightRawUpRecord;
	public Button btnLeftRawUpRecord;
	public Button btnInteractRecord;

	[HideInInspector]public bool interact = false;
	private bool firstFrame = true;

	protected virtual WordDetails GetWord(string label)
	{
		foreach (WordDetails details in AudioWordDetection.Words)
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
	
	/// <summary>
	/// Initialize the example
	/// </summary>
	protected virtual void Start()
	{
		if (null == AudioWordDetection ||
		    null == Mic)
		{
			Debug.LogError("Missing meta references");
			return;
		}
		
		// prepopulate words
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_NOISE });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_SITDOWN });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_FALLDOWN });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_STANDUP });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_RIGHTRAWUP });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_LEFTRAWUP });
		AudioWordDetection.Words.Add(new WordDetails() { Label = WORD_INTERACT });
		//subscribe detection event
		AudioWordDetection.WordDetectedEvent += WordDetectedHandler;

		foreach (string device in Microphone.devices)
		{
			if (string.IsNullOrEmpty(device))
			{
				continue;
			}
			
			Mic.DeviceName = device;
			break;
		}
	}
	
	/// <summary>
	/// Handle word detected event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	void WordDetectedHandler(object sender, WordDetection.WordEventArgs args)
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
	
	/// <summary>
	/// Refilter all the samples, removing noise
	/// </summary>
	protected void Refilter()
	{
		if (null == AudioWordDetection)
		{
			return;
		}
		for (int index = 1; index < AudioWordDetection.Words.Count; ++index)
		{
			WordDetails details = AudioWordDetection.Words[index];
			Mic.RemoveSpectrumNoise(GetWord(WORD_NOISE).SpectrumReal, details.SpectrumReal);
		}
	}
	
	protected virtual void SetupWordProfile(bool playAudio)
	{
		SetupWordProfile(playAudio, string.Compare(m_RecordingProfile, WORD_NOISE) == 0, m_RecordingProfile);
	}
	
	/// <summary>
	/// Setup the word profile
	/// </summary>
	protected virtual void SetupWordProfile(bool playAudio, bool isNoise, string wordLabel)
	{
		if (null == AudioWordDetection ||
		    null == Mic ||
		    string.IsNullOrEmpty(Mic.DeviceName))
		{
			return;
		}
		

		WordDetails details = GetWord(wordLabel);
		if (details == null)
			return;
		
		float[] wave = Mic.GetLastData();
		if (null != wave)
		{
			//allocate for the wave copy
			int size = wave.Length;
			if (null == details.Wave ||
			    details.Wave.Length != size)
			{
				details.Wave = new float[size];
				if (null != details.Audio)
				{
					UnityEngine.Object.DestroyImmediate(details.Audio, true);
					details.Audio = null;
				}
			}
			
			//trim the wave
			int position = Mic.GetPosition();
			
			//get the trim size
			int trim = 0;
			if (m_startPosition < position)
			{
				trim = position - m_startPosition;
			}
			else
			{
				trim = size - m_startPosition + position;
			}
			
			//zero the existing wave
			for (int index = 0; index < size; ++index)
			{
				details.Wave[index] = 0f;
			}
			
			//shift array
			for (int index = 0, i = m_startPosition; index < trim; ++index, i = (i + 1) % size)
			{
				details.Wave[index] = wave[i];
			}
			
			//clear existing mic data
			for (int index = 0; index < size; ++index)
			{
				wave[index] = 0;
			}
			
			if (NormalizeWave &&
			    !isNoise)
			{
				//normalize the array
				Mic.NormalizeWave(details.Wave);
			}
			
			SetupWordProfile(details, isNoise);
			
			//play the audio
			if (null == details.Audio)
			{
				details.Audio = AudioClip.Create(string.Empty, size, 1, Mic.SampleRate, false, false);
			}
			details.Audio.SetData(details.Wave, 0);
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().mute = false;
			if (playAudio)
			{
				EnableDetectWords(true);
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
	}
	
	protected virtual void SetupWordProfile(WordDetails details, bool isNoise)
	{
		if (null == AudioWordDetection ||
		    null == Mic ||
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
		if (AudioWordDetection.LoadProfilesPrefs(FILE_PROFILES))
		{
			for (int wordIndex = 0; wordIndex < AudioWordDetection.Words.Count; ++wordIndex)
			{
				WordDetails details = AudioWordDetection.Words[wordIndex];
				
				if (null != details.Wave &&
				    details.Wave.Length > 0)
				{
					if (null == details.Audio)
					{
						details.Audio = AudioClip.Create(string.Empty, details.Wave.Length, 1, Mic.SampleRate, false,
						                                 false);
					}
					details.Audio.SetData(details.Wave, 0);
					GetComponent<AudioSource>().loop = false;
					GetComponent<AudioSource>().mute = false;
				}
				
				SetupWordProfile(details, false);
			}
		}
	}

	
	public void ProfileSave()
	{
		AudioWordDetection.SaveProfilesPrefs(FILE_PROFILES);
	}	


	private void ButtonSetProfile(Button btn, string wordLabel)
	{
		if (btn == null)
			return;
		if (!btn.IsActive())
			return;

		RectTransform rectTransform = (btn.transform) as RectTransform;
		bool overButton = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null);
		if (string.IsNullOrEmpty(m_RecordingProfile) &&
		    m_timerStart == DateTime.MinValue &&
		    Input.GetMouseButton(0) &&
		    overButton)
		{
			//Debug.Log("Initial button down");
			m_RecordingProfile = wordLabel;
			m_startPosition = Mic.GetPosition();
			m_timerStart = DateTime.Now + TimeSpan.FromSeconds(Mic.CaptureTime);
		}
		if (string.Compare(m_RecordingProfile, wordLabel) == 0)
		{
			bool checkNoise = false;
			bool buttonUp = Input.GetMouseButtonUp(0);
			if (m_timerStart > DateTime.Now &&
			    !buttonUp)
			{
				//Debug.Log("Button still pressed");
			}
			else if (m_timerStart != DateTime.MinValue &&
			         m_timerStart < DateTime.Now)
			{
				//Debug.Log("Button timed out");
				SetupWordProfile(false);
				ProfileSave();
				m_timerStart = DateTime.MinValue;
				m_RecordingProfile = string.Empty;
				checkNoise = true;
			}
			else if (m_timerStart != DateTime.MinValue &&
			         buttonUp)
			{
				//Debug.Log("Button is no longer pressed");
				SetupWordProfile(true);
				ProfileSave();
				m_timerStart = DateTime.MinValue;
				m_RecordingProfile = string.Empty;
				checkNoise = true;
			}

			if(checkNoise)
			{
				WordDetails details = GetWord(WORD_NOISE);
				if(details.Wave == null)
				{
					m_timerStartNoise = DateTime.Now + TimeSpan.FromSeconds(Mic.CaptureTime);
				}
			}
		}
	}

	private void NoiseSetProfile()
	{
		if (string.IsNullOrEmpty(m_RecordingProfile) &&
		    m_timerStart == DateTime.MinValue)
		{
			//Debug.Log("Initial button down");
			m_RecordingProfile = WORD_NOISE;
			m_startPosition = Mic.GetPosition();
			m_timerStart = DateTime.Now + TimeSpan.FromSeconds(Mic.CaptureTime);
		}
		if (string.Compare(m_RecordingProfile, WORD_NOISE) == 0)
		{
			if (m_timerStart > DateTime.Now)
			{
				//Debug.Log("Button still pressed");
			}
			else if (m_timerStart != DateTime.MinValue &&
			         m_timerStart < DateTime.Now)
			{
				//Debug.Log("Button timed out");
				SetupWordProfile(false);
				ProfileSave();
				m_timerStart = DateTime.MinValue;
				m_RecordingProfile = string.Empty;
			}
		}
	}

	/// <summary>
	/// Fetch the microphone data for the next update frame
	/// </summary>
	void Update()
	{
		if (firstFrame) {
			firstFrame = false;
			ProfileLoad ();
		}

		Mic.GetData (0);
		//ButtonSetProfile (btnNoiseRecord, WORD_NOISE);
		ButtonSetProfile (btnSitDownRecord, WORD_SITDOWN);
		ButtonSetProfile (btnFallDownRecord, WORD_FALLDOWN);
		ButtonSetProfile (btnStandUpRecord, WORD_STANDUP);
		ButtonSetProfile (btnRightRawUpRecord, WORD_RIGHTRAWUP);
		ButtonSetProfile (btnLeftRawUpRecord, WORD_LEFTRAWUP);
		ButtonSetProfile (btnInteractRecord, WORD_INTERACT);

		if (m_timerStartNoise != DateTime.MinValue && m_timerStartNoise <= DateTime.Now) {
			NoiseSetProfile();
			if(m_RecordingProfile == string.Empty)
				m_timerStartNoise = DateTime.MinValue;
		}
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
		if (null != details.Audio)
			return true;

		return false;
	}

	public void EnableDetectWords(bool enable)
	{
		AudioWordDetection.UsePushToTalk = !enable;
	}

	public bool IsEnableDetectWords()
	{
		return !AudioWordDetection.UsePushToTalk;
	}
}