﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.Threading;
using System.Data;

namespace PackageSellSystemTrading{
 //현물 정상주문
    public class CSPAT00600Mng : XAQueryClass{

       
        public MainForm mainForm;
        private EBindingList<Xing_CSPAT00600> xing_CSPAT00600List;
        private EBindingList<Xing_CSPAT00800> xing_CSPAT00800List;
        private EBindingList<Xing_CSPAT00700> xing_CSPAT00700List;

        // 생성자
        public CSPAT00600Mng(MainForm mainForm) {
            this.mainForm = mainForm;
            this.xing_CSPAT00600List = new EBindingList<Xing_CSPAT00600>();
            this.xing_CSPAT00800List = new EBindingList<Xing_CSPAT00800>();
            this.xing_CSPAT00700List = new EBindingList<Xing_CSPAT00700>();
            for (int i=0;i<10;i++)
            {
                this.xing_CSPAT00600List.Add(new Xing_CSPAT00600(mainForm));
                this.xing_CSPAT00800List.Add(new Xing_CSPAT00800(mainForm));
                this.xing_CSPAT00700List.Add(new Xing_CSPAT00700(mainForm));
            }
        }// end function


        public Xing_CSPAT00600 get600()
        {
            Log.WriteLine("Xing_CSPAT00600 카운트::" + this.xing_CSPAT00600List.Count());
            for (int i=0;i< this.xing_CSPAT00600List.Count();i++)
            {
                if(this.xing_CSPAT00600List.ElementAt(i).completeAt)
                {
                    this.xing_CSPAT00600List.ElementAt(i).completeAt = false;
                    return this.xing_CSPAT00600List.ElementAt(i);
                }
            }
            Xing_CSPAT00600 item600 = new Xing_CSPAT00600(mainForm);
            item600.completeAt = false;
            this.xing_CSPAT00600List.Add(item600);
            return item600;
        }
        public Xing_CSPAT00800 get800()
        {
            Log.WriteLine("Xing_CSPAT00800 카운트::" + this.xing_CSPAT00800List.Count());
            for (int i = 0; i < this.xing_CSPAT00800List.Count(); i++)
            {
                if (this.xing_CSPAT00800List.ElementAt(i).completeAt)
                {
                    this.xing_CSPAT00800List.ElementAt(i).completeAt = false;
                    return this.xing_CSPAT00800List.ElementAt(i);
                }
            }
            Xing_CSPAT00800 item800 = new Xing_CSPAT00800(mainForm);
            item800.completeAt = false;
            this.xing_CSPAT00800List.Add(item800);
            return item800;
        }

        public Xing_CSPAT00700 get700()
        {
            Log.WriteLine("Xing_CSPAT00700 카운트::" + this.xing_CSPAT00700List.Count());
            for (int i = 0; i < this.xing_CSPAT00700List.Count(); i++)
            {
                if (this.xing_CSPAT00700List.ElementAt(i).completeAt)
                {
                    this.xing_CSPAT00700List.ElementAt(i).completeAt = false;
                    return this.xing_CSPAT00700List.ElementAt(i);
                }
            }
            Xing_CSPAT00700 item700 = new Xing_CSPAT00700(mainForm);
            item700.completeAt = false;
            this.xing_CSPAT00700List.Add(item700);
            return item700;
        }


    } //end class 
   
}// end namespace
