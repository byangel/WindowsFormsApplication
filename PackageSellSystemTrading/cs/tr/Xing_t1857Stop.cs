
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Data;
using System.Drawing;
using System.Threading;
namespace PackageSellSystemTrading{
    public class Xing_t1857Stop : XAQueryClass{

        public MainForm mainForm;


        private EBindingList<T1857Vo> t1857StopList = new EBindingList<T1857Vo>();
        
        public EBindingList<T1857Vo> getT1857StopList(){
            return this.t1857StopList;
        }

        private DataTable tmpDt;

        private DataTable t1857StopDt;
        public DataTable getT1857StopDt()
        {
            return this.t1857StopDt;
        }

        

        // 생성자
        public Xing_t1857Stop(){
            base.ResFileName = "₩res₩t1857.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            this.tmpDt = new DataTable();
            tmpDt.Columns.Add("종목코드", typeof(string));
            tmpDt.Columns.Add("종목명", typeof(string));
            tmpDt.Columns.Add("현재가", typeof(string));
            tmpDt.Columns.Add("전일대비구분", typeof(string));
            tmpDt.Columns.Add("전일대비", typeof(string));
            tmpDt.Columns.Add("등락율", typeof(string));
            tmpDt.Columns.Add("거래량", typeof(double));
            tmpDt.Columns.Add("연속봉수", typeof(string));
            tmpDt.Columns.Add("검색조건", typeof(string));
            tmpDt.Columns.Add("삭제여부", typeof(string));
            tmpDt.Columns.Add("설명", typeof(string));

        }   // end function

        


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            int blockCount = base.GetBlockCount("t1857OutBlock1");

            
            EBindingList<T1857Vo> tmpList =  new EBindingList<T1857Vo>();

            //String sunikrt;//수익률
            tmpDt.Clear();
            for (int i = 0; i < blockCount; i++) {
                DataRow tmpRow = tmpDt.NewRow();
               
                tmpRow["종목코드"     ] = base.GetFieldData("t1857OutBlock1", "shcode", i); //종목코드
                tmpRow["종목명"       ] = base.GetFieldData("t1857OutBlock1", "hname", i); //종목명
                tmpRow["현재가"       ] = base.GetFieldData("t1857OutBlock1", "price", i); //현재가
                tmpRow["전일대비구분" ] = base.GetFieldData("t1857OutBlock1", "sign", i); //전일대비구분 
                tmpRow["전일대비"     ] = base.GetFieldData("t1857OutBlock1", "change", i); //전일대비
                tmpRow["등락율"       ] = base.GetFieldData("t1857OutBlock1", "diff", i); //등락율
                tmpRow["거래량"       ] = base.GetFieldData("t1857OutBlock1", "volume", i); //거래량
                tmpRow["연속봉수"     ] = base.GetFieldData("t1857OutBlock1", "jobFlag", i); //연속봉수
                tmpDt.Rows.Add(tmpRow);


                T1857Vo t1857Vo = new T1857Vo();
                t1857Vo.shcode = base.GetFieldData("t1857OutBlock1", "shcode", i); //종목코드
                t1857Vo.hname  = base.GetFieldData("t1857OutBlock1", "hname" , i); //종목명
                t1857Vo.close  = base.GetFieldData("t1857OutBlock1", "close" , i); //현재가
                t1857Vo.sign   = base.GetFieldData("t1857OutBlock1", "sign"  , i); //전일대비구분 
                t1857Vo.change = base.GetFieldData("t1857OutBlock1", "change", i); //전일대비
                t1857Vo.diff   = base.GetFieldData("t1857OutBlock1", "diff"  , i); //등락율
                t1857Vo.volume = base.GetFieldData("t1857OutBlock1", "volume", i); //거래량

                tmpList.Add(t1857Vo);
            }
            //혹시몰라서 1857에서 금지종목을 참조할때 로스가 생길거같아서 이런방식을 써보았다.
            //매수금지종목 검색 그리드 초기화
            
            //복사
            this.t1857StopDt = tmpDt.Clone();
            mainForm.exCnt1.Text = blockCount.ToString();

            this.t1857StopList.Clear();
            this.t1857StopList = tmpList;
         

            mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "]손절종목]t1857_Exclude: 매수금지종목 조회 완료.";

          
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][손절종목]t1857_Exclude:" + nMessageCode + ":" + szMessage;
            }
            
        }


        /// <summary>
		/// 손절종목 검색 호출
		/// </summary>
		public void call_request(){
      
                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");            
                String conditionName = startupPath + "\\Resources\\Exclude1.ACF";
                //String conditionName = startupPath + "\\Resources\\Exclude1.ACF";
                base.SetFieldData("t1857InBlock", "sRealFlag"  , 0, "0");           // 실시간구분 : 0:조회, 1:실시간
                base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");         // 종목검색구분 : F:파일, S:서버
                base.SetFieldData("t1857InBlock", "query_index", 0, conditionName); 
                                                                                            
                int nSuccess = base.RequestService("t1857", "");

                if (nSuccess < 0)
                {
                    if (nSuccess == -23)
                    {
                        MessageBox.Show("TR정보를 찾을수 없습니다.");
                    }
                    mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][손절종목]e매수 금지 전송 에러.";
                }
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][손절종목]e매수 금지 검색 요청.";
               
        }

      
    } //end class 

   
}   // end namespace
