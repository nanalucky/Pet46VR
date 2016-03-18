using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example of detecting audio words
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class WordRecord : MonoBehaviour
{
	/// <summary>
	/// Reference to word detection
	/// </summary>
	private Word word = null;

	private string m_RecordingProfile = string.Empty;
	private Button m_RecordingBtn;
	private WordDetails m_RecordingDetails = new WordDetails();
	private WordDetails m_RecordingNoiseDetails = new WordDetails();

	protected int m_startPosition = 0;
	protected DateTime m_timerStart = DateTime.MinValue;

	public EventHandler<Word.WordEventArgs> WordRecordEvent = null;

	public enum State
	{
		None,
		Word,
		WordToNoise,
		Noise,
		Result,
	}
	public State state = State.None;


	void Start()
	{
		word = gameObject.GetComponent<Word> ();
	}

	public void StartRecord(Button btn, string wordLabel)
	{
		if(word.Mic == null || string.IsNullOrEmpty(word.Mic.DeviceName))
			return;

		if(state != State.None)
			return;

		if (btn == null)
			return;
		if (!btn.IsActive())
			return;
		if(string.IsNullOrEmpty(wordLabel))
			return;

		m_RecordingBtn = btn;
		m_RecordingProfile = wordLabel;

		state = State.Word;
		m_startPosition = word.Mic.GetPosition();
		m_timerStart = DateTime.Now + TimeSpan.FromSeconds(word.Mic.CaptureTime);
	}

	void GatherWave(bool isNoise)
	{
		if (null == word ||
		    null == word.Mic ||
		    string.IsNullOrEmpty(word.Mic.DeviceName))
		{
			return;
		}

		WordDetails details = m_RecordingDetails;
		if(isNoise)
			details = m_RecordingNoiseDetails;

		word.Mic.GetData (0);
		float[] wave = word.Mic.GetLastData();
		if (null != wave)
		{
			//allocate for the wave copy
			int size = wave.Length;
			if (null == details.Wave ||
			    details.Wave.Length != size)
			{
				details.Wave = new float[size];
			}
			
			//trim the wave
			int position = word.Mic.GetPosition();
			
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
			
			if (word.NormalizeWave &&
			    !isNoise)
			{
				//normalize the array
				word.Mic.NormalizeWave(details.Wave);
			}
			
			word.SetupWordProfile(details, false);
		}
	}

	/// <summary>
	/// Setup the word profile
	/// </summary>
	void SetupWordProfile(bool isNoise)
	{
		if (null == word ||
		    null == word.Mic ||
		    string.IsNullOrEmpty(word.Mic.DeviceName))
		{
			return;
		}
		
		WordDetails dstDetails = word.GetWord(m_RecordingProfile);
		if(isNoise)
			dstDetails = word.GetWord(Word.WORD_NOISE);
		if (dstDetails == null)
			return;

		WordDetails srcDetails = m_RecordingDetails;
		if(isNoise)
			srcDetails = m_RecordingNoiseDetails;

		float[] wave = srcDetails.Wave;
		if (null != wave)
		{
			//allocate for the wave copy
			int size = wave.Length;
			if (null == dstDetails.Wave ||
			    dstDetails.Wave.Length != size)
			{
				dstDetails.Wave = new float[size];
				if (null != dstDetails.Audio)
				{
					UnityEngine.Object.DestroyImmediate(dstDetails.Audio, true);
					dstDetails.Audio = null;
				}
			}
			
			//zero the existing wave
			for (int index = 0; index < size; ++index)
			{
				dstDetails.Wave[index] = wave[index];
			}
			

			word.SetupWordProfile(dstDetails, isNoise);
			
			//play the audio
			if (null == dstDetails.Audio)
			{
				dstDetails.Audio = AudioClip.Create(string.Empty, size, 1, word.Mic.SampleRate, false, false);
			}
			dstDetails.Audio.SetData(dstDetails.Wave, 0);
			GetComponent<AudioSource>().loop = false;
			GetComponent<AudioSource>().mute = false;
			if (!isNoise)
			{
				if (word.NormalizeWave)
				{
					GetComponent<AudioSource>().PlayOneShot(dstDetails.Audio, 0.1f);
				}
				else
				{
					GetComponent<AudioSource>().PlayOneShot(dstDetails.Audio);
				}
			}
		}
	}

	void SetupNoiseProfile()
	{
		if (null == word ||
		    null == word.Mic ||
		    string.IsNullOrEmpty(word.Mic.DeviceName))
		{
			return;
		}
		
		WordDetails dstDetails = word.GetWord(Word.WORD_NOISE);
		if (dstDetails == null)
			return;

		if(dstDetails.Wave != null)
			return;
		
			//allocate for the wave copy
		int size = word.Mic.SampleRate * word.Mic.CaptureTime;
		dstDetails.Wave = new float[size];

		//zero the existing wave
		for (int index = 0; index < size; ++index)
		{
			dstDetails.Wave[index] = 0;
		}
		
		word.SetupWordProfile(dstDetails, true);
		
		//play the audio
		if (null == dstDetails.Audio)
		{
			dstDetails.Audio = AudioClip.Create(string.Empty, size, 1, word.Mic.SampleRate, false, false);
		}
		dstDetails.Audio.SetData(dstDetails.Wave, 0);
	}

	void Update()
	{
		switch(state)
		{
		case State.Word:
		{
			if(Input.GetMouseButtonUp(0) || m_timerStart <= DateTime.Now)
			{
				GatherWave(false);

				//state = State.WordToNoise;
				//m_timerStart = DateTime.Now + TimeSpan.FromSeconds(word.RecordWordToNoiseInterval);
				state = State.Result;
				m_timerStart = DateTime.MinValue;
			}
		}
			break;

		case State.WordToNoise:
		{
			if(m_timerStart <= DateTime.Now)
			{
				state = State.Noise;
				m_startPosition = word.Mic.GetPosition();
				m_timerStart = DateTime.Now + TimeSpan.FromSeconds(word.Mic.CaptureTime);
			}
		}
			break;

		case State.Noise:
		{
			if(m_timerStart <= DateTime.Now)
			{
				GatherWave(true);
				
				state = State.Result;
				m_timerStart = DateTime.MinValue;
			}
		}
			break;

		case State.Result:
		{
			state = State.None;

			SetupNoiseProfile();
			WordDetails noiseDetails = word.GetWord(Word.WORD_NOISE);
			Word.WordEventArgs args = new Word.WordEventArgs();
			args.Details = word.GetWord(m_RecordingProfile);
			args.result = false;

			// success
			//if(word.Score(m_RecordingDetails.SpectrumReal, m_RecordingNoiseDetails.SpectrumReal) >= word.RecordScoreThreshhold)
			float score = word.Score(m_RecordingDetails.SpectrumReal, noiseDetails.SpectrumReal);
			GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().DebugShow(string.Format("{0}", score));
			if( score >= word.RecordScoreThreshhold)
			{
				args.result = true;
				SetupWordProfile(false);
				//SetupWordProfile(true);
				word.ProfileSave();
			}

			//Debug.Log(args.Details.Label);
			WordRecordEvent.Invoke(this, args);
		}
			break;
		}
	}

	public bool IsRecording()
	{
		return state != State.None;
	}
	
}