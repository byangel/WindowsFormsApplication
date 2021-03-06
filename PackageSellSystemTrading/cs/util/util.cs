﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;



namespace PackageSellSystemTrading
{
    class Util
    {
        static byte[] Skey = ASCIIEncoding.ASCII.GetBytes("11111111");                  // 암호화 키
        
        /// <summary>
        /// 천단위 변환된 숫자 리턴 
        /// </summary>
        /// <param name="szNumber"></param>
        /// <returns></returns>
        public static string GetNumberFormat(string szNumber)
        {
            if (isNaN(szNumber))
            {
                return String.Format("{0:#,##0}", Double.Parse(szNumber.Replace(",", "")));
            }
            else
            {
                return "0";
            }
        }


        /// <summary>
        /// 천단위 변환된 숫자 리턴
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static string GetNumberFormat(double iNumber)
        {
            return String.Format("{0:#,##0}", iNumber);
        }


        /// <summary>
        /// 암호화된 문자열을 리턴 
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static string Encrypt(string p_data)
        {
            // 암호화 알고리즘중 RC2 암호화를 하려면 RC를
            // DES알고리즘을 사용하려면 DESCryptoServiceProvider 객체를 선언한다.
            //RC2 rc2 = new RC2CryptoServiceProvider();
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            // 대칭키 배치
            rc2.Key = Skey;
            rc2.IV = Skey;

            // 암호화는 스트림(바이트 배열)을
            // 대칭키에 의존하여 암호화 하기때문에 먼저 메모리 스트림을 생성한다.
            MemoryStream ms = new MemoryStream();

            //만들어진 메모리 스트림을 이용해서 암호화 스트림 생성 
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);

            // 데이터를 바이트 배열로 변경
            byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray());

            // 암호화 스트림에 데이터 씀
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();

