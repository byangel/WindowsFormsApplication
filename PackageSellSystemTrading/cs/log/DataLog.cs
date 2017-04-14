using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

//1.최초 프로그램 시작하고 잔고 목록을 가져올때 "평균단가" 및 매매횟수가 필요하다.
//2.매도및 매수 주문 체결이 되었을때 최초주문말고 금일매도가 일어났을때 필요하지만 그냥 최초매수, 추가매수, 금일매도 이벤트 발생시 평균단가 계산후 잔고정보를 업데이트해줘야한다.
// -



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
        private  String filePath;
        private  EBindingList<DataLogVo> dataLogVoList;

        public MainForm mainForm;

        // ---- ini 파일 의 읽고 쓰기를 위한 API 함수 선언 ----
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
       


        // 생성자
        public DataLog()
        {
            this.dataLogVoList = new EBindingList<DataLogVo>();
            this.filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.ini";

            ////////////////////////////////////////////////////////////////////
            //디렉토리 체크
            String dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            // 기존에 생성된 data 로그 파일이 있다면...
            FileInfo file = new FileInfo(this.filePath);
            if (file.Exists)
            {
                fileStream = new FileStream(filePath, FileMode.Open);//Append 에러남
                //파일이있다면 파일을 읽어 voList와 싱크를 맞춘다.
                this.streamReader = new StreamReader(fileStream, Encoding.GetEncoding("euc-kr"));
                String lineString;
                String[] splitResult;
                while (!streamReader.EndOfStream)
                {
                    // 한줄을 읽습니다
                    lineString = streamReader.ReadLine();
                    splitResult = lineString.Split(new char[] { '=', '|' });
                    if (splitResult.Length > 1)
                    {
                        DataLogVo dataLogVo = new DataLogVo();
                        dataLogVo.ordno = splitResult[0]; //주문번호 
                        dataLogVo.date = splitResult[1];
                        dataLogVo.time = splitResult[2];
                        dataLogVo.accno = splitResult[3]; //계좌번호
                        dataLogVo.ordptncode = splitResult[4]; //주문구분 01:매도|02:매수   
                        dataLogVo.Isuno = splitResult[5].Replace("A", ""); //종목코드
                        dataLogVo.ordqty = splitResult[6]; //주문수량
                        dataLogVo.execqty = splitResult[7]; //체결수량
                        dataLogVo.ordprc = splitResult[8]; //주문가격
                        dataLogVo.execprc = splitResult[9]; //체결가격
                        dataLogVo.Isunm = splitResult[10];//종목명

                        this.dataLogVoList.Add(dataLogVo);
                        //vo에 저장
                    }
                }
                //여기서 닫아줘야 WriteLine 쓸수있다.
                streamReader.Close();
                fileStream.Close();
                Log.WriteLine("DataLog 생성자 :: dataLog 파일 로드 완료 [로드 종목수:" + dataLogVoList.Count() + "]");
            }else{
                fileStream = new FileStream(filePath, FileMode.Create);
                //여기서 닫아줘야 WriteLine 쓸수있다.
                fileStream.Close();
            }

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
       

        //종목 코드 --총매수금액과 매도가능수량을 구해서 평균단가를 리턴한다. 
        public HistoryVo getHistoryVo(String Isuno){
            

          

            HistoryVo historyVo = null;
           
            //종목 매매 목록을 구한다.
            var resultDataLogVoList =  from item in this.dataLogVoList
                                      where item.Isuno == Isuno && item.accno == mainForm.accountForm.account
                                      select item;
           
            //매도 거래 정보
            var groupDataLogVoList = from item in resultDataLogVoList
                                      group item by  item.ordptncode == "02" into g
                           select new {  goupKey    = g.Key 
                                       , groupVoList= g 
                                       , 매매횟수    = g.Count()
                                       , 거래금액합  = g.Sum( o => int.Parse(o.execqty) *int.Parse(o.execprc) )
                                       , 체결수량합  = g.Sum(o => int.Parse(o.execqty))
                                      };
            //groupping 매도:false | 매수:true

            //dataLogVoList에 매수 정보가 없다는것은 잔고목록과 매핑이 잘되지 않았다는거다...else구문을 구현해주낟.
            if (resultDataLogVoList.Count() > 0){
                historyVo = new HistoryVo();
                //로그출력
                double 총매수금액 = 0;
                double 매도가능수량 = 0;
                double 중간매도손익 = 0;
                foreach (var group in groupDataLogVoList) {

                    if (group.goupKey)//매수그룹
                    {
                        총매수금액 = 총매수금액 + group.거래금액합;
                        매도가능수량 = 매도가능수량 + group.체결수량합;
                        historyVo.buyCnt = group.매매횟수.ToString();
                    }
                    else//매도그룹
                    {
                        중간매도손익 = (group.거래금액합 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100));
                        //매도 거래금액합 - (2%수익금) 해주어야 
                        총매수금액 = 총매수금액 - (group.거래금액합 - (group.거래금액합 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100)));
                        매도가능수량 = 매도가능수량 - group.체결수량합;
                        historyVo.sellCnt = group.매매횟수.ToString();
                        historyVo.sellSunik = 중간매도손익.ToString();
                    }
                    Log.WriteLine("DataLog::[key:" + group.goupKey + "거래금액합:" + group.거래금액합 + "체결수량합:" + group.체결수량합 + "|매매횟수:" + group.매매횟수 + "|중간매도손익:" + 중간매도손익);
                    foreach (var item in group.groupVoList)
                    {
                        Log.WriteLine("DataLog::" + item.Isunm + "(" + item.Isuno + ")[거래일자:"+ item.date+ "|주문구분:" + item.ordptncode + "|주문수량:" + item.ordqty + "|체결수량:" + item.execqty + "|주문가격:" + item.ordprc + "|체결가격:" + item.execprc);

                        //Log.WriteLine("===========================================");
                    }
                }
                Log.WriteLine("DataLog:: [매도가능수량:" + 매도가능수량+ "|매수총금액:"+ 총매수금액 + "]");
                if (매도가능수량 != 0)
                {
                    double 평균단가 = (총매수금액 / 매도가능수량);
                    //historyVo.pamt2 = Util.GetNumberFormat(평균단가.ToString());
                    historyVo.pamt2 = 평균단가.ToString();
                    //Log.WriteLine("DataLog::[평균단가:" + 평균단가.ToString() + "매도가능수량:" + 매도가능수량);
                } else
                {
                    return null;
                }
                Log.WriteLine("===========================================");   
            } else {//종목번호로 데이타로그가 없을때. 데이타 로그에 등록해주고  historyVo를 설정해준다.
                EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
                int i = t0424VoList.Find("expcode", Isuno.Replace("A", ""));
                if (i >= 0) { 
                   
                    this.insertData(t0424VoList.ElementAt(i),"000"+i);

                    //종목히스토리 수동 설정
                    historyVo = new HistoryVo();
                    historyVo.buyCnt = "1";
                    historyVo.sellCnt = "";
                    historyVo.sellSunik ="";
                    historyVo.pamt2 = t0424VoList.ElementAt(i).pamt;
                }else {
                    Log.WriteLine("DataLog::뭐여? 왜 잔고그리드에 종목정보가 없어?" );
                    return null;
                }
            }
            return historyVo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
       public EBindingList<DataLogVo> getDataLogVoList()
        {
            return this.dataLogVoList;
        }


        //프로그램 시작시 호출 - data 로그파일이 있다면 voList 만들고 로그파일없으면 잔고목록을 기준으로 로그파일신규생성하고 voList만든다.
        //public void init() {
        //    //계좌잔고 목록
        //    EBindingList<T0424Vo> t0424VoList = mainForm.xing_t0424.getT0424VoList();
        //    //디렉토리 체크
        //    String dirPath  = Util.GetCurrentDirectoryWithPath() + "\\logs";
        //    if (!Directory.Exists(dirPath))
        //    {
        //        Directory.CreateDirectory(dirPath);
        //    }

        //    // 기존에 생성된 data 로그 파일이 있다면...
        //    FileInfo file = new FileInfo(this.filePath);
        //    if (file.Exists) {
        //        fileStream = new FileStream(filePath, FileMode.Open);//Append 에러남
        //            //파일이있다면 파일을 읽어 voList와 싱크를 맞춘다.
        //        this.streamReader = new StreamReader(fileStream, Encoding.GetEncoding("euc-kr"));
        //        String lineString;
        //        String[] splitResult;
        //        while (!streamReader.EndOfStream){
        //            // 한줄을 읽습니다
        //            lineString = streamReader.ReadLine();
        //            splitResult = lineString.Split(new char[] { '=','|' } );
        //            if (splitResult.Length> 1){
        //                DataLogVo dataLogVo  = new DataLogVo();
        //                dataLogVo.ordno      = splitResult[0]; //주문번호 
        //                dataLogVo.date       = splitResult[1];
        //                dataLogVo.time       = splitResult[2];
        //                dataLogVo.accno      = splitResult[3]; //계좌번호
        //                dataLogVo.ordptncode = splitResult[4]; //주문구분 01:매도|02:매수   
        //                dataLogVo.Isuno      = splitResult[5].Replace("A", ""); //종목코드
        //                dataLogVo.ordqty     = splitResult[6]; //주문수량
        //                dataLogVo.execqty    = splitResult[7]; //체결수량
        //                dataLogVo.ordprc     = splitResult[8]; //주문가격
        //                dataLogVo.execprc    = splitResult[9]; //체결가격
        //                dataLogVo.Isunm      = splitResult[10];//종목명

        //                dataLogVoList.Add(dataLogVo);
        //                //vo에 저장
        //            }
        //        }
        //        //여기서 닫아줘야 WriteLine 쓸수있다.
        //        streamReader.Close();
        //        fileStream.Close();
        //        Log.WriteLine("DataLog.init :: dataLog 파일 로드 완료 [로드 종목수:"+ dataLogVoList.Count()+"]");
        //    }else{// 신규로 파일 생성
               
        //        fileStream = new FileStream(filePath, FileMode.Create);
        //        //여기서 닫아줘야 WriteLine 쓸수있다.
        //        fileStream.Close();

        //        //데이타파일이없다면 잔고목록으로 초기 데이타로 사용한다.
        //        for (int i = 0; i < t0424VoList.Count(); i++)
        //        {   
        //            DataLogVo dataLogVo  = new DataLogVo();
        //            dataLogVo.ordno      = "000"+i.ToString();                  //주문번호
        //            dataLogVo.accno      = mainForm.exXASessionClass.account;   //계좌번호
        //            dataLogVo.ordptncode = "02";                                //주문구분 01:매도|02:매수
        //            dataLogVo.Isuno      = t0424VoList.ElementAt(i).expcode.Replace("A", "");   //종목코드
        //            dataLogVo.ordqty     = t0424VoList.ElementAt(i).mdposqt;   //주문수량 - 매도가능수량
        //            dataLogVo.execqty    = t0424VoList.ElementAt(i).mdposqt;   //체결수량 - 매도가능수량
        //            dataLogVo.ordprc     = t0424VoList.ElementAt(i).pamt;      //주문가격 - 평균단가
        //            dataLogVo.execprc    = t0424VoList.ElementAt(i).pamt;      //체결가격 - 평균단가
        //            dataLogVo.Isunm      = t0424VoList.ElementAt(i).hname;     //종목명

        //            this.insertData(dataLogVo);
        //        }
        //        Log.WriteLine("DataLog.init :: 거래이력 데이타 파일 생성 완료");
        //    }

        //    //계좌매매이력을 이용하여 평균단가를 다시 계산해준다.
        //    HistoryVo tmpHistoryVo = new HistoryVo();
        //    for (int i = 0; i < t0424VoList.Count(); i++)
        //    {
               
        //        tmpHistoryVo = getHistoryVo(t0424VoList.ElementAt(i).expcode);
        //        if (tmpHistoryVo != null)
        //        {
        //            if (tmpHistoryVo.pamt == null)
        //            {
        //                MessageBox.Show(tmpHistoryVo.buyCnt);
        //            }
                    
        //            t0424VoList.ElementAt(i).sellCnt   = tmpHistoryVo.sellCnt;
        //            t0424VoList.ElementAt(i).buyCnt    = tmpHistoryVo.buyCnt;
        //            t0424VoList.ElementAt(i).pamt2     = tmpHistoryVo.pamt; //평균단가2
        //            t0424VoList.ElementAt(i).sellSunik = tmpHistoryVo.sellSunik;
                    
        //            Util.setSunikrt2(t0424VoList.ElementAt(i));
        //        }
        //    }

        //    //금일매도 차익을 출력
        //    mainForm.input_toDayAtm.Text = this.getToDaySellAmt();
        //}

        

        /// <summary>
        /// Log 파일에 해당종목의 모든 기록을 삭제
        /// </summary>
        /// <param name="Isuno"></param>
        public void deleteData(String Isuno)
        {
            String section =  mainForm.accountForm.account +"_"+ Isuno.Replace("A", "");
            WritePrivateProfileString(section, null, null, this.filePath);
        }   // end function

        //파일을 읽어서 vo를 만들어준다.
        public DataLogVo readData(String Isuno, String ordno)
        {
            StringBuilder retOrd = new StringBuilder();
            String section = mainForm.accountForm.account + "_" + Isuno.Replace("A", "");
            GetPrivateProfileString(section, ordno, "", retOrd, 100, this.filePath);

            String[] splitResult = retOrd.ToString().Split(new char[] { '=', '|' });
            if (splitResult.Length > 1)
            {
                DataLogVo dataLogVo  = new DataLogVo();
                dataLogVo.ordno      = splitResult[0]; //주문번호 
                dataLogVo.date       = splitResult[1];
                dataLogVo.time       = splitResult[2];
                dataLogVo.accno      = splitResult[3]; //계좌번호
                dataLogVo.ordptncode = splitResult[4]; //주문구분 01:매도|02:매수   
                dataLogVo.Isuno      = splitResult[5].Replace("A", ""); //종목코드
                dataLogVo.ordqty     = splitResult[6]; //주문수량
                dataLogVo.execqty    = splitResult[7]; //체결수량
                dataLogVo.ordprc     = splitResult[8]; //주문가격
                dataLogVo.execprc    = splitResult[9]; //체결가격
                dataLogVo.Isunm      = splitResult[10];//종목명

                return dataLogVo;
                //vo에 저장
            }
            return null;
        }

        //금일매도매수체력수량을 업데이트한다.
        public void updateToDaySellExecqty(DataLogVo dataLogVo)
        {

        }


        //메모리및 ini 파일에 체결 정보를 동시에 기록
        public void insertData(T0424Vo t0424Vo, String ordno) {

            DataLogVo dataLogVo  = new DataLogVo();
            dataLogVo.ordno      = ordno;                    //주문번호
            dataLogVo.accno      = mainForm.accountForm.account; //계좌번호
            dataLogVo.ordptncode = "02";                              //주문구분 01:매도|02:매수
            dataLogVo.Isuno      = t0424Vo.expcode.Replace("A", "");  //종목코드
            dataLogVo.ordqty     = t0424Vo.mdposqt;                   //주문수량 - 매도가능수량
            dataLogVo.execqty    = t0424Vo.mdposqt;                   //체결수량 - 매도가능수량
            dataLogVo.ordprc     = t0424Vo.pamt;                      //주문가격 - 평균단가
            dataLogVo.execprc    = t0424Vo.pamt;                      //체결가격 - 평균단가
            dataLogVo.Isunm      = t0424Vo.hname;                     //종목명
            this.insertData(dataLogVo);
        }

            

        /// <summary>
        /// 메모리및 ini 파일에 체결 정보를 동시에 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public void insertData(DataLogVo dataLogVo)
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
                dataLogVo.Isuno = dataLogVo.Isuno.Replace("A", "");
                dataLogVoList.Add(dataLogVo);
            }
            
            //vo정보를 string 으로 나열한다.
            String ordno = dataLogVo.ordno; //주문번호 
            StringBuilder sb = new StringBuilder();
            sb.Append(""  + DateTime.Now.ToString("yyyyMMdd"));
            sb.Append("|" + DateTime.Now.ToString("HHmmss"));
            sb.Append("|" + dataLogVo.accno);     //계좌번호
            sb.Append("|" + dataLogVo.ordptncode);//주문구분 01:매도|02:매수   
            sb.Append("|" + dataLogVo.Isuno.Replace("A",""));     //종목코드
            sb.Append("|" + dataLogVo.ordqty);    //주문수량
            sb.Append("|" + dataLogVo.execqty);   //체결수량
            sb.Append("|" + dataLogVo.ordprc);    //주문가격
            sb.Append("|" + dataLogVo.execprc);   //체결가격
            sb.Append("|" + dataLogVo.Isunm);     //종목명

            //ini 쓰기 주문번호로 같은주문번호가 있으면 업데이트 없으면 추가.--폴더는 추가되지 않는다.
            String section = dataLogVo.accno + "_" + dataLogVo.Isuno.Replace("A", "");
            WritePrivateProfileString(section, ordno, sb.ToString(), this.filePath);

            Log.WriteLine("DataLog insertData::" + dataLogVo.Isunm + "(" + dataLogVo.Isuno.Replace("A", "") + ") [주문구분:"+ dataLogVo.ordptncode + "|주문수량: " + dataLogVo.ordqty + "|체결수량:"+ dataLogVo.execqty + "]");

        }   // end function


        //금일매도 차익 --아직로직정의가 안되었다 금일 매수/매도 건인지 알길이 없다. 임시방편으로 추정값을 구한다.
        public String getToDaySellAmt()
        {
            //금일매도 차익 출력
            //EBindingList<DataLogVo> dataLogVoList = mainForm.dataLog.getDataLogVoList();
            //계좌별 금일 거래 목록을 구한다.
            double 금일매도차익 = 0;
            var resultDataLogVoList = from item in this.dataLogVoList
                                      where item.accno == mainForm.accountForm.account
                                         && item.date == DateTime.Now.ToString("yyyyMMdd")
                                         && item.ordptncode == "01"
                                      select item;
            //Log.WriteLine("DataLog::  [카운트:" + resultDataLogVoList.Count()+"]");
            foreach (var item in resultDataLogVoList)
            {

                //금일 매도 매매정보만을 가지고 총매도금액 * 수익율을 이용하여 금일매도매수 차익을 임시방편으로 구해본다.
                if (item.ordptncode == "02")//매수
                {
                    //금일매도차익 = 금일매도차익 + (double.Parse(item.execprc) * double.Parse(item.execqty));
                }
                else if (item.ordptncode == "01") //매도
                {
                    //금일매도차익 = 금일매도차익 - (double.Parse(item.execprc) * double.Parse(item.execqty));
                    금일매도차익 = 금일매도차익 + (double.Parse(item.execprc) * double.Parse(item.execqty));
                }
                //Log.WriteLine("DataLog::  [날자:" + item.date + "|구분: " + item.ordptncode + "|금액:" + 금일매도차익 + "|종목며이:"+ item.Isunm+ "]");
            }
            금일매도차익 = 금일매도차익 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100);
            return 금일매도차익.ToString();
      
        }

    }   // end class



    //매매이력 정보
    public class DataLogVo {
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
    //종목별 매매이력 정보를 리턴한다.
    public class HistoryVo
    {
        public String pamt2     { set; get; } //평균단가
        public String buyCnt    { set; get; } //매수횟수
        public String sellCnt   { set; get; } //매도횟수
        public String sellSunik { set; get; } //중간매도손익
    }

}   // end namespace
