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




namespace PackageSellSystemTrading
{

 /// <summary>
 /// 실시간 체결정보를 기록하는 클래스 - 금일매수매도를 구현을 목표로한다.
 /// </summary>
 /// <returns>StreamWriter</returns>
    public class ChartData
    {

     //private DataTable historyDataTable;
        private EBindingList<ChartVo> chartVoList;//DataTable 로 교체 예정
        private DataTable dataTable;

        private Double sumDtsunik;//누적 실현손익
        public MainForm mainForm;
        private String connStr = @"Data Source=" + Util.GetCurrentDirectoryWithPath() + "\\logs\\history.db;Pooling=true;FailIfMissing=false";

        public EBindingList<ChartVo> getChartVoList()
        {
            return this.chartVoList;
        }
        public DataTable getDataTable()
        {
            return this.dataTable;
        }
        public Double getSumDtsunik()
        {
            return this.sumDtsunik;
        }

     // 생성자
        public ChartData()
        {
            try
            {

             // //디렉토리 체크
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
                    SQLiteCommand sqlCmd = new SQLiteCommand("SELECT COUNT(*) cnt FROM sqlite_master WHERE name = 'chart'", conn);

                    if (Convert.ToInt32(sqlCmd.ExecuteScalar()) <= 0)
                    {
                        sqlCmd.CommandText = "CREATE TABLE chart ("
                                                                + " date             VARCHAR(16)" //날자         
                                                                + ",d2Dps            VARCHAR(16)" //예수금(D2)    
                                                                + ",dpsastTotamt     VARCHAR(16)" //예탁자산총액     
                                                                + ",mamt             VARCHAR(16)" //매입금액       
                                                                + ",balEvalAmt       VARCHAR(16)" //매입평가금액     
                                                                + ",pnlRat           VARCHAR(16)" //손익율        
                                                                + ",tdtsunik         VARCHAR(16)" //평가손익       
                                                                + ",dtsunik          VARCHAR(16)" //실현손익       
                                                                + ",battingAtm       VARCHAR(16)" //배팅금액       
                                                                + ",toDaysunik       VARCHAR(16)" //당일매도 실현손익  
                                                                + ",dtsunik2         VARCHAR(16)" //실현손익2      
                                                                + ",investmentRatio  VARCHAR(16)" //투자율        
                                                                + ",itemTotalCnt     VARCHAR(16)" //계좌잔고(매수종목수)
                                                                + ",buyFilterCnt     VARCHAR(16)"  //매수금지종목수    
                                                                + ",buyCnt           VARCHAR(16)"  //매수횟수       
                                                                + ",sellCnt          VARCHAR(16)" //매도횟수       
                                                                + ", PRIMARY KEY(date));";
                        sqlCmd.ExecuteNonQuery();
                    }

                    sqlCmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

             //DB정보를 메모리에 저장(chartVoList)
             //this.historyDataTable = list();
                this.dbSync();
             /////////////////////////////DB 설정//////////////////////////////////   
            }
            catch (Exception ex)
            {
                Log.WriteLine("DataLog : " + ex.Message);
                Log.WriteLine("DataLog : " + ex.StackTrace);
            }
            finally
            {

            }

        }
     

        public void dbSync()
        {
         //this.historyDataTable = list();
            if (chartVoList == null)
            {
                this.chartVoList = new EBindingList<ChartVo>();
            }

            this.dataTable = list();
            this.chartVoList.Clear();
            foreach (DataRow dr in dataTable.Rows)
            {
                ChartVo chartVo = new ChartVo();
                chartVo.date            = dr["date"             ].ToString();//날자         
                chartVo.d2Dps           = Double.Parse(dr["d2Dps"            ].ToString());//예수금(D2)    
                chartVo.dpsastTotamt    = Double.Parse(dr["dpsastTotamt"     ].ToString());//예탁자산총액     
                chartVo.mamt            = Double.Parse(dr["mamt"             ].ToString());//매입금액       
                chartVo.balEvalAmt      = Double.Parse(dr["balEvalAmt"       ].ToString());//매입평가금액     
                chartVo.pnlRat          = Double.Parse(dr["pnlRat"           ].ToString());//손익율        
                chartVo.tdtsunik        = Double.Parse(dr["tdtsunik"         ].ToString());//평가손익       
                chartVo.dtsunik         = Double.Parse(dr["dtsunik"          ].ToString());//실현손익       
                chartVo.battingAtm      = Double.Parse(dr["battingAtm"       ].ToString());//배팅금액       
                chartVo.toDaysunik      = Double.Parse(dr["toDaysunik"       ].ToString());//당일매도 실현손익  
                chartVo.dtsunik2        = Double.Parse(dr["dtsunik2"         ].ToString());//실현손익2      
                chartVo.investmentRatio = Double.Parse(dr["investmentRatio"  ].ToString());//투자율        
                chartVo.itemTotalCnt    = Double.Parse(dr["itemTotalCnt"     ].ToString());//계좌잔고(매수종목수)
                chartVo.buyFilterCnt    = Double.Parse(dr["buyFilterCnt"     ].ToString());//매수금지종목수    
                chartVo.buyCnt          = Double.Parse(dr["buyCnt"           ].ToString());//매수횟수       
                chartVo.sellCnt = Double.Parse(dr["sellCnt"].ToString());//매도횟수       

                this.chartVoList.Add(chartVo);
            }

            //누적 실현손익 구한다.
            this.sumDtsunik = dataTable.AsEnumerable().Select(row => double.Parse(row["dtsunik"].ToString())).Sum();
           
        }

