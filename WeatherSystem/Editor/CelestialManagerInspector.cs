using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Wendy;

namespace WendyEditor{
	
[CustomEditor(typeof(CelestialManager))]
[System.Serializable]
public class CelestialManagerInspector : Editor
{
	private CelestialManager m_target;
	
	// Editor
	SerializedProperty TabNumberProp;
	
	// Weather
	SerializedProperty FogColorProp;
	SerializedProperty FogStormyColorProp;
	SerializedProperty CloudLightColorProp;
	SerializedProperty CloudBaseColorProp;
	SerializedProperty CloudStormyBaseColorProp;
	
	//Celestial
	SerializedProperty SunColorProp;
	SerializedProperty StormySunColorProp;
	SerializedProperty MoonColorProp;
	SerializedProperty SkyTintColorProp;
	SerializedProperty SkyColorProp;
	SerializedProperty AmbientSkyLightColorProp;
	SerializedProperty StormyAmbientSkyLightColorProp;
	SerializedProperty AmbientEquatorLightColorProp;
	SerializedProperty StormyAmbientEquatorLightColorProp;
	SerializedProperty AmbientGroundLightColorProp;
	SerializedProperty StormyAmbientGroundLightColorProp;
	SerializedProperty StarLightColorProp;
	SerializedProperty SunRevolutionProp;
	SerializedProperty SunIntensityCurveProp;
	SerializedProperty SunSizeProp;
	SerializedProperty SunFoldoutProp;
	SerializedProperty MoonIntensityCurveProp;
	SerializedProperty MoonSizeProp;
	SerializedProperty MoonPhaseColorProp;
	SerializedProperty MoonFoldoutProp;
	SerializedProperty MoonPhaseIndexProp;
	SerializedProperty MoonBrightnessProp;
	SerializedProperty AtmosphereFoldoutProp;
	SerializedProperty StarSpeedProp;
	SerializedProperty SunAngleProp;
	SerializedProperty MoonAngleProp;
	SerializedProperty HemisphereProp;
	SerializedProperty AtmosphereThicknessProp;
	
	//TODO: Atmosphere Settings SO Data
	SerializedProperty atmosphereSettingsProp;
	
	
	//Reorderable lists
	ReorderableList AllWeatherTypesList;
	ReorderableList MoonPhaseList;
	ReorderableList LightningFlashPatternsList;
	ReorderableList ThunderSoundsList;
	ReorderableList MorningSoundsList;
	ReorderableList DaySoundsList;
	ReorderableList EveningSoundsList;
	ReorderableList NightSoundsList;
	ReorderableList MorningMusicList;
	ReorderableList DayMusicList;
	ReorderableList EveningMusicList;
	ReorderableList NightMusicList;
	ReorderableList LightningFireTagsList;
	
