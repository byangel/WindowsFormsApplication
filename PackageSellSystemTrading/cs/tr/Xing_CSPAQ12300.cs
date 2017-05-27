
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Threading;

namespace PackageSellSystemTrading{
    //현물계좌 잔고내역 조회(API) - 계좌 비밀번호 체크에 사용한다.
    public class Xing_CSPAQ12300 : XAQueryClass{

        public Boolean completeAt = true;//완료여부.
        private String  account;
        private String  accountPw;
        
        public AccountForm accountForm;
        public MainForm    mainForm;

        
        // 생성자
        public Xing_CSPAQ12300()
        {
            base.ResFileName = "₩res₩CSPAQ12300.res";

            base.ReceiveData    += new _IXAQueryEvents_ReceiveDataEventHandler(receiveDataEventHandler);
            base.ReceiveMessage += new _IXAQueryEvents_ReceiveMessageEventHandler(receiveMessageEventHandler);

            completeAt = true;
        }   // end function

        // 소멸자
        ~Xing_CSPAQ12300()
        {
          
        }


        /// <summary>
		/// 데이터 응답 처리
		/// </summary>
		/// <param name="szTrCode">조회코드</param>
		void receiveDataEventHandler(string szTrCode){
            completeAt = true;
        }

        //이벤트 메세지.
        void receiveMessageEventHandler(bool bIsSystemError, string nMessageCode, string szMessage){

        
                //특정 오류없이 정상적으로 실행이 되었다면 화면 및 초기화를 해준다.
                if (nMessageCode == "00136" || nMessageCode == "00133") {
                   
                    //계좌정보가 정상 확인 되었으면 다른 프로그램에서 계좌번호와 비밀번호를 쓸수 있도록 메인폼 멤버변수에 저장한다.
                    mainForm.account = this.account;
                    mainForm.accountPw = this.accountPw;
                    //////////////////////////////////////////////
                    // 자동로그인 타이머 멈춤
                    mainForm.timerLogin.Stop();

                    // 장 운영 정보 실시간 등록
                    mainForm.real_jif.call_advise();

                    // HTS -> API 연동 등록
                    //mainForm.real_jif.call_advise_link_from_hts();

                    //실시간 체결정보 - 재접속했을때 안해주면 채결되도 그리드에서 종목을 제거해주지 못한다.
                    mainForm.real_SC1.AdviseRealData();

                    // 뉴스 정보 실시간 등록
                    //mainForm.mxRealNws.call_advise();

                    // 서버의 시간 검색 타이머 스타트 - 여기서 PC의 시간을 서버 시간과 동기화 시킴
                    //mainForm.Timer0167.Start();
                    //mainForm.Timer0167.Start();//tradingRun에 포함시킴

                    //////////////////////////////////////////
                    //접속이 귾겼다가 접속했을때...문제가 있어서 추가해준다. 잔고목록을 클리어 해준다.
                    //mainForm.xing_t0424.getT0424VoList().Clear();

                    //필요없는 데이타 삭제.
                    mainForm.dataLog.initDelete();

                    //로그인 완료시(계좌선택후) 미리 호출할 필료가 있는것들
                    //매수금지종목 조회 --데이타보장을 위해 타이머를 시작하지만 최초 매수금지목록을 확보후 타이머를 시작한다.
                    mainForm.xing_t1833Exclude.call_request();//매수금지 데이타
                    mainForm.xing_t0424.call_request(this.account, this.accountPw);//잔고 데이타
                    mainForm.xing_t0425.call_request(this.account, this.accountPw);//매매이력 데이타
                    mainForm.xing_CSPAQ12200.call_request(this.account, this.accountPw);//계좌정보
                    mainForm.xing_t0167.call_request();//시간데이타

                    
                    //타이머 시작 --여기서 타이머 시작해주면 타이머 스톱해줄일은 없어진다.그리고  잔고정보,잔고목록,매매이력 등등을 호출안해줘도 된다.
                    //mainForm.timer_t1833Exclude.Start();//진입검색 타이머
                    //mainForm.timer_common.Start();//계좌 및 미체결 검색 타이머
                    //mainForm.timer_test.Start();
                    
                    //실시간 체결정보 등록
                    mainForm.real_SC1.AdviseRealData();

                    //실시간 체결정보 해제
                    //real_SC1.UnadviseRealData();

                    //설정저장 버튼 활성화.
                    mainForm.btn_config_save.Enabled = true;
                    // 로그인 버튼 비활성
                    mainForm.btn_login.Enabled = false;

                    //계좌번호와 페스워드가 인증되었으면 계좌번호선택폼을 닫아준다.
                    accountForm.Close();
                   
                    //트레이딩 시작
                    mainForm.tradingRun();

                    Log.WriteLine("CSPAQ12300::" + nMessageCode + " :: " + szMessage);

                }else{
                    //MessageBox.Show("CSPAQ12300 :: " + nMessageCode + " :: " + szMessage);
                    Log.WriteLine("CSPAQ12300:: 예외 발생:" + nMessageCode + " :: " + szMessage);
                }
        
            completeAt = true;

        }


        //String isOpen = "Y";
        //public void call_request(String account, String accountPw,String isOpen)
        //{
        //    this.isOpen = isOpen;
        //    call_request(account, accountPw);
        //}
        /// <summary>
        /// 종목검색 호출
        /// </summary>
        public void call_request( String account, String accountPw)
        {
            //isOpen = "Y";
            this.account   = account;
            this.accountPw = accountPw;
            
            if (completeAt) {
                completeAt = false;//중복호출 방지
                //String account = mainForm.comBox_account.Text; //메인폼 계좌번호 참조
                //String accountPw = mainForm.input_accountPw.Text; //메인폼 비빌번호 참조

                base.SetFieldData("CSPAQ12300InBlock1", "AcntNo"         , 0, account);  // 계좌번호
                base.SetFieldData("CSPAQ12300InBlock1", "Pwd"            , 0, accountPw);// 비밀번호
                base.SetFieldData("CSPAQ12300InBlock1", "RecCnt"         , 0, "1");// 레코드수
                base.SetFieldData("CSPAQ12300InBlock1", "BalCreTp"       , 0, "1");      // 잔고생성구분      - 0:전체 | 1:현물 | 9:선물대용
                base.SetFieldData("CSPAQ12300InBlock1", "CmsnAppTpCode"  , 0, "0");      // 수수료적용구분    - 0:평가시수수료미적용 | 1:평가시수수료적용 ?값을변경해도 무슨차이인지 모르겠음.
                base.SetFieldData("CSPAQ12300InBlock1", "D2balBaseQryTp" , 0, "0");      //D2잔고기준조회구분 - 0:전부조회 | 1:D2잔고0이상만조회
                base.SetFieldData("CSPAQ12300InBlock1", "UprcTpCode"     , 0, "1");      //단가구분          - 0:평균단가 | 1:BEP단가

                base.Request(false);  //연속조회일경우 true
                

            } else {
                //mainForm.input_t0424_log.Text = "[중복]잔고조회를 요청을 하였습니다.";
                //MessageBox.Show("CSPAQ12300 :: 중복 조회 잠시후 시도해주세요.");
                Log.WriteLine("CSPAQ12300 :: 중복 조회 잠시후 시도해주세요.");
            }
            

        }	// end function


    } //end class 
   
}   // end namespace
