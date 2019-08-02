using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testValamis.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using testValamis.Elements;
using System.Threading;

namespace testValamis.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string author;
        private string feedback;
        public static Thread potok1;
        private string rating;
        private string myUrl = "https://www.delivery-club.ru/";
        readonly MainModel mainModel = new MainModel();

        public MainVM()
        {
            //mainModel.PropertyChanged += (s, e) => { RaisePropertyChanged(e.PropertyName); };
            potok1 = new Thread(new ParameterizedThreadStart(mainModel.StartTest(myUrl)));
            StartTest = new DelegateCommand<string>(str => { potok1.Start(); });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DelegateCommand<string> StartTest { get; }

        public string UrlSite
        {
            get { return myUrl; }
        }

        public string Author
        {
            get { return author; }
            set { if(author==value) return; author = OutputDataViewModel.Author; OnPropertyChanged("Author"); }
        }

        public string Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        public string Feedback
        {
            get { return feedback; }
            set { feedback = value; }
        }

        public void setAuthor(string value)
        {
            author = value;
        }
    }
}
