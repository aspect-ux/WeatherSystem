using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Wendy
{
	public enum EnableFeature
	{
		Enabled = 0, Disabled = 1
	}
	public class TimeManager : MonoBehaviour
	{
		#region Params For Editor
		public int selectedCalendarDay = 0;
		
		public int startMonth = 1,startYear = 2024,startDay = 1;
		#endregion
		#region Params For Scirpt
		
		/* Class cannot be used in inspector editor script
		public class WendyTime
		{
			public int Minute = 0;
		
			public int Hour = 0;
			
			public int Day = 1;
			
			public int Month = 1;
			
			public int Year = 2024;
			
			public WendyTime()
			{	
				Minute = 0;
				Hour = 0;
				Day = 1;
				Month = 1;
				Year = 2024;
			}
		}
		
		// Wendy Weather System Time
		public WendyTime wendyTime = new WendyTime();
		*/
		
		// Wendy Time
		public int Minute = 0;
		
		public int Hour = 0;
		
		public int Day = 1;
		
		public int Month = 1;
		
		public int Year = 2023;
		
		
		
		// Real Time 系统时间/真实时间
		public System.DateTime wendyDate;
		
		// Event
		public UnityEvent OnHourChangeEvent;
		public UnityEvent OnDayChangeEvent;		
		public UnityEvent OnMonthChangeEvent;
		public UnityEvent OnYearChangeEvent;
		
		
		
		public int StartingMinute = 0;
		
		public int StartingHour = 0;
		
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
		
		public CurrentTimeOfDayEnum CurrentTimeOfDay;
		public enum CurrentTimeOfDayEnum
		{
			Morning = 0, Day, Evening, Night
		}
		
		int m_LastHour;
		
		#endregion
		
		
		// Start is called before the first frame update
		void Start()
		{
			
		}
		public void Initialize()
		{
			// 由于WeatherSystem.Start中嵌套Start不能保证执行顺序，所以这里重新写一个函数用于初始化
			// 设置初始时间
			//Calculates our start time based off the user's input
			// 计算当前天数
			float startingMinuteFloat = (int)Minute;
			if (RealWorldTime == EnableFeature.Disabled)
			{
				m_TimeFloat = (float)Hour / 24 + startingMinuteFloat / 1440;
			}
			else if (RealWorldTime == EnableFeature.Enabled)
			{
				m_TimeFloat = (float)System.DateTime.Now.Hour / 24 + (float)System.DateTime.Now.Minute / 1440;
			}

			m_LastHour = Hour;
		}

		// Update is called once per frame
		void Update()
		{
			// TODO: if weather system initialized
			//Only calculate our time if TimeFlow is enabled
			if (TimeFlow == EnableFeature.Enabled)
			{
				if (RealWorldTime == EnableFeature.Disabled)
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
				else if (RealWorldTime == EnableFeature.Enabled)
				{
					// 开启时间流动 + 现实时间(天)
					m_TimeFloat = (float)System.DateTime.Now.Hour / 24 + (float)System.DateTime.Now.Minute / 1440;
				}

				// 当一天过去后
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
		}
		
		/// <summary>
		/// 根据具体时间计算当前是 早上，晚上，白天，凌晨
		/// 时间抽象表达
		/// </summary>
		void CalculatePeriodOfDay()
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
		
		
		//Calculates our days and updates our Animation curves.
		void CalculateDays()
		{
			//CalculatePrecipiation();
			//TODO: Temperature influence day time
			//TemperatureCurve.Evaluate(m_PreciseCurveTime);

			Day++; //Add a day to our Day variable
			CalculateMonths(); //Calculate our months
			//CalculateSeason(); //Calculate our seasons
			OnDayChangeEvent.Invoke(); //Invoke our day events
			GetDate(); //Calculate the DateTime

			//Clears our hourly forecast and generates a new one for the current day
			/*if (WeatherGenerationMethod == WeatherSystem.WeatherGenerationMethodEnum.Hourly)
			{
				WeatherForecast.Clear();
				GenerateWeather();
			}*/
		}

		//计算月份 Calculates our months for an accurate calendar that also calculates leap year
		void CalculateMonths()
		{
			// DateTime.DaysInMonth可以直接获取一个月中有几天
			// 1. 累计日期计算月份
			// 31天的月份(日期为1-31), caculate days into months when days number is over 31 
			if (Day >= 32 && (Month == 1 || Month == 3 || Month == 5 || Month == 7
				|| Month == 8 || Month == 10 || Month == 12)){
				Day %= 32;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			// 30天的月份(日期1-30)，caculate days into months when days number is over 30
			if (Day >= 31 && (Month == 4 || Month == 6 || Month == 9 || Month == 11)){
				Day %= 31;
				Day += 1;
				Month += 1;
				OnMonthChangeEvent.Invoke(); //Invoke our Month events
			}

			// 闰年Leap Year, 普通闰年和世纪闰年
			// normal: 4的倍数但不是100的倍数
			// century: 400的倍数
			// 闰年即有闰日的一年，2月份29天，非闰年28天
			if (Day >= 30 && Month == 2 && (Year % 4 == 0 && Year % 100 != 0) || (Year % 400 == 0)) {
				Day %= 30;
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

			// 2. 累计月份计算年份
			if (Month > 12) {
				Month = Month % 13;
				Year += 1;
				Month += 1;
				OnYearChangeEvent.Invoke(); //Invoke our Year events

				//Reset our m_roundingCorrection variable to 0
				m_roundingCorrection = 0;
			}
		}
		
		// 获取当前时间（分为真实时间和自定义时间）,系统时间有两种获取方式，wendyTime(自定义类,core)，和wendyDate(DateTime)
		public System.DateTime GetDate()
		{
			if (RealWorldTime == EnableFeature.Disabled)
			{
				wendyDate = new System.DateTime(Year, Month, Day, Hour, Minute, 0);
			}
			else if (RealWorldTime == EnableFeature.Enabled)
			{
				wendyDate = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 
				Hour, Minute, 0);// 自定义时间的小时与分钟和真实世界是一样的
				Year = wendyDate.Year;
				Month = wendyDate.Month;
				Day = wendyDate.Day;
			}

			return wendyDate;
		}
		
		
		
		#region Time Manager Editor Update
		/// <summary>
		/// 用于Editor日历的显示
		/// </summary>
		public void UpdateCalendar()
		{
			// Get the number of days in the current month
			int m_daysInMonth = DateTime.DaysInMonth(Year, Month);

			// Avoid selecting a date that does not exist
			//int day = Mathf.Clamp(Day, 1, m_daysInMonth);
			//int month = Mathf.Clamp(Month, 1, 12);
			//int year = Mathf.Clamp(Year, 0, 9999);

			// Creates a custom DateTime at the first day of the current month
			DateTime m_dateTime = new DateTime(Year, Month, 1);
			// 返回星期几(0-6)
			int m_dayOfWeek = (int) m_dateTime.DayOfWeek;
			// selectedCalendarDay是日历上显示的天数，需要根据inspector的显示美观性进行偏移
			// 范围是[0,36]
			selectedCalendarDay = Day - 1 + m_dayOfWeek;
			//selectedCalendarDay = Day;
			
			for (int i = 0; i < DayList.Length; i++)
			{
				// 超出范围的置空
				if (i < m_dayOfWeek || i >= (m_dayOfWeek + m_daysInMonth))
				{
					DayList[i] = "";
					continue;
				}

				// Sets the day number only on the valid buttons of the current month in use by the calendar.
				m_dateTime = new DateTime(Year, Month, (i - m_dayOfWeek) + 1);
				DayList[i] = m_dateTime.Day.ToString();
			}
		}
		
		/// <summary>
		/// String array that stores the name of each day of the week.
		/// </summary>
		public readonly string[] WeekList = new string[]
		{
			"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
		};
		
		/// <summary>
		/// String array that stores the name of each month.
		/// </summary>
		public readonly string[] MonthList = new string[]
		{
			"January", "February", "March", "April", "May", "June",
			"July", "August", "September", "October", "November", "December"
		};
		
		/// <summary>
		/// Array with 42 numeric strings used to fill a calendar.
		/// </summary>
		public readonly string[] DayList = new string[]
		{
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
			"11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
			"21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
			"31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
		};
		
		/// <summary>
		/// Sets a new custom date.
		/// </summary>
		public void SetNewDate(int year, int month, int day)
		{
			Year = year;
			Month = month;
			Day = day;
			UpdateCalendar();
		}
		
		/// <summary>
		/// Sets a new custom day.
		/// </summary>
		public void SetNewDay(int day)
		{
			Day = day;
			UpdateCalendar();
		}

		/// <summary>
		/// Sets a new custom month.
		/// </summary>
		public void SetNewMonth(int month)
		{
			Month = month;
			UpdateCalendar();
		}
		
		/// <summary>
		/// Sets a new custom year.
		/// </summary>
		public void SetNewYear(int year)
		{
			Year = year;
			UpdateCalendar();
		}
		
		/// <summary>
		/// Returns the current date converted to string using the default format used by Azure.
		/// </summary>
		public string GetDateString()
		{
			// Format: "MMMM dd, yyyy"
			return MonthList[Month - 1] + " " + Day.ToString("00") + ", " + Year.ToString("0000");
		}
		
		/// <summary>
		/// Returns the current date converted to string using a custom format.
		/// </summary>
		public string GetDateString(string format)
		{
			GetDate();
			return wendyDate.ToString(format);
		}
		
		/// <summary>
		/// Computes the time progression step based on the day length value.
		/// </summary>
		private float GetTimeProgressionStep()
		{
			if (DayLength > 0.0f)
				return (24.0f / 60.0f) / DayLength;
			else
				return 0.0f;
		}
		
		/// <summary>
		/// Gets the current day of the week and return an integer between 0 and 6.
		/// </summary>
		public int GetDayOfWeek()
		{
			GetDate();
			return (int) wendyDate.DayOfWeek;
		}
		
		/// <summary>
		/// 根据具体日期获取星期几(0-6)
		/// </summary>
		public int GetDayOfWeek(int year, int month, int day)
		{
			DateTime temp = new DateTime(year, month, day);
			return (int) temp.DayOfWeek;
		}
		
		/// <summary>
		/// Gets the current day of the week and return as string.
		/// </summary>
		public string GetDayOfWeekString()
		{
			GetDate();
			return WeekList[(int) wendyDate.DayOfWeek];
		}
		
		/// <summary>
		/// Gets the day of the week from a custom date and return as string.
		/// </summary>
		public string GetDayOfWeekString(int year, int month, int day)
		{
			GetDate();
			return WeekList[(int) wendyDate.DayOfWeek];
		}
	#endregion
	}

}
