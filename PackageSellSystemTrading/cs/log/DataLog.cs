using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;


namespace PackageSellSystemTrading
{

    /// <summary>
    /// 실시간 체결정보를 기록하는 클래스 - 금일매수매도를 구현을 목표로한다.
    /// </summary>
    /// <returns>StreamWriter</returns>
    public class DataLog
    {
        private  FileStream   fileStream;
        private  StreamReader streamReader;             // 파일 읽기 위한 스트림

        public  EBindingList<DataLogVo> dataLogVoList;

        public MainForm mainForm;

        // ---- ini 파일 의 읽고 쓰기를 위한 API 함수 선언 ----
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
       


        // 생성자
        public DataLog()
        {
            dataLogVoList = new EBindingList<DataLogVo>();
 
        }  
        // 소멸자
        ~DataLog()
        {         
            /// Log 파일 쓰기 스트림 종료
            if (streamReader != null)
            {
                streamReader.Close();
                fileStream.Close();
            }
        }

       

        public void init() { 
        
            //디렉토리 체크
            String filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.ini";
            String dirPath  = Util.GetCurrentDirectoryWithPath() + "\\logs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // 기존에 생성된 data 로그 파일이 있다면...
            FileInfo file = new FileInfo(filePath);
           
            if (file.Exists) {
                fileStream = new FileStream(filePath, FileMode.Open);//Append 에러남
                    //파일이있다면 파일을 읽어 voList와 싱크를 맞춘다.
                this.streamReader = new StreamReader(fileStream, Encoding.GetEncoding("euc-kr"));
                String lineString;
                String[] splitResult;
                while (!streamReader.EndOfStream){
                    // 한줄을 읽습니다
                    lineString = streamReader.ReadLine();
                    splitResult = lineString.Split(new char[] { '=','|' } );
                    if (splitResult.Length> 1){
                        DataLogVo dataLogVo  = new DataLogVo();
                        dataLogVo.ordno      = splitResult[0]; //주문번호 
                        dataLogVo.date       = splitResult[1];
                        dataLogVo.time       = splitResult[2];
                        dataLogVo.accno      = splitResult[3]; //계좌번호
                        dataLogVo.ordptncode = splitResult[4]; //주문구분 01:매도|02:매수   
                        dataLogVo.Isuno      = splitResult[5]; //종목코드
                        dataLogVo.ordqty     = splitResult[6]; //주문수량
                        dataLogVo.execqty    = splitResult[7]; //체결수량
                        dataLogVo.ordprc     = splitResult[8]; //주문가격
                        dataLogVo.execprc    = splitResult[9]; //체결가격
                        dataLogVo.Isunm      = splitResult[10];//종목명

                        dataLogVoList.Add(dataLogVo);
                        //vo에 저장
                    }
                }
                //여기서 닫아줘야 WriteLine 쓸수있다.
                streamReader.Close();
                fileStream.Close();
                Log.WriteLine("거래이력 데이타 파일 과 싱크 완료"+ dataLogVoList.Count());
            }
            else{// 신규로 파일 생성
               
                fileStream = new FileStream(filePath, FileMode.Create);
                //여기서 닫아줘야 WriteLine 쓸수있다.
                fileStream.Close();

                //데이타파일이없다면 잔고목록으로 초기 데이타로 사용한다.
                EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)mainForm.grd_t0424.DataSource);
                for (int i = 0; i < t0424VoList.Count(); i++)
                {   
                    DataLogVo dataLogVo  = new DataLogVo();
                    dataLogVo.ordno      = "000000"+i.ToString();                       //주문번호
                    dataLogVo.accno      = t0424VoList.ElementAt(i).expcode;   //계좌번호
                    dataLogVo.ordptncode = "02";                          //주문구분 01:매도|02:매수
                    dataLogVo.Isuno      = t0424VoList.ElementAt(i).expcode;   //종목코드
                    dataLogVo.ordqty     = t0424VoList.ElementAt(i).mdposqt;  //주문수량 - 매도가능수량
                    dataLogVo.execqty    = t0424VoList.ElementAt(i).mdposqt; //체결수량 - 매도가능수량
                    dataLogVo.ordprc     = t0424VoList.ElementAt(i).pamt;     //주문가격 - 평균단가
                    dataLogVo.execprc    = t0424VoList.ElementAt(i).pamt;    //체결가격 - 평균단가
                    dataLogVo.Isunm      = t0424VoList.ElementAt(i).hname;     //종목명

                    this.WriteLine(dataLogVo);
                }
                Log.WriteLine("거래이력 데이타 파일 생성 완료");
            }           
        }


        /// <summary>
        /// Log 파일에 메세지 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public  void WriteLine(DataLogVo dataLogVo)
        {
            
            //체결된 주분정보가 같은 주문번호여부를 따진다.
            int i = dataLogVoList.Find("ordno", dataLogVo.ordno);
            
            //dataLogVoList에 주문번호정보여부에따라 매수정보 수정및 추가를 해준다. 
            if (i >= 0)//같은주문번호가 있으면 체결량을 합해준다.
            {
                DataLogVo tmpDataLogVo = dataLogVoList.ElementAt(i);
                //매수 체결수량 = 매수 체결수량 + 매수 체결수량  -매도/매수 상관없이 체결수량을 합해준다.
                String tmpExecqty = (int.Parse(tmpDataLogVo.execqty) + int.Parse(dataLogVo.execqty)).ToString();

                tmpDataLogVo.execqty = tmpExecqty;
                dataLogVo.execqty    = tmpExecqty;

            } else {//같은주문번호가 없다면 목록에 추가해준다.
                dataLogVoList.Add(dataLogVo);
            }
            
            String ordno = dataLogVo.ordno; //주문번호 
            StringBuilder sb = new StringBuilder();
            sb.Append("" + DateTime.Now.ToString("yyyyMMdd|HH:mm:ss"));         
            sb.Append("|" + dataLogVo.accno);     //계좌번호
            sb.Append("|" + dataLogVo.ordptncode);//주문구분 01:매도|02:매수   
            sb.Append("|" + dataLogVo.Isuno);     //종목코드
            sb.Append("|" + dataLogVo.ordqty);    //주문수량
            sb.Append("|" + dataLogVo.execqty);   //체결수량
            sb.Append("|" + dataLogVo.ordprc);    //주문가격
            sb.Append("|" + dataLogVo.execprc);   //체결가격
            sb.Append("|" + dataLogVo.Isunm);     //종목명

            //ini 쓰기 주문번호로 같은주문번호가 있으면 업데이트 없으면 추가.--폴더는 추가되지 않는다.
            String filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.ini";
            WritePrivateProfileString("DATA", ordno, sb.ToString(), filePath);
          
            //ini 읽기
            //StringBuilder retOrd = new StringBuilder();
            //GetPrivateProfileString("LOGIN", ordno, "", retOrd, 100, filePath);
            Log.WriteLine("DataLog::" + dataLogVo.Isunm + "(" + dataLogVo.Isuno + ") 거래이력 기록 완료. [주문수량: " + dataLogVo.ordqty + "|체결수량:"+ dataLogVo.execqty + "]");

        }   // end function




    }   // end class


    public class DataLogVo
    {
        public String date    { set; get; }//일자
        public String time    { set; get; }//시간
        public String accno   { set; get; }//계좌번호
        public String ordno   { set; get; }//매수 주문번호
        public String Isuno   { set; get; }//종목코드
        public String Isunm   { set; get; }//종목명

        public String ordqty  { set; get; }// 주문수량
        public String ordprc  { set; get; }// 주문가격
        public String execqty { set; get; }// 체결수량
        public String execprc { set; get; }// 체결가격

        public String ordptncode { set; get; }//주문구분 01:매도|02:매수 
    }

}   // end namespace
