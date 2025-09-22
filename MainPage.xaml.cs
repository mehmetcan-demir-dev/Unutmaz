using Microsoft.Maui.Controls;
using System;
using System.Linq;
using Unutmaz.Pages;

namespace Unutmaz
{
    public partial class MainPage : ContentPage
    {
        private string _selectedCategory = "";
        private bool _isAlisverisExpanded = false;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Sayfa her göründüğünde başlangıç durumuna getir
            _selectedCategory = "";
            _isAlisverisExpanded = false;
            AlisverisButton.IsVisible = true;
            AlisverisSubButtons.IsVisible = false;
        }

        private void OnCategoryTapped(object sender, EventArgs e)
        {
            if (sender is Frame frame &&
                frame.GestureRecognizers.FirstOrDefault() is TapGestureRecognizer tap)
            {
                string category = tap.CommandParameter?.ToString();
                _selectedCategory = category;

                if (category == "Alisveris")
                {
                    // Alışveriş butonunu gizle, alt butonları göster
                    AlisverisButton.IsVisible = false;
                    AlisverisSubButtons.IsVisible = true;
                    _isAlisverisExpanded = true;
                }
                else
                {
                    // Diğer kategoriler için normal işlem
                    DisplayAlert("Kategori", $"{category} kategorisi seçildi", "Tamam");
                }
            }
        }

        private async void OnKisiselAlisverisClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KisiselAlisveris());
        }

        private void OnAileAlisverisClicked(object sender, EventArgs e)
        {
            DisplayAlert("Bilgi", "Aile alışverişi kategorisi seçildi", "Tamam");
        }

        protected override bool OnBackButtonPressed()
        {
            if (_isAlisverisExpanded)
            {
                // Eğer alışveriş genişletilmişse, geri butonu ile normal haline getir
                AlisverisButton.IsVisible = true;
                AlisverisSubButtons.IsVisible = false;
                _isAlisverisExpanded = false;
                return true; // Back butonunu tüket
            }
            return base.OnBackButtonPressed();
        }
    }
}