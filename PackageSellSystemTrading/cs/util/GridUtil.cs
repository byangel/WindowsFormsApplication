using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{
	public class GridUtil
    {
		private static DataGridView oGridDebug = null;
		
		/// <summary>
		/// 디버그용 그리드 설정
		/// </summary>
		/// <param name="oGrid">디비그용 그리드</param>
		public static void SetGridDebug(DataGridView oGrid) {
			if (oGridDebug == null) {
				oGridDebug = oGrid;

				// 그리드 타이틀 설정
				oGridDebug.ColumnCount = 2;
				oGridDebug.Columns[0].Name = "time";
				oGridDebug.Columns[1].Name = "내용";
				oGridDebug.Rows.Clear();
			}
		}	// end function


		/// <summary>
		/// 디버그 그리드 객체 가져오기 
		/// </summary>
		/// <returns></returns>
		private static DataGridView GetGridDebug() {
			if (oGridDebug == null)
			{
				MessageBox.Show("그리드 객체가 없습니다..!!");
			}

			return oGridDebug;
		}


		/// <summary>
		/// 그리드 내용 추가(디버그) 
		/// </summary>
		/// <param name="szContent"></param>
		public static void AddRowDebug(string szContent) {
			Log.WriteLine(szContent);


			//// 메인 프로그램일 경우 로그 정보를 파일에 기록
			//if (FormMain.mProgramId == 0)
			//{
			//    Log.WriteLine(szContent);
			//}
            

			//string[] row = new string[GetGridDebug().ColumnCount];

			//row[0] = DateTime.Now.ToString("HH:mm:ss");
			//row[1] = szContent;

			//GetGridDebug().Rows.Add(row);
            
			//// 최대 1000건 유지
			//if (GetGridDebug().Rows.Count > 1000) {
			//    GetGridDebug().Rows.RemoveAt(0);
			//}

			//// 마지막 스크롤 유지
			//GetGridDebug().FirstDisplayedScrollingRowIndex = GetGridDebug().Rows.Count - 1;
		}
	}	// end class
}	// end namespace