     //등록
        public int insert(ChartVo chartVo)
        {
            int result = 0;
            using (SQLiteConnection conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("insert into chart           ( date                                               "); //날자          
                sb.Append("                             ,d2Dps                                              "); //예수금(D2)     
                sb.Append("                             ,dpsastTotamt                                       "); //예탁자산총액      
                sb.Append("                             ,mamt                                               "); //매입금액        
                sb.Append("                             ,balEvalAmt                                         "); //매입평가금액      
                sb.Append("                             ,pnlRat                                             "); //손익율         
                sb.Append("                             ,tdtsunik                                           "); //평가손익        
                sb.Append("                             ,dtsunik                                            "); //실현손익        
                sb.Append("                             ,battingAtm                                         "); //배팅금액        
                sb.Append("                             ,toDaysunik                                         "); //당일매도 실현손익   
                sb.Append("                             ,dtsunik2                                           "); //실현손익2       
                sb.Append("                             ,investmentRatio                                    "); //투자율         
                sb.Append("                             ,itemTotalCnt                                       "); //계좌잔고(매수종목수) 
                sb.Append("                             ,buyFilterCnt                                       "); //매수금지종목수     
                sb.Append("                             ,buyCnt                                             "); //매수횟수        
                sb.Append("                             ,sellCnt                                            "); //매도횟수        
                sb.Append("                             )VALUES(                                            ");
                sb.Append("                              '" + chartVo.date              + "'            ");//날자              
                sb.Append("                             ,'" + chartVo.d2Dps             + "'            ");//예수금(D2)         
                sb.Append("                             ,'" + chartVo.dpsastTotamt      + "'            ");//예탁자산총액          
                sb.Append("                             ,'" + chartVo.mamt              + "'            ");//매입금액            
                sb.Append("                             ,'" + chartVo.balEvalAmt        + "'            ");//매입평가금액          
                sb.Append("                             ,'" + chartVo.pnlRat            + "'            ");//손익율             
                sb.Append("                             ,'" + chartVo.tdtsunik          + "'            ");//평가손익            
                sb.Append("                             ,'" + chartVo.dtsunik           + "'            ");//실현손익            
                sb.Append("                             ,'" + chartVo.battingAtm        + "'            ");//배팅금액            
                sb.Append("                             ,'" + chartVo.toDaysunik        + "'            ");//당일매도 실현손익       
                sb.Append("                             ,'" + chartVo.dtsunik2          + "'            ");//실현손익2           
                sb.Append("                             ,'" + chartVo.investmentRatio   + "'            ");//투자율             
                sb.Append("                             ,'" + chartVo.itemTotalCnt      + "'            ");//계좌잔고(매수종목수)     
                sb.Append("                             ,'" + chartVo.buyFilterCnt      + "'            ");//매수금지종목수         
                sb.Append("                             ,'" + chartVo.buyCnt            + "'            ");//매수횟수            
                sb.Append("                             ,'" + chartVo.sellCnt           + "' )          ");//매도횟수  


                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }




     ////수정
        public int update(ChartVo chartVo)
        {
            int result = 0;
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE chart           SET                                                         ");
                sb.Append("                              date             = '" + chartVo.date           + "'  ");//날자             
                sb.Append("                             ,d2Dps            = '" + chartVo.d2Dps          + "'  ");//예수금(D2)        
                sb.Append("                             ,dpsastTotamt     = '" + chartVo.dpsastTotamt   + "'  ");//예탁자산총액         
                sb.Append("                             ,mamt             = '" + chartVo.mamt           + "'  ");//매입금액           
                sb.Append("                             ,balEvalAmt       = '" + chartVo.balEvalAmt     + "'  ");//매입평가금액         
                sb.Append("                             ,pnlRat           = '" + chartVo.pnlRat         + "'  ");//손익율            
                sb.Append("                             ,tdtsunik         = '" + chartVo.tdtsunik       + "'  ");//평가손익           
                sb.Append("                             ,dtsunik          = '" + chartVo.dtsunik        + "'  ");//실현손익           
                sb.Append("                             ,battingAtm       = '" + chartVo.battingAtm     + "'  ");//배팅금액           
                sb.Append("                             ,toDaysunik       = '" + chartVo.toDaysunik     + "'  ");//당일매도 실현손익      
                sb.Append("                             ,dtsunik2         = '" + chartVo.dtsunik2       + "'  ");//실현손익2          
                sb.Append("                             ,investmentRatio  = '" + chartVo.investmentRatio + "'  ");//투자율            
                sb.Append("                             ,itemTotalCnt     = '" + chartVo.itemTotalCnt   + "'  ");//계좌잔고(매수종목수)    
                sb.Append("                             ,buyFilterCnt     = '" + chartVo.buyFilterCnt   + "'  ");//매수금지종목수        
                sb.Append("                             ,buyCnt           = '" + chartVo.buyCnt         + "'  ");//매수횟수           
                sb.Append("                             ,sellCnt          = '" + chartVo.sellCnt        + "'  ");//매도횟수           


                sb.Append("WHERE date = '" + chartVo.date + "'                                              "); //날자

                SQLiteCommand sqlCmd = new SQLiteCommand(sb.ToString(), conn);
             //Log.WriteLine(sb.ToString());
                result = sqlCmd.ExecuteNonQuery();

                sqlCmd.Dispose();
                conn.Close();
                conn.Dispose();
            }
            return result;
        }


