using SQLite;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Unutmaz.Models
{
    public class KisiselAlisverisArsivModel : INotifyPropertyChanged
    {
        private bool _isExpanded;

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string ListeBasligi { get; set; }
        public string ListeOgesiJson { get; set; }  // ObservableCollection<string> JSON'a çevrilip saklanacak
        public string Mod { get; set; }  // "Serbest" veya "Standart"
        public DateTime OlusturulmaTarihi { get; set; }  // Orijinal oluşturulma tarihi
        public DateTime ArsivelenmeTarihi { get; set; }  // Arşivlenme tarihi

        // UI için Liste öğelerini tutan property
        [Ignore]
        public ObservableCollection<AlisverisOgesiModel> ListeOgeleri { get; set; } = new ObservableCollection<AlisverisOgesiModel>();

        // Açık/Kapalı durumu için property
        [Ignore]
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Public metod - UI güncellemesi için
        public void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}