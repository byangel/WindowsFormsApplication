
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
    public class Xing_t1833Exclude : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        private EBindingList<T1833Vo> t1833ExcludeVoList        = new EBindingList<T1833Vo>();
        private EBindingList<T1833Vo> t1833excludeVoBasicList   = new EBindingList<T1833Vo>();
        public EBindingList<T1833Vo>  getT1833ExcludeVoList()
        {
            return this.t1833ExcludeVoList;
        }
        public EBindingList<T1833Vo> getT1833excludeVoBasicList()
        {
            return this.t1833excludeVoBasicList;
        }
        private int conditionTotalCnt = 2;
        private int conditionCallIndex = 0;
        private String[] conditionNm = { "매수금지", "손절종목"};

        public Boolean initAt = false;

        // 생성자
        public Xing_t1833Exclude(){
            base.ResFileName = "₩res₩t1833.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        // 소멸자
        ~Xing_t1833Exclude(){
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            int blockCount = base.GetBlockCount("t1833OutBlock1");

            
            EBindingList<T1833Vo> tmpList =  new EBindingList<T1833Vo>();

            //String sunikrt;//수익률
            for (int i = 0; i < blockCount; i++) {
                T1833Vo t1833Vo = new T1833Vo();
                t1833Vo.shcode = base.GetFieldData("t1833OutBlock1", "shcode", i); //종목코드
                t1833Vo.hname  = base.GetFieldData("t1833OutBlock1", "hname" , i); //종목명
                t1833Vo.close  = base.GetFieldData("t1833OutBlock1", "close" , i); //현재가
                t1833Vo.sign   = base.GetFieldData("t1833OutBlock1", "sign"  , i); //전일대비구분 
                t1833Vo.change = base.GetFieldData("t1833OutBlock1", "change", i); //전일대비
                t1833Vo.diff   = base.GetFieldData("t1833OutBlock1", "diff"  , i); //등락율
                t1833Vo.volume = base.GetFieldData("t1833OutBlock1", "volume", i); //거래량

                tmpList.Add(t1833Vo);
            }
            //혹시몰라서 1833에서 금지종목을 참조할때 로스가 생길거같아서 이런방식을 써보았다.
            //매수금지종목 검색 그리드 초기화
            if (conditionCallIndex == 0){
                this.t1833ExcludeVoList.Clear();
                this.t1833ExcludeVoList = tmpList;
                mainForm.exCnt.Text = blockCount.ToString();
            }else if (conditionCallIndex == 1){
                this.t1833excludeVoBasicList.Clear();
                this.t1833excludeVoBasicList = tmpList;
            }

            mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]t1833_Exclude: 매수금지종목 조회 완료.";

            //Exclude Condition Index Update 
            this.conditionCallIndex = this.conditionCallIndex + 1;
            this.conditionCallIndex = this.conditionCallIndex < this.conditionTotalCnt ? this.conditionCallIndex : 0;

            completeAt = true;//중복호출 여부
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]t1833_Exclude:" + nMessageCode + ":" + szMessage;
            }
            completeAt = true;//중복호출 방지
           
        }

        private int callCnt = 0;
        /// <summary>
		/// 종목검색 호출
		/// </summary>
		public void call_request(){
           
            if (completeAt) {
                //폼 메세지.
                completeAt = false;//중복호출 방지

                String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                base.RequestService("t1833", startupPath + "\\Resources\\Exclude" + conditionCallIndex + ".ADF");
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]조건검색 요청.";

                //String startupPath = Application.StartupPath.Replace("\\bin\\Debug", "");
                //base.RequestService("t1833", startupPath + "\\Resources\\"+mainForm.optionForm.EXCLUDE_NM);

                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "]exclude 조건검색 요청.";
            } else {
                //mainForm.input_t1833_log2.Text = "[중복]EXCLUDE 조건검색 요청.";
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "][중복]조건검색 요청.";

                //mainForm.input_t1833_log2.Text = "대기";
                callCnt++;
                if (callCnt == 5)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }
        }

      
    } //end class 


}   // end namespace
