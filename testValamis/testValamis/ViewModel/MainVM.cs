using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testValamis.Model;

namespace testValamis.ViewModel
{
    class MainVM : BindableBase
    {

        private string myUrl = "https://www.delivery-club.ru/";
        readonly MainModel mainModel = new MainModel();
        public MainVM()
        {
            mainModel.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            StartTest = new DelegateCommand<string>(str => { mainModel.StartTest(myUrl); });
        }
        public DelegateCommand<string> StartTest { get; }
        public string UrlSite
        {
            get { return myUrl; }
        }
    }
}
