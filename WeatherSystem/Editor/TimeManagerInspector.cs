using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Wendy;
namespace WendyEditor
{
[CustomEditor(typeof(TimeManager))]
public class TimeManagerInspector : Editor
{
	// SerializedProperty [Obsoleted]
	//SerializedProperty wendyTime;
	
	//================= 时间系统说明=====================================
	//=======分成两种，1. Real Time 2. Virtual Time=====================
	// 1. Real World Time: 不可以修改时间，实时根据系统时间Update，不能更改时间流速(Editor不生效，Play Mode实时更新)
	// 2. Virtual World Time: 可以自定义Calendar的日期，并且可以更改时间流速(Editor Mode时间不流动，只起到初始化的作用，不能实时显示效果)
	//================= End ============================================
	// 另，由于Inspector界面需要一个相对美观的可控制的日历界面，所以日期的范围和TimeManager会有出入
	
	// SerializedPropertys
	SerializedProperty RealWorldTimeProp;
	
	// Time Manager Editor
	private TimeManager m_target;
	private string m_dateButtonLabel;
	private string m_dayOfWeekLabel;
	private int m_buttonGridIndex;
	private bool m_isDaySelected;
	private bool m_showCustomDate;
	private int m_selectedCalendarDay = 0;
	private int[] m_dateSelector = new int[3];
	private readonly GUIContent[] m_dateSelectorContent = new[]
	{
		new GUIContent("Month", ""),
		new GUIContent("Day", ""),
		new GUIContent("Year", "")
	};

	// GUIContents
	private readonly GUIContent[] m_guiContent = new[]
	{
		new GUIContent("Real World Time", "Change between real world time and virtual world time"),
		new GUIContent("Repeat Mode", "The method used to change the date."),
		new GUIContent("Timeline", "The current 'time position' in the day-night cycle."),
		new GUIContent("Latitude", "The north-south angle of a position on the Earth's surface."),
		new GUIContent("Longitude", "The east-west angle of a position on the Earth's surface."),
		new GUIContent("Utc", "Universal Time Coordinated."),
		new GUIContent("Day Length", "Duration of the day-night cycle in minutes."),
		new GUIContent("Evaluate Time of Day by Curve?", "Will the 'time of day' be evaluated based on the timeline or based on the day-night length curve?"),
		new GUIContent("Current Time of Day:", "Displays the current 'time of day' based on the 'time position' of the day-night cycle"),
		new GUIContent("Update Mode", "The method used to update the position of the sun and moon in the sky."),
		new GUIContent("Time Direction", "The time direction.")
	};
	
	 	private void OnEnable()
		{
			// Get target
			m_target = (TimeManager) target;
			// class or struct cannot be serialized by unity,so you cannot FindProperty...
			//wendyTime = serializedObject.FindProperty("wendyTime");
			m_target.UpdateCalendar();
			m_dateButtonLabel = m_target.GetDateString();
			m_dayOfWeekLabel = m_target.GetDayOfWeekString();
			
			RealWorldTimeProp = serializedObject.FindProperty("RealWorldTime");

			// Find the serialized properties
			//m_timeSystem = serializedObject.FindProperty("timeSystem");
			/*m_timeDirection = serializedObject.FindProperty("timeDirection");
			m_repeatMode = serializedObject.FindProperty("repeatMode");
			m_timeline = serializedObject.FindProperty("timeline");
			m_latitude = serializedObject.FindProperty("latitude");
			m_longitude = serializedObject.FindProperty("longitude");
			m_utc = serializedObject.FindProperty("utc");
			m_dayLength = serializedObject.FindProperty("dayLength");
			m_isTimeEvaluatedByCurve = serializedObject.FindProperty("isTimeEvaluatedByCurve");
			m_dayLengthCurve = serializedObject.FindProperty("dayLengthCurve");*/
		}
		
