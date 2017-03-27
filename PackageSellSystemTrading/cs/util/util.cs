using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
                Log.WriteLine(ex.Message);
            }


            //암호화 한 데이터를 스트링으로 변환해서 리턴
            return Encoding.UTF8.GetString(ms.GetBuffer());
        }   // end function


        /// <summary>
        /// 틱 단위 리턴 
        /// </summary>
        /// <param name="iNumber"></param>
        /// <returns></returns>
        public static double GetTick(double iNumber)
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
        public static double GetPricePlus01(double iPrice)
        {
            return iPrice + GetTick(iPrice);
        }


        /// <summary>
        /// +1틱 적용된 가격 리턴 
        /// </summary>
        /// <param name="szPrice"></param>
        /// <returns></returns>
        public static double GetPricePlus01(string szPrice)
        {
            double iPrice = Double.Parse(szPrice);
            return iPrice + GetTick(iPrice);
        }


        /// <summary>
        /// -1틱 적용된 가격 리턴
        /// </summary>
        /// <param name="iPrice"></param>
        /// <returns></returns>
        public static double GetPriceMinus01(double iPrice)
        {
            return iPrice - GetTick(iPrice);
        }


        /// <summary>
        /// -1틱 적용된 가격 
        /// </summary>
        /// <param name="szPrice"></param>
        /// <returns></returns>
        public static double GetPriceMinus01(string szPrice)
        {
            double iPrice = Double.Parse(szPrice);
            return iPrice - GetTick(iPrice);
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
        //dpsastTotamt 예탁자잔총액
        public static String getBattingAmt(String dpsastTotamt)
        {
            
            double resultBattingAmt = 0; //배팅금액
            if (dpsastTotamt!="")
            {
          
                double paseVal = double.Parse(dpsastTotamt);
                if (paseVal < 50000000)//5천마넌보다 작으면 무조건 5만원으로 셋팅.
                {
                    resultBattingAmt = 50000;
                }
                else if (paseVal < 300000000)//3억이하면 /1000 - >5천부터 3억까지는 순차적으로배팅금액상승 (30만원까지)
                {
                    double totalAmt = paseVal / 10000000;
                    //소수점제거 후 배팅금액 구한다.
                    resultBattingAmt = (Math.Floor(totalAmt) * 10000000) / 1000;
                }
                else if (paseVal < 600000000)//6억이하면 30마넌 고정배팅 ->배팅횟수 늘어나는 효과 2000까지
                {
                    resultBattingAmt = 300000;
                }
                else if (paseVal < 2000000000) //20억이하 100마넌까지 배팅금액 상승
                {
                    double totalAmt = paseVal / 10000000;
                    //소수점제거 후 배팅금액 구한다.
                    resultBattingAmt = (Math.Floor(totalAmt) * 10000000) / 2000;
                }
                else if (paseVal > 2000000000) //20억이상 100만원으로 배팅금액 고정 ->평생 삼성주식은 못사겠군...ㅋㅋㅋ
                {
                    resultBattingAmt = 1000000;
                }
            }
            
            //소수점제거 후 배팅금액 구한다.
           
            return resultBattingAmt.ToString();
        }


        //수익률2를 vo에 설정한다.
        public static void setSunikrt2(T0424Vo t0424Vo)
        {
            //수익률 계산
            //수익율2 -> ((현재가*매독가능수량) / ((평균단가2*매도가능수량)+수수료+세금) *100)-100
            Double sunikrt2 = (((double.Parse(t0424Vo.price) * double.Parse(t0424Vo.mdposqt)) / ((double.Parse(t0424Vo.pamt2) * double.Parse(t0424Vo.mdposqt)) + double.Parse(t0424Vo.fee) + double.Parse(t0424Vo.tax))) * 100) - 100;
            t0424Vo.sunikrt2 = String.Format("{0:#0.#0}", sunikrt2);

        }
    }	// end class
}	// end namespace
