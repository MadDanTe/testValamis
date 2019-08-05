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
        private string feedback;
        public static Thread potok1;
        private int rating;
        private string author;
        private string date;
        private string status;
        private string myUrl = "https://www.delivery-club.ru/";
        readonly MainModel mainModel = new MainModel();

        public MainVM()
        {
            potok1 = new Thread(new ParameterizedThreadStart(x=>{ mainModel.StartTest(myUrl, this); }));
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
            set { author=value; OnPropertyChanged("Author"); }
        }

        public int Rating
        {
            get { return rating; }
            set { rating = value; OnPropertyChanged("Rating"); }
        }

        public string Feedback
        {
            get { return feedback; }
            set { feedback = value; OnPropertyChanged("Feedback"); }
        }

        public string Date
        {
            get { return date; }
            set { date = value; OnPropertyChanged("Date"); }
        }

        public string Status
        {
            get { return status; }
            set { status+="\n"+value; OnPropertyChanged("Status"); }
        }

    }
}
