using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PackageSellSystemTrading
{
    public partial class AccountForm : Form
    {
        public Xing_CSPAQ12300 xing_CSPAQ12300;//계좌정보 --계좌 비밀번호설정용으로 사용

        public ExXASessionClass exXASessionClass;

        public MainForm mainForm;

        public String account { get; set; }
        public String accountPw { get; set; }

        public AccountForm()
        {
            InitializeComponent();

            //this.exXASessionClass = exXASessionClass;
        }

        private void AccountForm_Load(object sender, EventArgs e){
            String account;
            this.listBox_account.Items.Clear();
            this.input_accountPw.Text = "";
            int accountListCount = this.exXASessionClass.GetAccountListCount();
            for (int i = 0; i < accountListCount; i++)
            {
                account = this.exXASessionClass.GetAccountList(i);
                this.listBox_account.Items.Add(account);
            }
            this.listBox_account.SelectedIndex = 0;


            this.xing_CSPAQ12300 = new Xing_CSPAQ12300();// 현물계좌 잔고내역 조회
            this.xing_CSPAQ12300.accountForm = this;
            this.xing_CSPAQ12300.mainForm = this.mainForm;

            //주식잔고2
            //mainForm.xing_t0424.call_request();
        }


        //확인
        private void btn_account_check_Click(object sender, EventArgs e){
            accountCheck();
        }

        //비밀번호 에서 엔터 이벤트.
        private void input_accountPw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter){
                accountCheck();
            }
        }


        private void accountCheck()
        {
            String listBox_account = this.listBox_account.Text;
            String input_accountPw = this.input_accountPw.Text;

            if (listBox_account == "" || input_accountPw == ""){
                MessageBox.Show("계좌번호 또는 비밀번호를 입력해주세요.");
            }else{
                this.xing_CSPAQ12300.call_request(listBox_account, input_accountPw);
            }
        }
    }
}
