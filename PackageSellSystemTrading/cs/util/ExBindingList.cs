using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PackageSellSystemTrading
{
    public class ExBindingList : ExBindingList
    {
        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override int FindCore(PropertyDescriptor prop, object key)
        {

        }
    }
}	// end namespace