     //목록
        public DataTable list()
        {
            DataTable dt = new DataTable();
         //string sql = "SELECT * FROM member WHERE Id>=2";
         //쿼리정의
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT  date                                    ");//날자         
            sb.Append("       ,d2Dps                                   ");//예수금(D2)    
            sb.Append("       ,dpsastTotamt                            ");//예탁자산총액     
            sb.Append("       ,mamt                                    ");//매입금액       
            sb.Append("       ,balEvalAmt                              ");//매입평가금액     
            sb.Append("       ,pnlRat                                  ");//손익율        
            sb.Append("       ,tdtsunik                                ");//평가손익       
            sb.Append("       ,dtsunik                                 ");//실현손익       
            sb.Append("       ,battingAtm                              ");//배팅금액       
            sb.Append("       ,toDaysunik                              ");//당일매도 실현손익  
            sb.Append("       ,dtsunik2                                ");//실현손익2      
            sb.Append("       ,investmentRatio                         ");//투자율        
            sb.Append("       ,itemTotalCnt                            ");//계좌잔고(매수종목수)
            sb.Append("       ,buyFilterCnt                            ");//매수금지종목수    
            sb.Append("       ,buyCnt                                  ");  //매수횟수  
            sb.Append("       ,sellCnt                                 ");//매도횟수                          
            sb.Append("FROM   chart                                    ");

            sb.Append("ORDER BY   date   DESC               "); //날자
            String test = sb.ToString();
            var adpt = new SQLiteDataAdapter(sb.ToString(), connStr);
            adpt.Fill(dt);
            return dt;
        }

     //누적 수익금액을 리턴한다.
        public String getAccumulate()
        {

            return "";
        }
    }// end class

   



 //매매이력 정보
    public class ChartVo
    {
        public String date              { set; get; }//날자
        public Double d2Dps             { set; get; }//예수금(D2)
        public Double dpsastTotamt      { set; get; }//예탁자산총액
        public Double mamt              { set; get; }//매입금액
        public Double balEvalAmt        { set; get; }//매입평가금액
        public Double pnlRat            { set; get; }//손익율
        public Double tdtsunik          { set; get; }//평가손익
        public Double dtsunik           { set; get; }//실현손익
        public Double battingAtm        { set; get; }//배팅금액
        public Double toDaysunik        { set; get; }//당일매도 실현손익
        public Double dtsunik2          { set; get; }//실현손익2
        public Double investmentRatio   { set; get; }//투자율
        public Double itemTotalCnt      { set; get; }//계좌잔고(매수종목수)
        public Double buyFilterCnt      { set; get; }//매수금지종목수
        public Double buyCnt            { set; get; }//매수횟수
        public Double sellCnt           { set; get; }//매도횟수
    }


}// end namespace
