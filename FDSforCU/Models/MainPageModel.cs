using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDSforCU.Models
{
    public class MainPageModel : ModelBase
    {
        private Type _PageType;
        private string _Title;

        public Type PageType
        {
            get => _PageType;
            set
            {
                _PageType = value;
                NotifyPropertyChanged();
            }
        }
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                NotifyPropertyChanged();
            }
        }
    }
}
