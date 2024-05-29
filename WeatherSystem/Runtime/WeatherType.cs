using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wendy
{
	[CreateAssetMenu(fileName = "New Weather Type", menuName = "WeatherSystem/New Weather Type")]
	public class WeatherType : ScriptableObject {

	public string WeatherTypeName = "New Weather Type";

	public float SunIntensity = 1;
	public float MoonIntensity = 1;

	//Fog
	public int MinimumFogLevel = 300;
	public int MaximumFogLevel = 600;

	public float FogDensity = 0.0015f;
	public float StormyFogDensity = 0.004f;
	public float MaximumFogDensity = 0.0015f;
	public float FogSpeedMultiplier = 1f;

	//Wind
	public float WindSpeedLevel = 0.85f;
	public float WindBendingLevel = 0.2f;
	public float WindIntensity = 0.25f;

	public ParticleSystem WeatherEffect;
	public int ParticleEffectAmount = 200;
	public Vector3 ParticleEffectVector = new Vector3(0, 28, 0);
	public ParticleSystem AdditionalWeatherEffect;
	public int AdditionalParticleEffectAmount = 200;
	public Vector3 AdditionalParticleEffectVector = new Vector3(0, 28, 0);

	//Audio
	public float WeatherVolume = 1;
	public AudioClip WeatherSound;

	//Particle Effect
	public WeatherSetting PrecipitationWeatherType = WeatherSetting.No;
	public WeatherSetting UseWeatherSound = WeatherSetting.No;
	public WeatherSetting UseWeatherEffect = WeatherSetting.No;
	public WeatherSetting UseAdditionalWeatherEffect = WeatherSetting.No;
	public WeatherSetting SpecialWeatherType = WeatherSetting.No;

	public SeasonEnum Season;
	public enum SeasonEnum
	{
		All = 0,
		Spring = 1,
		Summer = 2,
		Fall = 3,
		Winter = 4
	}
	
	public CloudLevelEnum CloudLevel;
	public enum CloudLevelEnum
	{
		Clear = 0,
		MostlyClear = 1,
		PartyCloudy = 2,
		MostlyCloudy = 3,
		Cloudy = 4,
		DontChange = 5
	}

	public TemperatureTypeEnum TemperatureType = TemperatureTypeEnum.AboveFreezing;
	public enum TemperatureTypeEnum
	{
		BelowFreezing = 0,
		AboveFreezing = 1,
		Both = 2
	}

	public ShaderControlEnum ShaderControl = ShaderControlEnum.None;
	public enum ShaderControlEnum
	{
		Rain = 0,
		Snow = 1,
		None = 2
	}

	//public WeatherSetting UseParticleEffect = WeatherSetting.Yes;
	public WeatherSetting UseLightning = WeatherSetting.No;
	public WeatherSetting CustomizeWeatherIcon = WeatherSetting.No;
	public Texture WeatherIcon;
	public enum WeatherSetting
	{
		Yes = 0,
		No = 1
	}

	public void CreateWeatherSound()
	{
		//WeatherSystem WeatherSystemObject = GameObject.Find("Wendy Weather System").GetComponent<WeatherSystem>();
		ResourceManager resourceManager = GameObject.Find("Wendy Resource Manager").GetComponent<ResourceManager>();
		//if (WeatherSystemObject.enabled)
		{
			GameObject Temp = new GameObject();
			Temp.AddComponent<AudioSource>();
			AudioSource _AS = Temp.GetComponent<AudioSource>();
			_AS.clip = WeatherSound;
			_AS.volume = 0;
			_AS.loop = true;
			UnityEngine.Audio.AudioMixer m_AudioMixer = Resources.Load("Audios/WendyAudioMixer") as UnityEngine.Audio.AudioMixer;
			_AS.outputAudioMixerGroup = m_AudioMixer.FindMatchingGroups("Master/Weather")[0];
			Temp.name = WeatherTypeName + " (Wendy)";
			Temp.transform.SetParent(GameObject.Find("Wendy Sounds").transform);
			Temp.transform.position = new Vector3(Temp.transform.parent.position.x, Temp.transform.parent.position.y, Temp.transform.parent.position.z);
			resourceManager.WeatherSoundsList.Add(_AS);
		}
	}
	//将资源迁移到ResourceManager
	public void CreateWeatherEffect()
	{
		WeatherSystem WeatherSystemObject = GameObject.Find("Wendy Weather System").GetComponent<WeatherSystem>();
		ResourceManager resourceManager = GameObject.Find("Wendy Resource Manager").GetComponent<ResourceManager>();
		ParticleSystem Temp = Instantiate(WeatherEffect, Vector3.zero, Quaternion.AngleAxis(-90, Vector3.right));
		Temp.transform.SetParent(GameObject.Find("Wendy Effects").transform);
		Temp.name = Temp.name.Replace("(Clone)", " (Wendy)");
		ParticleSystem.EmissionModule CurrentEmission = Temp.emission;
		CurrentEmission.enabled = true;
		CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
		CurrentEmission.SetBursts(new ParticleSystem.Burst[] { });
		resourceManager.WeatherEffectsList.Add(Temp);
		Debug.Log("add effect" + Temp.name);
	}

	public void CreateAdditionalWeatherEffect()
	{
		//WeatherSystem WeatherSystemObject = GameObject.Find("Wendy Weather System").GetComponent<WeatherSystem>();
		ResourceManager resourceManager = GameObject.Find("Wendy Resource Manager").GetComponent<ResourceManager>();
		ParticleSystem Temp = Instantiate(AdditionalWeatherEffect, Vector3.zero, Quaternion.AngleAxis(-90, Vector3.right));
		Temp.transform.SetParent(GameObject.Find("Wendy Effects").transform);
		Temp.name = Temp.name.Replace("(Clone)", " (Wendy)");
		ParticleSystem.EmissionModule CurrentEmission = Temp.emission;
		CurrentEmission.enabled = true;
		CurrentEmission.rateOverTime = new ParticleSystem.MinMaxCurve(0);
		resourceManager.AdditionalWeatherEffectsList.Add(Temp);
	}
	}
}

