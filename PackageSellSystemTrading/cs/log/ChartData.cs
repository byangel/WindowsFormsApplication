﻿using System;
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
    public class ChartData
    {

        //private DataTable historyDataTable;
        private EBindingList<ChartDataVo> chartDataVoList;
        public MainForm mainForm;
        private String connStr = @"Data Source=" + Util.GetCurrentDirectoryWithPath() + "\\logs\\history.db;Pooling=true;FailIfMissing=false";

        public EBindingList<ChartDataVo> getDataLogVoList()
        {
            return this.chartDataVoList;
        }

        // 생성자
        public ChartData()
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
                                                                           + ",ordprc       VARCHAR(16)" //주문가격 
                                                                           + ",execprc      VARCHAR(16)" //체결가격
                                                                           + ",ordptnDetail VARCHAR(16)" //상세 주문구분 신규매수|반복매수|금일매도|청산|
                                                                           + ",upOrdno      VARCHAR(16)" //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                                                                           + ",upExecprc    VARCHAR(16)" //상위체결금액
                                                                           + ",sellOrdAt    VARCHAR(1)" //매도주문 여부 YN default:N 금일매도일때 의미있다.
                                                                           + ",cancelOrdAt  VARCHAR(1)" //취소주문 여부 YN default:N 
                                                                           + ",useYN        VARCHAR(16)" //사용여부    

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
        ~ChartData()
        {         
            
        }
       
        public void dbSync()
        {
            //this.historyDataTable = list();
            if(chartDataVoList == null)
            {
                this.chartDataVoList = new EBindingList<ChartDataVo>();
            }
            

            DataTable dt = list();
            this.chartDataVoList.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ChartDataVo dataLogVo     = new ChartDataVo();
                dataLogVo.ordno         = dr["ordno"].ToString();    //주문번호
                dataLogVo.dt            = dr["dt"].ToString();       //일시
                dataLogVo.accno         = dr["accno"].ToString();    //계좌번호
                dataLogVo.Isuno         = dr["Isuno"].ToString();    //종목코드
                dataLogVo.Isunm         = dr["Isunm"].ToString();    //종목명
                dataLogVo.ordptncode    = dr["ordptncode"].ToString();//주문구분 01:매도|02:매수 
                dataLogVo.ordqty        = dr["ordqty"].ToString();   //주문수량  
                dataLogVo.execqty       = dr["execqty"].ToString();  //체결수량 
                dataLogVo.ordprc        = dr["ordprc"].ToString();   //주문가격 
                dataLogVo.execprc       = dr["execprc"].ToString();  //체결가격 
                dataLogVo.ordptnDetail  = dr["ordptnDetail"].ToString();//상세 주문구분 신규매수|반복매수|금일매도|청산|
                dataLogVo.upOrdno       = dr["upOrdno"].ToString();  //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                dataLogVo.upExecprc     = dr["upExecprc"].ToString();//상위체결금액
                dataLogVo.sellOrdAt     = dr["sellOrdAt"].ToString();//매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                dataLogVo.cancelOrdAt   = dr["cancelOrdAt"].ToString();//매도주문 여부 YN default:N -02:매 일때만 값이 있어야한다.
                dataLogVo.useYN         = dr["useYN"].ToString();    //사용여부 

                this.chartDataVoList.Add(dataLogVo);
            }
        }



        //public DataTable getHistoryDataTable()
        //{
        //    return this.historyDataTable;
        //}


        

        //등록
        public int insert(ChartDataVo dataLogVo){
            int result=0;
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
                sb.Append("                             ,ordprc                                         "); //주문가격
                sb.Append("                             ,execprc                                        "); //체결가격
                sb.Append("                             ,ordptnDetail                                   "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
                sb.Append("                             ,upOrdno                                        ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                             ,upExecprc                                      "); //상위체결금액
                sb.Append("                             ,sellOrdAt                                      "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
                sb.Append("                             ,cancelOrdAt                                    "); //취소주문 여부 YN
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
                sb.Append("                                         ,'" + dataLogVo.useYN + "' )         ");//사용여부 

                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
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
            sb.Append("       ,ordprc                               "); //주문가격  
            sb.Append("       ,execprc                              "); //체결가격
            sb.Append("       ,ordptnDetail                         "); //상세 주문구분 신규매수|반복매수|금일매도|청산|
            sb.Append("       ,upOrdno                              "); //상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
            sb.Append("       ,upExecprc                            "); //상위체결금액
            sb.Append("       ,sellOrdAt                            "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
            sb.Append("       ,cancelOrdAt                          "); //매도주문 여부
            sb.Append("       ,useYN                                "); //사용여부 
            sb.Append("FROM  trading_history                        "); //사용여부 
            sb.Append("WHERE ordno = '" + ordno + "'                "); //주문번호
            sb.Append("AND   Isuno = '" + Isuno + "'                "); //종목코드
            sb.Append("AND   accno = '" + mainForm.account + "'     "); //계좌번호
            
            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }




        ////수정
        public int update(ChartDataVo dataLogVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE trading_history SET                                                         ");
                sb.Append("                              ordqty           = '" + dataLogVo.ordqty       + "'  "); //주문수량  
                sb.Append("                             ,execqty          = '" + dataLogVo.execqty      + "'  "); //체결수량  
                sb.Append("                             ,execprc          = '" + dataLogVo.execprc      + "'  "); //체결가격
                sb.Append("                             ,ordptnDetail     = '" + dataLogVo.ordptnDetail + "'  "); //상세 0 신규매수|반복매수|금일매도|청산|
                sb.Append("                             ,upOrdno          = '" + dataLogVo.upOrdno      + "'  ");//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
                sb.Append("                             ,upExecprc        = '" + dataLogVo.upExecprc    + "'  "); //상위체결금액
                sb.Append("                             ,sellOrdAt        = '" + dataLogVo.sellOrdAt    + "'  "); //매도주문 여부 YN default:N 금일매도일때 의미있다.
                sb.Append("                             ,cancelOrdAt      = '" + dataLogVo.cancelOrdAt  + "'  "); //금일매도주문 여부 YN
                sb.Append("                             ,useYN 			  = '" + dataLogVo.useYN        + "'  "); //사용여부 
                sb.Append("WHERE Isuno = '" + dataLogVo.Isuno + "'                                            "); //주문번호
                sb.Append("AND   ordno = '" + dataLogVo.ordno + "'                                            "); //종목코드
                sb.Append("AND   accno = '" + mainForm.account + "'                                           "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }
       


        //삭제
        public int delete(String Isuno, String ordno)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("DELETE FROM trading_history ordno           ");
                sb.Append(" WHERE Isuno = '" + Isuno + "'              "); //종목코드
                sb.Append("   AND ordno = '" + ordno + "'              "); //주문번호
                sb.Append("   AND accno = '" + mainForm.account + "'   "); //계좌번호
                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }

        //목록 리턴
        public DataTable list()
        {
            return list("");
        }
        //목록
        public DataTable list(String Isuno)
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
            sb.Append("FROM  trading_history                        "); //사용여부 
            //sb.Append("WHERE useYN = 'Y'                          "); //사용여부
            //sb.Append("AND   accno = '" + mainForm.account + "'     "); //계좌번호--프로그램 로드시 계좌정보가 없다 (모든정보를 메모리에 로드를 기본 정책으로한다.)
            if (Isuno != ""){
                sb.Append("AND   Isuno = '" + Isuno + "'            "); //종목코드
            }
            sb.Append("ORDER BY   accno, Isuno, dt                  "); //주문번호

            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }


        
        


    }   // end class


    //매매이력 정보
    public class ChartDataVo
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
        public String ordptnDetail { set; get; }//상세 주문구분 신규매수|반복매수|금일매도|청산
        public String upOrdno      { set; get; }//상위 매수 주문번호 -값이없으면 자신의 주문번호로 넣는다.
        public String upExecprc    { set; get; }//상위체결금액
        public String sellOrdAt    { set; get; }//금일매도주문 여부 YN default:N     -02:매 일때만 값이 있어야한다.
        public String cancelOrdAt  { set; get; }//주문취소 여부 YN default:N    
        public String useYN        { set; get; }//사용여부

    }
    

}   // end namespace
