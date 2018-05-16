﻿
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
    public class Xing_t1857Exclude : XAQueryClass{

        private Boolean completeAt = true;//완료여부.
        public MainForm mainForm;

        private EBindingList<T1857Vo> t1857ExcludeVoList        = new EBindingList<T1857Vo>();
        private EBindingList<T1857Vo> t1857excludeVoBasicList   = new EBindingList<T1857Vo>();
        public EBindingList<T1857Vo>  getT1857ExcludeVoList()
        {
            return this.t1857ExcludeVoList;
        }
        public EBindingList<T1857Vo> getT1857excludeVoBasicList()
        {
            return this.t1857excludeVoBasicList;
        }
        private int conditionTotalCnt = 2;
        private int conditionCallIndex = 0;
        private String[] conditionNm = { "매수금지", "손절종목"};

        public Boolean initAt = false;

        // 생성자
        public Xing_t1857Exclude(){
            base.ResFileName = "₩res₩t1857.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);
        }   // end function

        


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){

            int blockCount = base.GetBlockCount("t1857OutBlock1");

            
            EBindingList<T1857Vo> tmpList =  new EBindingList<T1857Vo>();

            //String sunikrt;//수익률
            for (int i = 0; i < blockCount; i++) {
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
            if (conditionCallIndex == 0){
                this.t1857ExcludeVoList.Clear();
                this.t1857ExcludeVoList = tmpList;
                mainForm.exCnt.Text = blockCount.ToString();
            }else if (conditionCallIndex == 1){
                this.t1857excludeVoBasicList.Clear();
                this.t1857excludeVoBasicList = tmpList;
            }

            mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]t1857_Exclude: 매수금지종목 조회 완료.";

            //Exclude Condition Index Update 
            this.conditionCallIndex = this.conditionCallIndex + 1;
            this.conditionCallIndex = this.conditionCallIndex < this.conditionTotalCnt ? this.conditionCallIndex : 0;

            completeAt = true;//중복호출 여부
        }

        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){
          
            if (nMessageCode == "00000") {//정상동작일때는 메세지이벤트헨들러가 아예 호출이 안되는것같다
                ;
            } else { 
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]t1857_Exclude:" + nMessageCode + ":" + szMessage;
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
                String conditionName = startupPath + "\\Resources\\Exclude" + conditionCallIndex + ".ACF";
                base.SetFieldData("t1857InBlock", "sRealFlag", 0, "0");           // 실시간구분 : 0:조회, 1:실시간
                base.SetFieldData("t1857InBlock", "sSearchFlag", 0, "F");         // 종목검색구분 : F:파일, S:서버
                base.SetFieldData("t1857InBlock", "query_index", 0, conditionName); 
                                                                                            
                int nSuccess = base.RequestService("t1857", "");

                if (nSuccess < 0)
                {
                    if (nSuccess == 23)
                    {
                        MessageBox.Show("TR정보를 찾을수 없습니다.");
                    }
                    mainForm.input_t1833_log1.Text = "[" + mainForm.label_time.Text + "][" + this.conditionNm[conditionCallIndex] + "]e매수 금지 전송 에러.";
                }
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "]e매수 금지 검색 요청.";
               

            }
            else {
                //mainForm.input_t1857_log2.Text = "[중복]EXCLUDE 조건검색 요청.";
                mainForm.input_t1833_log2.Text = "[" + mainForm.label_time.Text + "][" + conditionNm[conditionCallIndex] + "][중복]e매수금지.";

                //mainForm.input_t1857_log2.Text = "대기";
                callCnt++;
                if (callCnt == 5)
                {
                    this.completeAt = true;
                    callCnt = 0;
                }
            }
        }

      
    } //end class 

    public class T1857Vo
    {
        public String shcode { set; get; } //종목코드
        public String hname { set; get; } //종목명
        public String close { set; get; } //현재가
        public String sign { set; get; } //전일대비구분 
        public String change { set; get; } //전일대비
        public String diff { set; get; } //등락율
        public String volume { set; get; } //거래량
        public Boolean deleteAt { set; get; } //삭제여부
        public String searchMod { set; get; } //검색조건명
    }
}   // end namespace
