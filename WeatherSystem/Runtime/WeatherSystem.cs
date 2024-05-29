using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Wendy
{
	public class WeatherSystem : MonoBehaviour
	{
		public static WeatherSystem Instance = null;
		
		public AtmosphereSettings atmosphereSettings;
		
		//General Enums
		public enum EnableFeature
		{
			Enabled = 0, Disabled = 1
		}
		
		
		// System Events
		
		public UnityEvent OnHourChangeEvent;
		
		public UnityEvent OnDayChangeEvent;
		
		public UnityEvent OnMonthChangeEvent;
		
		public UnityEvent OnYearChangeEvent;
		
		public UnityEvent OnWeatherChangeEvent;

		//UI Elements
		
		//[BoxGroup("UI Elements", centerLabel: true)]
		public Slider TimeSlider;
		
		public GameObject WeatherButtonGameObject;
		
		public GameObject TimeSliderGameObject;
		
		public Dropdown WeatherDropdown;
		
		public EnableFeature useMenu = EnableFeature.Enabled;
		
		public KeyCode menuKey = KeyCode.Escape;
		
		public GameObject wendyCanvas;
		
		public bool m_MenuToggle = true;

		//Audio Mixer Volumes
		
		public float WeatherSoundsVolume = 1;
		
		public float AmbienceVolume = 1;
		
		public float MusicVolume = 1;
		
		public List<AudioSource> WeatherSoundsList = new List<AudioSource>();
		
		public UnityEngine.Audio.AudioMixer wendyAudioMixer;

		//Time
		
		public System.DateTime WendyDate;
		
		public int StartingMinute = 0;
		
		public int StartingHour = 0;
		
		public int Minute = 1;
		
		public int Hour = 0;
		
		public int Day = 0;
		
		public int Month = 0;
		
		public int Year = 0;
		
		public int DayLength = 10;
		
		public int NightLength = 10;
		
		public float m_TimeFloat;
		
		public EnableFeature TimeFlow = EnableFeature.Enabled;
		
		public EnableFeature RealWorldTime = EnableFeature.Disabled;
		
		float m_roundingCorrection;
		
		float m_PreciseCurveTime;
		
		public bool m_HourUpdate = false;
		float m_TimeOfDaySoundsTimer = 0;
		int m_TimeOfDaySoundsSeconds = 10;
		
		public int TimeOfDaySoundsSecondsMin = 10;
		
		public int TimeOfDaySoundsSecondsMax = 30;

		//Camera & Player
		
		public Transform PlayerTransform;
		
		public Camera PlayerCamera;
		
		public bool m_PlayerFound = false;
		
		public EnableFeature WendyFollowsPlayer = EnableFeature.Disabled;
		
		public EnableFeature GetPlayerAtRuntime = EnableFeature.Disabled;
		
		public EnableFeature UseRuntimeDelay = EnableFeature.Disabled;
		
		public GetPlayerMethodEnum GetPlayerMethod = GetPlayerMethodEnum.ByTag;
		
		public enum GetPlayerMethodEnum {ByTag, ByName};
		
		public string PlayerTag = "Player";
		
		public string PlayerName = "Player";
		
		public string CameraTag = "MainCamera";
		
		public string CameraName = "MainCamera";

		[HideInInspector]
		public int TimeOfDayMusicDelay = 1;
		float m_CurrentMusicClipLength = 0;
		float m_TimeOfDayMusicTimer = 0;
		[HideInInspector]
		public EnableFeature TimeOfDaySoundsDuringPrecipitationWeather = EnableFeature.Disabled;
		float m_CurrentClipLength = 0;
		int m_LastHour;

		public CurrentTimeOfDayEnum CurrentTimeOfDay;
		public enum CurrentTimeOfDayEnum
		{
			Morning = 0, Day, Evening, Night
		}

		public WeatherGenerationMethodEnum WeatherGenerationMethod = WeatherGenerationMethodEnum.Daily;
		public List<WeatherType> WeatherForecast = new List<WeatherType>();
		public enum WeatherGenerationMethodEnum
		{
			Hourly = 0, Daily = 1
		}
		
		
		//Weather
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
		
		Material m_LightningFlashMaterial;

		
		public LayerMask DetectionLayerMask;
		
		public List<string> LightningFireTags = new List<string>();
		
		public float LightningLightIntensityMin = 1;
		
		public float LightningLightIntensityMax = 3;
			   
		public int LightningGenerationDistance = 100;
		
		public int LightningDetectionDistance = 20;

		public float CurrentFogAmount;

		[HideInInspector]
		public GameObject m_SoundTransform;
		[HideInInspector]
		public GameObject m_EffectsTransform;

		
		//Weather Types
		public List<WeatherType> weatherTypesList = new List<WeatherType>();
		
		public WeatherType currentWeatherType;
		
		public WeatherType nextWeatherType;
		
		public List<WeatherType> NonPrecipiationWeatherTypes = new List<WeatherType>();
		
		public List<WeatherType> PrecipiationWeatherTypes = new List<WeatherType>();
		
		public AnimationCurve PrecipitationGraph = AnimationCurve.Linear(1, 0, 13, 100);
		 
		public int CloudSpeed = 8;
		
		
		[HideInInspector]
		public List<ParticleSystem> ParticleSystemList = new List<ParticleSystem>();
		//[HideInInspector]
		public List<ParticleSystem> WeatherEffectsList = new List<ParticleSystem>();
		
		public List<ParticleSystem> AdditionalParticleSystemList = new List<ParticleSystem>();
		
		public List<ParticleSystem> AdditionalWeatherEffectsList = new List<ParticleSystem>();
		
		//Celestial
		Renderer m_CloudDomeRenderer;
		Renderer m_CloudDomeLightningRenderer;
		Material m_CloudDomeMaterial;
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

		//Colors
		
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
		float m_ReceivedCloudValue;

		
		public Gradient DefaultCloudBaseColor;
		GradientColorKey[] CloudColorKeySwitcher;

		
		public Gradient DefaultFogBaseColor;
		GradientColorKey[] FogColorKeySwitcher;

		
		public Gradient DefaultAmbientSkyLightBaseColor;
		GradientColorKey[] AmbientSkyLightColorKeySwitcher;

		
		public Gradient DefaultAmbientEquatorLightBaseColor;
		GradientColorKey[] AmbientEquatorLightColorKeySwitcher;

		
		public Gradient DefaultAmbientGroundLightBaseColor;
		GradientColorKey[] AmbientGroundLightColorKeySwitcher;

		
		public Gradient DefaultSunLightBaseColor;
		GradientColorKey[] SunLightColorKeySwitcher;
		
		//temp
		WeatherType tempWeatherType;
		[HideInInspector]
		public int Temperature;
		[HideInInspector]
		public CurrentSeasonEnum CurrentSeason;
		public enum CurrentSeasonEnum
		{
			Spring = 1, Summer = 2, Fall = 3, Winter = 4
		}

		//Temperature
		[HideInInspector]
		public TemperatureTypeEnum TemperatureType = TemperatureTypeEnum.Fahrenheit;
		public enum TemperatureTypeEnum
		{
			Fahrenheit, Celsius
		}
		[HideInInspector]
		public int m_CloudSeed;
		[HideInInspector]
		public bool WendyInitialized = false;
		//[HideInInspector]
		public ParticleSystem CurrentParticleSystem;
		[HideInInspector]
		public ParticleSystem AdditionalCurrentParticleSystem;
		int m_FreezingTemperature;
		[HideInInspector]
		public int m_PrecipitationOdds = 50;
		float m_CurrentPrecipitationAmountFloat = 1;
		int m_CurrentPrecipitationAmountInt = 1;
		[HideInInspector]
		public static bool m_IsFading;
		[HideInInspector]
		public int TransitionSpeed = 45;
		[HideInInspector]
		public int HourToChangeWeather;
		int m_GeneratedOdds;
		bool m_WeatherGenerated = false;
		Coroutine CloudCoroutine, FogCoroutine, WeatherEffectCoroutine, AdditionalWeatherEffectCoroutine, ParticleFadeCoroutine;
		Coroutine AdditionalParticleFadeCoroutine, SunCoroutine, MoonCoroutine, WindCoroutine, SoundInCoroutine, SoundOutCoroutine;
		Coroutine LightningCloudsCoroutine, ColorCoroutine, CloudHeightCoroutine, RainShaderCoroutine, SnowShaderCoroutine;
		
		public TimeManager timeManager;
		public CelestialManager celestialManager;
		public ResourceManager resourceManager;

		
		
		void Awake()
		{
			GameObject m_WWManager = new GameObject();
			m_WWManager.transform.SetParent(this.transform);
			m_WWManager.AddComponent<WeatherManager>();
			m_WWManager.name = "Wendy Weather Manager";
			Instance = this;
		}

		// Start is called before the first frame update
		void Start()
		{
			// 如果找到玩家，可以选择延迟初始化和立即初始化两种选项
			if (GetPlayerAtRuntime == EnableFeature.Enabled)
			{
				//Make sure our PlayerTransform is null because we will be looking it up via Unity tag or by name.
				PlayerTransform = null;

				//If our player is being received at runtime, wait to intilialize Wendy until the player has been found.
				if (UseRuntimeDelay == EnableFeature.Enabled)
				{
					StartCoroutine(InitializeDelay());
				}
				//If our player is being received at runtime and UseRuntimeDelay is disabled, get our player immediately by tag.
				else if (UseRuntimeDelay == EnableFeature.Disabled)
				{
					if (GetPlayerMethod == GetPlayerMethodEnum.ByTag)
					{
						PlayerTransform = GameObject.FindWithTag(PlayerTag).transform;
						PlayerCamera = GameObject.FindWithTag(CameraTag).GetComponent<Camera>();
					}
					else if (GetPlayerMethod == GetPlayerMethodEnum.ByName)
					{
						PlayerTransform = GameObject.Find(PlayerName).transform;
						PlayerCamera = GameObject.Find(CameraName).GetComponent<Camera>();
					}
					InitializeWeatherSystem();
				}
			}
			//If our player is not being received at runtime, initialize Wendy immediately.
			else if (GetPlayerAtRuntime == EnableFeature.Disabled)
			{
				InitializeWeatherSystem();
			}
		}

		// Update is called once per frame
		void Update()
		{
			//Only run Wendy if it has been initialized.
			if (WendyInitialized)
			{
				if (useMenu == EnableFeature.Enabled)
				{
					//Some versions of Unity cannot have the Canvas disabled without causing issues with dropdown menus.
					//So, disable the button and slider gameobjects then move the dropdown menu up 300 units so it is no longer visible. 
					//Revese everything when the menu is enabled again.
					if (Input.GetKeyDown(menuKey))
					{
						ToggleWendyMenu();
					}
				}
				/*
				//Only calculate our time if TimeFlow is enabled
				if (TimeFlow == WeatherSystem.EnableFeature.Enabled)
				{
					if (RealWorldTime == WeatherSystem.EnableFeature.Disabled)
					{
						// 开启时间流动 + 自定义时间
						if (Hour > 6 && Hour <= 18)
						{
							m_TimeFloat = m_TimeFloat + Time.deltaTime / DayLength / 120;
						}

						if (Hour > 18 || Hour <= 6)
						{
							m_TimeFloat = m_TimeFloat + Time.deltaTime / NightLength / 120;
						}
					}
					else if (RealWorldTime == WeatherSystem.EnableFeature.Enabled)
					{
						// 开启时间流动 + 现实时间(h)
						m_TimeFloat = (float)System.DateTime.Now.Hour / 24 + (float)System.DateTime.Now.Minute / 1440;
					}

					if (m_TimeFloat >= 1.0f)
					{
						m_TimeFloat = 0;
						CalculateDays();
					}
				}

				//Calculate our time
				// 计算当前精准时间
				float m_HourFloat = m_TimeFloat * 24;
				Hour = (int)m_HourFloat;
				float m_MinuteFloat = m_HourFloat * 60;
				Minute = (int)m_MinuteFloat % 60;

				// 如果启用了Menu并且打开，设置Slider的值
				if (useMenu == EnableFeature.Enabled && !m_MenuToggle)
				{
					if (wendyCanvas != null && wendyCanvas.activeSelf)
					{
						TimeSlider.value = m_TimeFloat;
					}
				}

				//Update all hourly related settings
				if (m_LastHour != Hour)
				{
					m_LastHour = Hour;
					HourlyUpdate();
				}*/
				// 移动太阳和月亮(两者方向相反)
				//MoveSun();

				//UpdateColors();
				//TODO: remove comment
				//PlayTimeOfDaySound();
				//PlayTimeOfDayMusic();
				//CalculateTimeOfDay();

				//Generate our lightning, if the randomized lightning seconds have been met
				/*if (currentWeatherType.UseLightning == WeatherType.WeatherSetting.Yes && currentWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
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
				}*/

				//Update our FollowPlayer function
				if (WendyFollowsPlayer == WeatherSystem.EnableFeature.Enabled)
				{
					FollowPlayer();
				}

			}
			else if (GetPlayerAtRuntime == WeatherSystem.EnableFeature.Enabled && !WendyInitialized)
			{
				//Continue to look for our player until it's found. Once it is, Wendy can be initialized.
				try
				{
					PlayerTransform = GameObject.FindWithTag(PlayerTag).transform;
					m_PlayerFound = true;
				}
				catch
				{
					m_PlayerFound = false;
				}

			}
		}

		//If follow player is enabled, adjust the distant Wendy components to the player's position
		void FollowPlayer ()
		{
			m_CloudDomeRenderer.transform.position = new Vector3(PlayerTransform.position.x, PlayerTransform.position.y - 550, PlayerTransform.position.z);
			m_MoonLight.transform.position = PlayerTransform.position;
			m_StarsRenderer.transform.position = PlayerTransform.position;
		}
		
		
		void InitializeWeatherSystem()
		{
			// 1. 【Preparation】: 延迟初始化，直到找到Player
			StopCoroutine(InitializeDelay());
			
			if (PlayerTransform == null || PlayerCamera == null)
			{
				Debug.LogWarning("(Wendy has been disabled) - No player/camera has been assigned on the Player Transform/Player Camera slot." +
					"Please go to the Player & Camera tab and assign one.");
				GetComponent<WeatherSystem>().enabled = false;
			}
			else if (!PlayerTransform.gameObject.activeSelf || !PlayerCamera.gameObject.activeSelf)
			{
				Debug.LogWarning("(Wendy has been disabled) - The player/camera game object is disabled on the Player Transform/Player Camera slot is disabled. " +
					"Please go to the Player & Camera tab and ensure your player/camera is enabled.");
				GetComponent<WeatherSystem>().enabled = false;
			}

			//If our current weather type is not a part of the available weather type lists, assign it to the proper category.
			/*if (!weatherTypesList.Contains(currentWeatherType))
			{
				weatherTypesList.Add(currentWeatherType);
			}*/

			//Setup the proper settings for our camera
			PlayerCamera.farClipPlane = 16000;


			// 2. Audio Settings
			/*if (MusicVolume == 0)
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
			// 如果声音和粒子特效的资产找不到，则将当前天气的相关特性禁用或重新生成
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
			}*/
			// temporary time param initialization
			timeManager = GameObject.Find("Wendy Time Manager").GetComponent<TimeManager>();
			timeManager.Initialize();
			
			celestialManager = GameObject.Find("Wendy Celestial Manager").GetComponent<CelestialManager>();
			celestialManager.Initialize();
			
			resourceManager = GameObject.Find("Wendy Resource Manager").GetComponent<ResourceManager>();
			resourceManager.Initialize();
			// 5. 暴雨天气与普通天气的颜色渐变设置
			/*CloudColorKeySwitcher = new GradientColorKey[8];
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
			DefaultSunLightBaseColor.colorKeys = SunColor.colorKeys;*/

			// 6. 开始计算天气

			// 计算降水
			//CalculatePrecipiation();

			// 添加太阳月亮
			//CreateSun();// Find sun light transform and set RenderingSettings.sun,skybox 
			//CreateMoon();// Create Multi-Phase Moon Object accoording to moon light, set material and transform

			//Intialize our other components and set the proper settings from within the editor
			/*
			GameObject TempAudioSource = new GameObject("Wendy Time of Day Sounds");
			TempAudioSource.transform.SetParent(this.transform);
			TempAudioSource.transform.localPosition = Vector3.zero;
			TempAudioSource.AddComponent<AudioSource>();
			TimeOfDayAudioSource = TempAudioSource.GetComponent<AudioSource>();
			TimeOfDayAudioSource.outputAudioMixerGroup = wendyAudioMixer.FindMatchingGroups("Master/Ambience")[0];
			m_TimeOfDaySoundsSeconds = Random.Range(TimeOfDaySoundsSecondsMin, TimeOfDaySoundsSecondsMax+1);

			GameObject TempAudioSourceMusic = new GameObject("Wendy Time of Day Music");
			TempAudioSourceMusic.transform.SetParent(this.transform);
			TempAudioSourceMusic.transform.localPosition = Vector3.zero;
			TempAudioSourceMusic.AddComponent<AudioSource>();
			TimeOfDayMusicAudioSource = TempAudioSourceMusic.GetComponent<AudioSource>();
			TimeOfDayMusicAudioSource.outputAudioMixerGroup = wendyAudioMixer.FindMatchingGroups("Master/Music")[0];*/

			//WendyWindZone = GameObject.Find("Wendy WindZone").GetComponent<WindZone>();

			// 找到云朵和星星的Renderer并初始化材质的属性
			/*m_StarsRenderer = GameObject.Find("Wendy Star").GetComponent<Renderer>();
			m_StarsMaterial = m_StarsRenderer.material;
			m_StarsMaterial.SetFloat("_StarSpeed", StarSpeed);

			m_CloudDomeRenderer = GameObject.Find("Wendy Cloud").GetComponent<Renderer>();
			m_CloudDomeMaterial = m_CloudDomeRenderer.material;
			m_CloudDomeMaterial.SetVector("_CloudSpeed", new Vector4(CloudSpeed * 0.0001f, 0, 0, 0));

			m_CloudDomeLightningRenderer = GameObject.Find("Wendy Cloud Lightning").GetComponent<Renderer>();
			m_LightningFlashMaterial = m_CloudDomeLightningRenderer.material;

			*/
			
			//TODO:1
			/*//Calculates our start time based off the user's input
			// 计算当前天数
			float StartingMinuteFloat = (int)Minute;
			if (RealWorldTime == WeatherSystem.EnableFeature.Disabled)
			{
				m_TimeFloat = (float)Hour / 24 + StartingMinuteFloat / 1440;
			}
			else if (RealWorldTime == WeatherSystem.EnableFeature.Enabled)
			{
				m_TimeFloat = (float)System.DateTime.Now.Hour / 24 + (float)System.DateTime.Now.Minute / 1440;
			}

			m_LastHour = Hour;*/

			// 根据当天的时间初始化 太阳月亮强度，并设置天空盒和基本属性
			/*m_SunLight.intensity = SunIntensityCurve.Evaluate((float)Hour) * SunIntensity;
			m_MoonLight.intensity = MoonIntensityCurve.Evaluate((float)Hour) * MoonIntensity * MoonPhaseIntensity;

			m_SkyBoxMaterial = (Material)Resources.Load("Materials/Skybox") as Material;
			RenderSettings.skybox = m_SkyBoxMaterial;
			m_SkyBoxMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness.Evaluate((float)Hour));
			m_SkyBoxMaterial.SetColor("_NightSkyTint", SkyTintColor.Evaluate((float)Hour));*/
			/*TODO: temperature
			Temperature = (int)TemperatureCurve.Evaluate(m_PreciseCurveTime) + (int)TemperatureFluctuation.Evaluate((float)StartingHour);

			if (TemperatureType == TemperatureTypeEnum.Fahrenheit)
			{
				m_FreezingTemperature = 32;
			}
			else if (TemperatureType == TemperatureTypeEnum.Celsius)
			{
				m_FreezingTemperature = 0;
			}*/

			transform.position = new Vector3(PlayerTransform.position.x, transform.position.y, PlayerTransform.position.z);
			/*
			// 预测天气(以h或d为单位随机天气，根据温度之类的信息判断nextWeather的可能性)
			GenerateWeather();

			// 创建闪电系统，添加Lightning Light,Audio
			CreateLightning();

			// 更新颜色和光强：sun,moon,skybox,cloud,ambient,fog
			UpdateColors();

			// 随机初始化云朵流速和Noise Seed
			CalculateClouds();

			// 月亮分成多个阶段存储在List里，计算当前Phase的结果
			CalculateMoonPhase();
			*/
			// 从当前的天气SO中读取参数并初始化场景中已经实例化好的物体,SO->Effect,Audio,Material etc.
			InitializeWeather(true);

			// 根据具体时间 计算 抽象时间(早，中，晚等概括性表达)
			CalculateTimeOfDay();
			//CalculateSeason();

			if (WeatherGenerationMethod == WeatherGenerationMethodEnum.Hourly)
			{
				WeatherForecast[Hour] = currentWeatherType;
			}

			//Only create  UI if it is enabled
			// 开启菜单，提前创建加载好
			if (useMenu == WeatherSystem.EnableFeature.Enabled)
			{
				CreateWendyMenu();
			}

			WendyInitialized = true;
		}
		
		//直到找到Player才开始初始化
		IEnumerator InitializeDelay()
		{
			yield return new WaitWhile(() => PlayerTransform == null);
			if (GetPlayerMethod == GetPlayerMethodEnum.ByTag)
			{
				PlayerTransform = GameObject.FindWithTag(PlayerTag).transform;
				PlayerCamera = GameObject.FindWithTag(CameraTag).GetComponent<Camera>();
			}
			else if (GetPlayerMethod == GetPlayerMethodEnum.ByName)
			{
				PlayerTransform = GameObject.Find(PlayerName).transform;
				PlayerCamera = GameObject.Find(CameraName).GetComponent<Camera>();
			}
			InitializeWeatherSystem();
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
		
		//Calculate all of our hourly updates
		void HourlyUpdate ()
		{
			/*TODO: Update Hourly
			OnHourChangeEvent.Invoke();

			Temperature = (int)TemperatureCurve.Evaluate(m_PreciseCurveTime) + (int)TemperatureFluctuation.Evaluate((float)Hour);

			if (WeatherGenerationMethod == WeatherGenerationMethodEnum.Hourly)
			{
				if (Hour < 23)
				{
					currentWeatherType = WeatherForecast[Hour];
					nextWeatherType = WeatherForecast[Hour + 1];
				}
				else
				{
					currentWeatherType = WeatherForecast[Hour];
					nextWeatherType = WeatherForecast[0];
				}
			}

			CheckWeather();

			//If the hour is equal to 12, update our moon phase.
			if (Hour == 12)
			{
				MoonPhaseIndex++;
				CalculateMoonPhase();
			}*/
		}
		
		void CalculateTimeOfDay()
		{
			if (Hour >= 6 && Hour <= 7)
			{
				CurrentTimeOfDay = CurrentTimeOfDayEnum.Morning;
			}
			else if (Hour >= 8 && Hour <= 16)
			{
				CurrentTimeOfDay = CurrentTimeOfDayEnum.Day;
			}
			else if (Hour >= 17 && Hour <= 18)
			{
				CurrentTimeOfDay = CurrentTimeOfDayEnum.Evening;
			}
			else if (Hour >= 19 && Hour <= 23 || Hour >= 0 && Hour <= 5)
			{
				CurrentTimeOfDay = CurrentTimeOfDayEnum.Night;
			}
		}
	
		//Calculate our precipitation odds based on the Wendy date
		void CalculatePrecipiation()
		{
			CalculateMonths(); //Calculate our months
			GetDate(); //Calculate our Wendy date

			//This algorithm uses an Animation curve to calculate the precipitation odds given the date from the Animation Curve
			m_roundingCorrection = WendyDate.DayOfYear * 0.00273972602f;
			m_PreciseCurveTime = ((WendyDate.DayOfYear / 28.07692307692308f)) + 1 - m_roundingCorrection;
			m_PreciseCurveTime = Mathf.Round(m_PreciseCurveTime * 10f) / 10f;

			m_CurrentPrecipitationAmountFloat = PrecipitationGraph.Evaluate(m_PreciseCurveTime);
			m_CurrentPrecipitationAmountInt = (int)Mathf.Round(m_CurrentPrecipitationAmountFloat);
			m_PrecipitationOdds = m_CurrentPrecipitationAmountInt;
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

		//Sets Up Sun Light
		void CreateSun()
		{
			m_SunLight = GameObject.Find("Sun Light").GetComponent<Light>();
			m_SunLight.transform.localEulerAngles = new Vector3(0, SunAngle, 0);
			m_CelestialAxisTransform = GameObject.Find("Celestial").transform;
			RenderSettings.sun = m_SunLight;
			m_SkyBoxMaterial = RenderSettings.skybox;
		}
		
		//Generate our weather according to the precipitation odds for the current time of year.
		//Check the weather type's conditions, if they are not met, reroll weather within the same category.
		public void GenerateWeather()
		{
			if (WeatherGenerationMethod == WeatherGenerationMethodEnum.Daily)
			{
				m_GeneratedOdds = UnityEngine.Random.Range(1, 101);
				HourToChangeWeather = UnityEngine.Random.Range(0, 23);

				if (HourToChangeWeather == Hour)
				{
					HourToChangeWeather = Hour - 1;
				}

				CheckGeneratedWeather();
			}
			else if (WeatherGenerationMethod == WeatherGenerationMethodEnum.Hourly)
			{
				for (int i = 0; i < 24; i++)
				{
					m_GeneratedOdds = UnityEngine.Random.Range(1, 101);
					CheckGeneratedWeather();
				}
			}
		}

		//Check our generated weather for seasonal and temperature conditions. 
		//Reroll the weather if they are not met until an appropriate weather type in the same category is found.
		void CheckGeneratedWeather()
		{
			/*
			if (m_GeneratedOdds <= m_PrecipitationOdds && PrecipiationWeatherTypes.Count != 0)
			{
				tempWeatherType = PrecipiationWeatherTypes[Random.Range(0, PrecipiationWeatherTypes.Count)];
			}
			else if (m_GeneratedOdds > m_PrecipitationOdds && NonPrecipiationWeatherTypes.Count != 0)
			{
				tempWeatherType = NonPrecipiationWeatherTypes[Random.Range(0, NonPrecipiationWeatherTypes.Count)];
			}

			while (tempWeatherType.TemperatureType == WeatherType.TemperatureTypeEnum.AboveFreezing && Temperature <= m_FreezingTemperature
				|| tempWeatherType.Season != WeatherType.SeasonEnum.All && (int)tempWeatherType.Season != (int)CurrentSeason
			|| tempWeatherType.TemperatureType == WeatherType.TemperatureTypeEnum.BelowFreezing && Temperature > m_FreezingTemperature
			|| tempWeatherType.SpecialWeatherType == WeatherType.WeatherSetting.Yes)
			{
				if (tempWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.No)
				{
					tempWeatherType = NonPrecipiationWeatherTypes[Random.Range(0, NonPrecipiationWeatherTypes.Count)];
				}
				else if (tempWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
				{
					tempWeatherType = PrecipiationWeatherTypes[Random.Range(0, PrecipiationWeatherTypes.Count)];
				}
				else
				{
					break;
				}
			}

			if(WeatherGenerationMethod == WeatherGenerationMethodEnum.Daily)
			{
				NextWeatherType = tempWeatherType;
			}
			else if (WeatherGenerationMethod == WeatherGenerationMethodEnum.Hourly)
			{
				WeatherForecast.Add(tempWeatherType);
			}
			m_WeatherGenerated = true;*/
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
			m_WendyLightningSystem.PlayerTransform = PlayerTransform;
			m_WendyLightningSystem.LightningGenerationDistance = LightningGenerationDistance;
			m_LightningSeconds = Random.Range(LightningSecondsMin, LightningSecondsMax);
			m_WendyLightningSystem.LightningLightIntensityMin = LightningLightIntensityMin;
			m_WendyLightningSystem.LightningLightIntensityMax = LightningLightIntensityMax;
		}
		
		//Continuously update our colors based on the time of day
		void UpdateColors()
		{
			m_SunLight.color = SunColor.Evaluate(m_TimeFloat);
			m_MoonLight.color = MoonColor.Evaluate(m_TimeFloat);
			m_StarsMaterial.color = StarLightColor.Evaluate(m_TimeFloat);
			m_SkyBoxMaterial.SetColor("_SkyTint", SkyColor.Evaluate(m_TimeFloat));
			m_SkyBoxMaterial.SetFloat("_AtmosphereThickness", AtmosphereThickness.Evaluate(m_TimeFloat*24));
			m_SkyBoxMaterial.SetColor("_NightSkyTint", SkyTintColor.Evaluate(m_TimeFloat));
			m_CloudDomeMaterial.SetColor("_LightColor", CloudLightColor.Evaluate(m_TimeFloat));
			m_CloudDomeMaterial.SetColor("_BaseColor", CloudBaseColor.Evaluate(m_TimeFloat));
			m_CloudDomeMaterial.SetFloat("_Attenuation", m_SunLight.intensity + m_MoonLight.intensity);
			RenderSettings.ambientSkyColor = AmbientSkyLightColor.Evaluate(m_TimeFloat);
			RenderSettings.ambientEquatorColor = AmbientEquatorLightColor.Evaluate(m_TimeFloat);
			RenderSettings.ambientGroundColor = AmbientGroundLightColor.Evaluate(m_TimeFloat);
			RenderSettings.fogColor = FogColor.Evaluate(m_TimeFloat);
			CurrentFogColor = FogColor.Evaluate(m_TimeFloat);
			m_SunLight.intensity = SunIntensityCurve.Evaluate(m_TimeFloat * 24) * SunIntensity;
			m_SkyBoxMaterial.SetFloat("_SunSize", SunSize.Evaluate(m_TimeFloat * 24) * 0.01f);
			m_MoonLight.intensity = MoonIntensityCurve.Evaluate(m_TimeFloat * 24) * MoonIntensity * MoonPhaseIntensity;
			m_MoonTransform.localScale = MoonSize.Evaluate(m_TimeFloat * 24) * m_MoonStartingSize;
		}
		
		//Calculates our days and updates our Animation curves.
		void CalculateDays()
		{
			CalculatePrecipiation();
			//TODO: Temperature influence day time
			//TemperatureCurve.Evaluate(m_PreciseCurveTime);

			Day++; //Add a day to our Day variable
			CalculateMonths(); //Calculate our months
			CalculateSeason(); //Calculate our seasons
			OnDayChangeEvent.Invoke(); //Invoke our day events
			GetDate(); //Calculate the DateTime

			//Clears our hourly forecast and generates a new one for the current day
			if (WeatherGenerationMethod == WeatherSystem.WeatherGenerationMethodEnum.Hourly)
			{
				WeatherForecast.Clear();
				GenerateWeather();
			}
		}

		//Calculates our months for an accurate calendar that also calculates leap year
		void CalculateMonths()
		{
			//Calculates our days into months
			if (Day >= 32 && Month == 1 || Day >= 32 && Month == 3 || Day >= 32 && Month == 5 || Day >= 32 && Month == 7
				|| Day >= 32 && Month == 8 || Day >= 32 && Month == 10 || Day >= 32 && Month == 12) {
				Day = Day % 32;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			if (Day >= 31 && Month == 4 || Day >= 31 && Month == 6 || Day >= 31 && Month == 9 || Day >= 31 && Month == 11) {
				Day = Day % 31;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			//Calculates Leap Year
			if (Day >= 30 && Month == 2 && (Year % 4 == 0 && Year % 100 != 0) || (Year % 400 == 0)) {
				Day = Day % 30;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			else if (Day >= 29 && Month == 2 && Year % 4 != 0) {
				Day = Day % 29;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			//Calculates our months into years
			if (Month > 12) {
				Month = Month % 13;
				Year += 1;
				Month += 1;
				OnYearChangeEvent.Invoke(); //Invoke our Year events

				//Reset our m_roundingCorrection variable to 0
				m_roundingCorrection = 0;
			}
		}
		
		//Generate and return a random cloud intensity based on the current weather type cloud level
		float GetCloudLevel(float MaxValue)
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
	
		//Generate a cloud seed for our clouds
		void CalculateClouds()
		{
			m_CloudSeed = Random.Range(-9999, 10000);
			m_CloudDomeMaterial.SetFloat("_Seed", m_CloudSeed);
		}
		
		//Used for controlling Wendy's time slider
		public void CalculateTimeSlider()
		{
			m_TimeFloat = TimeSlider.value;
		}

		
		//If enabled, create Weather System UI and Canvas.
		void CreateWendyMenu()
		{
			//Resource load UI here
			wendyCanvas = Instantiate((GameObject)Resources.Load("Prefabs/Wendy Canvas") as GameObject, transform.position, Quaternion.identity);
			wendyCanvas.name = "Wendy Canvas";

			TimeSlider = GameObject.Find("Time Slider").GetComponent<Slider>();
			TimeSliderGameObject = TimeSlider.gameObject;
			TimeSlider.onValueChanged.AddListener(delegate { CalculateTimeSlider(); }); //Create an event to control Wendy's time with a slider
			TimeSlider.maxValue = 0.995f;

			WeatherButtonGameObject = GameObject.Find("Change Weather Button");

			WeatherDropdown = GameObject.Find("Weather Dropdown").GetComponent<Dropdown>();
			GameObject.Find("Change Weather Button").GetComponent<Button>().onClick.AddListener(delegate { ChangeWeatherUI(); });

			List<string> m_DropOptions = new List<string> { };

			for (int i = 0; i < resourceManager.weatherTypesList.Count; i++)
			{
				m_DropOptions.Add(resourceManager.weatherTypesList[i].WeatherTypeName);
			}

			WeatherDropdown.AddOptions(m_DropOptions);
			TimeSlider.value = m_TimeFloat;

			WeatherDropdown.value = resourceManager.weatherTypesList.IndexOf(currentWeatherType);
			
			if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
			{
				GameObject m_EventSystem = new GameObject();
				m_EventSystem.name = "EventSystem";
				m_EventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
				m_EventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
			}

			m_MenuToggle = false;
			ToggleWendyMenu();
		}
		
		//A public function for Wendy's UI Menu to change the weather with a dropdown
		public void ChangeWeatherUI ()
		{	
			currentWeatherType = resourceManager.weatherTypesList[WeatherDropdown.value];
			
			// Get FadeSequence
			FadeSequence FS = gameObject.GetComponent<FadeSequence>();
			if(FS == null)
			{
				FS = gameObject.AddComponent<FadeSequence>();
				//FS.CurrentParticleSystem = resourceManager.CurrentParticleSystem;
				FS.m_CloudDomeMaterial = celestialManager.m_CloudDomeMaterial;
				FS.m_LightningFlashMaterial = celestialManager.m_LightningFlashMaterial;
				FS.AdditionalCurrentParticleSystem = resourceManager.AdditionalCurrentParticleSystem;
			}
			FS.currentWeatherType = currentWeatherType;
			celestialManager.TransitionWeather(FS);
			resourceManager.TransitionWeather(FS);
		}
		
		public void ToggleWendyMenu()
		{
			WeatherButtonGameObject.SetActive(m_MenuToggle);
			TimeSliderGameObject.SetActive(m_MenuToggle);
			m_MenuToggle = !m_MenuToggle;

			int m_AdjustedMenuHeight = 0;

			if (m_MenuToggle)
			{
				m_AdjustedMenuHeight = 300;
			}
			else
			{
				m_AdjustedMenuHeight = -300;
			}

			RectTransform U_Dropdown = GameObject.Find("Weather Dropdown").GetComponent<RectTransform>();
			Vector3 V3 = U_Dropdown.position;
			U_Dropdown.position = new Vector3(V3.x, V3.y + m_AdjustedMenuHeight, V3.z);
		}
		
		//Gets a custom DateTime using Wendy's current date
		public System.DateTime GetDate()
		{
			if (RealWorldTime == WeatherSystem.EnableFeature.Disabled)
			{
				WendyDate = new System.DateTime(Year, Month, Day, Hour, Minute, 0);
			}
			else if (RealWorldTime == WeatherSystem.EnableFeature.Enabled)
			{
				WendyDate = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, Hour, Minute, 0);
				Year = WendyDate.Year;
				Month = WendyDate.Month;
				Day = WendyDate.Day;
			}

			return WendyDate;
		}

		//Move our sun according to the time of day
		public void MoveSun()
		{
			m_CelestialAxisTransform.eulerAngles = new Vector3(m_TimeFloat * 360 - 100, SunRevolution, 180);
		}
		
		//Calculate our seasons based on either the Norhtern or Southern Hemisphere
		public void CalculateSeason()
		{
			/*
			if (Month == 3 && Day >= 20 || Month == 4 || Month == 5 || Month == 6 && Day <= 20)
			{
				if (Hemisphere == HemisphereEnum.Northern)
				{
					CurrentSeason = CurrentSeasonEnum.Spring;
				}
				else if (Hemisphere == HemisphereEnum.Southern)
				{
					CurrentSeason = CurrentSeasonEnum.Fall;
				}
			}
			else if (Month == 6 && Day >= 21 || Month == 7 || Month == 8 || Month == 9 && Day <= 21)
			{
				if (Hemisphere == HemisphereEnum.Northern)
				{
					CurrentSeason = CurrentSeasonEnum.Summer;
				}
				else if (Hemisphere == HemisphereEnum.Southern)
				{
					CurrentSeason = CurrentSeasonEnum.Winter;
				}
			}
			else if (Month == 9 && Day >= 22 || Month == 10 || Month == 11 || Month == 12 && Day <= 20)
			{
				if (Hemisphere == HemisphereEnum.Northern)
				{
					CurrentSeason = CurrentSeasonEnum.Fall;
				}
				else if (Hemisphere == HemisphereEnum.Southern)
				{
					CurrentSeason = CurrentSeasonEnum.Spring;
				}
			}
			else if (Month == 12 && Day >= 21 || Month == 1 || Month == 2 || Month == 3 && Day <= 19)
			{
				if (Hemisphere == HemisphereEnum.Northern)
				{
					CurrentSeason = CurrentSeasonEnum.Winter;
				}
				else if (Hemisphere == HemisphereEnum.Southern)
				{
					CurrentSeason = CurrentSeasonEnum.Summer;
				}
			}*/
		}
		
		
		//Initialize our starting weather so it fades in instantly on start
		public void InitializeWeather(bool UseWeatherConditions)
		{
			//If our starting weather type's conditions are not met, keep rerolling weather until an appropriate one is found.
			//tempWeatherType = currentWeatherType;
			/*
			if (UseWeatherConditions)
			{
				while (tempWeatherType.TemperatureType == WeatherType.TemperatureTypeEnum.AboveFreezing && Temperature <= m_FreezingTemperature
					|| tempWeatherType.Season != WeatherType.SeasonEnum.All && (int)tempWeatherType.Season != (int)CurrentSeason
				|| tempWeatherType.TemperatureType == WeatherType.TemperatureTypeEnum.BelowFreezing && Temperature > m_FreezingTemperature)
				{
					if (tempWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.No)
					{
						tempWeatherType = NonPrecipiationWeatherTypes[Random.Range(0, NonPrecipiationWeatherTypes.Count)];
					}
					else if (tempWeatherType.PrecipitationWeatherType == WeatherType.WeatherSetting.Yes)
					{
						tempWeatherType = PrecipiationWeatherTypes[Random.Range(0, PrecipiationWeatherTypes.Count)];
					}
					else
					{
						break;
					}
				}
			}
			*/
			//currentWeatherType = tempWeatherType;

			// Set Celestial Mats Params
			//celestialManager.SetCelestialParams(currentWeatherType);
			/*m_ReceivedCloudValue = GetCloudLevel(m_ReceivedCloudValue);
			m_CloudDomeMaterial.SetFloat("_CloudCover", m_ReceivedCloudValue);
			RenderSettings.fogDensity = currentWeatherType.FogDensity;
			CurrentFogAmount = RenderSettings.fogDensity;
			//TODO:WindZone WendyWindZone.windMain = currentWeatherType.WindIntensity;
			CurrentWindIntensity = currentWeatherType.WindIntensity;
			SunIntensity = currentWeatherType.SunIntensity;
			MoonIntensity = currentWeatherType.MoonIntensity;

			/*if (currentWeatherType.ShaderControl == WeatherType.ShaderControlEnum.Rain)
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
			}*/
			// 粒子数量初始化归零
			/*for (int i = 0; i < WeatherEffectsList.Count; i++)
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
			}*/
			/*
			//Initialize our weather type's additional particle effetcs
			if (currentWeatherType.UseAdditionalWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				for (int i = 0; i < AdditionalWeatherEffectsList.Count; i++)
				{
					if (AdditionalWeatherEffectsList[i].name == currentWeatherType.AdditionalWeatherEffect.name + " (Wendy) ")
					{
						AdditionalCurrentParticleSystem = AdditionalWeatherEffectsList[i];
						ParticleSystem.EmissionModule CurrentEmission = AdditionalCurrentParticleSystem.emission;
						CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve((float)currentWeatherType.AdditionalParticleEffectAmount);
						
					}
				}

				AdditionalCurrentParticleSystem.transform.localPosition = currentWeatherType.AdditionalParticleEffectVector;
			}

			if (currentWeatherType.UseLightning == WeatherType.WeatherSetting.Yes && currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				Color C = m_LightningFlashMaterial.color;
				C.a = 0.5f;
				m_LightningFlashMaterial.color = C;
			}*/
			
			/*//Instantly change all of our gradients to the stormy gradients
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
			}*/
			/*
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
			}*/

			/*if (currentWeatherType.CloudLevel == WeatherType.CloudLevelEnum.MostlyCloudy)
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
			}*/
		}
		void TransitionWeather()
		{
			OnWeatherChangeEvent.Invoke(); //Invoke our weather change event
			if (CloudCoroutine != null) { StopCoroutine(CloudCoroutine); }
			if (FogCoroutine != null) { StopCoroutine(FogCoroutine); }
			if (WeatherEffectCoroutine != null) { StopCoroutine(WeatherEffectCoroutine); }
			if (AdditionalWeatherEffectCoroutine != null) { StopCoroutine(AdditionalWeatherEffectCoroutine); }
			if (ParticleFadeCoroutine != null) { StopCoroutine(ParticleFadeCoroutine); }
			if (AdditionalParticleFadeCoroutine != null) { StopCoroutine(AdditionalParticleFadeCoroutine); }
			if (SunCoroutine != null) { StopCoroutine(SunCoroutine); }
			if (MoonCoroutine != null) { StopCoroutine(MoonCoroutine); }
			if (WindCoroutine != null) { StopCoroutine(WindCoroutine); }
			if (SoundInCoroutine != null) { StopCoroutine(SoundInCoroutine); }
			if (SoundOutCoroutine != null) { StopCoroutine(SoundOutCoroutine); }
			if (LightningCloudsCoroutine != null) { StopCoroutine(LightningCloudsCoroutine); }
			if (ColorCoroutine != null) { StopCoroutine(ColorCoroutine); }
			if (CloudHeightCoroutine != null) { StopCoroutine(CloudHeightCoroutine); }
			if (RainShaderCoroutine != null) { StopCoroutine(RainShaderCoroutine); }
			if (SnowShaderCoroutine != null) { StopCoroutine(SnowShaderCoroutine); }
			
			// Get FadeSequence
			FadeSequence FS = gameObject.GetComponent<FadeSequence>();
			if(FS == null)
			{
				FS = gameObject.AddComponent<FadeSequence>();
				FS.currentWeatherType = currentWeatherType;
				FS.m_CloudDomeMaterial = m_CloudDomeMaterial;
				FS.m_LightningFlashMaterial = m_LightningFlashMaterial;
				FS.CurrentParticleSystem = CurrentParticleSystem;
				FS.AdditionalCurrentParticleSystem = AdditionalCurrentParticleSystem;
			}

			//Reset our time of day sounds timer so it doesn't play right after a weather change
			m_TimeOfDaySoundsTimer = 0;

			//Get randomized cloud amount based on cloud level from weather type.
			if (currentWeatherType.CloudLevel != WeatherType.CloudLevelEnum.DontChange)
			{
				m_ReceivedCloudValue = celestialManager.GetCloudLevel(m_ReceivedCloudValue);
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
			{   // currentWeatherType.SunIntensity SO中的数值, celestialManager.SunIntensity 暂存变量，用于计算
				SunCoroutine = StartCoroutine(FS.SunFadeSequence(TransitionSpeed, currentWeatherType.SunIntensity,celestialManager.SunIntensity));
				MoonCoroutine = StartCoroutine(FS.MoonFadeSequence(TransitionSpeed, currentWeatherType.MoonIntensity,celestialManager.MoonIntensity));
				
				ColorCoroutine = StartCoroutine(celestialManager.ColorFadeSequence(TransitionSpeed * 20, 0, 1));
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
				SunCoroutine = StartCoroutine(FS.SunFadeSequence(TransitionSpeed / 1.75f, currentWeatherType.SunIntensity,celestialManager.SunIntensity));
				MoonCoroutine = StartCoroutine(FS.MoonFadeSequence(TransitionSpeed, currentWeatherType.MoonIntensity,celestialManager.MoonIntensity));
				// 暂时将Color渐变 放在CelestialManager里
				ColorCoroutine = StartCoroutine(celestialManager.ColorFadeSequence(TransitionSpeed * 100, 0, 1));

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

			if (currentWeatherType.UseWeatherEffect == WeatherType.WeatherSetting.Yes)
			{
				for (int i = 0; i < WeatherEffectsList.Count; i++)
				{
					if (WeatherEffectsList[i].name == currentWeatherType.WeatherEffect.name + " (Wendy)")
					{
						CurrentParticleSystem = WeatherEffectsList[i];
						CurrentParticleSystem.transform.localPosition = currentWeatherType.ParticleEffectVector;
					}
				}

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
		IEnumerator CloudFadeSequence(float TransitionTime, float MaxValue)
		{
			bool FadingOut = (TransitionTime < 0.0f);
			float m_LocalTransitionSpeed = 1.0f / TransitionTime;
			float CurrentValue = m_CloudDomeMaterial.GetFloat("_CloudCover");

			while ((CurrentValue >= MaxValue && FadingOut) || (CurrentValue <= MaxValue && !FadingOut))
			{
				CurrentValue += Time.deltaTime * m_LocalTransitionSpeed;
				m_CloudDomeMaterial.SetFloat("_CloudCover", CurrentValue);
				Debug.Log(CurrentValue + "cloudcover");
				yield return null;
			}
		}

		IEnumerator FogFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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

		IEnumerator WindFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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

		IEnumerator SunFadeSequence(float TransitionTime, float MaxValue)
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

		IEnumerator MoonFadeSequence(float TransitionTime, float MaxValue)
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

		IEnumerator ParticleFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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
				Debug.Log(CurrentValue);
				yield return null;
			}
		}

		IEnumerator ParticleFadeOutSequence(float TransitionTime, float MinValue, ParticleSystem EffectToFade)
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

		IEnumerator AdditionalParticleFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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

		IEnumerator AdditionalParticleFadeOutSequence(float TransitionTime, float MinValue, ParticleSystem EffectToFade)
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

		IEnumerator SoundFadeInSequence(float TransitionTime, float MaxValue, AudioSource SourceToFade)
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

		IEnumerator SoundFadeOutSequence(float TransitionTime, float MinValue, AudioSource SourceToFade)
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

		IEnumerator CloudHeightFadeSequence(float TransitionTime, int MinValue, int MaxValue)
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

		IEnumerator RainShaderFadeInSequence(float TransitionTime, float MaxValue)
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

		IEnumerator RainShaderFadeOutSequence(float TransitionTime, float MinValue)
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

		IEnumerator SnowShaderFadeInSequence(float TransitionTime, float MaxValue)
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

		IEnumerator SnowShaderFadeOutSequence(float TransitionTime, float MinValue)
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

		IEnumerator LightningCloudsFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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

		IEnumerator ColorFadeSequence(float TransitionTime, float MinValue, float MaxValue)
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
	}
}

