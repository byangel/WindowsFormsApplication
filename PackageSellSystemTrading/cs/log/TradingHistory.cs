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
    public class TradingHistory
    {

        //private DataTable historyDataTable;
        //private EBindingList<TradingHistoryVo> tradingHistoryVoList;
        public MainForm mainForm;
        private String connStr = @"Data Source=" + Util.GetCurrentDirectoryWithPath() + "\\logs\\history.db;Pooling=true;FailIfMissing=false";

        //public EBindingList<TradingHistoryVo> getTradingHistoryVoList()
        //{
        //    return this.tradingHistoryVoList;
        //}
        private DataTable tradingHistoryDt;
        public DataTable getTradingHistoryDt()
        {
            return this.tradingHistoryDt;
        }
        // 생성자
        public TradingHistory()
        {
            try {
                
                //    //디렉토리 체크
                String dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                    Log.WriteLine("DataLog ::디렉토리 생성[" + dirPath + "]");
                }

                
                /////////////////////////////DB 테이블 설정//////////////////////////////////
                using (var conn = new SQLiteConnection(connStr))
                {
                   conn.Open();//파일이 없으면 자동 생성 //conn.Close();

                    //테이블이 있는지 확인 후 없으면 테이블 생성
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT COUNT(*) cnt FROM sqlite_master WHERE name = 'trading'", conn);

                    if (Convert.ToInt32(sqlCmd.ExecuteScalar()) <= 0)
                    {
                        sqlCmd.CommandText = "CREATE TABLE trading          ("
                                                                           + " ordno        VARCHAR(16)" //주문번호
                                                                           + ",dt           VARCHAR(14)" //일시
                                                                           + ",accno        VARCHAR(16)" //계좌번호
                                                                           + ",Isuno        VARCHAR(16)" //종목코드
                                                                           + ",Isunm        VARCHAR(16)" //종목명
                                                                           + ",ordptncode   VARCHAR(16)" //주문구분 01:매도|02:매수 
                                                                           + ",ordqty       VARCHAR(16)" //주문수량  
                                                                           + ",execqty      VARCHAR(16)" //체결수량
                                                                           + ",ordprc       VARCHAR(16)" //주문가격 
                                                                           + ",execprc      VARCHAR(16)" //체결가격
                                                                           + ",ordptnDetail VARCHAR(16)" //상세 주문구분 신규매수|반복매수|금일매도|청산|
                                                                           + ",upOrdno      VARCHAR(16)" //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                                                                           + ",upExecprc    VARCHAR(16)" //상위체결금액
                                                                           + ",sellOrdAt    VARCHAR(1)" //매도주문 여부 YN default:N 금일매도일때 의미있다.
                                                                           + ",cancelOrdAt  VARCHAR(1)" //취소주문 여부 YN default:N 
                                                                           + ",useYN        VARCHAR(16)" //사용여부    

                                                                           + ",ordermtd     VARCHAR(16)" //주문매체
                                                                           + ",targClearPrc VARCHAR(16)" //목표청산가격 
                                                                           + ",secEntPrc    VARCHAR(16)" //2차진입가격 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
                                                                           + ",secEntAmt    VARCHAR(16)" //2차진입비중금액 
                                                                           + ",stopPrc      VARCHAR(16)" //손절가격 
                                                                           + ",exclWatchAt  VARCHAR(1)"  //감시제외여부 

                        + ", PRIMARY KEY(accno,Isuno,ordno));";
                        sqlCmd.ExecuteNonQuery();
                    }

                    sqlCmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

                //DB정보를 메모리에 저장(dataLogVoList)
                //this.historyDataTable = list();
                this.dbSync();
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
        ~TradingHistory()
        {         
            
        }
       
        public void dbSync()
        {
            //this.historyDataTable = list();
            if(tradingHistoryDt == null)
            {
                this.tradingHistoryDt = new DataTable();
            }

            tradingHistoryDt.Clear();
            tradingHistoryDt = list();
            //this.tradingHistoryVoList.Clear();
            //foreach (DataRow dr in DataTable.Rows)
            //{
            //    TradingHistoryVo dataLogVo     = new TradingHistoryVo();
            //    dataLogVo.ordno         = dr["ordno"        ].ToString(); //주문번호
            //    dataLogVo.dt            = dr["dt"           ].ToString(); //일시
            //    dataLogVo.accno         = dr["accno"        ].ToString(); //계좌번호
            //    dataLogVo.Isuno         = dr["Isuno"        ].ToString(); //종목코드
            //    dataLogVo.Isunm         = dr["Isunm"        ].ToString(); //종목명
            //    dataLogVo.ordptncode    = dr["ordptncode"   ].ToString(); //주문구분 01:매도|02:매수 
            //    dataLogVo.ordqty        = dr["ordqty"       ].ToString(); //주문수량  
            //    dataLogVo.execqty       = dr["execqty"      ].ToString(); //체결수량 
            //    dataLogVo.ordprc        = dr["ordprc"       ].ToString(); //주문가격 
            //    dataLogVo.execprc       = dr["execprc"      ].ToString(); //체결가격 
            //    dataLogVo.ordptnDetail  = dr["ordptnDetail" ].ToString(); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            //    dataLogVo.upOrdno       = dr["upOrdno"      ].ToString(); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            //    dataLogVo.upExecprc     = dr["upExecprc"    ].ToString(); //상위체결금액
            //    dataLogVo.sellOrdAt     = dr["sellOrdAt"    ].ToString(); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
            //    dataLogVo.cancelOrdAt   = dr["cancelOrdAt"  ].ToString(); //매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
            //    dataLogVo.useYN         = dr["useYN"        ].ToString(); //사용여부
                 
            //    dataLogVo.ordermtd      = dr["ordermtd"     ].ToString(); //주문매체
            //    dataLogVo.targClearPrc  = dr["targClearPrc" ].ToString(); //목표청산가격 
            //    dataLogVo.secEntPrc     = dr["secEntPrc"    ].ToString(); //2차진입가격 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
            //    dataLogVo.secEntAmt     = dr["secEntAmt"    ].ToString(); //2차진입비중금액 
            //    dataLogVo.stopPrc       = dr["stopPrc"      ].ToString(); //손절 
            //    dataLogVo.exclWatchAt   = dr["exclWatchAt"  ].ToString(); //감시제외여부 

            //    this.tradingHistoryVoList.Add(dataLogVo);
            //}
        }



        //public DataTable getHistoryDataTable()
        //{
        //    return this.historyDataTable;
        //}


        

        //등록
        public int insert(TradingHistoryVo dataLogVo){
            int result=0;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into trading          ( ordno                                          ");
                sb.Append("                             ,dt                                             "); //일시
                sb.Append("                             ,accno                                          "); //계좌번호
                sb.Append("                             ,Isuno                                          "); //종목코드
                sb.Append("                             ,Isunm                                          "); //종목명
                sb.Append("                             ,ordptncode                                     "); //주문구분 01:매도|02:매수 
                sb.Append("                             ,ordqty                                         ");//주문수량  
                sb.Append("                             ,execqty                                        "); //체결수량  
                sb.Append("                             ,ordprc                                         "); //주문가격
                sb.Append("                             ,execprc                                        "); //체결가격
                sb.Append("                             ,ordptnDetail                                   "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
                sb.Append("                             ,upOrdno                                        ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                             ,upExecprc                                      "); //상위체결금액
                sb.Append("                             ,sellOrdAt                                      "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
                sb.Append("                             ,cancelOrdAt                                    "); //취소주문 여부 YN
                sb.Append("                             ,ordermtd                                       "); //주문매체
                sb.Append("                             ,useYN ) VALUES(                                "); //사용여부 

                sb.Append("                                         '"  + dataLogVo.ordno + "'           "); //주문번호
                sb.Append("                                         ,'" + DateTime.Now.ToString("yyyyMMddHHmmss") + "'              "); //일시
                sb.Append("                                         ,'" + dataLogVo.accno + "'           "); //계좌번호
                sb.Append("                                         ,'" + dataLogVo.Isuno + "'           ");//종목코드
                sb.Append("                                         ,'" + dataLogVo.Isunm + "'           ");//종목명
                sb.Append("                                         ,'" + dataLogVo.ordptncode + "'      ");//주문구분 01:매도|02:매수 
                sb.Append("                                         ,'" + dataLogVo.ordqty + "'          ");//주문수량  
                sb.Append("                                         ,'" + dataLogVo.execqty + "'         ");//체결수량 
                sb.Append("                                         ,'" + dataLogVo.ordprc + "'          ");//주문가격
                sb.Append("                                         ,'" + dataLogVo.execprc + "'         ");//체결가격
                sb.Append("                                         ,'" + dataLogVo.ordptnDetail + "'    ");//상세 주문구분 신규매수|반복매수|금일매도|청산|
                sb.Append("                                         ,'" + dataLogVo.upOrdno + "'         ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                                         ,'" + dataLogVo.upExecprc + "'       ");//상위체결금액
                sb.Append("                                         ,'" + dataLogVo.sellOrdAt + "'       ");//매도주문 여부 YN default:N 금일매도일때 의미있다.
                sb.Append("                                         ,'" + dataLogVo.cancelOrdAt + "'     ");//매도주문 여부 YN
                sb.Append("                                         ,'" + dataLogVo.ordermtd + "'        ");//주문매체
                sb.Append("                                         ,'" + dataLogVo.useYN + "' )         ");//사용여부 

                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            dbSync();
            return result;
        }
        //읽기
        public DataTable read(TradingHistoryVo dataLogVo)
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
            sb.Append("       ,ordprc                               "); //주문가격  
            sb.Append("       ,execprc                              "); //체결가격
            sb.Append("       ,ordptnDetail                         "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            sb.Append("       ,upOrdno                              "); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("       ,upExecprc                            "); //상위체결금액
            sb.Append("       ,sellOrdAt                            "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
            sb.Append("       ,cancelOrdAt                          "); //매도주문 여부
            sb.Append("       ,useYN                                "); //사용여부 
  
            sb.Append("       ,ordermtd                             "); //주문매체
            sb.Append("       ,targClearPrc                         "); //목표청산가격 
            sb.Append("       ,secEntPrc                            "); //2차진입가격 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
            sb.Append("       ,secEntAmt                            "); //2차진입비중금액 
            sb.Append("       ,stopPrc                              "); //손절 
            sb.Append("       ,exclWatchAt                          "); //감시제외여부 
            
            sb.Append("FROM  trading                                "); //사용여부 
            sb.Append("WHERE ordno = '" + dataLogVo.ordno + "'      "); //주문번호
            sb.Append("AND   Isuno = '" + dataLogVo.Isuno + "'      "); //종목코드
            sb.Append("AND   accno = '" + dataLogVo.accno + "'      "); //계좌번호
            
            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }

        //감시제외 여부 상태값 업데이트 - 종목 일괄적용
        public int watchUpdate(TradingHistoryVo dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading SET exclWatchAt = '" + dataLogVo.exclWatchAt + "'  "); //감시제외여부  
                
                sb.Append("WHERE Isuno = '" + dataLogVo.Isuno + "'                                            "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account + "'                                           "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            this.dbSync();
            return result;
        }
        //사용여부 업데이트 - 종목 일괄적용
        public int useYnUpdate(DataRow dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading  SET   useYN = '" + dataLogVo["useYN"].ToString() + "'  "); //감시제외여부  
                sb.Append("WHERE Isuno = '" + dataLogVo["Isuno"].ToString()                      + "'  "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account                                   + "'  "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            this.dbSync();
            return result;
        }
        //취소주문 상태업데이트-종목일괄적용
        public int cancelOrdAtUpdate(TradingHistoryVo dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading SET cancelOrdAt = '" + dataLogVo.cancelOrdAt + "'  "); //체결수량
                sb.Append("WHERE  Isuno = '" + dataLogVo.Isuno + "'   "); //종목코드
                sb.Append("AND    accno = '" + mainForm.account + "'   "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return result;
        }
        //취소주문 상태업데이트-종목일괄적용
        public int cancelOrdAtUpdate(DataRow dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading SET cancelOrdAt = '" + dataLogVo["cancelOrdAt"].ToString() + "'  "); //체결수량
                sb.Append("WHERE  ordno = '" + dataLogVo["ordno"].ToString()                        + "'   "); //종목코드
                sb.Append("AND    accno = '" + mainForm.account                                     + "'   "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return result;
        }
        //체결수량업데이트 --체셜가격도같이 수정
        public int execqtyUpdate(DataRow dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading SET  execqty  = '" + dataLogVo["execqty"].ToString() + "'  "); //체결수량 
                sb.Append("                   ,execprc  = '" + dataLogVo["execprc"].ToString() + "'  "); //체결가격
                sb.Append("WHERE ordno = '" + dataLogVo["ordno"].ToString() + "'                "); //주문번호
                sb.Append("AND   Isuno = '" + dataLogVo["Isuno"].ToString() + "'                "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account              + "'                "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            return result;
        }
        //금일 매도 관련 매도주문 업데이트 sellOrdAt
        public int sellOrdAtUpdate(DataRow dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading  SET sellOrdAt = '" + dataLogVo["sellOrdAt"].ToString() + "'  "); //매도주문여부
                sb.Append(" WHERE accno = '" + mainForm.account                                   + "'  "); //계좌번호
                sb.Append("   AND ordno = '" + dataLogVo["ordno"].ToString()                      + "'  "); //주문번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
   
            return result;
        }
        //수동주문 진입청산 값 수정
        public int clearUpdate(TradingHistoryVo dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading  SET targClearPrc  = '" + dataLogVo.targClearPrc + "'  "); //목표청산가격  
                sb.Append("                   ,secEntPrc     = '" + dataLogVo.secEntPrc    + "'  "); //2차진입가격 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
                sb.Append("                   ,secEntAmt     = '" + dataLogVo.secEntAmt    + "'  "); //2차진입비중금액 
                sb.Append("                   ,stopPrc       = '" + dataLogVo.stopPrc      + "'  "); //손절
                sb.Append("WHERE accno = '" + mainForm.account + "'                              "); //계좌번호
                sb.Append("AND   Isuno = '" + dataLogVo.Isuno + "'                               "); //주문번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
    
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        ////금일매수 건 매도 여부 업데이트
        //public int execqtyUpdate(TradingHistoryVo dataLogVo)
        //{
        //    int result = 0;
        //    using (var conn = new SQLiteConnection(connStr))
        //    {
        //        conn.Open();

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("UPDATE trading SET sellOrdAt = '" + dataLogVo.sellOrdAt + "'  "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
        //        sb.Append("                   ,execqty     = '" + dataLogVo.execqty + "'  ");
        //        sb.Append("                   ,execprc     = '" + dataLogVo.execprc + "'  ");
        //        sb.Append("WHERE accno = '" + mainForm.account + "'     "); //계좌번호
        //        sb.Append("AND   ordno = '" + dataLogVo.ordno  + "'     "); //주문번호
        //        sb.Append("AND   Isuno = '" + dataLogVo.Isuno  + "'     "); //종목코드
        //        SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
        //        //Log.WriteLine(sb.ToString());
        //        result = sqlCmd.ExecuteNonQuery();

        //        sqlCmd.Dispose();
        //        conn.Close();
        //        conn.Dispose();
        //    }
        //    dbSync();
        //    return result;
        //}

        ////수정
        //public int update(DataTable dataLogVo)
        //{
        //    int result = 0;
        //    using (var conn = new SQLiteConnection(connStr))
        //    {
        //        conn.Open();

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("UPDATE trading         SET                                                         ");
        //        sb.Append("                              ordqty           = '" + dataLogVo.ordqty       + "'  "); //주문수량  
        //        sb.Append("                             ,execqty          = '" + dataLogVo.execqty      + "'  "); //체결수량  
        //        sb.Append("                             ,execprc          = '" + dataLogVo.execprc      + "'  "); //체결가격
        //        sb.Append("                             ,ordptnDetail     = '" + dataLogVo.ordptnDetail + "'  "); //상세 0 신규매수|반복매수|금일매도|청산|
        //        sb.Append("                             ,upOrdno          = '" + dataLogVo.upOrdno      + "'  ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        //        sb.Append("                             ,upExecprc        = '" + dataLogVo.upExecprc    + "'  "); //상위체결금액
        //        sb.Append("                             ,sellOrdAt        = '" + dataLogVo.sellOrdAt    + "'  "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
        //        sb.Append("                             ,cancelOrdAt      = '" + dataLogVo.cancelOrdAt  + "'  "); //금일매도주문 여부 YN
        //        sb.Append("                             ,useYN 			  = '" + dataLogVo.useYN        + "'  "); //사용여부 
        //        sb.Append("WHERE accno = '" + mainForm.account + "'                                           "); //계좌번호
        //        sb.Append("AND   ordno = '" + dataLogVo.ordno + "'                                            "); //주문번호
        //        sb.Append("AND   Isuno = '" + dataLogVo.Isuno + "'                                            "); //종목코드
        //        SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
        //        //Log.WriteLine(sb.ToString());
        //        result = sqlCmd.ExecuteNonQuery();

        //        sqlCmd.Dispose();
        //        conn.Close();
        //        conn.Dispose();
        //    }
        //    dbSync();
        //    return result;
        //}
       
        //로그인 할때 필요없는 매매 정보를 삭제해준다.
        public int initDelete()
        {
            //select * from trading where dt not like '20170727%' and useYN='N'
            //select * from trading where dt  not like'20170807%' and useYN='N'
            String date = DateTime.Now.ToString("yyyyMMdd");
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading                  ");
                sb.Append(" WHERE useYN       = 'N'             "); //종목코드
                sb.Append("   AND dt not like ('" + date + "%') "); //오늘날자
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            dbSync();
            return result;
        }
        
        //주문취소중 체결수가 0인것을 삭제하는 함수
        public int execqtyDelete(String isuno)
        {
            //select * from trading where dt not like '20170727%' and useYN='N'
            //select * from trading where dt  not like'20170807%' and useYN='N'
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading                  ");
                sb.Append(" WHERE Isuno       = '" + isuno + "' "); //종목코드
                sb.Append("   AND execqty     = '0'             ");
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            dbSync();
            return result;
        }
        //종목코드 기준으로 일괄삭제.
        public int isunoDelete(String isuno)
        {
            //select * from trading where dt not like '20170727%' and useYN='N'
            //select * from trading where dt  not like'20170807%' and useYN='N'
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading                  ");
                sb.Append(" WHERE Isuno       = '" + isuno + "' "); //종목코드
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            dbSync();
            return result;
        }
        //주문번호 단건 삭제.
        public int ordnoByDelete(String ordno)
        {
            //select * from trading where dt not like '20170727%' and useYN='N'
            //select * from trading where dt  not like'20170807%' and useYN='N'
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading                  ");
                sb.Append(" WHERE ordno       = '" + ordno + "' "); //종목코드
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            dbSync();
            return result;
        }



        //목록 리턴
        //public DataTable list()
        //{
        //    return list("");
        //}
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
            sb.Append("       ,ordprc                               "); //주문가격
            sb.Append("       ,execprc                              "); //체결가격
            sb.Append("       ,ordptnDetail                         "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            sb.Append("       ,upOrdno                              "); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("       ,upExecprc                            "); //상위체결금액
            sb.Append("       ,sellOrdAt                            "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
            sb.Append("       ,cancelOrdAt                          "); //매도주문 여부 YN
            sb.Append("       ,useYN                                "); //사용여부 

            sb.Append("       ,ordermtd                             "); //주문매체
            sb.Append("       ,targClearPrc                         "); //목표청산가격 
            sb.Append("       ,secEntPrc                            "); //2차진입가격 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
            sb.Append("       ,secEntAmt                            "); //2차진입비중금액
            sb.Append("       ,stopPrc                              "); //손절 
            sb.Append("       ,exclWatchAt                          "); //감시제외여부 

            sb.Append("FROM  trading                                ");  
            //sb.Append("WHERE useYN = 'Y'                            "); //사용여부
            //sb.Append("AND   accno = '" + mainForm.account + "'     "); //계좌번호--프로그램 로드시 계좌정보가 없다 (모든정보를 메모리에 로드를 기본 정책으로한다.)
            //if (Isuno != ""){
            //    sb.Append("WHERE   Isuno = '" + Isuno + "'            "); //종목코드
            //}
            sb.Append("ORDER BY   accno, Isuno, dt                  "); //주문번호

            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }
        
        public SummaryVo getSummaryVo(String Isuno)
        {
            SummaryVo summaryVo = new SummaryVo();

            //로그출력
            String 최초진입 = "";
            double 매입금액 = 0;
            double 총매수금액 = 0;
            double 총매도금액 = 0;
            double 총매도체결수량 = 0;
            double 총매수체결수량 = 0;
            double 매도가능수량 = 0;
            double 중간매도손익 = 0;
            int 매도횟수 = 0;
            int 매수횟수 = 0;
            if (Isuno == "095700")
            {
                int test = 0;
            }
            //DataTable dt = list(Isuno);
            //if (dt.Rows.Count == 0) { return null;}
            var items = from item in this.tradingHistoryDt.AsEnumerable()
                        where item["accno"].ToString() == mainForm.account
                           && item["Isuno"].ToString() == Isuno.Replace("A", "")
                           && item["useYN"].ToString() == "Y"
                       select item;

            if (items.Count() <= 0)
            {
                return null;
            }
           
            foreach (DataRow item in items)
            {
                if (item["ordptncode"].ToString() == "02" && item["useYN"].ToString() == "Y")//매수그룹
                {   //총매수금액 + 체결수량+체결가격
                    Double test = Double.Parse(item["execqty"].ToString());
                    Double t2 = Double.Parse(item["execprc"].ToString());
                    총매수금액 = 총매수금액 + (Double.Parse(item["execqty"].ToString()) * Double.Parse(item["execprc"].ToString()));
                    //매도가능수량 = 매도가능수량 + Double.Parse(item.execqty);
                    총매수체결수량 = 총매수체결수량 + Double.Parse(item["execqty"].ToString());
                    매수횟수 = 매수횟수 + 1;
                    if (매수횟수 == 1)
                    {
                        최초진입 = item["dt"].ToString();
                    }
                }
                else if (item["ordptncode"].ToString() == "01" && item["useYN"].ToString() == "Y")
                {//매도그룹
                    
                    총매도금액 = 총매도금액 + (Double.Parse(item["execqty"].ToString()) * Double.Parse(item["execprc"].ToString()));

                    //매도가능수량 = 매도가능수량 - Double.Parse(item.execqty);
                    총매도체결수량 = 총매도체결수량 + Double.Parse(item["execqty"].ToString());
                    매도횟수     = 매도횟수 + 1;
                    중간매도손익  = 중간매도손익 + ((Double.Parse(item["execqty"].ToString()) * Double.Parse(item["execprc"].ToString())) - (Double.Parse(item["execqty"].ToString()) * Double.Parse(item["upExecprc"].ToString())));
                }

            }
            매입금액     = 총매수금액 - 총매도금액;
            매도가능수량 = 총매수체결수량 - 총매도체결수량;

            //double 평균단가         = (매입금액 / 매도가능수량);
            double 평균단가 = (총매수금액 / 총매수체결수량);

            summaryVo.pamt2         = 평균단가.ToString(); //평균단가
            summaryVo.buyCnt        = 매수횟수.ToString(); //매수횟수
            summaryVo.sellCnt       = 매도횟수.ToString();//매도횟수
            summaryVo.sellSunik     = 중간매도손익.ToString(); //중간매도손익
            summaryVo.firstBuyDt    = 최초진입;
            summaryVo.sumMdposqt = 매도가능수량.ToString();

            if (items.First()["ordermtd"].ToString().Equals("")){
                summaryVo.ordermtd = "XING API";
            }else{
                summaryVo.ordermtd = items.First()["ordermtd"].ToString();       //주문매체 - 감시제외 일때 사용
            }
            //summaryVo.ordermtd      = items.First()["ordermtd"      ].ToString();       //주문매체 - 감시제외 일때 사용
            summaryVo.targClearPrc  = items.First()["targClearPrc"  ].ToString();   //목표청산가격    - 감시제외 일때 사용
            summaryVo.secEntPrc     = items.First()["secEntPrc"     ].ToString();      //2차진입가격     - 감시제외 일때 사용 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
            summaryVo.secEntAmt     = items.First()["secEntAmt"     ].ToString();      //2차진입비중가격 - 감시제외 일때 사용
            summaryVo.stopPrc       = items.First()["stopPrc"       ].ToString();        //손절가격 - 감시제외 일때 사용
            summaryVo.exclWatchAt   = items.First()["exclWatchAt"   ].ToString();    //감시제외여부

            return summaryVo;

        }
        


    }   // end class


    //매매이력 정보
    public class TradingHistoryVo
    {
        public String ordno        { set; get; }//주문번호 key
        public String dt           { set; get; }//일시
        public String accno        { set; get; }//계좌번호
        public String Isuno        { set; get; }//종목코드
        public String Isunm        { set; get; }//종목명
        public String ordptncode   { set; get; }//주문구분 01:매도|02:매수 
        public String ordqty       { set; get; }// 주문수량
        public String ordprc       { set; get; }// 주문가격
        public String execqty      { set; get; }// 체결수량
        public String execprc      { set; get; }// 체결가격
        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산 - 2차추가매수|수동신규매수|수동반복매수
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public String upExecprc    { set; get; }//상위체결금액
        public String sellOrdAt    { set; get; }//금일매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String cancelOrdAt  { set; get; }//주문취소 여부 YN default:N    
        public String useYN        { set; get; }//사용여부

        //목표수익율-안만들어도 될듯 당일 기준으로 업데이트 해주자.
        public String ordermtd     { set; get; }//주문매체 - 감시제외 일때 사용
        public String targClearPrc { set; get; }//목표청산가격    - 감시제외 일때 사용
        public String secEntPrc    { set; get; }//2차진입가격     - 감시제외 일때 사용 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
        public String secEntAmt    { set; get; }//2차진입비중가격 - 감시제외 일때 사용
        public String stopPrc      { set; get; }//손절가격 - 감시제외 일때 사용
        public String exclWatchAt  { set; get; }//감시제외여부

    }
    //종목별 매매이력 정보를 리턴한다.
    public class SummaryVo
    {
        public String pamt2     { set; get; } //평균단가
        public String buyCnt    { set; get; } //매수횟수
        public String sellCnt   { set; get; } //매도횟수
        public String sellSunik { set; get; } //중간매도손익
        public String firstBuyDt{ set; get; } //최초진입일시
        public String sumMdposqt{ set; get; } //매도가능수량

        public String ordermtd { set; get; }//주문매체 - 감시제외 일때 사용
        public String targClearPrc { set; get; }//목표청산가격    - 감시제외 일때 사용
        public String secEntPrc { set; get; }//2차진입가격     - 감시제외 일때 사용 - 이값이 설정되어있지않으면 2차진입이 실행 되었거나 설정을 안한 케이스.
        public String secEntAmt { set; get; }//2차진입비중가격 - 감시제외 일때 사용
        public String stopPrc { set; get; }//손절가격 - 감시제외 일때 사용
        public String exclWatchAt { set; get; }//감시제외여부
    }

}   // end namespace
