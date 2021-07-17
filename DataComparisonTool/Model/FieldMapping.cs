using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class FieldMapping : INotifyPropertyChanged
    {
        private Guid _id;

        private Field _source;

        private Field _target;

        public FieldMapping()
        {
            _id = Guid.NewGuid();
        }

        [Browsable(false)]
        public Guid Id
        {
            get
            {
                return _id;
            }
        }

        [Browsable(false)]
        public string Description { get; set; }

        [DisplayName("Mapped")]
        public bool Mapped
        {
            get
            {
                return TargetField != null;
            }

        }


        [DisplayName("Source Column")]
        public string Source
        {
            get
            {
                return SourceField.ToString();
            }
        }

        [DisplayName("Target Column")]
        public string Target
        {
            get
            {
                return TargetField != null ? TargetField.ToString() : string.Empty;
            }
        }

        [Browsable(false)]
        public Field SourceField
        {
            get { return _source; }
            set
            {
                _source = value;

                OnPropertyChanged("SourceField");
            }
        }

        [Browsable(false)]
        public Field TargetField
        {
            get { return _target; }
            set
            {
                _target = value;

                OnPropertyChanged("TargetField");
            }
        }

        public int SourcePosition
        {
            get
            {
                return _source.Position;
            }
        }

        public int TargetPosition
        {
            get
            {
                if (_target != null)
                    return _target.Position;
                else
                    return 0;
            }
        }

        bool _key;

        [Browsable(true)]
        public bool IsKey
        {
            get { return _key; }
            set
            {
                _key = value;

                OnPropertyChanged("IsKey");
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
