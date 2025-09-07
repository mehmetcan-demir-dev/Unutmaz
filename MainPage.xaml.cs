using Microsoft.Maui.Controls;
using System;
using System.Linq;
using Unutmaz.Pages;

namespace Unutmaz
{
    public partial class MainPage : ContentPage
    {
        private string _selectedCategory = "";

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Sayfa her göründüğünde geçici tüm öğeleri gizle
            SelectionOverlay.IsVisible = false;
            MainMenu.IsEnabled = true;

            KisiselButton.IsVisible = false;
            AileButton.IsVisible = false;
            AlisverisBigButton.IsVisible = false;
            PlusNextButtonGrid.IsVisible = false;

            _selectedCategory = "";
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
                    SelectionOverlay.IsVisible = true;
                    MainMenu.IsEnabled = false;

                    AlisverisBigButton.IsVisible = true;
                    KisiselButton.IsVisible = true;
                    AileButton.IsVisible = true;

                    PlusNextButtonGrid.IsVisible = false;
                }
                else
                {
                    SelectionOverlay.IsVisible = false;
                    MainMenu.IsEnabled = true;
                }
            }
        }

        private void OnKisiselClicked(object sender, EventArgs e)
        {
            PlusNextButtonGrid.IsVisible = !PlusNextButtonGrid.IsVisible;
        }

        private void OnAileClicked(object sender, EventArgs e)
        {
            PlusNextButtonGrid.IsVisible = false;
            DisplayAlert("Bilgi", "Aile kategorisi seçildi", "Tamam");
        }

        private async void OnPlusButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KisiselAlisverisOlustur());
        }

        private async void OnNextButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new KisiselAlisveris());
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            SelectionOverlay.IsVisible = false;
            PlusNextButtonGrid.IsVisible = false;

            _selectedCategory = "";
            MainMenu.IsEnabled = true;
        }

        protected override bool OnBackButtonPressed()
        {
            if (SelectionOverlay.IsVisible)
            {
                OnBackButtonClicked(null, null);
                return true;
            }
            return base.OnBackButtonPressed();
        }
    }
}
