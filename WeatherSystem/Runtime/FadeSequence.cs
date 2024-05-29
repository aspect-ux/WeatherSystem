using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wendy
{
	/// <summary>
	/// FadeSequence 天气切换 渐变次序
	/// </summary>
	public class FadeSequence : MonoBehaviour
	{
		public WeatherType currentWeatherType;
		
		public Material m_CloudDomeMaterial,m_LightningFlashMaterial;
		
		public ParticleSystem CurrentParticleSystem,AdditionalCurrentParticleSystem;
		
		// Start is called before the first frame update
		void Start()
		{
			
		}

		// Update is called once per frame
		void Update()
		{
			
		}
		
		public IEnumerator CloudFadeSequence(float TransitionTime, float MaxValue)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = m_CloudDomeMaterial.GetFloat("_CloudCover");

			while ((CurrentValue >= MaxValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				m_CloudDomeMaterial.SetFloat("_CloudCover", CurrentValue);
				//Debug.Log(CurrentValue + "cloudcover");
				yield return null;
			}
		}

		public IEnumerator FogFadeSequence(float TransitionTime, float MinValue, float MaxValue,float CurrentFogAmount)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.9f);
			}

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = RenderSettings.fogDensity;
			CurrentFogAmount = RenderSettings.fogDensity;

			//AQUAS support comming soon
			/*
			#if AQUAS_PRESENT
			if (m_AquasPresent && m_Aquas.underWater)
			{
				CurrentValue = m_AQUAS_CurrentFogValue;
				CurrentFogAmount = m_AQUAS_CurrentFogValue;
			}
			#endif
			*/

			while ((CurrentValue >= MinValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;

				RenderSettings.fogDensity = CurrentValue;
				CurrentFogAmount = RenderSettings.fogDensity;
				//AQUAS support comming soon
				/*
				#if AQUAS_PRESENT
				if (m_AquasPresent)
				{
					if (!m_Aquas.underWater)
					{
						RenderSettings.fogDensity = CurrentValue;
						CurrentFogAmount = RenderSettings.fogDensity;
					}
				}
				else
				RenderSettings.fogDensity = CurrentValue;
				CurrentFogAmount = RenderSettings.fogDensity;
				#else
				RenderSettings.fogDensity = CurrentValue;
				CurrentFogAmount = RenderSettings.fogDensity;
				#endif
				*/

				yield return null;
			}

			CurrentFogAmount = CurrentValue;
		}

		public IEnumerator WindFadeSequence(float TransitionTime, float MinValue, float MaxValue)
		{
			yield return null;
			/*
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = WendyWindZone.windMain;
			CurrentWindIntensity = CurrentValue;


			while ((CurrentValue >= MinValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				WendyWindZone.windMain = CurrentValue;
				CurrentWindIntensity = CurrentValue;

				yield return null;
			}*/
		}

		public IEnumerator SunFadeSequence(float TransitionTime, float MaxValue,float SunIntensity)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.8f);
				TransitionTime = TransitionTime / 3;
			}

			if (SunIntensity > currentWeatherType.SunIntensity)
			{
				TransitionTime = TransitionTime * -1;
			}

			if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.DontChange)
			{
				yield break;
			}

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = SunIntensity;

			while ((CurrentValue >= MaxValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				SunIntensity = CurrentValue;

				yield return null;
			}
		}

		public IEnumerator MoonFadeSequence(float TransitionTime, float MaxValue,float MoonIntensity)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.8f);
			}

			if (MoonIntensity > currentWeatherType.MoonIntensity)
			{
				TransitionTime = TransitionTime * -1;
			}

			if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.DontChange)
			{
				yield break;
			}

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = MoonIntensity;

			while ((CurrentValue >= MaxValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				MoonIntensity = CurrentValue;

				yield return null;
			}
		}

		public IEnumerator ParticleFadeSequence(float TransitionTime, float MinValue, float MaxValue)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{Debug.Log(m_CloudDomeMaterial.GetFloat("_CloudCover"));
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.9f);
			}
			
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = CurrentParticleSystem.emission.rateOverTime.constant;
			Debug.Log(CurrentValue);
			while ((CurrentValue >= MinValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed * 2000;
				ParticleSystem.EmissionModule CurrentEmission = CurrentParticleSystem.emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(CurrentValue);
				yield return null;
			}
		}

		public IEnumerator ParticleFadeOutSequence(float TransitionTime, float MinValue, ParticleSystem EffectToFade)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = EffectToFade.emission.rateOverTime.constant;

			while ((CurrentValue >= MinValue && FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed * 2000;
				ParticleSystem.EmissionModule CurrentEmission = EffectToFade.emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(CurrentValue);

				yield return null;
			}
		}

		public IEnumerator AdditionalParticleFadeSequence(float TransitionTime, float MinValue, float MaxValue)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.9f);
			}

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = AdditionalCurrentParticleSystem.emission.rateOverTime.constant;

			while ((CurrentValue >= MinValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed * 2000;
				ParticleSystem.EmissionModule CurrentEmission = AdditionalCurrentParticleSystem.emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(CurrentValue);

				yield return null;
			}
		}

		public IEnumerator AdditionalParticleFadeOutSequence(float TransitionTime, float MinValue, ParticleSystem EffectToFade)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = EffectToFade.emission.rateOverTime.constant;

			while ((CurrentValue >= MinValue && FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed * 2000;
				ParticleSystem.EmissionModule CurrentEmission = EffectToFade.emission;
				CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(CurrentValue);

				yield return null;
			}
		}

		public IEnumerator SoundFadeInSequence(float TransitionTime, float MaxValue, AudioSource SourceToFade)
		{
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.9f);
			}

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = SourceToFade.volume;

			while ((CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				SourceToFade.volume = CurrentValue;

				yield return null;
			}
		}

		public IEnumerator SoundFadeOutSequence(float TransitionTime, float MinValue, AudioSource SourceToFade)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = SourceToFade.volume;

			while ((CurrentValue >= MinValue && FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				SourceToFade.volume = CurrentValue;

				yield return null;
			}
		}

		public IEnumerator CloudHeightFadeSequence(float TransitionTime, int MinValue, int MaxValue)
		{
			yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.8f);

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = m_CloudDomeMaterial.GetFloat("_WorldPosGlobal");

			while ((CurrentValue >= MinValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed * 2000;
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", CurrentValue);

				yield return null;
			}
		}

		public IEnumerator RainShaderFadeInSequence(float TransitionTime, float MaxValue)
		{
			yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 1.0f);
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = Shader.GetGlobalFloat("_WetnessStrength");

			while ((CurrentValue < MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				Shader.SetGlobalFloat("_WetnessStrength", CurrentValue);

				yield return null;
			}
		}

		public IEnumerator RainShaderFadeOutSequence(float TransitionTime, float MinValue)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = Shader.GetGlobalFloat("_WetnessStrength");

			while ((CurrentValue >= 0 && FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				Shader.SetGlobalFloat("_WetnessStrength", CurrentValue);

				yield return null;
			}
		}

		public IEnumerator SnowShaderFadeInSequence(float TransitionTime, float MaxValue)
		{
			yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 1.0f);
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = Shader.GetGlobalFloat("_SnowStrength");

			while ((CurrentValue < MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				Shader.SetGlobalFloat("_SnowStrength", CurrentValue);

				yield return null;
			}
		}

		public IEnumerator SnowShaderFadeOutSequence(float TransitionTime, float MinValue)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = Shader.GetGlobalFloat("_SnowStrength");

			while ((CurrentValue >= 0 && FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				Shader.SetGlobalFloat("_SnowStrength", CurrentValue);

				yield return null;
			}
		}

		public IEnumerator LightningCloudsFadeSequence(float TransitionTime, float MinValue, float MaxValue)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = m_LightningFlashMaterial.color.a;

			while ((CurrentValue >= 0f && FadingOut) || (CurrentValue <= 0.5f && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				Color C = m_LightningFlashMaterial.color;
				C.a = CurrentValue;
				m_LightningFlashMaterial.color = C;

				yield return null;
			}
		}

		
	}
}