		public override void OnInspectorGUI()
		{
			// Initializations
			m_buttonGridIndex = 0;
			m_selectedCalendarDay = m_target.selectedCalendarDay;
			m_dateButtonLabel = m_target.GetDateString();
			m_dayOfWeekLabel = m_target.GetDayOfWeekString();

			// Start custom inspector
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			// Calendar header buttons
			EditorGUILayout.BeginHorizontal("box");
			if (GUILayout.Button("<<", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
				DecreaseYear();
			if (GUILayout.Button("<", EditorStyles.miniButtonMid, GUILayout.Width(25)))
				DecreaseMonth();
			if (GUILayout.Button(m_dateButtonLabel, EditorStyles.miniButtonMid))
			{
				m_showCustomDate = !m_showCustomDate;
				m_dateSelector[0] = m_target.Month;
				m_dateSelector[1] = m_target.Day;
				m_dateSelector[2] = m_target.Year;
				
			}
			if (GUILayout.Button(">", EditorStyles.miniButtonMid, GUILayout.Width(25)))
				IncreaseMonth();
			if (GUILayout.Button(">>", EditorStyles.miniButtonRight, GUILayout.Width(25)))
				IncreaseYear();
			EditorGUILayout.EndHorizontal();

			if (m_showCustomDate)
			{
				GUILayout.Space(-4);
				EditorGUILayout.BeginVertical("box");
				EditorGUI.MultiIntField(EditorGUILayout.GetControlRect(), m_dateSelectorContent, m_dateSelector);
				m_dateSelector[0] = Mathf.Clamp(m_dateSelector[0], 1, 12);
				m_dateSelector[1] = Mathf.Min(m_dateSelector[1], DateTime.DaysInMonth(m_target.Year, m_target.Month));
				m_dateSelector[2] = Mathf.Clamp(m_dateSelector[2], 0, 9999);
				if (GUILayout.Button("Go To", EditorStyles.miniButtonMid))
				{
					m_target.SetNewDate(m_dateSelector[2], m_dateSelector[0], m_dateSelector[1]);
					m_selectedCalendarDay = m_target.selectedCalendarDay;
					m_showCustomDate = false;
				}
				EditorGUILayout.EndVertical();
				GUILayout.Space(3);
			}

			// Draws the days of the week strings above the selectable grid
			GUILayout.Space(-5);
			EditorGUILayout.BeginHorizontal("box");
			GUILayout.Label("Sun");
			GUILayout.Label("Mon");
			GUILayout.Label("Tue");
			GUILayout.Label("Wed");
			GUILayout.Label("Thu");
			GUILayout.Label("Fri");
			GUILayout.Label("Sat");
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(-5);

			// Creates the calendar selectable grid
			EditorGUILayout.BeginVertical("Box");
			for (int i = 0; i < 6; i++)
			{
				EditorGUILayout.BeginHorizontal();
				for (int j = 0; j < 7; j++)
				{
					m_isDaySelected = m_selectedCalendarDay == m_buttonGridIndex ? true : false;
					if (GUILayout.Toggle(m_isDaySelected, m_target.DayList[m_buttonGridIndex], GUI.skin.button, GUILayout.MinWidth(30)))
					{
						if (m_target.DayList[m_buttonGridIndex] != "")
							m_selectedCalendarDay = m_buttonGridIndex;
					}

					m_buttonGridIndex++;
				}

				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.EndVertical();

			// Options
			EditorGUILayout.PropertyField(RealWorldTimeProp, m_guiContent[0]);
			//EditorGUILayout.PropertyField(m_timeSystem, m_guiContent[0]);
			//EditorGUILayout.PropertyField(m_timeDirection, m_guiContent[10]);
			//EditorGUILayout.PropertyField(m_repeatMode, m_guiContent[1]);

			// Sliders
			/*EditorGUILayout.Slider(m_timeline, 0.0f, 24.0f, m_guiContent[2]);
			EditorGUILayout.Slider(m_latitude, -90.0f, 90.0f, m_guiContent[3]);
			EditorGUILayout.Slider(m_longitude, -180.0f, 180.0f, m_guiContent[4]);
			EditorGUILayout.Slider(m_utc, -12.0f, 12.0f, m_guiContent[5]);
			EditorGUILayout.PropertyField(m_dayLength, m_guiContent[6]);

			// Day-Night length curve
			EditorGUILayout.BeginVertical("Box");
			GUILayout.Space(-5);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			GUILayout.Label("Day and Night Length", EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();
			// Toggle
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(m_guiContent[7]);
			EditorGUILayout.PropertyField(m_isTimeEvaluatedByCurve, GUIContent.none, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal();
			// Reset Button
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("R", GUILayout.Width(25), GUILayout.Height(25)))
				m_dayLengthCurve.animationCurveValue = AnimationCurve.Linear(0, 0, 24, 24);
			// Curve field
			EditorGUILayout.CurveField(m_dayLengthCurve, Color.yellow, new Rect(0, 0, 24, 24), GUIContent.none, GUILayout.Height(25));
			EditorGUILayout.EndHorizontal();
			// Current time display
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(m_guiContent[8]);
			GUILayout.Label(m_dayOfWeekLabel
							+ " " + m_target.hour.ToString("00")
							+ ":" + m_target.minute.ToString("00"),GUILayout.ExpandWidth(false));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();*/
			
			// Update the inspector when there is a change
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(m_target, "Undo Wendy Time Manager");
				if (m_target.selectedCalendarDay != m_selectedCalendarDay)
				{
					m_target.Day = m_selectedCalendarDay + 1 - m_target.GetDayOfWeek(m_target.Year, m_target.Month, 1);
					m_target.selectedCalendarDay = m_selectedCalendarDay;
					//Debug.Log("几号:"+m_target.Day+"calendar day:" + m_selectedCalendarDay + "星期："+m_target.GetDayOfWeek(m_target.Year, m_target.Month, 1));
				}
				
				serializedObject.ApplyModifiedProperties();
				m_target.UpdateCalendar();
			}

			// Updates the calendar if the undo command is performed
			if (Event.current.commandName == "UndoRedoPerformed")
			{
				m_target.UpdateCalendar();
			}
		}
		
		#region UI Operations,这一部分需要注意只用于Editor Mode调试
		private void DecreaseMonth()
		{
			Undo.RecordObject(m_target, "Undo Wendy Time Manager");
			m_target.Month--;

			if (m_target.Month < 1) m_target.Month = 12;
		}

		private void IncreaseMonth()
		{
			Undo.RecordObject(m_target, "Undo Wendy Time Manager");
			m_target.Month++;

			if (m_target.Month > 12) m_target.Month = 1;
		}

		private void DecreaseYear()
		{
			Undo.RecordObject(m_target, "Undo Wendy Time Manager");
			m_target.Year--;

			if (m_target.Year < 0) m_target.Year = 9999;
		}

		private void IncreaseYear()
		{
			Undo.RecordObject(m_target, "Undo Wendy Time Manager");
			m_target.Year++;

			if (m_target.Year > 9999) m_target.Year = 0;
		}
		#endregion
	
}
}

