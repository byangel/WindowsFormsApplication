using System;
using System.Runtime.InteropServices;

namespace PackageSellSystemTrading
{
	public class SystemTimeUtil
	{
		[DllImport("kernel32")]
		public static extern int SetSystemTime(ref SYSTEMTIME lpSystemTime);

		public struct SYSTEMTIME
		{
			public short wYear;			// 년도
			public short wMonth;		// 월
			public short wDayOfWeek;	// 요일
			public short wDay;			// 일
			public short wHour;			// 시
			public short wMinute;		// 분
			public short wSecond;		// 초
			public short wMilliseconds; // 1/100초
		}

		/// <summary>
		/// PC의 시스템 시간을 설정
		/// </summary>
		/// <param name="szDate">날짜 :: 20140101</param>
		/// <param name="szTime">시간 :: 010101001</param>
		public static void set_system_time(string szDate, string szTime)
		{
			string szYear = szDate.Substring(0, 4);
			string szMonth = szDate.Substring(4, 2);
			string szDay = szDate.Substring(6, 2);

			string szHour = szTime.Substring(0, 2);
			string szMinute = szTime.Substring(2, 2);
			string szSecond = szTime.Substring(4, 2);
			string szMiliSecond = szTime.Substring(6, 3);

			SYSTEMTIME sTime = new SYSTEMTIME();

			sTime.wYear = Convert.ToInt16(szYear);
			sTime.wMonth = Convert.ToInt16(szMonth); ;
			sTime.wDayOfWeek = 1;								// 일요일을 한주의 시작으로 설정
			sTime.wDay = Convert.ToInt16(szDay);
			sTime.wHour = (short)(Convert.ToInt16(szHour) - 9);	// 표준시 계산
			sTime.wMinute = Convert.ToInt16(szMinute);
			sTime.wSecond = Convert.ToInt16(szSecond);
			sTime.wMilliseconds = Convert.ToInt16(szMiliSecond);

			SetSystemTime(ref sTime);
		}	// end function
	}	// end class
}	// end namespace