	void OnEnable () {
		// Get target
		m_target = (CelestialManager) target;
		
		// Editor
		TabNumberProp = serializedObject.FindProperty("TabNumber");
			
		// Celestial
		SunColorProp = serializedObject.FindProperty ("SunColor");
		StormySunColorProp = serializedObject.FindProperty("StormySunColor");
		MoonColorProp = serializedObject.FindProperty ("MoonColor");
		SkyTintColorProp = serializedObject.FindProperty ("SkyTintColor");
		SkyColorProp = serializedObject.FindProperty("SkyColor");
		AmbientSkyLightColorProp = serializedObject.FindProperty ("AmbientSkyLightColor");
		StormyAmbientSkyLightColorProp = serializedObject.FindProperty("StormyAmbientSkyLightColor");
		AmbientEquatorLightColorProp = serializedObject.FindProperty ("AmbientEquatorLightColor");
		StormyAmbientEquatorLightColorProp = serializedObject.FindProperty("StormyAmbientEquatorLightColor");
		AmbientGroundLightColorProp = serializedObject.FindProperty ("AmbientGroundLightColor");
		StormyAmbientGroundLightColorProp = serializedObject.FindProperty("StormyAmbientGroundLightColor");
		StarLightColorProp = serializedObject.FindProperty ("StarLightColor");
		CloudLightColorProp = serializedObject.FindProperty ("CloudLightColor");
		CloudBaseColorProp = serializedObject.FindProperty ("CloudBaseColor");
		SunRevolutionProp = serializedObject.FindProperty ("SunRevolution");
		SunIntensityCurveProp = serializedObject.FindProperty ("SunIntensityCurve");
		SunSizeProp = serializedObject.FindProperty ("SunSize");
		SunFoldoutProp = serializedObject.FindProperty ("SunFoldout");
		MoonIntensityCurveProp = serializedObject.FindProperty ("MoonIntensityCurve");
		MoonSizeProp = serializedObject.FindProperty ("MoonSize");
		MoonPhaseColorProp = serializedObject.FindProperty("MoonPhaseColor");
		MoonFoldoutProp = serializedObject.FindProperty ("MoonFoldout");
		MoonPhaseIndexProp = serializedObject.FindProperty ("MoonPhaseIndex");
		MoonBrightnessProp = serializedObject.FindProperty ("MoonBrightness");
		AtmosphereFoldoutProp = serializedObject.FindProperty ("AtmosphereFoldout");
		StarSpeedProp = serializedObject.FindProperty("StarSpeed");
		SunAngleProp = serializedObject.FindProperty("SunAngle");
		MoonAngleProp = serializedObject.FindProperty("MoonAngle");
		HemisphereProp = serializedObject.FindProperty("Hemisphere");
		AtmosphereThicknessProp = serializedObject.FindProperty("AtmosphereThickness");
		
		atmosphereSettingsProp = serializedObject.FindProperty("atmosphereSettings");
		
		//Reorderable lists
		//Put our lists into reorderable lists because Unity allows these lists to be serialized
		//All weather types
		AllWeatherTypesList = new ReorderableList(serializedObject, serializedObject.FindProperty("AllWeatherTypes"), true, true, true, true);
		AllWeatherTypesList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "All Weather Types", EditorStyles.boldLabel);
		};
		AllWeatherTypesList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = AllWeatherTypesList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Moon Phases New
		MoonPhaseList = new ReorderableList(serializedObject, serializedObject.FindProperty("MoonPhaseList"), true, true, true, true);
		MoonPhaseList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				rect.y += 1;
				var element = MoonPhaseList.serializedProperty.GetArrayElementAtIndex(index);

				//Give our MoonPhaseIntensity an initialized value of 1.
				if (element.FindPropertyRelative("MoonPhaseIntensity").floatValue == 0)
				{
					element.FindPropertyRelative("MoonPhaseIntensity").floatValue = 1;
				}

