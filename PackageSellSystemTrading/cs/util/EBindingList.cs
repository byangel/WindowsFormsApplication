using System.ComponentModel;
using System.Reflection;

namespace PackageSellSystemTrading
{  
    public class EBindingList<T> : BindingList<T> { 

        protected override bool SupportsSearchingCore
        {
            get
            {
                return true;
            }
        }


        protected override int FindCore(PropertyDescriptor prop, object key)
        {
         // Get the property info for the specified property.
            PropertyInfo propInfo = typeof(T).GetProperty(prop.Name);
            T item;

            if (key != null)
            {
             // Loop through the items to see if the key
             // value matches the property value.
                for (int i = 0; i < Count; ++i)
                {
                    item = (T)Items[i];
                    if (propInfo.GetValue(item, null).Equals(key))
                        return i;
                }
            }
            return -1;
        }

   
        public int Find(string property, object key)
        {
         // Check the properties for a property with the specified name.
            PropertyDescriptorCollection properties =  TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);

         // If there is not a match, return -1 otherwise pass search to
         // FindCore method.
            if (prop == null)
                return -1;
            else
                return FindCore(prop, key);
        }

    }
}	// end namespace
