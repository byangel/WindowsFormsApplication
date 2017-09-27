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
            //Search("000030");
        }
        //매매 이력 조회
        private void btn_search_Click(object sender, EventArgs e)
        {
            //mainForm.tradingHistory.dbSync();
            this.Search(input_searchString.Text);
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

        public void Search(String searchString)
        {
            var items = from item in mainForm.tradingHistory.getTradingHistoryDt().AsEnumerable()
                        where item["accno"].ToString() == mainForm.account
                           && (item["Isuno"].ToString() == searchString.Replace("A", "") || item["Isunm"].ToString() == searchString)
                        //|| SqlMethods.Like(item["Isunm"].ToString() , "%"+ searchString + "%") )

                        select item;
            if(items.Count() > 0)
            {
                this.grd_history.DataSource = items.CopyToDataTable();
            }else
            {
                this.grd_history.DataSource = null; //검색카운트가 없을때...
            }

            //조회조건이 없을때
            if(searchString == "")
            {
                this.grd_history.DataSource = mainForm.tradingHistory.getTradingHistoryDt();
            }
           
        }
    }//class end
}//name end
