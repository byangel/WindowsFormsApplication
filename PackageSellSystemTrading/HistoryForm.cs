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
            //mainForm.tradingHistory.dbSync();
        }
        //매매 이력 조회
        private void btn_search_Click(object sender, EventArgs e)
        {
            //mainForm.tradingHistory.dbSync();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < grd_history.RowCount; i++)
            {

                if (this.grd_history.Rows[i].Cells["check_flag"].FormattedValue.ToString() == "True")
                {
                    String ordno = this.grd_history.Rows[i].Cells["ordno"].Value.ToString();
                    int delCnt = mainForm.tradingHistory.ordnoByDelete(ordno);

                    this.grd_history.DataSource = mainForm.tradingHistory.getTradingHistoryDt();
                    MessageBox.Show(delCnt.ToString() + "건을 삭제 하였습니다.");
                    
                }

            }
        }
    }//class end
}//name end
