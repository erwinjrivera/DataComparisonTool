using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataComparisonTool.Model
{
    public class Map : IList<FieldMapping>, INotifyPropertyChanged
    {
        private List<FieldMapping> _maps = new List<FieldMapping>();

        private bool _isDirty;

        public bool IsDirty 
        { 
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;

                OnPropertyChanged("IsDirty");
            }
        }

        public FieldMapping this[int index] 
        { 
            get { return _maps[index]; }
            set
            {
                _maps[index] = value;

                OnPropertyChanged("FieldMapping");
            }
        }

        public int Count
        {
            get { return _maps.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(FieldMapping item)
        {
            item.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Model_PropertyChanged);

            _maps.Add(item);

            OnPropertyChanged("Add");
        }

        public void Clear()
        {
            _maps.Clear();

            OnPropertyChanged("Clear");
        }

        public bool Contains(FieldMapping item)
        {
            return _maps.Contains(item);
        }

        public void CopyTo(FieldMapping[] array, int arrayIndex)
        {
            _maps.CopyTo(array, arrayIndex);
        }

        public IEnumerator<FieldMapping> GetEnumerator()
        {
            return _maps.GetEnumerator();
        }

        public int IndexOf(FieldMapping item)
        {
            return _maps.IndexOf(item);
        }

        public void Insert(int index, FieldMapping item)
        {
            _maps.Insert(index, item);

            OnPropertyChanged("Insert");
        }

        public bool Remove(FieldMapping item)
        {
            IsDirty = true;
            OnPropertyChanged("Remove");

            return _maps.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _maps.RemoveAt(index);

            OnPropertyChanged("RemoveAt");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _maps.GetEnumerator();
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

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SourceField":
                    //
                    break;
                case "TargetField":
                    IsDirty = true;
                    break;
            }
        }

    }
}
