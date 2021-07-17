using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class FieldMap : INotifyPropertyChanged
    {
        private Column _source;

        private Column _target;

        [Browsable(false)]
        public string Id { get; set; }

        public Column Source 
        {
            get { return _source; }
            set
            {
                _source = value;

                OnPropertyChanged("Source");
            }
        }

        public Column Target 
        {
            get { return _target; }
            set
            {
                _target = value;

                OnPropertyChanged("Target");
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
