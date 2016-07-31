using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices.ComTypes;

using XA_DATASETLib;
using XA_SESSIONLib;


namespace WindowsFormsApplication2
{
    public partial class mForm : Form
    {

        public mForm() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //접속가능
            //실운영 : hts.ebestsec.co.kr
            //모의투자 : demo.ebestsec.co.kr
           

        }
        //로그인 버튼 클릭 이벤트
        private void loginBtn_Click(object sender, EventArgs e)
        {
            try
            {
                XASession ixaSession = new XASession();
                // 서버에 접속
                if (ixaSession.IsConnected() == false)
                {
                    ixaSession.ConnectServer("demo.ebestsec.co.kr", 20001);
                }

                // 로그인
               Boolean test =  ixaSession.Login("neloi", "neloi1", "", 0, false);
                MessageBox.Show(test.ToString());


                String mAccount = ixaSession.GetAccountList(0);
                loginIdInput.Text =  mAccount;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //로그아웃 버튼 클릭 이벤트
        private void logOutBtn_Click(object sender, EventArgs e)
        {
            //if (ixaSession != null)  {
            //    ixaSession.Logout();
            //    ixaSession.DisconnectServer();
           // }
        }

        
    }
}

