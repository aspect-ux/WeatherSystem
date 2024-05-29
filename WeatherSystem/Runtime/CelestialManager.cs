using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Wendy
{
	//=============================Celestial Manager 天空管理器==============================
	//========================主要有三个方面的功能：================================
	// 1. 用于实时更新太阳，月亮，彗星等星体（颜色baseColor和运转逻辑）【天体】
	// 2. 用于实时更新大气散射效果(Atmosphere Scattering)        【大气散射】
	// 3. 用于实时更新由于天气变化引起的天空颜色变化	           【天气变化】
	//=========================End======================================

	public class CelestialManager : MonoBehaviour
	{
		// [Inspector] Sky Colors 
		public Gradient SunColor;
		
		public Gradient StormySunColor;
		
		public Gradient MoonColor;
		
		public Gradient SkyColor;
		
		public Gradient AmbientSkyLightColor;
		
		public Gradient StormyAmbientSkyLightColor;
		
		public Gradient AmbientEquatorLightColor;
		
		public Gradient StormyAmbientEquatorLightColor;
		
		public Gradient AmbientGroundLightColor;
		public Gradient StormyAmbientGroundLightColor;
		
		public Gradient StarLightColor;
		
		public Gradient FogColor;
		
		public Gradient FogStormyColor;
		
		public Gradient CloudLightColor;
		
		public Gradient CloudBaseColor;
		
		public Gradient CloudStormyBaseColor;
		
		public Gradient SkyTintColor;
		
		public Color MoonPhaseColor = Color.white;

		float m_FadeValue;
		public float m_ReceivedCloudValue;

		// TODO: to be Convealed 
		[HideInInspector]
		public Gradient DefaultCloudBaseColor;
		GradientColorKey[] CloudColorKeySwitcher;

		[HideInInspector]
		public Gradient DefaultFogBaseColor;
		GradientColorKey[] FogColorKeySwitcher;

		[HideInInspector]
		public Gradient DefaultAmbientSkyLightBaseColor;
		GradientColorKey[] AmbientSkyLightColorKeySwitcher;

		[HideInInspector]
		public Gradient DefaultAmbientEquatorLightBaseColor;
		GradientColorKey[] AmbientEquatorLightColorKeySwitcher;

		[HideInInspector]
		public Gradient DefaultAmbientGroundLightBaseColor;
		GradientColorKey[] AmbientGroundLightColorKeySwitcher;

		[HideInInspector]
		public Gradient DefaultSunLightBaseColor;
		GradientColorKey[] SunLightColorKeySwitcher;
		
		//Celestial
		Renderer m_CloudDomeRenderer;
		Renderer m_CloudDomeLightningRenderer;
		public Material m_CloudDomeMaterial;
		Material m_SkyBoxMaterial;
		Renderer m_StarsRenderer;
		Material m_StarsMaterial;
		Light m_SunLight;
		Transform m_CelestialAxisTransform;
		
		public int SunRevolution = -90;
		
		public float SunIntensity = 1;
		
		public float PrecipitationSunIntensity = 0.25f;
		
		public AnimationCurve SunIntensityCurve = AnimationCurve.Linear(0, 0, 24, 5);
		
		public AnimationCurve SunSize = AnimationCurve.Linear(0, 1, 24, 10);
		Light m_MoonLight;
		
		public int MoonPhaseIndex = 5;
		
		public float MoonBrightness = 0.7f;
		
		public Material m_MoonPhaseMaterial;
		Renderer m_MoonRenderer;
		Transform m_MoonTransform;
		
		public float MoonIntensity = 1;
		
		public float MoonPhaseIntensity = 1;
		
		public AnimationCurve MoonIntensityCurve = AnimationCurve.Linear(0, 0, 24, 5);
		
		public AnimationCurve MoonSize = AnimationCurve.Linear(0, 1, 24, 10);
		
		Vector3 m_MoonStartingSize;
		GameObject m_MoonParent;
		
		public AnimationCurve AtmosphereThickness = AnimationCurve.Linear(0, 1, 24, 3);
		
		public float StarSpeed = 0.75f;
		
		public int SunAngle = 10;
		
		public int MoonAngle = -10;
		
		float m_TimeFloat;
		
		public int TransitionSpeed = 45;
		
		public HemisphereEnum Hemisphere = HemisphereEnum.Northern;
		public enum HemisphereEnum
		{
			Northern = 0, Southern
		}
		
		[System.Serializable]
		public class MoonPhaseClass
		{
			public Texture MoonPhaseTexture = null;
			public float MoonPhaseIntensity = 1;
		}
		
		public List<MoonPhaseClass> MoonPhaseList = new List<MoonPhaseClass>();

		
		public Color CurrentFogColor;
		
		public float SnowAmount = 0;
		
		public float CurrentWindIntensity = 0;
		
		public int m_CloudSeed;
		public int CloudSpeed = 8;
		
		public float CurrentFogAmount;
		
		WeatherType currentWeatherType;
		
		Coroutine CloudCoroutine, FogCoroutine, SunCoroutine, MoonCoroutine, WindCoroutine;
		Coroutine LightningCloudsCoroutine, ColorCoroutine, CloudHeightCoroutine, RainShaderCoroutine, SnowShaderCoroutine;
		
		float m_TimeOfDaySoundsTimer = 0;
		
		#region Lightning Params
		Light m_LightningLight;
		LightningSystem m_WendyLightningSystem;
		
		public LightningStrike m_LightningStrikeSystem;
		
		public int LightningSecondsMin = 5;
		
		public int LightningSecondsMax = 10;
		int m_LightningSeconds;
		float m_LightningTimer;
		
		public List<AnimationCurve> LightningFlashPatterns = new List<AnimationCurve>();
		
		public List<AudioClip> ThunderSounds = new List<AudioClip>();
		
		public int LightningGroundStrikeOdds = 50;
		
		public GameObject LightningStrikeEffect;
		
		public GameObject LightningStrikeFire;
		
		public EnableFeature LightningOnClouds = EnableFeature.Enabled;
		
		public Material m_LightningFlashMaterial;

		
		public LayerMask DetectionLayerMask;
		
		public List<string> LightningFireTags = new List<string>();
		
		public float LightningLightIntensityMin = 1;
		
		public float LightningLightIntensityMax = 3;
			   
		public int LightningGenerationDistance = 100;
		
		public int LightningDetectionDistance = 20;
		
		public WindZone WendyWindZone;
		
		// 用于实时更新大气散射，并融合云朵等其它天空效果
		public AtmosphereSettings atmosphereSettings;
		
		// Editor
		public bool TimeFoldout = true, DateFoldout = true, TimeSoundsFoldout = true, TimeMusicFoldout = true,
		SunFoldout = true, MoonFoldout = true, AtmosphereFoldout = true,
		WeatherFoldout = true, LightningFoldout = true, CameraFoldout = true, SettingsFoldout = true;
		public int TabNumber;
		#endregion
		
		// Two Parts:
		// 1. Start
		// 2. Update (update per frame + update mannually)
		
		// Start is called before the first frame update
		// 由于WeatherSystem.Start中嵌套Start不能保证执行顺序，所以这里重新写一个函数用于初始化
		void Start()
		{
			
		}
		
		public void Initialize()
		{
			currentWeatherType = WeatherSystem.Instance.currentWeatherType;
			
			InitializeGradient();
			
			CreateSun();
			CreateMoon();
			
			InitializeMaterials();
			
			// 初始化大气散射Settings
			//TODO
			InitializeAtmosphere();
			
			
			// 预测天气(以h或d为单位随机天气，根据温度之类的信息判断nextWeather的可能性)
			//GenerateWeather();
			WendyWindZone = GameObject.Find("Wendy WindZone").GetComponent<WindZone>();

			// 创建闪电系统，添加Lightning Light,Audio
			CreateLightning();
			
			UpdateSkyColor();
			
			CalculateClouds();
			CalculateMoonPhase();
			
			
			
			SetCelestialParams(currentWeatherType);
		}

		// Update is called once per frame
		void Update()
		{
			// [天体]
			MoveSunAndMoon();

			// 更新天空的颜色
			UpdateSkyColor();
			
			UpdateLightning();
			
			m_TimeFloat = WeatherSystem.Instance.m_TimeFloat;
			
		}
		
	#region Periodic Function Part
	
		private void InitializeAtmosphere()
		{
			// pass atmosphere settings value，基本值，不会改变
			//SunIntensity = WeatherSystem.Instance.atmosphereSettings.SunLightIntensity;
			
			// Initialize Sun/Moon,Atmosphere
			//TODO: to be fixed
			var Hour = WeatherSystem.Instance.timeManager.Hour;
			m_SunLight.intensity = SunIntensityCurve.Evaluate((float)Hour) * SunIntensity;
			// 计算当前时间点太阳亮度后传回用于大气计算
			WeatherSystem.Instance.atmosphereSettings.SunLightIntensity = m_SunLight.intensity;
			m_MoonLight.intensity = MoonIntensityCurve.Evaluate((float)Hour) * MoonIntensity * MoonPhaseIntensity;
			/*
			m_SkyBoxMaterial = (Material)Resources.Load("Materials/Skybox") as Material;
			RenderSettings.skybox = m_SkyBoxMaterial;
			m_SkyBoxMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness.Evaluate((float)Hour));
			m_SkyBoxMaterial.SetColor("_NightSkyTint", SkyTintColor.Evaluate((float)Hour));*/
		}
	
		/// <summary>
		/// Initialize Material and Renderers
		/// </summary>
		private void InitializeMaterials()
		{
			// Prepare mats and renderers
			// 找到云朵和星星的Renderer并初始化材质的属性
			m_StarsRenderer = GameObject.Find("Wendy Star").GetComponent<Renderer>();
			m_StarsMaterial = m_StarsRenderer.material;
			m_StarsMaterial.SetFloat("_StarSpeed", StarSpeed);

			m_CloudDomeRenderer = GameObject.Find("Wendy Cloud").GetComponent<Renderer>();
			m_CloudDomeMaterial = m_CloudDomeRenderer.material;
			m_CloudDomeMaterial.SetVector("_CloudSpeed", new Vector4(CloudSpeed * 0.0001f, 0, 0, 0));

			m_CloudDomeLightningRenderer = GameObject.Find("Wendy Cloud Lightning").GetComponent<Renderer>();
			m_LightningFlashMaterial = m_CloudDomeLightningRenderer.material;
			
		}
		/// //Move our sun according to the time of day
		public void MoveSunAndMoon()
		{
			m_CelestialAxisTransform.eulerAngles = new Vector3(m_TimeFloat * 360 - 100, SunRevolution, 180);
		}
		
		/// <summary>
		/// 根据【时间】实时更新颜色
		/// </summary>
		void UpdateSkyColor()
		{
			m_SunLight.color = SunColor.Evaluate(m_TimeFloat);
			m_MoonLight.color = MoonColor.Evaluate(m_TimeFloat);
			m_StarsMaterial.color = StarLightColor.Evaluate(m_TimeFloat);
			
			// Built-in Procedural Skybox Mat
			//m_SkyBoxMaterial.SetColor("_SkyTint", SkyColor.Evaluate(m_TimeFloat));
			//m_SkyBoxMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness.Evaluate(m_TimeFloat*24));
			//m_SkyBoxMaterial.SetColor("_NightSkyTint", SkyTintColor.Evaluate(m_TimeFloat));
			
			// Customed Skybox, reference from ue4
			
			// TODO:云朵与大气的结合，这里只做简单的混合
			m_CloudDomeMaterial.SetColor("_LightColor", CloudLightColor.Evaluate(m_TimeFloat) * m_SunLight.color);
			m_CloudDomeMaterial.SetColor("_BaseColor", CloudBaseColor.Evaluate(m_TimeFloat));
			m_CloudDomeMaterial.SetFloat("_Attenuation", m_SunLight.intensity + m_MoonLight.intensity);
			
			RenderSettings.ambientSkyColor = AmbientSkyLightColor.Evaluate(m_TimeFloat);
			RenderSettings.ambientEquatorColor = AmbientEquatorLightColor.Evaluate(m_TimeFloat);
			RenderSettings.ambientGroundColor = AmbientGroundLightColor.Evaluate(m_TimeFloat);
			RenderSettings.fogColor = FogColor.Evaluate(m_TimeFloat);
			CurrentFogColor = FogColor.Evaluate(m_TimeFloat);
			
			m_SunLight.intensity = SunIntensityCurve.Evaluate(m_TimeFloat * 24) * SunIntensity;
			// renderfeature update
			WeatherSystem.Instance.atmosphereSettings.SunLightIntensity = m_SunLight.intensity;
			
			m_SkyBoxMaterial.SetFloat("_SunSize", SunSize.Evaluate(m_TimeFloat * 24) * 0.01f);
			m_MoonLight.intensity = MoonIntensityCurve.Evaluate(m_TimeFloat * 24) * MoonIntensity * MoonPhaseIntensity;
			m_MoonTransform.localScale = MoonSize.Evaluate(m_TimeFloat * 24) * m_MoonStartingSize;
		}
		
		void UpdateLightning()
		{
			//Generate our lightning, if the randomized lightning seconds have been met
			if (currentWeatherType.UseLightning == WeatherType.WeatherSetting.Yes && currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				m_LightningTimer += Time.deltaTime;

				//Only create a lightning strike if the clouds have fully faded in
				if (m_LightningTimer >= m_LightningSeconds && m_CloudDomeMaterial.GetFloat("_CloudCover") >= 1)
				{
					m_WendyLightningSystem.LightningCurve = LightningFlashPatterns[Random.Range(0, LightningFlashPatterns.Count)];
					m_WendyLightningSystem.GenerateLightning();
					m_LightningSeconds = Random.Range(LightningSecondsMin, LightningSecondsMax);
					m_LightningTimer = 0;
				}
			}
		}
		
		/// <summary>
		/// 根据【天气变化】颜色渐变
		/// </summary>
		public void InitializeGradient()
		{
			// 5. 暴雨天气与普通天气的颜色渐变设置
			CloudColorKeySwitcher = new GradientColorKey[8];
			CloudColorKeySwitcher = CloudBaseColor.colorKeys;
			DefaultCloudBaseColor.colorKeys = new GradientColorKey[8];
			DefaultCloudBaseColor.colorKeys = CloudBaseColor.colorKeys;

			FogColorKeySwitcher = new GradientColorKey[8];
			FogColorKeySwitcher = FogColor.colorKeys;
			DefaultFogBaseColor.colorKeys = new GradientColorKey[8];
			DefaultFogBaseColor.colorKeys = FogColor.colorKeys;

			AmbientSkyLightColorKeySwitcher = new GradientColorKey[8];
			AmbientSkyLightColorKeySwitcher = AmbientSkyLightColor.colorKeys;
			DefaultAmbientSkyLightBaseColor.colorKeys = new GradientColorKey[8];
			DefaultAmbientSkyLightBaseColor.colorKeys = AmbientSkyLightColor.colorKeys;

			AmbientEquatorLightColorKeySwitcher = new GradientColorKey[8];
			AmbientEquatorLightColorKeySwitcher = AmbientEquatorLightColor.colorKeys;
			DefaultAmbientEquatorLightBaseColor.colorKeys = new GradientColorKey[8];
			DefaultAmbientEquatorLightBaseColor.colorKeys = AmbientEquatorLightColor.colorKeys;

			AmbientGroundLightColorKeySwitcher = new GradientColorKey[8];
			AmbientGroundLightColorKeySwitcher = AmbientGroundLightColor.colorKeys;
			DefaultAmbientGroundLightBaseColor.colorKeys = new GradientColorKey[8];
			DefaultAmbientGroundLightBaseColor.colorKeys = AmbientGroundLightColor.colorKeys;

			SunLightColorKeySwitcher = new GradientColorKey[6];
			SunLightColorKeySwitcher = SunColor.colorKeys;
			DefaultSunLightBaseColor.colorKeys = new GradientColorKey[6];
			DefaultSunLightBaseColor.colorKeys = SunColor.colorKeys;
		}
		//Sets Up Sun Light
		void CreateSun()
		{
			m_SunLight = GameObject.Find("Sun Light").GetComponent<Light>();
			m_SunLight.transform.localEulerAngles = new Vector3(0, SunAngle, 0);
			m_CelestialAxisTransform = GameObject.Find("Celestial Transform").transform;
			RenderSettings.sun = m_SunLight;
			m_SkyBoxMaterial = RenderSettings.skybox;
		}
		
		//Create and positioned moon
		void CreateMoon()
		{
			m_MoonLight = GameObject.Find("Moon Light").GetComponent<Light>();
			m_MoonLight.transform.localEulerAngles = new Vector3(-180, MoonAngle, 0);
			GameObject m_CreatedMoon = Instantiate((GameObject)Resources.Load("Prefabs/Moon Object") as GameObject, transform.position, Quaternion.identity);
			m_CreatedMoon.name = "Moon Object";
			m_MoonRenderer = GameObject.Find("Moon Object").GetComponent<Renderer>();
			m_MoonTransform = m_MoonRenderer.transform;
			m_MoonStartingSize = m_MoonTransform.localScale;
			m_MoonPhaseMaterial = m_MoonRenderer.material;
			m_MoonPhaseMaterial.SetColor("_MoonColor", MoonPhaseColor);
			m_MoonTransform.parent = m_MoonLight.transform;
			m_MoonTransform.localPosition = new Vector3(0, 0, -11000);
			m_MoonTransform.localEulerAngles = new Vector3(270, 0, 0);
			m_MoonTransform.localScale = new Vector3(m_MoonTransform.localScale.x, m_MoonTransform.localScale.y, m_MoonTransform.localScale.z);
		}
		
		//Generate a cloud seed for our clouds
		void CalculateClouds()
		{
			m_CloudSeed = Random.Range(-9999, 10000);
			m_CloudDomeMaterial.SetFloat("_Seed", m_CloudSeed);
		}
		
		//Calculates our moon phases. This is updated daily at exactly 12:00.
		void CalculateMoonPhase()
		{
			if (MoonPhaseList.Count > 0)
			{
				if (MoonPhaseIndex == MoonPhaseList.Count) {
					MoonPhaseIndex = 0;
				}
				m_MoonPhaseMaterial.SetTexture("_MainTex", MoonPhaseList[MoonPhaseIndex].MoonPhaseTexture);
				m_MoonRenderer.material = m_MoonPhaseMaterial;
				m_MoonPhaseMaterial.SetFloat("_MoonBrightness", MoonBrightness);
				MoonPhaseIntensity = MoonPhaseList[MoonPhaseIndex].MoonPhaseIntensity;
				m_MoonPhaseMaterial.SetColor("_MoonColor", MoonPhaseColor);
			}
		}

		//Create, setup, and assign all needed lightning components
		void CreateLightning()
		{
			GameObject CreatedLightningSystem = new GameObject("Wendy Lightning System");
			CreatedLightningSystem.AddComponent<LightningSystem>();
			m_WendyLightningSystem = CreatedLightningSystem.GetComponent<LightningSystem>();
			m_WendyLightningSystem.transform.SetParent(this.transform);

			for (int i = 0; i < ThunderSounds.Count; i++)
			{
				m_WendyLightningSystem.ThunderSounds.Add(ThunderSounds[i]);
			}

			GameObject CreatedLightningLight = new GameObject("Wendy Lightning Light");
			CreatedLightningLight.AddComponent<Light>();
			m_LightningLight = CreatedLightningLight.GetComponent<Light>();
			m_LightningLight.type = LightType.Directional;
			m_LightningLight.transform.SetParent(this.transform);
			m_LightningLight.transform.localPosition = Vector3.zero;
			m_LightningLight.intensity = 0;
			m_LightningLight.shadows = LightShadows.Hard;
			m_WendyLightningSystem.LightningLightSource = m_LightningLight;
			m_WendyLightningSystem.PlayerTransform = WeatherSystem.Instance.PlayerTransform;
			m_WendyLightningSystem.LightningGenerationDistance = LightningGenerationDistance;
			m_LightningSeconds = Random.Range(LightningSecondsMin, LightningSecondsMax);
			m_WendyLightningSystem.LightningLightIntensityMin = LightningLightIntensityMin;
			m_WendyLightningSystem.LightningLightIntensityMax = LightningLightIntensityMax;
		}
	#endregion
	
	#region Initialize Weather Part
		public void SetCelestialParams(WeatherType currentWeatherType)
		{
			m_ReceivedCloudValue = GetCloudLevel(m_ReceivedCloudValue);
			m_CloudDomeMaterial.SetFloat("_CloudCover", m_ReceivedCloudValue);
			RenderSettings.fogDensity = currentWeatherType.FogDensity;
			CurrentFogAmount = RenderSettings.fogDensity;
			WendyWindZone.windMain = currentWeatherType.WindIntensity;
			CurrentWindIntensity = currentWeatherType.WindIntensity;
			SunIntensity = currentWeatherType.SunIntensity;
			MoonIntensity = currentWeatherType.MoonIntensity;
			
			
			// Shader Params
			if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.Rain)
			{
				Shader.SetGlobalFloat("_WetnessStrength", 1);
				Shader.SetGlobalFloat("_SnowStrength", 0);
			}
			else if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.Snow)
			{
				Shader.SetGlobalFloat("_SnowStrength", 1);
				Shader.SetGlobalFloat("_WetnessStrength", 0);
			}
			else
			{
				Shader.SetGlobalFloat("_WetnessStrength", 0);
				Shader.SetGlobalFloat("_SnowStrength", 0);
			}

			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", 2500);
			}
			
			
			//Instantly change all of our gradients to the stormy gradients
			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				for (int i = 0; i < CloudBaseColor.colorKeys.Length; i++)
				{
					CloudColorKeySwitcher[i].color = Color.Lerp(CloudColorKeySwitcher[i].color, CloudStormyBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < FogColor.colorKeys.Length; i++)
				{
					FogColorKeySwitcher[i].color = Color.Lerp(FogColorKeySwitcher[i].color, FogStormyColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < AmbientSkyLightColor.colorKeys.Length; i++)
				{
					AmbientSkyLightColorKeySwitcher[i].color = Color.Lerp(AmbientSkyLightColorKeySwitcher[i].color, StormyAmbientSkyLightColor.colorKeys[i].color, 1);
				}
				
				for (int i = 0; i < AmbientEquatorLightColor.colorKeys.Length; i++)
				{
					AmbientEquatorLightColorKeySwitcher[i].color = Color.Lerp(AmbientEquatorLightColorKeySwitcher[i].color, StormyAmbientEquatorLightColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < AmbientGroundLightColor.colorKeys.Length; i++)
				{
					AmbientGroundLightColorKeySwitcher[i].color = Color.Lerp(AmbientGroundLightColorKeySwitcher[i].color, StormyAmbientGroundLightColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < SunColor.colorKeys.Length; i++)
				{
					SunLightColorKeySwitcher[i].color = Color.Lerp(SunLightColorKeySwitcher[i].color, StormySunColor.colorKeys[i].color, 1);
				}

				FogColor.SetKeys(FogColorKeySwitcher, FogColor.alphaKeys);
				CloudBaseColor.SetKeys(CloudColorKeySwitcher, CloudBaseColor.alphaKeys);
				AmbientSkyLightColor.SetKeys(AmbientSkyLightColorKeySwitcher, AmbientSkyLightColor.alphaKeys);
				AmbientEquatorLightColor.SetKeys(AmbientEquatorLightColorKeySwitcher, AmbientEquatorLightColor.alphaKeys);
				AmbientGroundLightColor.SetKeys(AmbientGroundLightColorKeySwitcher, AmbientGroundLightColor.alphaKeys);
				SunColor.SetKeys(SunLightColorKeySwitcher, SunColor.alphaKeys);
			}
			//Instantly change all of our gradients to the regular gradients
			else if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.No)
			{
				for (int i = 0; i < CloudBaseColor.colorKeys.Length; i++)
				{
					CloudColorKeySwitcher[i].color = Color.Lerp(CloudColorKeySwitcher[i].color, DefaultCloudBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < FogColor.colorKeys.Length; i++)
				{
					FogColorKeySwitcher[i].color = Color.Lerp(FogColorKeySwitcher[i].color, DefaultFogBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < AmbientSkyLightColor.colorKeys.Length; i++)
				{
					AmbientSkyLightColorKeySwitcher[i].color = Color.Lerp(AmbientSkyLightColorKeySwitcher[i].color, DefaultAmbientSkyLightBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < AmbientEquatorLightColor.colorKeys.Length; i++)
				{
					AmbientEquatorLightColorKeySwitcher[i].color = Color.Lerp(AmbientEquatorLightColorKeySwitcher[i].color, DefaultAmbientEquatorLightBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < AmbientGroundLightColor.colorKeys.Length; i++)
				{
					AmbientGroundLightColorKeySwitcher[i].color = Color.Lerp(AmbientGroundLightColorKeySwitcher[i].color, DefaultAmbientGroundLightBaseColor.colorKeys[i].color, 1);
				}

				for (int i = 0; i < SunColor.colorKeys.Length; i++)
				{
					SunLightColorKeySwitcher[i].color = Color.Lerp(SunLightColorKeySwitcher[i].color, DefaultSunLightBaseColor.colorKeys[i].color, 1);
				}

				FogColor.SetKeys(FogColorKeySwitcher, FogColor.alphaKeys);
				CloudBaseColor.SetKeys(CloudColorKeySwitcher, CloudBaseColor.alphaKeys);
				AmbientSkyLightColor.SetKeys(AmbientSkyLightColorKeySwitcher, AmbientSkyLightColor.alphaKeys);
				AmbientEquatorLightColor.SetKeys(AmbientEquatorLightColorKeySwitcher, AmbientEquatorLightColor.alphaKeys);
				AmbientGroundLightColor.SetKeys(AmbientGroundLightColorKeySwitcher, AmbientGroundLightColor.alphaKeys);
				SunColor.SetKeys(SunLightColorKeySwitcher, SunColor.alphaKeys);
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", 0);
			}
			
			if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.MostlyCloudy)
			{ 
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", 500);
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.Cloudy)
			{
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", 2500);
			}
			else
			{
				m_CloudDomeMaterial.SetFloat("_WorldPosGlobal", 0);
			}
		}
		
		//Generate and return a random cloud intensity based on the current weather type cloud level
		public float GetCloudLevel(float MaxValue)
		{
			
			if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.Clear)
			{
				MaxValue = 0.5f;
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.MostlyClear)
			{
				MaxValue = Random.Range(0.66f, 0.72f);
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.PartyCloudy)
			{
				MaxValue = Random.Range(0.78f, 0.86f);
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.MostlyCloudy)
			{
				MaxValue = Random.Range(0.9f, 0.96f);
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.Cloudy)
			{
				MaxValue = 1.2f;
			}
			else if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.DontChange)
			{
				MaxValue = m_CloudDomeMaterial.GetFloat("_CloudCover");
			}

			return MaxValue;
		}
	#endregion
	
	public IEnumerator ColorFadeSequence(float TransitionTime, float MinValue, float MaxValue)
		{

			yield return new WaitUntil(() => m_CloudDomeMaterial.GetFloat("_CloudCover") > 0.9f);

			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = 0;

			while ((CurrentValue >= 0f && FadingOut) || (CurrentValue <= 0.01f && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;

				if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
				{
					for (int i = 0; i < CloudBaseColor.colorKeys.Length; i++)
					{
						CloudColorKeySwitcher[i].color = Color.Lerp(CloudColorKeySwitcher[i].color, CloudStormyBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < FogColor.colorKeys.Length; i++)
					{
						FogColorKeySwitcher[i].color = Color.Lerp(FogColorKeySwitcher[i].color, FogStormyColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientSkyLightColor.colorKeys.Length; i++)
					{
						AmbientSkyLightColorKeySwitcher[i].color = Color.Lerp(AmbientSkyLightColorKeySwitcher[i].color, StormyAmbientSkyLightColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientEquatorLightColor.colorKeys.Length; i++)
					{
						AmbientEquatorLightColorKeySwitcher[i].color = Color.Lerp(AmbientEquatorLightColorKeySwitcher[i].color, StormyAmbientEquatorLightColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientGroundLightColor.colorKeys.Length; i++)
					{
						AmbientGroundLightColorKeySwitcher[i].color = Color.Lerp(AmbientGroundLightColorKeySwitcher[i].color, StormyAmbientGroundLightColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < SunColor.colorKeys.Length; i++)
					{
						SunLightColorKeySwitcher[i].color = Color.Lerp(SunLightColorKeySwitcher[i].color, StormySunColor.colorKeys[i].color, CurrentValue);
					}

					FogColor.SetKeys(FogColorKeySwitcher, FogColor.alphaKeys);
					CloudBaseColor.SetKeys(CloudColorKeySwitcher, CloudBaseColor.alphaKeys);
					AmbientSkyLightColor.SetKeys(AmbientSkyLightColorKeySwitcher, AmbientSkyLightColor.alphaKeys);
					AmbientEquatorLightColor.SetKeys(AmbientEquatorLightColorKeySwitcher, AmbientEquatorLightColor.alphaKeys);
					AmbientGroundLightColor.SetKeys(AmbientGroundLightColorKeySwitcher, AmbientGroundLightColor.alphaKeys);
					SunColor.SetKeys(SunLightColorKeySwitcher, SunColor.alphaKeys);
				}
				else if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.No)
				{
					for (int i = 0; i < CloudBaseColor.colorKeys.Length; i++)
					{
						CloudColorKeySwitcher[i].color = Color.Lerp(CloudColorKeySwitcher[i].color, DefaultCloudBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < FogColor.colorKeys.Length; i++)
					{
						FogColorKeySwitcher[i].color = Color.Lerp(FogColorKeySwitcher[i].color, DefaultFogBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientSkyLightColor.colorKeys.Length; i++)
					{
						AmbientSkyLightColorKeySwitcher[i].color = Color.Lerp(AmbientSkyLightColorKeySwitcher[i].color, DefaultAmbientSkyLightBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientEquatorLightColor.colorKeys.Length; i++)
					{
						AmbientEquatorLightColorKeySwitcher[i].color = Color.Lerp(AmbientEquatorLightColorKeySwitcher[i].color, DefaultAmbientEquatorLightBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < AmbientGroundLightColor.colorKeys.Length; i++)
					{
						AmbientGroundLightColorKeySwitcher[i].color = Color.Lerp(AmbientGroundLightColorKeySwitcher[i].color, DefaultAmbientGroundLightBaseColor.colorKeys[i].color, CurrentValue);
					}

					for (int i = 0; i < SunColor.colorKeys.Length; i++)
					{
						SunLightColorKeySwitcher[i].color = Color.Lerp(SunLightColorKeySwitcher[i].color, DefaultSunLightBaseColor.colorKeys[i].color, CurrentValue);
					}

					FogColor.SetKeys(FogColorKeySwitcher, FogColor.alphaKeys);
					CloudBaseColor.SetKeys(CloudColorKeySwitcher, CloudBaseColor.alphaKeys);
					AmbientSkyLightColor.SetKeys(AmbientSkyLightColorKeySwitcher, AmbientSkyLightColor.alphaKeys);
					AmbientEquatorLightColor.SetKeys(AmbientEquatorLightColorKeySwitcher, AmbientEquatorLightColor.alphaKeys);
					AmbientGroundLightColor.SetKeys(AmbientGroundLightColorKeySwitcher, AmbientGroundLightColor.alphaKeys);
					SunColor.SetKeys(SunLightColorKeySwitcher, SunColor.alphaKeys);
				}

				yield return null;
			}
		}
	
	
		public void TransitionWeather(FadeSequence FS)
		{
			//OnWeatherChangeEvent.Invoke(); //Invoke our weather change event
			if (CloudCoroutine != null) { StopCoroutine(CloudCoroutine); }
			if (FogCoroutine != null) { StopCoroutine(FogCoroutine); }
			
			if (SunCoroutine != null) { StopCoroutine(SunCoroutine); }
			if (MoonCoroutine != null) { StopCoroutine(MoonCoroutine); }
			if (WindCoroutine != null) { StopCoroutine(WindCoroutine); }
	
			if (LightningCloudsCoroutine != null) { StopCoroutine(LightningCloudsCoroutine); }
			if (ColorCoroutine != null) { StopCoroutine(ColorCoroutine); }
			if (CloudHeightCoroutine != null) { StopCoroutine(CloudHeightCoroutine); }
			if (RainShaderCoroutine != null) { StopCoroutine(RainShaderCoroutine); }
			if (SnowShaderCoroutine != null) { StopCoroutine(SnowShaderCoroutine); }
			
			currentWeatherType = FS.currentWeatherType;

			//Reset our time of day sounds timer so it doesn't play right after a weather change
			m_TimeOfDaySoundsTimer = 0;

			//Get randomized cloud amount based on cloud level from weather type.
			if (currentWeatherType.CloudLevel != WeatherType.CloudLevelEnum.DontChange)
			{
				m_ReceivedCloudValue = GetCloudLevel(m_ReceivedCloudValue);
			}

			//Clouds
			if (m_CloudDomeMaterial.GetFloat("_CloudCover") < m_ReceivedCloudValue)
			{
				CloudCoroutine = StartCoroutine(FS.CloudFadeSequence(TransitionSpeed, m_ReceivedCloudValue));
			}
			else
			{
				CloudCoroutine = StartCoroutine(FS.CloudFadeSequence(-TransitionSpeed, m_ReceivedCloudValue));
			}

			//Wind
			if (CurrentWindIntensity < currentWeatherType.WindIntensity)
			{
				WindCoroutine = StartCoroutine(FS.WindFadeSequence(TransitionSpeed / 8, 0, currentWeatherType.WindIntensity));
			}
			else
			{
				WindCoroutine = StartCoroutine(FS.WindFadeSequence(-TransitionSpeed / 8, currentWeatherType.WindIntensity, 1));
			}

			//Fog
			if (RenderSettings.fogDensity < currentWeatherType.FogDensity)
			{
				FogCoroutine = StartCoroutine(FS.FogFadeSequence(TransitionSpeed * 350 / currentWeatherType.FogSpeedMultiplier, currentWeatherType.FogDensity, currentWeatherType.FogDensity,CurrentFogAmount));
			}
			else
			{
				FogCoroutine = StartCoroutine(FS.FogFadeSequence(-TransitionSpeed * 10, currentWeatherType.FogDensity, currentWeatherType.FogDensity,CurrentFogAmount));
			}


			if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{   // currentWeatherType.SunIntensity SO中的数值, SunIntensity 暂存变量，用于计算
				SunCoroutine = StartCoroutine(FS.SunFadeSequence(TransitionSpeed, currentWeatherType.SunIntensity,SunIntensity));
				MoonCoroutine = StartCoroutine(FS.MoonFadeSequence(TransitionSpeed, currentWeatherType.MoonIntensity,MoonIntensity));
				
				ColorCoroutine = StartCoroutine(ColorFadeSequence(TransitionSpeed * 20, 0, 1));
				CloudHeightCoroutine = StartCoroutine(FS.CloudHeightFadeSequence(TransitionSpeed / 2, 0, 2500));

				if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.Rain)
				{
					RainShaderCoroutine = StartCoroutine(FS.RainShaderFadeInSequence(TransitionSpeed / 2, 1));
					SnowShaderCoroutine = StartCoroutine(FS.SnowShaderFadeOutSequence(-TransitionSpeed, 0));
				}
				else if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.Snow)
				{
					SnowShaderCoroutine = StartCoroutine(FS.SnowShaderFadeInSequence(TransitionSpeed * 2, 1f));
					RainShaderCoroutine = StartCoroutine(FS.RainShaderFadeOutSequence(-TransitionSpeed / 2, 0));
				}
				else if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.None)
				{
					SnowShaderCoroutine = StartCoroutine(FS.SnowShaderFadeOutSequence(-TransitionSpeed, 0));
					RainShaderCoroutine = StartCoroutine(FS.RainShaderFadeOutSequence(-TransitionSpeed / 2, 0));
				}
			}
			else if (currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.No)
			{
				SunCoroutine = StartCoroutine(FS.SunFadeSequence(TransitionSpeed / 1.75f, currentWeatherType.SunIntensity,SunIntensity));
				MoonCoroutine = StartCoroutine(FS.MoonFadeSequence(TransitionSpeed, currentWeatherType.MoonIntensity,MoonIntensity));
				// 暂时将Color渐变 放在CelestialManager里
				ColorCoroutine = StartCoroutine(ColorFadeSequence(TransitionSpeed * 100, 0, 1));

				if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.None)
				{
					SnowShaderCoroutine = StartCoroutine(FS.SnowShaderFadeOutSequence(-TransitionSpeed, 0));
					RainShaderCoroutine = StartCoroutine(FS.RainShaderFadeOutSequence(-TransitionSpeed / 2, 0));
				}

				if (currentWeatherType.CloudLevel != WeatherType.CloudLevelEnum.Cloudy && currentWeatherType.CloudLevel != WeatherType.CloudLevelEnum.MostlyCloudy
					&& currentWeatherType.CloudLevel != WeatherType.CloudLevelEnum.DontChange)
				{
					if (m_CloudDomeMaterial.GetFloat("_WorldPosGlobal") > 500)
					{
						CloudHeightCoroutine = StartCoroutine(FS.CloudHeightFadeSequence(-TransitionSpeed / 3.5f, 0, 2500));
					}
					else if (m_CloudDomeMaterial.GetFloat("_WorldPosGlobal") <= 500)
					{
						CloudHeightCoroutine = StartCoroutine(FS.CloudHeightFadeSequence(-TransitionSpeed * 2, 0, 2500));
					}
				}
				else
				{
					if (m_CloudDomeMaterial.GetFloat("_WorldPosGlobal") > 500)
					{
						CloudHeightCoroutine = StartCoroutine(FS.CloudHeightFadeSequence(-TransitionSpeed / 3.5f, 500, 500));
					}
					else if (m_CloudDomeMaterial.GetFloat("_WorldPosGlobal") < 500)
					{
						CloudHeightCoroutine = StartCoroutine(FS.CloudHeightFadeSequence(TransitionSpeed / 2, 0, 500));
					}
				}
			}

			if (currentWeatherType.UseLightning == WeatherType.WeatherSetting.Yes && currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
			{
				LightningCloudsCoroutine = StartCoroutine(FS.LightningCloudsFadeSequence(TransitionSpeed, 0, 0.5f));
			}
			else if (currentWeatherType.UseLightning == WeatherType.WeatherSetting.No)
			{
				LightningCloudsCoroutine = StartCoroutine(FS.LightningCloudsFadeSequence(-TransitionSpeed / 8, 0, 0.5f));
			}

			
		
		}
	}
}

