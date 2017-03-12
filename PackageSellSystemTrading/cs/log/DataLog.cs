using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{

    /// <summary>
    /// 실시간 체결정보를 기록하는 클래스 - 금일매수매도를 구현을 목표로한다.
    /// </summary>
    /// <returns>StreamWriter</returns>
    class DataLog
    {
        private  FileStream mStream;
        private  StreamWriter mStreamWriter;             // 파일 쓰기를 위한 스트림

        public  List<DataLogVo> dataLogVoList;

        // 생성자
        public DataLog()
        {
            mStreamWriter = GetStreamWriter();
            dataLogVoList = new List<DataLogVo>();
        }   // end function

        // 소멸자
        ~DataLog()
        {         
            /// Log 파일 쓰기 스트림 종료
            if (mStreamWriter != null)
            {
                mStreamWriter.Close();
                mStream.Close();
            }
        }



       
        private  StreamWriter GetStreamWriter()
        {
            if (mStreamWriter == null)
            {
                String  filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.txt";
                String  dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                    // 기존에 생성된 data 로그 파일이 있다면...
                    FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                   mStream = new FileStream(filePath, FileMode.Append);   
                }
                // 신규로 파일 생성
                else
                {
                    mStream = new FileStream(filePath, FileMode.Create);
                }

                mStreamWriter = new StreamWriter(mStream, Encoding.GetEncoding("euc-kr"));
                mStreamWriter.AutoFlush = true;
            }

            return mStreamWriter;
        }   // end function


        /// <summary>
        /// Log 파일에 메세지 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public  void WriteLine(string szMsg)
        {
            GetStreamWriter().WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " :: " + szMsg);
        }   // end function

        /// <summary>
        /// 매수 이벤트가 발생하면 listVo 및 dataLog파일에 정보를 추가를 담당한다.
        /// </summary>
        /// <param name="szMsg"></param>
        public  void insertMerge(DataLogVo dataLogVo)
        {
            //dataLogVoList 가 null이면 dataLog 파일을 읽어들여 동기화한다.
            if (dataLogVoList == null)
            {
                init();
            }

            //매수주문번호로 커리 호출
            DataLogVo tmpDataLogVo;
            var result_dataLogVo = from item in dataLogVoList
                                  where item.ordno == dataLogVo.ordno
                                  select item;

            //dataLogVoList에 주문번호정보여부에따라 매수정보 수정및 추가를 해준다. 
            if (result_dataLogVo.Count() > 0)
            {
                tmpDataLogVo = result_dataLogVo.ElementAt(0);
                //매수 체결수량 = 매수 체결수량 + 매수 체결수량
                tmpDataLogVo.buyExecQty = (int.Parse(tmpDataLogVo.buyExecQty) + int.Parse(dataLogVo.buyExecQty)).ToString();
            }
            else
            {
                dataLogVoList.Add(dataLogVo);
            }

            
            //data Create 모드이면 파일 append 
            //data update 모드일때 파일 원하는 라인만 update  어떻게 하나?
            //StringBuilder sb = new StringBuilder() ;
          
            //sb.Append(dataLogVo.datetime);          //일자
            //sb.Append("|" + dataLogVo.shcode);      //종목코드
            //sb.Append("|" + dataLogVo.hname);       //종목명

            //sb.Append("|" + dataLogVo.ordno);       //매수 주문번호

            //sb.Append("|" + dataLogVo.buyOrdQty);   //매수 주문수량
            //sb.Append("|" + dataLogVo.buyOrdPrc);   //매수 주문가격
            //sb.Append("|" + dataLogVo.buyExecQty);  //매수 체결수량
            //sb.Append("|" + dataLogVo.buyExecPrc);  //매수 체결가격

            //sb.Append("|" + dataLogVo.sellOrdQty);  //매도 주문수량
            //sb.Append("|" + dataLogVo.sellOrdPrc);  //매도 주문가격
            //sb.Append("|" + dataLogVo.sellExecQty); //매도 체결수량
            //sb.Append("|" + dataLogVo.sellExecPrc); //매도 체결가격

            
            //GetStreamWriter().WriteLine(sb.ToString());
        }


       

        public  void init()
        {
            dataLogVoList = new List<DataLogVo>();
        }

    


    }   // end class

    public class DataLogVo
    {
        public String datetime    { set; get; }//일자
        public String shcode      { set; get; }//종목코드
        public String hname       { set; get; }//종목명

        public String ordno       { set; get; }//매수 주문번호

        public String buyOrdQty   { set; get; }//매수 주문수량
        public String buyOrdPrc   { set; get; }//매수 주문가격
        public String buyExecQty  { set; get; }//매수 체결수량
        public String buyExecPrc  { set; get; }//매수 체결가격

        public String sellOrdQty  { set; get; }//매도 주문수량
        public String sellOrdPrc  { set; get; }//매도 주문가격
        public String sellExecQty { set; get; }//매도 체결수량
        public String sellExecPrc { set; get; }//매도 체결가격

    }

}   // end namespace
