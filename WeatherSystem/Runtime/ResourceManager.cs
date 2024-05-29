using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wendy
{
	public class ResourceManager : MonoBehaviour
	{
		// Particle Effects,Audios and so on
		
		Coroutine WeatherEffectCoroutine, AdditionalWeatherEffectCoroutine, ParticleFadeCoroutine,AdditionalParticleFadeCoroutine,
		SoundInCoroutine, SoundOutCoroutine;
		
		public WeatherType currentWeatherType;
		
		public int TransitionSpeed = 45;
		
		public ParticleSystem CurrentParticleSystem;
		[HideInInspector]
		public ParticleSystem AdditionalCurrentParticleSystem;
		
		public List<ParticleSystem> ParticleSystemList = new List<ParticleSystem>();
		[HideInInspector]
		public List<ParticleSystem> WeatherEffectsList = new List<ParticleSystem>();
		public List<ParticleSystem> AdditionalParticleSystemList = new List<ParticleSystem>();
		public List<ParticleSystem> AdditionalWeatherEffectsList = new List<ParticleSystem>();
		
		//Audio Mixer Volumes
		
		public float WeatherSoundsVolume = 1;
		
		public float AmbienceVolume = 1;
		
		public float MusicVolume = 1;
		
		public List<AudioSource> WeatherSoundsList = new List<AudioSource>();
		
		public UnityEngine.Audio.AudioMixer wendyAudioMixer;
		
		public List<WeatherType> weatherTypesList = new List<WeatherType>();
		
		public GameObject m_SoundTransform;

		public GameObject m_EffectsTransform;
		
		public WeatherType nextWeatherType;
		
		public List<WeatherType> NonPrecipiationWeatherTypes = new List<WeatherType>();
		
		public List<WeatherType> PrecipiationWeatherTypes = new List<WeatherType>();
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		
		public void Initialize() {
			//If our current weather type is not a part of the available weather type lists, assign it to the proper category.
			if (!weatherTypesList.Contains(currentWeatherType))
			{
				weatherTypesList.Add(currentWeatherType);
			}
			var PlayerTransform = WeatherSystem.Instance.PlayerTransform;
			
			// 2. Audio Settings
			if (MusicVolume == 0)
			{
				MusicVolume = 0.001f;
			}
			if (AmbienceVolume == 0)
			{
				AmbienceVolume = 0.001f;
			}
			if (WeatherSoundsVolume == 0)
			{
				WeatherSoundsVolume = 0.001f;
			}
			
			wendyAudioMixer = Resources.Load("Audios/WendyAudioMixer") as UnityEngine.Audio.AudioMixer;
			wendyAudioMixer.SetFloat("MusicVolume", Mathf.Log(MusicVolume) * 20);
			wendyAudioMixer.SetFloat("AmbienceVolume", Mathf.Log(AmbienceVolume) * 20);
			wendyAudioMixer.SetFloat("WeatherVolume", Mathf.Log(WeatherSoundsVolume) * 20);

			//Setup our sound holder
			m_SoundTransform = new GameObject();
			m_SoundTransform.name = "Wendy Sounds";
			m_SoundTransform.transform.SetParent(PlayerTransform);
			m_SoundTransform.transform.localPosition = Vector3.zero;


			// 3. Setup Particles
			//Setup our particle effects holder
			m_EffectsTransform = new GameObject();
			m_EffectsTransform.name = "Wendy Effects";
			m_EffectsTransform.transform.SetParent(PlayerTransform);
			m_EffectsTransform.transform.localPosition = Vector3.zero;

			for (int i = 0; i < weatherTypesList.Count; i++)
			{
				if (weatherTypesList[i].PrecipitationWeatherType == WeatherType.WeatherSetting.Yes && !PrecipiationWeatherTypes.Contains(weatherTypesList[i]))
				{
					PrecipiationWeatherTypes.Add(weatherTypesList[i]);
				}
				else if (weatherTypesList[i].PrecipitationWeatherType == WeatherType.WeatherSetting.No && !NonPrecipiationWeatherTypes.Contains(weatherTypesList[i]))
				{
					NonPrecipiationWeatherTypes.Add(weatherTypesList[i]);
				}
			}

			// 4. Inspect Audio and Particle assets
			// 如果声音和粒子特效的资产找不到，则将当前天气的相关特性禁用或重新生成(从SO中获取生成)
			for (int i = 0; i < weatherTypesList.Count; i++)
			{
				//If our weather types have certain features enabled, but there are none detected, disable the feature.
				if (weatherTypesList[i].UseWeatherSound == WeatherType.WeatherSetting.Yes && weatherTypesList[i].WeatherSound == null)
				{
					weatherTypesList[i].UseWeatherSound = WeatherType.WeatherSetting.No;
				}

				if (weatherTypesList[i].UseWeatherEffect == WeatherType.WeatherSetting.Yes && weatherTypesList[i].WeatherEffect == null)
				{
					weatherTypesList[i].UseWeatherEffect = WeatherType.WeatherSetting.No;
				}

				if (weatherTypesList[i].UseAdditionalWeatherEffect == WeatherType.WeatherSetting.Yes && weatherTypesList[i].AdditionalWeatherEffect == null)
				{
					weatherTypesList[i].UseAdditionalWeatherEffect = WeatherType.WeatherSetting.No;
				}
				
				//Add all of our weather effects to a list to be controlled when needed.
				if (!ParticleSystemList.Contains(weatherTypesList[i].WeatherEffect) && weatherTypesList[i].WeatherEffect != null)
				{
					weatherTypesList[i].CreateWeatherEffect();
					ParticleSystemList.Add(weatherTypesList[i].WeatherEffect);
					
				}

				//Add all of our additional weather effects to a list to be controlled when needed.
				if (!AdditionalParticleSystemList.Contains(weatherTypesList[i].AdditionalWeatherEffect))
				{
					if (weatherTypesList[i].UseAdditionalWeatherEffect == WeatherType.WeatherSetting.Yes)
					{
						weatherTypesList[i].CreateAdditionalWeatherEffect();
						AdditionalParticleSystemList.Add(weatherTypesList[i].AdditionalWeatherEffect);
					}
				}

				//Create a weather sound for each weather type that has one.
				if (weatherTypesList[i].UseWeatherSound == WeatherType.WeatherSetting.Yes && weatherTypesList[i].WeatherSound != null)
				{
					weatherTypesList[i].CreateWeatherSound();
				}
			}
			
			InitializeWeather();
		}

		// Update is called once per frame
		void Update()
		{
			
		}
		
		void InitializeWeather()
		{
			// 粒子数量初始化归零
			for (int i = 0; i < WeatherEffectsList.Count; i++)
			{
				ParticleSystem.EmissionModule CurrentEmission = WeatherEffectsList[i].emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			}

			for (int i = 0; i < AdditionalWeatherEffectsList.Count; i++)
			{
				ParticleSystem.EmissionModule CurrentEmission = AdditionalWeatherEffectsList[i].emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
			}
			// 判断是否使用effect,是则从SO中读取粒子数量进行初始化
			//Initialize our weather type's particle effetcs
			if (currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				for (int i = 0; i < WeatherEffectsList.Count; i++)
				{
					if (WeatherEffectsList[i].name == currentWeatherType.WeatherEffect.name + " (Wendy)")
					{
						CurrentParticleSystem = WeatherEffectsList[i];
						ParticleSystem.EmissionModule CurrentEmission = CurrentParticleSystem.emission;
						CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve((float)currentWeatherType.ParticleEffectAmount);
						break;
					}
				}

				CurrentParticleSystem.transform.localPosition = currentWeatherType.ParticleEffectVector;
			}
			
			// Audio
			foreach (AudioSource A in WeatherSoundsList)
			{
				A.volume = 0;
			}

			if (currentWeatherType.UseWeatherSound == WeatherType.WeatherSetting.Yes)
			{
				foreach (AudioSource A in WeatherSoundsList)
				{
					if (A.gameObject.name == currentWeatherType.WeatherTypeName + " (Wendy)")
					{
						A.Play();
						A.volume = currentWeatherType.WeatherVolume;
					}
				}
			}
		}
		
		public void TransitionWeather(FadeSequence FS)
		{
			if (WeatherEffectCoroutine != null) { StopCoroutine(WeatherEffectCoroutine); }
			if (AdditionalWeatherEffectCoroutine != null) { StopCoroutine(AdditionalWeatherEffectCoroutine); }
			if (ParticleFadeCoroutine != null) { StopCoroutine(ParticleFadeCoroutine); }
			if (AdditionalParticleFadeCoroutine != null) { StopCoroutine(AdditionalParticleFadeCoroutine); }
			if (SoundInCoroutine != null) { StopCoroutine(SoundInCoroutine); }
			if (SoundOutCoroutine != null) { StopCoroutine(SoundOutCoroutine); }
			
			currentWeatherType = FS.currentWeatherType;
			// Effect&Audio TransitionWeather
			if (currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				// CurrentParticleSystem 切换成 需要转换的天气
				for (int i = 0; i < WeatherEffectsList.Count; i++)
				{
					if (WeatherEffectsList[i].name == currentWeatherType.WeatherEffect.name + " (Wendy)")
					{
						CurrentParticleSystem = WeatherEffectsList[i];
						CurrentParticleSystem.transform.localPosition = currentWeatherType.ParticleEffectVector;
					}
				}
				FS.CurrentParticleSystem = CurrentParticleSystem;//TODO: to be fixed
				// 粒子生成的渐变
				if (CurrentParticleSystem.emission.rateOverTime.constant < currentWeatherType.ParticleEffectAmount)
				{
					WeatherEffectCoroutine = StartCoroutine(FS.ParticleFadeSequence(TransitionSpeed, 0, currentWeatherType.ParticleEffectAmount));
				}
				else
				{
					ParticleFadeCoroutine = StartCoroutine(FS.ParticleFadeOutSequence(-TransitionSpeed / 6, currentWeatherType.ParticleEffectAmount, CurrentParticleSystem));
				}
			}

			if (currentWeatherType.UseAdditionalWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				for (int i = 0; i < AdditionalWeatherEffectsList.Count; i++)
				{
					if (AdditionalWeatherEffectsList[i].name == currentWeatherType.AdditionalWeatherEffect.name + " (Wendy)")
					{
						AdditionalCurrentParticleSystem = AdditionalWeatherEffectsList[i];
						AdditionalCurrentParticleSystem.transform.localPosition = currentWeatherType.AdditionalParticleEffectVector;
					}
				}

				if (AdditionalCurrentParticleSystem.emission.rateOverTime.constant < currentWeatherType.AdditionalParticleEffectAmount)
				{
					AdditionalWeatherEffectCoroutine = StartCoroutine(FS.AdditionalParticleFadeSequence(TransitionSpeed * 10, 0, currentWeatherType.AdditionalParticleEffectAmount));
				}
				else
				{
					AdditionalParticleFadeCoroutine = StartCoroutine(FS.AdditionalParticleFadeOutSequence(-TransitionSpeed * 2.5f, currentWeatherType.AdditionalParticleEffectAmount, AdditionalCurrentParticleSystem));
				}
			}

			if (currentWeatherType.UseWeatherSound == WeatherType.WeatherSetting.Yes)
			{
				foreach (AudioSource A in WeatherSoundsList)
				{
					if (A.gameObject.name == currentWeatherType.WeatherTypeName + " (Wendy)")
					{
						A.Play();
						SoundInCoroutine = StartCoroutine(FS.SoundFadeInSequence(TransitionSpeed / 4, currentWeatherType.WeatherVolume, A));
					}
				}
			}

			if (currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.No)
			{
				CurrentParticleSystem = null;

				if (currentWeatherType.UseAdditionalWeatherEffect == WeatherType.WeatherSetting.No)
				{
					AdditionalCurrentParticleSystem = null;
				}
			}

			foreach (ParticleSystem P in WeatherEffectsList)
			{
				if (P != CurrentParticleSystem && P.emission.rateOverTime.constant > 0 ||
					currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.No && P.emission.rateOverTime.constant > 0)
				{
					ParticleFadeCoroutine = StartCoroutine(FS.ParticleFadeOutSequence(-TransitionSpeed / 6, 0, P));
				}
			}

			foreach (ParticleSystem P in AdditionalWeatherEffectsList)
			{
				if (P != AdditionalCurrentParticleSystem && P.emission.rateOverTime.constant > 0 ||
					currentWeatherType.UseAdditionalWeatherEffect == WeatherType.WeatherSetting.No && P.emission.rateOverTime.constant > 0)
				{
					AdditionalParticleFadeCoroutine = StartCoroutine(FS.AdditionalParticleFadeOutSequence(-TransitionSpeed * 2.5f, 0, P));
				}
			}

			foreach (AudioSource A in WeatherSoundsList)
			{
				if (A.gameObject.name != currentWeatherType.WeatherTypeName + " (Wendy)" && A.volume > 0 || currentWeatherType.UseWeatherSound == WeatherType.WeatherSetting.No && A.volume > 0)
				{
					SoundOutCoroutine = StartCoroutine(FS.SoundFadeOutSequence(-TransitionSpeed / 8, 0, A));
				}
			}
		}
	}
}


	