				EditorGUI.PropertyField(new Rect(rect.x + 200, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("MoonPhaseIntensity"), GUIContent.none);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("MoonPhaseTexture"), GUIContent.none);
			};

		MoonPhaseList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "   Moon Phase Texture  " + "               Light Intensity", EditorStyles.boldLabel);
		};

		//Lightning patterns
		LightningFlashPatternsList = new ReorderableList(serializedObject, serializedObject.FindProperty("LightningFlashPatterns"), true, true, true, true);
		LightningFlashPatternsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField (rect, "Lightning Flash Patterns", EditorStyles.boldLabel);
		};
		LightningFlashPatternsList.drawElementCallback = 
			(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = LightningFlashPatternsList.serializedProperty.GetArrayElementAtIndex (index);
			EditorGUI.PropertyField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
		};

		//Thunder Sounds
		ThunderSoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("ThunderSounds"), true, true, true, true);
		ThunderSoundsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField (rect, "Thunder Sounds", EditorStyles.boldLabel);
		};
		ThunderSoundsList.drawElementCallback = 
			(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = ThunderSoundsList.serializedProperty.GetArrayElementAtIndex (index);
			EditorGUI.PropertyField (new Rect (rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
		};

		//Morning Sounds
		MorningSoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("MorningSounds"), true, true, true, true);
		MorningSoundsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Morning Sounds", EditorStyles.boldLabel);
		};
		MorningSoundsList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = MorningSoundsList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Day Sounds
		DaySoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("DaySounds"), true, true, true, true);
		DaySoundsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Day Sounds", EditorStyles.boldLabel);
		};
		DaySoundsList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = DaySoundsList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Evening Sounds
		EveningSoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("EveningSounds"), true, true, true, true);
		EveningSoundsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Evening Sounds", EditorStyles.boldLabel);
		};
		EveningSoundsList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = EveningSoundsList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Night Sounds
		NightSoundsList = new ReorderableList(serializedObject, serializedObject.FindProperty("NightSounds"), true, true, true, true);
		NightSoundsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Night Sounds", EditorStyles.boldLabel);
		};
		NightSoundsList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = NightSoundsList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Morning Music
		MorningMusicList = new ReorderableList(serializedObject, serializedObject.FindProperty("MorningMusic"), true, true, true, true);
		MorningMusicList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Morning Music", EditorStyles.boldLabel);
		};
		MorningMusicList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = MorningMusicList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Day Music
		DayMusicList = new ReorderableList(serializedObject, serializedObject.FindProperty("DayMusic"), true, true, true, true);
		DayMusicList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Day Music", EditorStyles.boldLabel);
		};
		DayMusicList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = DayMusicList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Evening Music
		EveningMusicList = new ReorderableList(serializedObject, serializedObject.FindProperty("EveningMusic"), true, true, true, true);
		EveningMusicList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Evening Music", EditorStyles.boldLabel);
		};
		EveningMusicList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = EveningMusicList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Night Music
		NightMusicList = new ReorderableList(serializedObject, serializedObject.FindProperty("NightMusic"), true, true, true, true);
		NightMusicList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Night Music", EditorStyles.boldLabel);
		};
		NightMusicList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = NightMusicList.serializedProperty.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
			};

		//Lightning Fire Tags
		LightningFireTagsList = new ReorderableList(serializedObject, serializedObject.FindProperty("LightningFireTags"), true, true, true, true);
		LightningFireTagsList.drawHeaderCallback = rect => {
			EditorGUI.LabelField(rect, "Lightning Fire Tags", EditorStyles.boldLabel);
		};
		LightningFireTagsList.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
				var element = LightningFireTagsList.serializedProperty.GetArrayElementAtIndex(index);

				if (element.stringValue == "")
				{
					element.stringValue = "Untagged";
				}

				element.stringValue = EditorGUI.TagField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.stringValue);
			};
	}
	
	
	void UpdateColorProperties ()
	{
		SunColorProp = serializedObject.FindProperty("SunColor");
		StormySunColorProp = serializedObject.FindProperty("StormySunColor");
		MoonColorProp = serializedObject.FindProperty("MoonColor");
		SkyTintColorProp = serializedObject.FindProperty("SkyTintColor");
		SkyColorProp = serializedObject.FindProperty("SkyColor");
		AmbientSkyLightColorProp = serializedObject.FindProperty("AmbientSkyLightColor");
		StormyAmbientSkyLightColorProp = serializedObject.FindProperty("StormyAmbientSkyLightColor");
		AmbientEquatorLightColorProp = serializedObject.FindProperty("AmbientEquatorLightColor");
		StormyAmbientEquatorLightColorProp = serializedObject.FindProperty("StormyAmbientEquatorLightColor");
		AmbientGroundLightColorProp = serializedObject.FindProperty("AmbientGroundLightColor");
		StormyAmbientGroundLightColorProp = serializedObject.FindProperty("StormyAmbientGroundLightColor");
		StarLightColorProp = serializedObject.FindProperty("StarLightColor");
		//FogColorProp = serializedObject.FindProperty("FogColor");
		//FogStormyColorProp = serializedObject.FindProperty("FogStormyColor");
		CloudLightColorProp = serializedObject.FindProperty("CloudLightColor");
		CloudBaseColorProp = serializedObject.FindProperty("CloudBaseColor");
		//CloudStormyBaseColorProp = serializedObject.FindProperty("CloudStormyBaseColor");
	}
	
	public override void OnInspectorGUI ()
	{
		// Prepare
		GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
		myFoldoutStyle.fontStyle = FontStyle.Bold;
		myFoldoutStyle.fontSize = 12;
		myFoldoutStyle.active.textColor = Color.black;
		myFoldoutStyle.focused.textColor = Color.white;
		myFoldoutStyle.onHover.textColor = Color.black;
		myFoldoutStyle.normal.textColor = Color.white;
		myFoldoutStyle.onNormal.textColor = Color.black;
		myFoldoutStyle.onActive.textColor = Color.black;
		myFoldoutStyle.onFocused.textColor = Color.black;
		Color myStyleColor = Color.black;
		
		/*
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		TabNumberProp.intValue = GUILayout.SelectionGrid (TabNumberProp.intValue, TabButtons, 5, GUILayout.Height(28), GUILayout.Width(90 * Screen.width/100));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();*/
		
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.BeginVertical("Box",GUILayout.Width(90 * Screen.width/100));
/*
		if (GUILayout.Button(new GUIContent(HelpIcon), HelpStyle, GUILayout.ExpandWidth(true), GUILayout.Height(22.5f)))
		{
			Application.OpenURL("https://docs.google.com/document/d/1uL_oMqHC_EduRGEnOihifwcpkQmWX9rubGw8qjkZ4b4/edit#heading=h.g75asyk1ag9n");
		}*/

		var style = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter};
		EditorGUILayout.LabelField(new GUIContent(/*CelestialIcon*/), style, GUILayout.ExpandWidth(true), GUILayout.Height(32));
		EditorGUILayout.LabelField("Celestial Settings", style, GUILayout.ExpandWidth(true));
		/*
		GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f, 1);
		EditorGUILayout.LabelField("The Celestial Settings allow you to control various celestial settings and colors for Wendy's sun, moon, stars, and atmosphere.", EditorStyles.helpBox);

		GUI.backgroundColor = new Color(1f, 1, 0.5f, 0.5f);
		EditorGUILayout.LabelField("If you need help, you can hover over each field with your mouse for a tooltip description. " +
			"Documentation for this section can be found by pressing the ? in the upper right hand corner.", EditorStyles.helpBox);
		GUI.backgroundColor = Color.white;*/

		GUILayout.Space(5);

		EditorGUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		//Sun Settings
		if (!SunFoldoutProp.boolValue)
		{
			GUI.backgroundColor = new Color(0.5f,0.5f,0.5f,1.0f);
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();

		SunFoldoutProp.boolValue = Foldout(SunFoldoutProp.boolValue, "Sun Settings", true, myFoldoutStyle);
		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (SunFoldoutProp.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(15);
			EditorGUILayout.BeginVertical();

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SunColorProp, 
				new GUIContent("Sun Color", "Sun color of the day"));
			EditorGUILayout.PropertyField(StormySunColorProp,
				new GUIContent("Stormy Sun Color", "precipitation weather sun gradient"));

			EditorGUILayout.Space();
			CustomIntSlider(new Rect(), new GUIContent(), SunAngleProp,
					new GUIContent("Sun Tilt Angle", "Controls the tilt angle of the Sun."), -45, 45);

			EditorGUILayout.Space();
			CustomIntSlider(new Rect(), new GUIContent(), SunRevolutionProp, 
				new GUIContent("Sun Revolution", "Controls the direction in which Wendy's sun sets and rise."), -180, 180);
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(SunIntensityCurveProp,
				new GUIContent("Sun Intensity Curve", "Controls the intensity of Wendy's sun. X represents the hour and Y represents the intensity."));
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(SunSizeProp,
				new GUIContent("Sun Size Curve", "Controls the size of Wendy's sun. X represents the hour and Y represents the size."));
			EditorGUILayout.Space();

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
		GUI.backgroundColor = Color.white;

		//Moon Settings
		if (!MoonFoldoutProp.boolValue)
		{
			GUI.backgroundColor = new Color(0.5f,0.5f,0.5f,1.0f);
		}
		else
		{
			GUI.backgroundColor = Color.white;
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();

		MoonFoldoutProp.boolValue = Foldout(MoonFoldoutProp.boolValue, "Moon Settings", true, myFoldoutStyle);
		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (MoonFoldoutProp.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(15);
			EditorGUILayout.BeginVertical();

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(MoonColorProp, 
				new GUIContent("Moon Light Color", "A gradient that controls Wendy's moon light color."));

			EditorGUILayout.Space();
			CustomIntSlider(new Rect(), new GUIContent(), MoonAngleProp,
					new GUIContent("Moon Tilt Angle", "Controls the tilt angle of the Moon."), -45, 45);
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(MoonIntensityCurveProp,
				new GUIContent("Moon Intensity Curve", "Controls the intensity of Wendy's moon. X represents the hour and Y represents the intensity."));
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(MoonSizeProp,
				new GUIContent("Moon Size Curve", "Controls the size of Wendy's moon. X represents the hour and Y represents the size."));
			EditorGUILayout.Space();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			GUILayout.Box(new GUIContent("What's This?", "A list of moon phase textures that Wendy will use when creating Wendy's moon. " +
				"Each texture applied to the list will be used as a moon phase and be applied in order of the current moon phase. Each moon phase " +
				"has an individual light intensity to allow each moon phase to give off different amounts of light."),
				EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
			MoonPhaseList.DoLayoutList();
			EditorGUILayout.Space();

			GUIStyle TitleStyle = new GUIStyle();
			TitleStyle.fontStyle = FontStyle.Bold;
			TitleStyle.fontSize = 14;
			TitleStyle.alignment = TextAnchor.MiddleCenter;


			if (MoonPhaseIndexProp.intValue == m_target.MoonPhaseList.Count && m_target.MoonPhaseList.Count > 0)
			{
				MoonPhaseIndexProp.intValue = m_target.MoonPhaseList.Count - 1;
			}

			if (m_target.MoonPhaseList.Count > 0)
			{
				EditorGUILayout.LabelField(new GUIContent("Current Moon Phase", "Displays all moon phases by adjusting the slider. " +
				"The Current Moon Phase also controls the moon phase Wendy will start with."), TitleStyle);
				GUI.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
				EditorGUILayout.BeginVertical("Box");
				GUI.backgroundColor = Color.white;
				GUILayout.Space(5);
				EditorGUILayout.LabelField(new GUIContent(m_target.MoonPhaseList[MoonPhaseIndexProp.intValue].MoonPhaseTexture), style, GUILayout.Height(50));
				GUILayout.Space(5);
				EditorGUILayout.EndVertical();
			}

			if (m_target.MoonPhaseList.Count-1 != 0 && m_target.MoonPhaseList.Count > 0)
			{
				EditorGUILayout.Space();
				CustomIntSliderNoTooltipTest(new Rect(), new GUIContent(), MoonPhaseIndexProp, "", 0, m_target.MoonPhaseList.Count - 1);
			}

			if (m_target.MoonPhaseList.Count == 0)
			{
				GUI.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
				EditorGUILayout.HelpBox("There are currently no moon phases. To creat one, " +
					"press the + sign on the list above and assign a texture to the newly created slot.", MessageType.Warning, true);
				GUI.backgroundColor = Color.white;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(MoonPhaseColorProp,
				new GUIContent("Moon Phase Color", "A gradient that controls the color of Wendy's moon phases."));  

			EditorGUILayout.Space();
			CustomFloatSlider(new Rect(), new GUIContent(), MoonBrightnessProp, 
				new GUIContent("Moon Phase Brightness", "Controls the brightness of all moon phase textures."), 0.25f, 1.0f);

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
		GUI.backgroundColor = Color.white;

		//Atmosphere Settings
		if (!AtmosphereFoldoutProp.boolValue)
		{
			GUI.backgroundColor = new Color(0.5f,0.5f,0.5f,1.0f);
		}

		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();

		AtmosphereFoldoutProp.boolValue = Foldout(AtmosphereFoldoutProp.boolValue, "Atmosphere Settings", true, myFoldoutStyle);
		GUI.backgroundColor = Color.white;

		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();

		if (AtmosphereFoldoutProp.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(15);
			EditorGUILayout.BeginVertical();

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(atmosphereSettingsProp,
				new GUIContent("Atmosphere Scattering", "Atmosphere Scattering Data"));
				
			EditorGUILayout.PropertyField(HemisphereProp,
				new GUIContent("Hemisphere", "Controls whether Wendy's seasons are calculated in either the Northern or Southern Hemisphere."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(AtmosphereThicknessProp,
				new GUIContent("Atmosphere Thickness", "Controls the thickness of Wendy's atmosphere for each time of day. This is mainly used for sunrises and sunsets. " +
				"The higher the value, the darker and more red the sunrises and sunsets will be."));       

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(AmbientSkyLightColorProp, 
				new GUIContent("Ambient Sky Color", "A gradient that controls the Ambient Sky Color during non-precipitation weather types. Each element is a transition into the next time of day."));
			EditorGUILayout.PropertyField(StormyAmbientSkyLightColorProp,
				new GUIContent("Stormy Ambient Sky Color", "A gradient that controls the Ambient Sky Color during precipitation weather types. Each element is a transition into the next time of day."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(AmbientEquatorLightColorProp,
				new GUIContent("Ambient Equator Color", "A gradient that controls the Ambient Equator Color during non-precipitation weather types. Each element is a transition into the next time of day."));
			EditorGUILayout.PropertyField(StormyAmbientEquatorLightColorProp,
				new GUIContent("Stormy Ambient Equator Color", "A gradient that controls the Ambient Equator Color during precipitation weather types. Each element is a transition into the next time of day."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(AmbientGroundLightColorProp,
				new GUIContent("Ambient Ground Color", "A gradient that controls the Ambient Ground Color during non-precipitation weather types. Each element is a transition into the next time of day."));
			EditorGUILayout.PropertyField(StormyAmbientGroundLightColorProp,
				new GUIContent("Stormy Ambient Ground Color", "A gradient that controls the Ambient Ground Color during precipitation weather types. Each element is a transition into the next time of day."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SkyColorProp,
				new GUIContent("Sky Color", "A gradient that controls the overall sky color of Wendy's Skybox. " +
				"Each element is a transition into the next time of day."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(SkyTintColorProp,
				new GUIContent("Sky Tint Color", "A gradient that controls the tint color of Wendy's Skybox. " +
				"Each element is a transition into the next time of day."));

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(StarLightColorProp, 
				new GUIContent("Starlight Color", "A gradient that controls the color and transparency of Wendy's stars. Each element is a transition into the next time of day."));
				
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(CloudLightColorProp, 
				new GUIContent("Cloud Light Color", "A gradient that controls the color and transparency of Wendy Cloud's light color"));
				
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(CloudBaseColorProp, 
				new GUIContent("Cloud Base Color", "A gradient that controls the color and transparency of Wendy Cloud's ambient color ."));

			EditorGUILayout.Space();
			CustomFloatSlider(new Rect(), new GUIContent(), StarSpeedProp,
				new GUIContent("Star Speed", "Controls how fast the stars will move in the sky at night."), 0, 3.0f);

			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();
		GUI.backgroundColor = Color.white;
		
		
		// Lightning Settings
		/*EditorGUILayout.BeginHorizontal();
		GUILayout.Space(15);
		EditorGUILayout.BeginVertical();

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		CustomIntSlider(new Rect(), new GUIContent(), LightningGenerationDistanceProp,
		new GUIContent("Generation Distance", "Controls the maximum distance lightning can be generated around the player."), 50, 300);
		EditorGUILayout.Space();

		CustomIntSlider(new Rect(), new GUIContent(), LightningSecondsMinProp,
		new GUIContent("Min Lightning Seconds", "Controls the minimum seconds for lightning to be generated."), 2, self.LightningSecondsMax);
		
		CustomIntSlider(new Rect(), new GUIContent(), LightningSecondsMaxProp,
		new GUIContent("Max Lightning Seconds", "Controls the maximum seconds for lightning to be generated."), self.LightningSecondsMin+1, 60);

		EditorGUILayout.Space();
		CustomFloatSlider(new Rect(), new GUIContent(), LightningLightIntensityMinProp,
		new GUIContent("Min Lightning Intensity", "Controls the minimum light intensity for the lightning to be generated."), 1, self.LightningLightIntensityMax);

		CustomFloatSlider(new Rect(), new GUIContent(), LightningLightIntensityMaxProp,
		new GUIContent("Max Lightning Intensity", "Controls the maximum light intensity for the lightning to be generated."), 1, 4);
		EditorGUILayout.Space();

		CustomIntSlider(new Rect(), new GUIContent(), LightningDetectionDistanceProp,
		new GUIContent("Detection Distance", "Controls the distance of UniStorm's lightning strike collider. The larger the radius, the more likely lightning will strike " +
		"objects instead of the ground."), 5, 60);
		EditorGUILayout.Space();

		EditorGUILayout.Space();
		EditorGUI.BeginChangeCheck();
		var layersSelection = EditorGUILayout.MaskField(new GUIContent("Lightning Strike Layers", "Controls what layers UniStorm's procedural lightning can strike."), LayerMaskToField(DetectionLayerMaskProp.intValue), InternalEditorUtility.layers);
		EditorGUILayout.Space();
		if (EditorGUI.EndChangeCheck())
		{
			Undo.RecordObject(self, "Layers changed");
			DetectionLayerMaskProp.intValue = FieldToLayerMask(layersSelection);
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Box(new GUIContent("What's This?", "A list of tags that will create a fire particle effect when struck by lightning."),
			EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
		LightningFireTagsList.DoLayoutList();
		EditorGUILayout.Space();

		CustomIntSlider(new Rect(), new GUIContent(), LightningStrikeOddsProp, 
			new GUIContent("Ground Strike Odds", "Controls the odds in which UniStorm's lightning will strike the ground or other objects of the appropriate tag."), 0, 100);

		EditorGUILayout.Space();
		CustomObjectField(new Rect(), new GUIContent(), LightningStrikeEffectProp, 
			new GUIContent("Lightning Strike Effect","The effect that plays when lightning strikes the ground."), typeof(GameObject), true);

		EditorGUILayout.Space();
		CustomObjectField(new Rect(), new GUIContent(), LightningStrikeFireProp, 
			new GUIContent("Lightning Strike Fire", "The fire effect that plays when lightning strikes an object of the appropriate tag."), typeof(GameObject), true);

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Box(new GUIContent("What's This?", "A list of possible lightning flash patterns that UniStorm will use during lightning Weather Types."),
			EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
		LightningFlashPatternsList.DoLayoutList();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		GUILayout.Box(new GUIContent("What's This?", "A list of possible thunder sounds that UniStorm will play during lightning Weather Types."),
			EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
		ThunderSoundsList.DoLayoutList();*/

	}
	
	
	
	public static bool Foldout(bool foldout, GUIContent content, bool toggleOnLabelClick, GUIStyle style){
		Rect position = GUILayoutUtility.GetRect(40f, 40f, 16f, 16f, style);
		return EditorGUI.Foldout(position, foldout, content, toggleOnLabelClick, style);
	}

	public static bool Foldout(bool foldout, string content, bool toggleOnLabelClick, GUIStyle style){
		return Foldout(foldout, new GUIContent(content), toggleOnLabelClick, style);
	}
	
	
	//Custom functions for handling serialized properties.
	void CustomIntSlider (Rect position, GUIContent label, SerializedProperty property, GUIContent NameAndTip, int MinValue, int MaxValue) {
		label = EditorGUI.BeginProperty (position, label, property);
		EditorGUI.BeginChangeCheck ();
		var newValue = EditorGUILayout.IntSlider (NameAndTip, property.intValue, MinValue, MaxValue);
		if (EditorGUI.EndChangeCheck ())
			property.intValue = newValue;

		EditorGUI.EndProperty ();
	}

	void CustomIntSliderNoTooltip(Rect position, GUIContent label, SerializedProperty property, string Name, int MinValue, int MaxValue)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();
		var newValue = EditorGUILayout.IntSlider(Name, property.intValue, MinValue, MaxValue);
		if (EditorGUI.EndChangeCheck())
			property.intValue = newValue;

		EditorGUI.EndProperty();
	}

	void CustomIntSliderNoTooltipTest(Rect position, GUIContent label, SerializedProperty property, string Name, int MinValue, int MaxValue)
	{
		label = EditorGUI.BeginProperty(position, label, property);
		EditorGUI.BeginChangeCheck();
		var newValue = EditorGUILayout.IntSlider(Name, property.intValue, MinValue, MaxValue);
		if (EditorGUI.EndChangeCheck())
			property.intValue = newValue;

		EditorGUI.EndProperty();
	}

	void CustomFloatSlider (Rect position, GUIContent label, SerializedProperty property, GUIContent NameAndTip, float MinValue, float MaxValue) {
		label = EditorGUI.BeginProperty (position, label, property);
		EditorGUI.BeginChangeCheck ();
		var newValue = EditorGUILayout.Slider (NameAndTip, property.floatValue, MinValue, MaxValue);
		if (EditorGUI.EndChangeCheck ())
			property.floatValue = newValue;

		EditorGUI.EndProperty ();
	}
}

}
