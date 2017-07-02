using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PackageSellSystemTrading
{
    public partial class HistoryForm : Form
    {
        public MainForm mainForm;

       

        //생성자
        public HistoryForm()
        {
            InitializeComponent();

            
        }

        
       

        private void btn_test_Click(object sender, EventArgs e)
        {
            mainForm.dataLog.dbSync();
        }
        //매매 이력 조회
        private void btn_search_Click(object sender, EventArgs e)
        {
            mainForm.dataLog.dbSync();
        }
    }//class end
}//name end