            // 암호화 완료 (string으로 컨버팅해서 반환)
            return Convert.ToBase64String(ms.ToArray());
        }


        /// <summary>
        /// 암호화 해독된 문자열을 리턴
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        public static string Decrypt(string p_data)
        {
            // 암호화는 스트림(바이트 배열)을
            // 대칭키에 의존하여 암호화 하기때문에 먼저 메모리 스트림을 생성한다.
            MemoryStream ms = new MemoryStream();

            try
            {
                // 암호화 알고리즘중 RC2 암호화를 하려면 RC를
                // DES알고리즘을 사용하려면 DESCryptoServiceProvider 객체를 선언한다.
                //RC2 rc2 = new RC2CryptoServiceProvider();
                DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

                // 대칭키 배치
                rc2.Key = Skey;
                rc2.IV = Skey;

                // 암호화는 스트림(바이트 배열)을
                // 대칭키에 의존하여 암호화 하기때문에 먼저 메모리 스트림을 생성한다.
                ms = new MemoryStream();

                //만들어진 메모리 스트림을 이용해서 암호화 스트림 생성 
                CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);

                //데이터를 바이트배열로 변경한다.
                byte[] data = Convert.FromBase64String(p_data);

                //변경된 바이트배열을 암호화 한다.
                cryStream.Write(data, 0, data.Length);

                cryStream.FlushFinalBlock();
            }
            catch (Exception ex)
            {
                Log.WriteLine("util : " + ex.Message);
                Log.WriteLine("util : " + ex.StackTrace);
            }


            //암호화 한 데이터를 스트링으로 변환해서 리턴
            return Encoding.UTF8.GetString(ms.GetBuffer());
        }   // end function

        
            public static double GetTickMinus(double iNumber)
        {
            if (iNumber >= 500000)
            {
                return 1000;
            }
            else if (iNumber > 100000 && iNumber <= 500000)
            {
                return 500;
            }
            else if (iNumber > 50000 && iNumber <= 100000)
            {
                return 100;
            }
            else if (iNumber > 10000 && iNumber <= 50000)
            {
                return 50;
            }
            else if (iNumber > 5000 && iNumber <= 10000)
            {
                return 10;
            }
            else if (iNumber > 1000 && iNumber <= 5000)
            {
                return 5;
            }
            else
            {
                return 1;
            }
        }   // end fucntion
        /// <summary>
        /// 틱 단위 리턴 
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static double GetTickPlus(double iNumber)
        {
            if (iNumber >= 500000)
            {
                return 1000;
            }
            else if (iNumber >= 100000 && iNumber < 500000)
            {
                return 500;
            }
            else if (iNumber >= 50000 && iNumber < 100000)
            {
                return 100;
            }
            else if (iNumber >= 10000 && iNumber < 50000)
            {
                return 50;
            }
            else if (iNumber >= 5000 && iNumber < 10000)
            {
                return 10;
            }
            else if (iNumber >= 1000 && iNumber < 5000)
            {
                return 5;
            }
            else
            {
                return 1;
            }
        }   // end fucntion


        /// <summary>
        /// +1틱 적용된 가격 리턴
        /// </summary>
        /// <param name="iPrice"></param>
        /// <returns></returns>
        public static Double GetPricePlus01(Double iPrice)
        {
            return iPrice + GetTickPlus(iPrice);
        }
        
        /// <summary>
        /// -1틱 적용된 가격 리턴
        /// </summary>
        /// <param name="iPrice"></param>
        /// <returns></returns>
        public static Double GetPriceMinus01(Double iPrice)
        {
            return iPrice - GetTickMinus(iPrice);
        }
        
        public static Double getTickPrice(Double price, Double tick)
        {
            Double returnValue = price;

            if (tick.Equals("시장가"))
            {
                return returnValue;
            }
            if (tick < 0){
                tick = tick * tick;
                for (int i=0; i< tick; i++)
                {
                    returnValue = GetPriceMinus01(returnValue);
                }
            } else {
                for (int i = 0; i < int.Parse(tick.ToString()); i++)
                {
                    returnValue = GetPricePlus01(returnValue); 
                }
            }
            return returnValue;
        }

        /// <summary>
        /// % 적용된 금액 리턴 
        /// </summary>
        /// <param name="iPrice"></param>
        /// <param name="iPercent"></param>
        /// <returns></returns>
        public static double GetPricePercent(double iPrice, double iPercent)
        {
            return iPrice + (iPrice * iPercent / 100);
        }


        /// <summary>
        /// 현재 디렉토리명 리턴
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectoryName()
        {
            string[] aTmp = System.Environment.CurrentDirectory.Split('\\');

            return aTmp[aTmp.Length - 1];
        }


        /// <summary>
        /// 현재 디렉토리명 리턴(전체경로 포함) 
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDirectoryWithPath()
        {
            return System.Environment.CurrentDirectory;
        }


        /// <summary>
        /// 해당 값이 숫자인지 아닌지 체크
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool isNaN(string str)
        {
            double iNum;
            bool bNum = double.TryParse(str, out iNum);
            return bNum;
        }

        //// idx 시작값 생성
        //public static string GenerateIdx()
        //{
        //    double iNum;
        //    bool bNum = double.TryParse(GetCurrentDirectoryName(), out iNum);

        //    if (bNum)
        //    {
        //        return iNum.ToString();
        //    }
        //    else 
        //    {
        //        return "0";
        //    }
        //}

        //배팅금액을 리턴한다.
        //dpsastTotamt 예탁자잔총액 대비 진입비중 금액을 리턴한다.
        //dpsastTotamtMax :최대예탁자산
        //battingRate : 최대예탁자산 대비 배팅 비율
        //public static String getBattingAmt(String dpsastTotamtMax,String battingRate)
        //{
        //    String returnVal="0";
        //    if (isNaN(battingRate)) {
        //        battingRate = nvl(battingRate, "0");
        //        double resultBattingAmt = Math.Round((double.Parse(dpsastTotamtMax) * double.Parse(battingRate)), 0);

        //        returnVal = resultBattingAmt.ToString(); //배팅금액 리턴
        //    }
        //    return returnVal;
            
        //}


        
        public static String nvl(String val,String tmpVal)
        {
            val = val == null ? tmpVal : val;
            val = val == ""   ? tmpVal : val;
            return val;
        }


        public static String getRegValue(String keyName)
        {
            String keyValue = null;
            try{
                RegistryKey subkey = Registry.LocalMachine.OpenSubKey("Software\\AngelSystem");

                keyValue = subkey.GetValue(keyName).ToString();
                Single fsize = Convert.ToSingle(subkey.GetValue("FontName"));
                
            }
            catch
            {
                MessageBox.Show("레지스트리 불러오기 실패");
            }
            return keyValue;
        }

        public static void setRegValue(String key, String value)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey newkey = rk.CreateSubKey("AngelSystem");

                newkey.SetValue(key, value);
            }
            catch
            {
                MessageBox.Show("레지스트리 저장 실패");
            }
        }

        //총자산대비 투자 비율을 리턴한다.
        public static Double getInputRate(Double 매입금액합, Double 예수금D2 )
        {
            Double returnValue = 0;
            try
            {
               
                //Double 매입금액 = mainForm.xing_t0424.mamt;
                //Double 매입금액합 = Double.Parse(mainForm.tradingInfoDt.Rows[0]["매입금액합"].ToString());
                Double 자본금 = 매입금액합 + 예수금D2;
                //-투자금액 제한 옵션이 참이면 AMT_LIMIT 값을 강제로 삽입해준다.- 자본금이 최대운영자금까지는 복리로 운영이 된다.
                if (Properties.Settings.Default.MAX_FUNDS_LIMITED_AT)
                {
                    //이런날이 올까?
                    Double maxFundsLimit = Double.Parse(Properties.Settings.Default.MAX_FUNDS_LIMITED_AMT) * 10000;
                    if (자본금 > maxFundsLimit)
                    {
                        자본금 = maxFundsLimit;
                    }
                }
                
                returnValue = Math.Round(((매입금액합 / 자본금) * 100), 2);
            }
            catch (Exception ex)
            {
                Log.WriteLine("t1833 : " + ex.Message);
                Log.WriteLine("t1833 : " + ex.StackTrace);
            }
            return returnValue;
        }

        //비율계산
        public static Double getRate(Double val0, Double val1)
        {
            Double returnValue = 0;
            
            returnValue = Math.Round(((val0 / val1) * 100), 2);
           
            return returnValue;
        }

        //옵션에서 사용 파일선택 콤보 박스에서 파일명을 추출하여 key,value 로 추가한다.--나중에 배열 처리도 추가해야한다.
        public static void setComBoByKeyValue(ComboBox combo, String fileFullNm)
        {
            if (fileFullNm != "선택")
            {
                String lastString = getShortFileNm(fileFullNm);
                Dictionary<String, String> buySearchCbxSource = new Dictionary<String, String>();
                buySearchCbxSource.Add("선택", "");
                buySearchCbxSource.Add(lastString, fileFullNm);
                combo.DataSource = null;
                combo.DataSource = new BindingSource(buySearchCbxSource, null);
                combo.DisplayMember = "Key";
                combo.ValueMember = "Value";
                combo.SelectedIndex = 1;

               
               
            }
            else
            {
  
                Dictionary<String, String> buySearchCbxSource = new Dictionary<String, String>();
                buySearchCbxSource.Add("선택", "");
                combo.DataSource = null;
                combo.DataSource = new BindingSource(buySearchCbxSource, null);
                combo.DisplayMember = "Key";
                combo.ValueMember = "Value";
                combo.SelectedIndex = 0;

            }



        }
        //확장자를 제거한 파일명만 리턴한다.
        public static String getShortFileNm(String fileFullNm)
        {   String returnVal="";
            if (fileFullNm != "" && fileFullNm != "선택")
            {
                returnVal = fileFullNm.Substring(fileFullNm.LastIndexOf("\\") + 1);
                if (returnVal.IndexOf(".") < 0) return "";
                returnVal = returnVal.Substring(0, returnVal.IndexOf("."));
            }
            return returnVal;
        }

        public static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            while(AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
                Thread.Sleep(1);
            }
            return DateTime.Now;
        }

        public static void AppendTextCrossThread(Label label, string appendText)
        {
           
            if (label.InvokeRequired)
            {
                label.BeginInvoke(new MethodInvoker(delegate ()
                {
                    label.Text = appendText;
                }));
            }
            else
            {
                label.Text = appendText;
            }
        }

        //매수가능시간 비교
        public static Boolean isBuyTime()
        {
            
            //TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
            TimeSpan nowTimeSpan = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            DateTime buyTimeFrom = Properties.Settings.Default.BUY_TIME_FROM;
            DateTime buyTimeTo   = Properties.Settings.Default.BUY_TIME_TO;
            if (nowTimeSpan >= buyTimeFrom.TimeOfDay && nowTimeSpan <= buyTimeTo.TimeOfDay){
                return true;
            }
            return false;
        }

        //추가매수 가능시간 여부
        public static Boolean isAddBuyTime()
        {

            //TimeSpan nowTimeSpan = TimeSpan.Parse(mainForm.xing_t0167.hour + ":" + mainForm.xing_t0167.minute + ":" + mainForm.xing_t0167.second);
            TimeSpan nowTimeSpan = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            DateTime buyTimeFrom = Properties.Settings.Default.ADD_BUY_SEARCH_TIME_FROM;
            DateTime buyTimeTo = Properties.Settings.Default.ADD_BUY_SEARCH_TIME_TO;
            if (nowTimeSpan >= buyTimeFrom.TimeOfDay && nowTimeSpan <= buyTimeTo.TimeOfDay)
            {
                return true;
            }
            return false;
        }

        //매도가능시간 체크
        public static Boolean isSellTime() { 
            //매도타임
            TimeSpan nowTimeSpan = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //매수 가능 시간 비교
            DateTime sellTimeFrom = Properties.Settings.Default.SELL_TIME_FROM;
            DateTime sellTimeTo   = Properties.Settings.Default.SELL_TIME_TO;
            if (nowTimeSpan >= sellTimeFrom.TimeOfDay && nowTimeSpan <= sellTimeTo.TimeOfDay){
                return true;
            }
            
            return false;
        }

        //손절가능시간 체크
        public static Boolean isStopTime()
        {
            //매도타임
            TimeSpan nowTimeSpan = TimeSpan.Parse(DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //매수 가능 시간 비교
            DateTime sellTimeFrom = Properties.Settings.Default.STOP_TIME_FROM;
            DateTime sellTimeTo = Properties.Settings.Default.STOP_TIME_TO;
            if (nowTimeSpan >= sellTimeFrom.TimeOfDay && nowTimeSpan <= sellTimeTo.TimeOfDay)
            {
                return true;
            }

            return false;
        }

        //코스피 매수 가능 여부 - 코스피 등락율과 등락포인트를 인자로 넣어준다.
        public static Boolean isKosBuyAt(Double ks_drate, Double ks_change)
        {
            Boolean returnVal = true;
            
            //코스피 매수가능여부
            if (Properties.Settings.Default.KOS_AT)//코스닥여부
            {
                //전일대비
                if (Properties.Settings.Default.KOS_YESTERDAY_AT)
                {
                    Double kosYesterdayVal = Double.Parse(Properties.Settings.Default.KOS_YESTERDAY_VAL);//전일대비 등락율 설정값
                    String kosYesterdayValSe = Properties.Settings.Default.KOS_YESTERDAY_VAL_SE;
                    if (kosYesterdayValSe.Equals("%"))
                    {
                        
                        if (ks_drate < kosYesterdayVal){
                            returnVal = false;
                        }
                    }
                    if (kosYesterdayValSe.Equals("pt")){
                        if (ks_change < kosYesterdayVal){
                            returnVal = false;
                        }
                    }
                }
                //시가대비 
                if (Properties.Settings.Default.KOS_START_AT)
                {
                    Double kosStartVal = Double.Parse(Properties.Settings.Default.KOS_START_VAL);//전일대비 등락율 설정값
                    String kosStartValSe = Properties.Settings.Default.KOS_START_VAL_SE;
                    if (kosStartValSe.Equals("%")){
                      
                        if (ks_drate < kosStartVal){
                            returnVal = false;
                        }
                    }
                    if (kosStartValSe.Equals("pt")){
                       
                        if (ks_change < kosStartVal){
                            returnVal = false;
                        }
                    }
                }
            }
            return returnVal;
        }
     
        //코스닥 매수 가능 여부- 코스닥 등락율과 등락 포인트를 인자로 넣어준다.
        public static Boolean isKodBuyAt(Double kd_drate, Double kd_change)
        {
            Boolean returnVal = true;
            //코스닥 매수가능여부
            if (Properties.Settings.Default.KOD_AT)
            {
                //전일대비
                if (Properties.Settings.Default.KOS_YESTERDAY_AT)
                {
                    Double kodYesterdayVal = Double.Parse(Properties.Settings.Default.KOD_YESTERDAY_VAL);//전일대비 등락율 설정값
                    String kodYesterdayValSe = Properties.Settings.Default.KOD_YESTERDAY_VAL_SE;
                    if (kodYesterdayValSe.Equals("%")){
                        if (kd_drate < kodYesterdayVal){
                            returnVal = false;
                        }
                    }
                    if (kodYesterdayValSe.Equals("pt")){
                        
                        if (kd_change < kodYesterdayVal){
                            returnVal = false;
                        }
                    }
                }
                //시가대비 
                if (Properties.Settings.Default.KOS_START_AT)
                {
                    Double kodStartVal = Double.Parse(Properties.Settings.Default.KOD_START_VAL);//전일대비 등락율 설정값
                    String kodStartValSe = Properties.Settings.Default.KOD_START_VAL_SE;
                    if (kodStartValSe.Equals("%")){
                        
                        if (kd_drate < kodStartVal){
                            returnVal = false;
                        }
                    }
                    if (kodStartValSe.Equals("pt")){
                        
                        if (kd_change < kodStartVal){
                            returnVal = false;
                        }
                    }
                }
            }
            return returnVal;
        }

    }	// end class
}	// end namespace
