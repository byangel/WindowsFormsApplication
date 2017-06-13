using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Data.SQLite;
using System.Data;
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
        private FileStream   fileStream;
        private StreamReader streamReader;             // 파일 읽기 위한 스트림
        private String filePath;
        private EBindingList<DataLogVo> dataLogVoList;
        private EBindingList<DataLogVo> dataLogVoList2;

        public MainForm mainForm;

        // ---- ini 파일 의 읽고 쓰기를 위한 API 함수 선언 ----
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        

        // 생성자
        public DataLog()
        {
            try { 
                this.dataLogVoList = new EBindingList<DataLogVo>();
                this.dataLogVoList2 = new EBindingList<DataLogVo>();
                this.filePath        = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.ini";
            
                //파일 로드
                this.dataFileLoad();

                /////////////////////////////DB 테이블 설정//////////////////////////////////
                using (var conn = new SQLiteConnection(connStr))
                {
                   conn.Open();//파일이 없으면 자동 생성 //conn.Close();

                    //테이블이 있는지 확인 후 없으면 테이블 생성
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT COUNT(*) cnt FROM sqlite_master WHERE name = 'trading_history'", conn);

                    if (Convert.ToInt32(sqlCmd.ExecuteScalar()) <= 0)
                    {
                        sqlCmd.CommandText = "CREATE TABLE trading_history ("
                                                                           + " ordno        VARCHAR(16)" //주문번호
                                                                           + ",dt           VARCHAR(14)" //일시
                                                                           + ",accno        VARCHAR(16)" //계좌번호
                                                                           + ",Isuno        VARCHAR(16)" //종목코드
                                                                           + ",Isunm        VARCHAR(16)" //종목명
                                                                           + ",ordptncode   VARCHAR(16)" //주문구분 01:매도|02:매수 
                                                                           + ",ordqty       VARCHAR(16)" //주문수량  
                                                                           + ",execqty      VARCHAR(16)" //체결수량  
                                                                           + ",execprc      VARCHAR(16)" //체결가격
                                                                           + ",ordptnDetail VARCHAR(16)" //상세 주문구분 신규매수|반복매수|금일매도|청산|
                                                                           + ",upOrdno      VARCHAR(16)" //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                                                                           + ",upExecprc    VARCHAR(16)" //상위체결금액
                                                                           + ",sellOrdAt    VARCHAR(16)" //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                                                                           + ",useYN        VARCHAR(16)" //사용여부    

                                                                           + ", PRIMARY KEY(accno,Isuno,ordno));";
                        sqlCmd.ExecuteNonQuery();
                    }

                    sqlCmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

                //DB정보를 메모리에 저장(dataLogVoList)
                dbSync();
                /////////////////////////////DB 설정//////////////////////////////////   
            }
            catch (Exception ex){
                Log.WriteLine("DataLog : " + ex.Message);
                Log.WriteLine("DataLog : " + ex.StackTrace);
            }
            finally{
                
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
                                  where item.accno == mainForm.account
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
        // 1.계좌정보 설정후 호출하자...오늘 거래일자가 없는 청산 종목을 객체 생성할때 체크 하여 삭제 하여준다.
        // 2.t0425 내용이 있다면 t0425기준으로 db 동기화 해준다.
        public void initDelete()
        {
            //1.db에서 필요없는필드 삭제.
            //2.싱크 메소드 호출.
            //섹션삭제
            //String section =  mainForm.accountForm.account +"_"+ Isuno.Replace("A", "");
            //WritePrivateProfileString(section, null, null, this.filePath);

            //매도 거래 정보
            var varDataLogVoList = from item in this.dataLogVoList
                                  where item.useYN == "N" && item.accno == mainForm.account && item.date != DateTime.Now.ToString("yyyyMMdd")
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
                        section = mainForm.account + "_" + varDataLogVoList.ElementAt(i).Isuno.Replace("A", "");
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
            DataLogVo dataLogVo;
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


            ///////////////////////////////////////////
            //DB의 모른 목록 조회
            DataTable dt = list();
            foreach (DataRow dr in dt.Rows){
                dataLogVo = new DataLogVo();
                dataLogVo.ordno        = dr["accno"].ToString();    //주문번호
                dataLogVo.dt           = dr["dt"].ToString();       //일시
                dataLogVo.accno        = dr["accno"].ToString();    //계좌번호
                dataLogVo.Isuno        = dr["Isuno"].ToString();    //종목코드
                dataLogVo.Isunm        = dr["Isunm"].ToString();    //종목명
                dataLogVo.ordptncode   = dr["ordptncode"].ToString();//주문구분 01:매도|02:매수 
                dataLogVo.ordqty       = dr["ordqty"].ToString();   //주문수량  
                dataLogVo.execqty      = dr["execqty"].ToString();  //체결수량  
                dataLogVo.execprc      = dr["execprc"].ToString();  //체결가격
                dataLogVo.ordptnDetail = dr["ordptnDetail"].ToString();//상세 주문구분 신규매수|반복매수|금일매도|청산|
                dataLogVo.upOrdno      = dr["upOrdno"].ToString();  //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                dataLogVo.upExecprc    = dr["upExecprc"].ToString();//상위체결금액
                dataLogVo.sellOrdAt    = dr["sellOrdAt"].ToString();//매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                dataLogVo.useYN        = dr["useYN"].ToString();    //사용여부 

                this.dataLogVoList.Add(dataLogVo);
            }
        }

        public void dbSync()
        {
            DataLogVo dataLogVo;

            this.dataLogVoList2.Clear();
            
            DataTable dt = list();
            foreach (DataRow dr in dt.Rows)
            {
                dataLogVo = new DataLogVo();
                dataLogVo.ordno = dr["accno"].ToString();    //주문번호
                dataLogVo.dt = dr["dt"].ToString();       //일시
                dataLogVo.accno = dr["accno"].ToString();    //계좌번호
                dataLogVo.Isuno = dr["Isuno"].ToString();    //종목코드
                dataLogVo.Isunm = dr["Isunm"].ToString();    //종목명
                dataLogVo.ordptncode = dr["ordptncode"].ToString();//주문구분 01:매도|02:매수 
                dataLogVo.ordqty = dr["ordqty"].ToString();   //주문수량  
                dataLogVo.execqty = dr["execqty"].ToString();  //체결수량  
                dataLogVo.execprc = dr["execprc"].ToString();  //체결가격
                dataLogVo.ordptnDetail = dr["ordptnDetail"].ToString();//상세 주문구분 신규매수|반복매수|금일매도|청산|
                dataLogVo.upOrdno = dr["upOrdno"].ToString();  //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                dataLogVo.upExecprc = dr["upExecprc"].ToString();//상위체결금액
                dataLogVo.sellOrdAt = dr["sellOrdAt"].ToString();//매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                dataLogVo.useYN = dr["useYN"].ToString();    //사용여부 

                this.dataLogVoList2.Add(dataLogVo);
            }
        }

        //파일을 읽어서 vo를 만들어준다.
        public DataLogVo readData(String Isuno, String ordno)
        {
            StringBuilder retOrd = new StringBuilder();
            String section = mainForm.account + "_" + Isuno.Replace("A", "");
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
            dataLogVo.accno        = mainForm.account;      //계좌번호
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
            //////////////////////DB에 저장////////////////////////
            this.insert(dataLogVo);
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

        //String connStr = @"Data Source=C:\Temp\mydb.db";
        private String connStr = @"Data Source=" + Util.GetCurrentDirectoryWithPath() + "\\logs\\history.db;Pooling=true;FailIfMissing=false";

        //등록
        public void insert(DataLogVo dataLogVo){
            
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into trading_history ( ordno                                          ");
                sb.Append("                             ,dt                                             "); //일시
                sb.Append("                             ,accno                                          "); //계좌번호
                sb.Append("                             ,Isuno                                          "); //종목코드
                sb.Append("                             ,Isunm                                          "); //종목명
                sb.Append("                             ,ordptncode                                     "); //주문구분 01:매도|02:매수 
                sb.Append("                             ,ordqty                                         ");//주문수량  
                sb.Append("                             ,execqty                                        "); //체결수량  
                sb.Append("                             ,execprc                                        "); //체결가격
                sb.Append("                             ,ordptnDetail                                   "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
                sb.Append("                             ,upOrdno                                        ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                             ,upExecprc                                      "); //상위체결금액
                sb.Append("                             ,sellOrdAt                                      "); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                sb.Append("                             ,useYN ) VALUES(                                "); //사용여부 

                sb.Append("                                         '"  + dataLogVo.ordno + "'           "); //주문번호
                sb.Append("                                         ,'" + DateTime.Now.ToString("yyyyMMddHHmmss") + "'              "); //일시
                sb.Append("                                         ,'" + dataLogVo.accno + "'           "); //계좌번호
                sb.Append("                                         ,'" + dataLogVo.Isuno + "'           ");//종목코드
                sb.Append("                                         ,'" + dataLogVo.Isunm + "'           ");//종목명
                sb.Append("                                         ,'" + dataLogVo.ordptncode + "'      ");//주문구분 01:매도|02:매수 
                sb.Append("                                         ,'" + dataLogVo.ordqty + "'          ");//주문수량  
                sb.Append("                                         ,'" + dataLogVo.execqty + "'         ");//체결수량  
                sb.Append("                                         ,'" + dataLogVo.execprc + "'         ");//체결가격
                sb.Append("                                         ,'" + dataLogVo.ordptnDetail + "'    ");//상세 주문구분 신규매수|반복매수|금일매도|청산|
                sb.Append("                                         ,'" + dataLogVo.upOrdno + "'         ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                                         ,'" + dataLogVo.upExecprc + "'       ");//상위체결금액
                sb.Append("                                         ,'" + dataLogVo.sellOrdAt + "'       ");//매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                sb.Append("                                         ,'" + dataLogVo.useYN + "' )          ");//사용여부 

                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            
        }
        //읽기
        public DataTable read(String Isuno, String ordno)
        {
            DataTable dt = new DataTable();
            //string connStr = @"Data Source=C:\Temp\mydb.db";
            //SQLiteDataAdapter 클래스를 이용 비연결 모드로 데이타 읽기기
            //string sql = "SELECT * FROM member";

            //쿼리정의
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  ordno                                ");
            sb.Append("       ,dt                                   "); //일시
            sb.Append("       ,accno                                "); //계좌번호
            sb.Append("       ,Isuno                                "); //종목코드
            sb.Append("       ,Isunm                                "); //종목명
            sb.Append("       ,ordptncode                           "); //주문구분 01:매도|02:매수 
            sb.Append("       ,ordqty                               "); //주문수량  
            sb.Append("       ,execqty                              "); //체결수량  
            sb.Append("       ,execprc                              "); //체결가격
            sb.Append("       ,ordptnDetail                         "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            sb.Append("       ,upOrdno                              "); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("       ,upExecprc                            "); //상위체결금액
            sb.Append("       ,sellOrdAt                            "); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
            sb.Append("       ,useYN                                "); //사용여부 
            sb.Append("FROM  trading_history                        "); //사용여부 
            sb.Append("WHERE ordno = '" + ordno + "'                "); //주문번호
            sb.Append("AND   Isuno = '" + Isuno + "'                "); //종목코드
            sb.Append("AND   accno = '" + mainForm.account + "'     "); //계좌번호
            
            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }



       
        //수정
        public void update(DataLogVo dataLogVo)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading_history SET  (                                                       ");
                sb.Append("                             ,ordqty           = '" + dataLogVo.ordqty + "'         "); //주문수량  
                sb.Append("                             ,execqty          = '" + dataLogVo.execqty + "'        "); //체결수량  
                sb.Append("                             ,execprc          = '" + dataLogVo.execprc + "'        "); //체결가격
                sb.Append("                             ,ordptnDetail     = '" + dataLogVo.ordptnDetail + "'   "); //상세 0 신규매수|반복매수|금일매도|청산|
                sb.Append("                             ,upOrdno          = '" + dataLogVo.upOrdno + "'        ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                             ,upExecprc        = '" + dataLogVo.upExecprc + "'      "); //상위체결금액
                sb.Append("                             ,sellOrdAt        = '" + dataLogVo.sellOrdAt + "'      "); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                sb.Append("                             ,useYN 			  = '" + dataLogVo.useYN + "')         "); //사용여부 
                sb.Append("WHERE Isuno = '" + dataLogVo.Isuno + "'                                             "); //주문번호
                sb.Append("AND   ordno = '" + dataLogVo.ordno + "'                                             "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account + "'                                            "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }
        //체결수량 업데이트

        //삭제
        public void delete(String Isuno, String ordno)
        {

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading_history ordno           ");
                sb.Append(" WHERE Isuno = '" + Isuno + "'              "); //종목코드
                sb.Append("   AND ordno = '" + ordno + "'              "); //주문번호
                sb.Append("   AND accno = '" + mainForm.account + "'   "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }

        //목록
        public DataTable list()
        {
            DataTable dt = new DataTable();
            //string sql = "SELECT * FROM member WHERE Id>=2";
            //쿼리정의
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  ordno                                ");
            sb.Append("       ,dt                                   "); //일시
            sb.Append("       ,accno                                "); //계좌번호
            sb.Append("       ,Isuno                                "); //종목코드
            sb.Append("       ,Isunm                                "); //종목명
            sb.Append("       ,ordptncode                           "); //주문구분 01:매도|02:매수 
            sb.Append("       ,ordqty                               "); //주문수량  
            sb.Append("       ,execqty                              "); //체결수량  
            sb.Append("       ,execprc                              "); //체결가격
            sb.Append("       ,ordptnDetail                         "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            sb.Append("       ,upOrdno                              "); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("       ,upExecprc                            "); //상위체결금액
            sb.Append("       ,sellOrdAt                            "); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
            sb.Append("       ,useYN                                "); //사용여부 
            sb.Append("FROM  trading_history                        "); //사용여부 
            sb.Append("WHERE useYN = 'Y'                            "); //사용여부
            sb.Append("AND   accno = '" + mainForm.account + "'     "); //계좌번호
            sb.Append("ORDER BY   accno, Isuno, dt                  "); //주문번호

            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
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
                dataLogVo.Isunm = realSc1Vo.Isunm;
                dataLogVo.execprc = realSc1Vo.execprc;//체결가격

                this.insertMergeData(dataLogVoList.ElementAt(findIndex));


                ///////////////////////////////////
                using (var conn = new SQLiteConnection(connStr))
                {
                    conn.Open();

                    StringBuilder sb = new StringBuilder();
                    sb.Append("UPDATE trading_history SET  (                                                        ");
                    sb.Append("                             ,execqty          = '" + dataLogVo.execqty + "'         "); //체결수량   
                    sb.Append("                             ,execprc          = '" + dataLogVo.execprc + "'  )      "); //체결가격   
                    sb.Append("WHERE Isuno = '" + realSc1Vo.Isuno + "'                                              "); //주문번호
                    sb.Append("AND   ordno = '" + realSc1Vo.ordno + "'                                              "); //종목코드
                    sb.Append("AND   accno = '" + mainForm.account + "'                                             "); //계좌번호
                    SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);

                    sqlCmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

                ////////////////////////////////////
                return true;
            }


            

            return false;
        }

        //금일매도일경우 상위매수주문 정보에 매도주문상태를 Y로 상태 업데이트 해준다.
        public void updateSellOrdAt(String upOrdno, String state)
        {
            int findIndex = dataLogVoList.Find("ordno", upOrdno);
            if (findIndex >= 0)
            {
                DataLogVo dataLogVo = dataLogVoList.ElementAt(findIndex);
                dataLogVo.sellOrdAt = state;

                this.insertMergeData(dataLogVo);
            }


            /////////////////////////////////////////////////
        
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading_history SET  (                                         ");
                sb.Append("                             ,sellOrdAt        = '" + state + "')     "); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.     
                sb.Append("WHERE ordno = '" + upOrdno + "'                                       "); //주문번호
                //sb.Append("AND   Isuno = '" + Isuno + "'                                       "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account + "'                              "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            /////////////////////////////////////////////////

        }

        //사용여부 수정
        public void updateUseYN(DataLogVo dataLogVo)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading_history SET  (                                                       ");
                sb.Append("                             ,useYN 			  = '" + dataLogVo.useYN + "')         "); //사용여부 
                sb.Append("WHERE Isuno = '" + dataLogVo.Isuno + "'                                             "); //주문번호
                sb.Append("AND   accno = '" + mainForm.account + "'                                            "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
        }


        //종목 코드 --총매수금액과 매도가능수량을 구해서 평균단가를 리턴한다. 
        public SummaryVo getSummaryVo(String Isuno)
        {


            SummaryVo summaryVo = null;

            //종목 매매 목록을 구한다.
            var resultDataLogVoList = from item in this.dataLogVoList
                                      where item.Isuno == Isuno.Replace("A", "") && item.accno == mainForm.account && item.useYN == "Y"
                                      select item;

            //매도 거래 정보
            var groupDataLogVoList = from item in resultDataLogVoList
                                     group item by item.ordptncode == "02" into g
                                     select new
                                     {
                                         goupKey        = g.Key
                                         ,groupVoList   = g
                                         ,매매횟수       = g.Count()
                                         ,거래금액합      = g.Sum(o => int.Parse(o.execqty) * double.Parse(o.execprc))
                                         ,상위거래금액합  = g.Sum(o => int.Parse(o.execqty) * double.Parse(o.upExecprc))
                                         ,체결수량합     = g.Sum(o => int.Parse(o.execqty))
                                     };


            //dataLogVoList에 매수 정보가 없다는것은 잔고목록과 매핑이 잘되지 않았다는거다...else구문을 구현해주자.
            if (resultDataLogVoList.Count() > 0)
            {
                summaryVo = new SummaryVo();
                //로그출력
                double 총매수금액 = 0;
                double 매도가능수량 = 0;
                double 중간매도손익 = 0;
                foreach (var group in groupDataLogVoList)
                {

                    if (group.goupKey)//매수그룹
                    {
                        총매수금액 = 총매수금액 + group.거래금액합;
                        매도가능수량 = 매도가능수량 + group.체결수량합;
                        summaryVo.buyCnt = group.매매횟수.ToString();
                    }
                    else//매도그룹
                    {
                        중간매도손익 = (group.거래금액합 - group.상위거래금액합);
                        //매도 거래금액합 - (2%수익금) 해주어야 
                        //총매수금액 = 총매수금액 - (group.거래금액합 - (group.거래금액합 * (double.Parse(Properties.Settings.Default.STOP_PROFIT_TARGET) / 100)));
                        총매수금액 = 총매수금액 - group.상위거래금액합;
                        매도가능수량 = 매도가능수량 - group.체결수량합;
                        summaryVo.sellCnt = group.매매횟수.ToString();
                        summaryVo.sellSunik = 중간매도손익.ToString();
                    }

                }
                //Log.WriteLine("DataLog:: [매도가능수량:" + 매도가능수량+ "|매수총금액:"+ 총매수금액 + "]");
                if (매도가능수량 != 0)
                {
                    double 평균단가 = (총매수금액 / 매도가능수량);
                    summaryVo.pamt2 = Util.GetNumberFormat(평균단가.ToString());
                    //historyVo.pamt2 = 평균단가.ToString();
                    //Log.WriteLine("DataLog::[평균단가:" + 평균단가.ToString() + "매도가능수량:" + 매도가능수량);
                }
                else
                {
                    Log.WriteLine("DataLog:: [매도가능수량:" + 매도가능수량 + "|매수총금액:" + 총매수금액 + "]");
                    return null;
                }
                //Log.WriteLine("===========================================");   
            }
            else
            {//종목번호로 데이타로그가 없을때. 데이타 로그에 등록해주고  historyVo를 설정해준다.
                return null;
            }
            return summaryVo;
        }

        //private static void Select_Reader()
        //{


        //    using (var conn = new SQLiteConnection(connStr))
        //    {
        //        conn.Open();
        //        string sql = "SELECT * FROM member WHERE Id>=2";

        //        //SQLiteDataReader를 이용하여 연결 모드로 데이타 읽기기
        //        SQLiteCommand cmd = new SQLiteCommand(sql, conn);
        //        SQLiteDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            Console.WriteLine(rdr["name"]);
        //        }
        //        rdr.Close();
        //    }
        //}




    }   // end class


    //매매이력 정보
    public class DataLogVo {
        public String ordno        { set; get; }//주문번호 key
        public String dt           { set; get; }//일시
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
    public class SummaryVo
    {
        public String pamt2     { set; get; } //평균단가
        public String buyCnt    { set; get; } //매수횟수
        public String sellCnt   { set; get; } //매도횟수
        public String sellSunik { set; get; } //중간매도손익
    }

}   // end namespace
