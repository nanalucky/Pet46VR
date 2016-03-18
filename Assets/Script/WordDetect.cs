//#define USE_WORD_MIN_SCORE

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

/// <summary>
/// Handle word detection
/// </summary>
public class WordDetect : MonoBehaviour
{
	/// <summary>
	/// Reference to word detection
	/// </summary>
	private Word word = null;

	/// <summary>
	/// The closest matched word index
	/// </summary>
	private int ClosestIndex = 0;
	

	/// <summary>
	/// Word detection event
	/// </summary>
	public EventHandler<Word.WordEventArgs> WordDetectEvent = null;
	
	/// <summary>
	/// The zero index is noise
	/// </summary>
	private const int FIRST_WORD_INDEX = 0;
	
	/// <summary>
	/// The normalized wave
	/// </summary>
	private float[] m_wave = null;
	
	/// <summary>
	/// The spectrum data, real
	/// </summary>
	private float[] m_spectrumReal = null;
	
	/// <summary>
	/// The spectrum data, imaginary
	/// </summary>
	private float[] m_spectrumImag = null;
	

	public bool EnableDetect = false;
	

	void Start()
	{
		word = gameObject.GetComponent<Word> ();
	}

	void Update()
	{
		if(!EnableDetect)
			return;

		if (null == word.Mic ||
		    string.IsNullOrEmpty(word.Mic.DeviceName))
		{
			return;
		}

		word.Mic.GetData (0);
		float[] wave = word.Mic.GetLastData();
		if (null != wave)
		{
			//allocate for the wave copy
			int size = wave.Length;
			int halfSize = size/2;
			if (null == m_wave ||
			    m_wave.Length != size)
			{
				m_wave = new float[size];
			}
			
			//trim the wave
			int position = word.Mic.GetPosition();
			
			//shift array
			for (int index = 0, i = position; index < size; ++index, i = (i + 1)%size)
			{
				m_wave[index] = wave[i];
			}
			
			if (word.NormalizeWave)
			{
				//normalize the array
				word.Mic.NormalizeWave(m_wave);
			}
			
			if (null == m_spectrumReal ||
			    m_spectrumReal.Length != halfSize)
			{
				m_spectrumReal = new float[halfSize];
			}
			
			if (null == m_spectrumImag ||
			    m_spectrumImag.Length != halfSize)
			{
				m_spectrumImag = new float[halfSize];
			}
			
			//get the spectrum for the normalized wave
			word.Mic.GetSpectrumData(m_wave, m_spectrumReal, m_spectrumImag, FFTWindow.Rectangular);

			DetectWords(wave);
		}
	}
	
	public void DetectWords(float[] wave)
	{
		float minScore = 0f;
		
		int closestIndex = 0;
		WordDetails closestWord = null;
		
		int size = wave.Length;
		int halfSize = size / 2;
		for (int wordIndex = FIRST_WORD_INDEX; wordIndex < word.Words.Count; ++wordIndex)
		{
			WordDetails details = word.Words[wordIndex];
			
			if (null == details)
			{
				continue;
			}
			
			if (word.WordsToIgnore.Contains(details.Label))
			{
				continue;
			}
			
			float[] spectrum = details.SpectrumReal;
			if (null == spectrum)
			{
				//Debug.LogError(string.Format("Word profile not set: {0}", details.Label));
				details.Score = -1;
				continue;
			}
			if (null == m_spectrumReal)
			{
				details.Score = -1;
				continue;
			}
			if (spectrum.Length != halfSize ||
			    m_spectrumReal.Length != halfSize)
			{
				details.Score = -1;
				continue;
			}
			
			float score = word.Score(m_spectrumReal, spectrum);
			details.Score = score;
			
			#if USE_WORD_MIN_SCORE
			details.AddMinScore(score);
			score = details.GetMinScore(DateTime.Now - TimeSpan.FromSeconds(1));
			#endif
			
			if (wordIndex == FIRST_WORD_INDEX)
			{
				closestIndex = wordIndex;
				minScore = score;
				closestWord = details;
			}
			else if (score < minScore)
			{
				closestIndex = wordIndex;
				minScore = score;
				closestWord = details;
			}
		}
		
		//GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().DebugShow(string.Format("1111 minScore:{0}, closestIndex:{1}", minScore, closestIndex));
		if (ClosestIndex != closestIndex && minScore < word.DetectScoreThreshhold)
		{
			ClosestIndex = closestIndex;
			if (null != WordDetectEvent)
			{
				Word.WordEventArgs args = new Word.WordEventArgs();
				args.Details = closestWord;
				//Debug.Log(args.Details.Label);
				WordDetectEvent.Invoke(this, args);
				ClearMicData();

				GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>().DebugShow(string.Format("2222 minScore:{0}, closestIndex:{1}", minScore, closestIndex));
			}
		}
	}

	public void ClearMicData()
	{
		word.Mic.GetData (0);
		float[] wave = word.Mic.GetLastData();
		int size = wave.Length;
		for(int index = 0; index < size; index++)
		{
			wave[index] = 0;
		}
	}

	public void Enable(bool enable)
	{
		EnableDetect = enable;
		ClearMicData();
		ClosestIndex = 0;
	}
}