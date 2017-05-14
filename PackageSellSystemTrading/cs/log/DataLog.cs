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
            this.filePath        = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.ini";
            ////////////////////////////////////////////////////////////////////
            //파일 로드
            this.dataFileLoad();

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
            var resultDataLogVoList =  from  item in this.dataLogVoList
                                      where  item.Isuno == Isuno.Replace("A", "") && item.accno == mainForm.accountForm.account && item.useYN=="Y"
                                      select item;
           
            //매도 거래 정보
            var groupDataLogVoList = from item in resultDataLogVoList
                                      group item by  item.ordptncode == "02" into g
                           select new {  goupKey       = g.Key 
                                       , groupVoList   = g 
                                       , 매매횟수       = g.Count()
                                       , 거래금액합     = g.Sum( o => int.Parse(o.execqty) *double.Parse(o.execprc) )
                                       , 상위거래금액합 = g.Sum( o => int.Parse(o.execqty) *double.Parse(o.upExecprc) )
                                       , 체결수량합  = g.Sum(o => int.Parse(o.execqty))
                                      };
            
           
            //dataLogVoList에 매수 정보가 없다는것은 잔고목록과 매핑이 잘되지 않았다는거다...else구문을 구현해주자.
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
                        중간매도손익 = (group.거래금액합  - group.상위거래금액합);
                        //매도 거래금액합 - (2%수익금) 해주어야 
                        //총매수금액 = 총매수금액 - (group.거래금액합 - (group.거래금액합 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100)));
                        총매수금액   = 총매수금액 - group.상위거래금액합;
                        매도가능수량 = 매도가능수량 - group.체결수량합;
                        historyVo.sellCnt = group.매매횟수.ToString();
                        historyVo.sellSunik = 중간매도손익.ToString();
                    }
                   
                }
                //Log.WriteLine("DataLog:: [매도가능수량:" + 매도가능수량+ "|매수총금액:"+ 총매수금액 + "]");
                if (매도가능수량 != 0)
                {
                    double 평균단가 = (총매수금액 / 매도가능수량);
                    historyVo.pamt2 = Util.GetNumberFormat(평균단가.ToString());
                    //historyVo.pamt2 = 평균단가.ToString();
                    //Log.WriteLine("DataLog::[평균단가:" + 평균단가.ToString() + "매도가능수량:" + 매도가능수량);
                } else {
                    Log.WriteLine("DataLog:: [매도가능수량:" + 매도가능수량 + "|매수총금액:" + 총매수금액 + "]");
                    return null;
                }
                //Log.WriteLine("===========================================");   
            } else {//종목번호로 데이타로그가 없을때. 데이타 로그에 등록해주고  historyVo를 설정해준다.
               return null;
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



        /// <summary>
        /// Log 파일에 해당종목의 모든 기록을 삭제
        /// 해당종목 모든 매매이력 목록을 사용안함으로 업데이트한다.
        /// </summary>
        /// <param name="Isuno">종목코드번호</param>
        public void deleteData(String Isuno)
        {
            //섹션삭제
            //String section =  mainForm.accountForm.account +"_"+ Isuno.Replace("A", "");
            //WritePrivateProfileString(section, null, null, this.filePath);

            //해당종목 매매이력에 사용안함 업데이트
            var varDataLogVoList = from item in this.dataLogVoList
                                  where item.accno == mainForm.accountForm.account
                                         && item.Isuno == Isuno.Replace("A","")
                                 select item;
            
            for (int i=0;i< varDataLogVoList.Count();i++)
            {
                //Log.WriteLine("dataLog::"+ varDataLogVoList.ElementAt(i).Isunm+"("+ varDataLogVoList.ElementAt(i).Isuno+ ")청산호출됨 주문번호:"+ varDataLogVoList.ElementAt(i).ordno);
                varDataLogVoList.ElementAt(i).useYN = "N";
                this.insertMergeData(varDataLogVoList.ElementAt(i));
            }
           
           
        }   // end function

        /// <summary>
        // 계좌정보 설정후 호출하자...오늘 거래일자가 없는 청산 종목을 객체 생성할때 체크 하여 삭제 하여준다.
        public void initDelete()
        {
            //섹션삭제
            //String section =  mainForm.accountForm.account +"_"+ Isuno.Replace("A", "");
            //WritePrivateProfileString(section, null, null, this.filePath);

            //매도 거래 정보
            var varDataLogVoList = from item in this.dataLogVoList
                                  where item.useYN == "N" && item.accno == mainForm.accountForm.account && item.date != DateTime.Now.ToString("yyyyMMdd")
                                select item;

            //MessageBox.Show(varDataLogVoList.Count().ToString());
            if (varDataLogVoList.Count() >0)
            {
                int findIndex;
                String section;
                Log.WriteLine("DataLog  :: 삭제전 데이타수 : "+ this.dataLogVoList.Count());
                for (int i = 0; i < varDataLogVoList.Count(); i++)
                {
                    findIndex = this.dataLogVoList.Find("ordno", varDataLogVoList.ElementAt(i).ordno);
                    if (findIndex >= 0)
                    {
                        //파일에 내용삭제
                        section = mainForm.accountForm.account + "_" + varDataLogVoList.ElementAt(i).Isuno.Replace("A", "");
                        WritePrivateProfileString(section, null, null, this.filePath);

                        this.dataLogVoList.RemoveAt(findIndex);
                        i--;

                        
                    }
                }
                Log.WriteLine("DataLog  :: 삭제후 데이타수 : " + this.dataLogVoList.Count());

                //vo ->  파일 동기화 (파일내용 삭제후 vo내용을 덮어쒸운다.)
                //for (int i = 0; i < this.dataLogVoList.Count(); i++)
                //{
                //    String section = mainForm.accountForm.account + "_" + this.dataLogVoList.ElementAt(i).Isuno.Replace("A", "");
                //    WritePrivateProfileString(section, null, null, this.filePath);
                //}

                for (int i = 0; i < this.dataLogVoList.Count(); i++)
                {
                    this.insertMergeData(this.dataLogVoList.ElementAt(i));
                }
            }

        }   // end function

        public void dataFileLoad()
        {
            this.dataLogVoList = new EBindingList<DataLogVo>();
            //디렉토리 체크
            String dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
                Log.WriteLine("DataLog ::디렉토리 생성[" + dirPath + "]");
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
                DataLogVo dataLogVo;

                while (!streamReader.EndOfStream)
                {
                    // 한줄을 읽습니다
                    lineString = streamReader.ReadLine();
                    splitResult = lineString.Split(new char[] { '=', ',' });
                    dataLogVo = parserDataLogVo(lineString);
                    if (dataLogVo != null)
                    {
                        this.dataLogVoList.Add(dataLogVo);
                    }
                }
               
                //여기서 닫아줘야 WriteLine 쓸수있다.
                streamReader.Close();
                fileStream.Close();
                Log.WriteLine("DataLog 생성자 :: dataLog 파일 로드 완료 [로드 종목수:" + dataLogVoList.Count() + "]");
            }
            else
            {
                fileStream = new FileStream(filePath, FileMode.Create);
                //여기서 닫아줘야 WriteLine 쓸수있다.
                fileStream.Close();
            }
        }

        //파일을 읽어서 vo를 만들어준다.
        public DataLogVo readData(String Isuno, String ordno)
        {
            StringBuilder retOrd = new StringBuilder();
            String section = mainForm.accountForm.account + "_" + Isuno.Replace("A", "");
            GetPrivateProfileString(section, ordno, "", retOrd, 100, this.filePath);
            return parserDataLogVo(retOrd.ToString());
 
        }

        public DataLogVo parserDataLogVo(String dataStr)
        {
            DataLogVo dataLogVo = null;
            String[] splitResult = dataStr.ToString().Split(new char[] { '=', ',' });
            if (splitResult.Length > 1)
            {
                dataLogVo = new DataLogVo();
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

                dataLogVo.ordptnDetail = splitResult[11];//상세 주문구분 신규매수|반복매수|금일매도|청산
                dataLogVo.upOrdno      = splitResult[12];//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                dataLogVo.upExecprc    = splitResult[13];//상위 체결가걱
                dataLogVo.sellOrdAt    = splitResult[14];//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
                dataLogVo.useYN        = splitResult[15];//삭제여부

            }
            return dataLogVo;
        }



        //t0424에있고 dataLog에 없을때 호출 된다.
        public void insertDataT0424(T0424Vo t0424Vo, String ordno)
        {
            //MessageBox.Show(dataLogVoList.Count().ToString());
            DataLogVo dataLogVo    = new DataLogVo();
            dataLogVo.ordno = ordno; //주문번호

            dataLogVo.date         = DateTime.Now.ToString("yyyyMMdd");
            dataLogVo.time         = DateTime.Now.ToString("HHmmss");
            dataLogVo.accno        = mainForm.accountForm.account;      //계좌번호
            dataLogVo.ordptncode   = "02";                              //주문구분 01:매도|02:매수
            dataLogVo.Isuno        = t0424Vo.expcode.Replace("A", "");  //종목코드
            dataLogVo.ordqty       = t0424Vo.mdposqt;                   //주문수량 - 매도가능수량
            dataLogVo.execqty      = t0424Vo.mdposqt;                   //체결수량 - 매도가능수량
            dataLogVo.ordprc       = t0424Vo.pamt.Replace(",","");      //주문가격 - 평균단가
            dataLogVo.execprc      = t0424Vo.pamt.Replace(",", "");     //체결가격 - 평균단가
            dataLogVo.Isunm        = t0424Vo.hname;                     //종목명
            dataLogVo.ordptnDetail = "신규매수";                         //상세 주문구분 신규매수|반복매수|금일매도|청산 
            dataLogVo.upOrdno      = ordno;                             //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            dataLogVo.upExecprc    = "0";                               //상위체결가격
            dataLogVo.sellOrdAt    = "N";                               //매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
            dataLogVo.useYN        = "Y";                               //사용여부
            this.insertData(dataLogVo); 
        }


        //체결수량을 업데이트한다.
        public Boolean updateDataExecqty(RealSc1Vo realSc1Vo)
        {

            int findIndex = dataLogVoList.Find("ordno", realSc1Vo.ordno);
    
            //dataLogVoList에 주문번호정보여부에따라 매수정보 수정및 추가를 해준다. 
            if (findIndex >= 0)//같은주문번호가 있으면 체결량을 합해준다.
            {
                DataLogVo dataLogVo = dataLogVoList.ElementAt(findIndex);
                //매수 체결수량 = 매수 체결수량 + 매수 체결수량  -매도/매수 상관없이 체결수량을 합해준다.
                String execqty = (int.Parse(dataLogVo.execqty) + int.Parse(realSc1Vo.execqty)).ToString();

                dataLogVo.execqty = execqty;
                dataLogVo.Isunm   = realSc1Vo.Isunm;
                dataLogVo.execprc = realSc1Vo.execprc;//체결가격
                
                this.insertMergeData(dataLogVoList.ElementAt(findIndex));
                return true;
            }

            return false;
        }

        

        /// <summary>
        /// 메모리및 ini 파일에 체결 정보를 동시에 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public void insertData(DataLogVo dataLogVo){
            int findIndex = this.dataLogVoList.Find("ordno", dataLogVo.ordno);
            if(findIndex >= 0){
                DataLogVo tmpDataLogVo = this.dataLogVoList.ElementAt(findIndex);
                tmpDataLogVo = dataLogVo;
            }else{
                dataLogVoList.Add(dataLogVo);
            }

            //insertMergeData
            this.insertMergeData(dataLogVo);
        }   // end function


       
        public void insertMergeData(DataLogVo dataLogVo)
        {
            //1.vo정보를 string 으로 나열한다.
            String ordno = dataLogVo.ordno; //주문번호 
            String date = dataLogVo.date == "" || dataLogVo.date == null ? DateTime.Now.ToString("yyyyMMdd") : dataLogVo.date;
            String time = dataLogVo.time == "" || dataLogVo.time == null ? DateTime.Now.ToString("HHmmss") : dataLogVo.time;
            StringBuilder sb = new StringBuilder();
            sb.Append(""  + date);
            sb.Append("," + time);
            sb.Append("," + dataLogVo.accno);       //계좌번호
            sb.Append("," + dataLogVo.ordptncode);  //주문구분 01:매도|02:매수   
            sb.Append("," + dataLogVo.Isuno.Replace("A", ""));     //종목코드
            sb.Append("," + dataLogVo.ordqty);      //주문수량
            sb.Append("," + dataLogVo.execqty);     //체결수량
            sb.Append("," + dataLogVo.ordprc);      //주문가격
            sb.Append("," + dataLogVo.execprc);     //체결가격
            sb.Append("," + dataLogVo.Isunm);       //종목명
            sb.Append("," + dataLogVo.ordptnDetail);//상세 주문구분 신규매수|반복매수|금일매도|청산
            sb.Append("," + dataLogVo.upOrdno);     //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("," + dataLogVo.upExecprc);     //상위 체결가격
            sb.Append("," + dataLogVo.sellOrdAt);      //매도주문실행 여부 YN default:N     -02:매 일때만 값이 있어야한다.
            sb.Append("," + dataLogVo.useYN);      //삭제여부(청산주문후 실시간 매도가능수량0이면 삭제 상태 업데이트) | 상위매수단가 | 


            //ini 쓰기 주문번호로 같은주문번호가 있으면 업데이트 없으면 추가.--폴더는 추가되지 않는다.
            String section = dataLogVo.accno + "_" + dataLogVo.Isuno.Replace("A", "");
            WritePrivateProfileString(section, ordno, sb.ToString(), this.filePath);

            //2.금일매수/매도이면 상위 매수 정보에 매도여부 상태값을 업데이트 해준다.--나중에 지우던지 하자
            //if (dataLogVo.ordptnDetail == "금일매도")
            //{
            //    this.updateSellOrdAt(dataLogVo.upOrdno, "Y");
            //}
        }

        //금일매도일경우 상위매수주문 정보에 매도주문상태를 Y로 상태 업데이트 해준다.
        public void updateSellOrdAt(String upOrdno, String state){
            int findIndex = dataLogVoList.Find("ordno", upOrdno);
            if (findIndex>=0){
                DataLogVo dataLogVo = dataLogVoList.ElementAt(findIndex);
                dataLogVo.sellOrdAt = state;

                this.insertMergeData(dataLogVo);
            }

        }

        ////금일매도 차익 --아직로직정의가 안되었다 금일 매수/매도 건인지 알길이 없다. 임시방편으로 추정값을 구한다.
        //public String getToDaySellAmt(){
        //    //금일매도 차익 출력
        //    //EBindingList<DataLogVo> dataLogVoList = mainForm.dataLog.getDataLogVoList();
        //    //계좌별 금일 거래 목록을 구한다.
        //    double 금일매도차익 = 0;
        //    var resultDataLogVoList = from item in mainForm.xing_t0425.getT0425VoList()
        //                              where item.ordptnDetail == "금일매도"
        //                                 //&& item.date == DateTime.Now.ToString("yyyyMMdd")
        //                                 //&& item.accno == mainForm.accountForm.account
        //                              select item;
        //    //Log.WriteLine("DataLog::  [카운트:" + resultDataLogVoList.Count()+"]");
        //    foreach (var item in resultDataLogVoList){
        //        //매도금액 - 매수금액
        //        금일매도차익 = 금일매도차익 + ((double.Parse(item.cheprice) * double.Parse(item.cheqty)) - (double.Parse(item.cheqty) * double.Parse(item.upExecprc)) );
        //        //MessageBox.Show(item.execprc+"/"+ item.execqty + "/"+ item.execqty + "/"+ item.upExecprc + "/");
        //    }
           
        //    //금일매도차익 = 금일매도차익 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100);
        //    return 금일매도차익.ToString();
      
        //}
        ////금일매도 차익 --아직로직정의가 안되었다 금일 매수/매도 건인지 알길이 없다. 임시방편으로 추정값을 구한다.
        //public String getToDayShSunik()
        //{
        //    //금일매도 차익 출력
        //    //EBindingList<DataLogVo> dataLogVoList = mainForm.dataLog.getDataLogVoList();
        //    //계좌별 금일 거래 목록을 구한다.
        //    double 금일매도차익 = 0;
        //    var resultDataLogVoList = from item in mainForm.xing_t0425.getT0425VoList()
        //                              where item.ordptnDetail == "청산"
        //                              //&& item.date == DateTime.Now.ToString("yyyyMMdd")
        //                              //&& item.accno == mainForm.accountForm.account
        //                              select item;
        //    //Log.WriteLine("DataLog::  [카운트:" + resultDataLogVoList.Count()+"]");
        //    foreach (var item in resultDataLogVoList)
        //    {
        //        //매도금액 - 매수금액
        //        금일매도차익 = 금일매도차익 + ((double.Parse(item.cheprice) * double.Parse(item.cheqty)) - (double.Parse(item.cheqty) * double.Parse(item.upExecprc)));
        //        //MessageBox.Show(item.execprc+"/"+ item.execqty + "/"+ item.execqty + "/"+ item.upExecprc + "/");
        //    }

        //    //금일매도차익 = 금일매도차익 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100);
        //    return 금일매도차익.ToString();

        //}
   

    }   // end class


    //매매이력 정보
    public class DataLogVo {
        public String ordno        { set; get; }//주문번호 key
        public String date         { set; get; }//일자
        public String time         { set; get; }//시간
        public String accno        { set; get; }//계좌번호
        public String Isuno        { set; get; }//종목코드
        public String Isunm        { set; get; }//종목명

        public String ordptncode   { set; get; }//주문구분 01:매도|02:매수 
        public String ordqty       { set; get; }// 주문수량
        public String ordprc       { set; get; }// 주문가격
        public String execqty      { set; get; }// 체결수량
        public String execprc      { set; get; }// 체결가격

        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public String upExecprc    { set; get; }//상위체결금액
        
        public String sellOrdAt    { set; get; }//매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String useYN        { set; get; }//사용여부

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
