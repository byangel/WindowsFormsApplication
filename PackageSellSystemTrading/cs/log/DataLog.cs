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
    public class DataLog
    {
        private  FileStream   mStream;
        private  StreamWriter mStreamWriter;             // 파일 쓰기를 위한 스트림

        public  EBindingList<DataLogVo> dataLogVoList;

        public MainForm mainForm;

        // 생성자
        public DataLog()
        {
            dataLogVoList = new EBindingList<DataLogVo>();
        }  
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

        //로그파일도 생성하고 잔고그리드에서 데이타를 초기데이타로 사용한다.
        private  StreamWriter GetStreamWriter()
        {
            //dataLog 파일을 읽어서 dataLogVoList저장 
            //dataLog 파일이 없다면 계좌잔고를 가져와서 설정
            if (mStreamWriter == null)
            {

                String  filePath = Util.GetCurrentDirectoryWithPath() + "\\logs\\data.txt";
                String  dirPath = Util.GetCurrentDirectoryWithPath() + "\\logs";
                String tmpFileMode;
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                    // 기존에 생성된 data 로그 파일이 있다면...
                    FileInfo file = new FileInfo(filePath);
                if (file.Exists)
                {
                   mStream = new FileStream(filePath, FileMode.Append);
                    tmpFileMode = "append";
                }  else {// 신규로 파일 생성
                    mStream = new FileStream(filePath, FileMode.Create);
                    tmpFileMode = "create";


                }

                this.mStreamWriter = new StreamWriter(mStream, Encoding.GetEncoding("euc-kr"));
                this.mStreamWriter.AutoFlush = true;

                //새로 생성되었다면
                if (tmpFileMode == "create") {
                    //dataLog 파일이 없다면 계좌잔고를 가져와서 설정
                    EBindingList<T0424Vo> t0424VoList = ((EBindingList<T0424Vo>)mainForm.grd_t0424.DataSource);
                    for (int i = 0; i < t0424VoList.Count(); i++)
                    {
                        DataLogVo dataLogVo = new DataLogVo();
                        dataLogVo.accno = t0424VoList.ElementAt(0).expcode;  //계좌번호
                        dataLogVo.Isuno = t0424VoList.ElementAt(0).expcode;  //종목코드
                        dataLogVo.Isunm = t0424VoList.ElementAt(0).hname;    //종목명
                        dataLogVo.ordptncode = "02";                              //주문구분 01:매도|02:매수
                        dataLogVo.ordno = "";                                //주문번호
                        dataLogVo.ordqty = t0424VoList.ElementAt(0).mdposqt;  //주문수량 - 매도가능수량
                        dataLogVo.ordprc = t0424VoList.ElementAt(0).pamt;     //주문가격 - 평균단가
                        dataLogVo.execqty = t0424VoList.ElementAt(0).mdposqt;  //체결수량 - 매도가능수량
                        dataLogVo.execprc = t0424VoList.ElementAt(0).pamt;     //체결가격 - 평균단가

                        this.WriteLine(dataLogVo);
                    }
                }
               
            }

            return this.mStreamWriter;
        }   // end function


        /// <summary>
        /// Log 파일에 메세지 기록
        /// </summary>
        /// <param name="szMsg"></param>
        public  void WriteLine(DataLogVo dataLogVo)
        {
           
            StringBuilder sb = new StringBuilder();
             
            sb.Append("|" + dataLogVo.accno);    //계좌번호
            sb.Append("|" + dataLogVo.Isuno);    //종목코드
            sb.Append("|" + dataLogVo.Isunm);    //종목명
            sb.Append("|" + dataLogVo.ordptncode);    //주문구분 01:매도|02:매수
            sb.Append("|" + dataLogVo.ordno);    //주문번호
            sb.Append("|" + dataLogVo.ordqty);   //주문수량
            sb.Append("|" + dataLogVo.ordprc);   //주문가격
            sb.Append("|" + dataLogVo.execqty);  //체결수량
            sb.Append("|" + dataLogVo.execprc);  //체결가격
 
            //파일에기록
            this.GetStreamWriter().WriteLine(DateTime.Now.ToString("yyyy-MM-dd|HH:mm:ss") + sb.ToString());
            //메모리에기록
            this.insertMerge(dataLogVo);
        }   // end function


        ////해당 종목 매수 이력을 삭제함
        //public void delete(DataLogVo dataLogVo)
        //{
        //}

        ////평균단가를 계산해서 리턴함
        //public void read(DataLogVo dataLogVo)
        //{
        //}

        /// <summary>
        /// 매수체결 이벤트가 발생하면 주문번호별로 저장한다.
        /// 여기서는 충실하게 주문번호별 체결정보만을 등록한다. 
        /// 일괄매도된 종목 삭제및 다른기능은 다른곳에서 해준다.
        /// </summary>
        /// <param name="szMsg"></param>
        private void insertMerge(DataLogVo dataLogVo)
        {
           
            int i = dataLogVoList.Find("ordno", dataLogVo.ordno);
            //dataLogVoList에 주문번호정보여부에따라 매수정보 수정및 추가를 해준다. 
            if (i >= 0)
            {
                DataLogVo tmpDataLogVo = dataLogVoList.ElementAt(i);
                //매수 체결수량 = 매수 체결수량 + 매수 체결수량
                tmpDataLogVo.execqty = (int.Parse(tmpDataLogVo.execqty) + int.Parse(dataLogVo.execqty)).ToString();

                //1.매도면 해당종목의 모든 매수한 수량과 매도한 수량을 비교해서 모든 수량이 매도가 되었다면 해당종목로그를 삭제해준다. 
                //2.또는 팔린종목 그리드에서 삭제할때 데이타로그도 같이 삭제해주는 방법도 생각해보자.
            } else {
                dataLogVoList.Add(dataLogVo);
            }

        }
        //dataLogVoList  data를 파일로 저장
        public void dataLogVoListToLogFile()
        {
            //dataLogVoList;
        }

    }   // end class

    public class DataLogVo
    {
        public String datetime{ set; get; }//일자
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
