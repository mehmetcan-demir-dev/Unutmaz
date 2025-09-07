using Microsoft.Maui.Controls;
using Unutmaz.Models;
using Unutmaz.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Unutmaz.Pages;

public partial class KisiselAlisverisArsiv : ContentPage
{
    private ObservableCollection<KisiselAlisverisArsivModel> arsivListeleri;

    public KisiselAlisverisArsiv()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        YukleArsivListeleri();
    }

    private async void YukleArsivListeleri()
    {
        var database = App.Services.GetService<DatabaseService>();
        if (database == null)
        {
            await DisplayAlert("Hata", "Veritabaný servisine eriþilemiyor.", "Tamam");
            return;
        }

        var arsivListesi = await database.Database
            .Table<KisiselAlisverisArsivModel>()
            .OrderByDescending(x => x.ArsivelenmeTarihi)
            .ToListAsync();

        for (int i = 0; i < arsivListesi.Count; i++)
        {
            var arsivListe = arsivListesi[i];
            arsivListe.IsExpanded = false; // Arþivde varsayýlan olarak kapalý

            if (!string.IsNullOrEmpty(arsivListe.ListeOgesiJson))
            {
                try
                {
                    var stringListesi = JsonConvert.DeserializeObject<List<string>>(arsivListe.ListeOgesiJson);
                    arsivListe.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>(
                        stringListesi.Select(item => new AlisverisOgesiModel
                        {
                            ItemText = item,
                            IsChecked = true // Arþivde tüm öðeler iþaretli
                        })
                    );
                }
                catch
                {
                    arsivListe.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>();
                }
            }
            else
            {
                arsivListe.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>();
            }
        }

        arsivListeleri = new ObservableCollection<KisiselAlisverisArsivModel>(arsivListesi);
        ArsivCollectionView.ItemsSource = arsivListeleri;
    }

    private void OnListeBasligTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is KisiselAlisverisArsivModel arsivListe)
        {
            // Açýk/Kapalý durumunu deðiþtir
            arsivListe.IsExpanded = !arsivListe.IsExpanded;
        }
    }

    private async void OnMenuTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is KisiselAlisverisArsivModel arsivListe)
        {
            // Menü seçeneklerini göster
            var action = await DisplayActionSheet(
                "Ýþlemler",
                "Ýptal",
                "Sil",
                "Geri Yükle"
            );

            switch (action)
            {
                case "Geri Yükle":
                    await OnGeriYukleClicked(arsivListe);
                    break;
                case "Sil":
                    bool confirm = await DisplayAlert("Onay",
                        $"'{arsivListe.ListeBasligi}' listesini kalýcý olarak silmek istediðinizden emin misiniz?",
                        "Evet", "Hayýr");
                    if (confirm)
                    {
                        await OnSilClicked(arsivListe);
                    }
                    break;
            }
        }
    }

    private async Task OnGeriYukleClicked(KisiselAlisverisArsivModel arsivListe)
    {
        var database = App.Services.GetService<DatabaseService>();
        if (database == null)
        {
            await DisplayAlert("Hata", "Veritabaný servisine eriþilemiyor.", "Tamam");
            return;
        }

        try
        {
            // Arþivden ana listeye geri yükle
            var yeniListe = new KisiselAlisverisListesiModel
            {
                ListeBasligi = arsivListe.ListeBasligi,
                ListeOgesiJson = arsivListe.ListeOgesiJson,
                Mod = arsivListe.Mod,
                OlusturulmaTarihi = DateTime.Now // Yeni tarih
            };

            await database.Database.InsertAsync(yeniListe);
            await database.Database.DeleteAsync(arsivListe);

            // UI'ý güncelle
            arsivListeleri.Remove(arsivListe);

            await DisplayAlert("Baþarýlý",
                $"'{arsivListe.ListeBasligi}' listesi ana listeye geri yüklendi.",
                "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata",
                $"Geri yükleme sýrasýnda hata oluþtu: {ex.Message}",
                "Tamam");
        }
    }

    private async Task OnSilClicked(KisiselAlisverisArsivModel arsivListe)
    {
        var database = App.Services.GetService<DatabaseService>();
        if (database == null)
        {
            await DisplayAlert("Hata", "Veritabaný servisine eriþilemiyor.", "Tamam");
            return;
        }

        try
        {
            await database.Database.DeleteAsync(arsivListe);
            arsivListeleri.Remove(arsivListe);

            await DisplayAlert("Baþarýlý",
                $"'{arsivListe.ListeBasligi}' listesi kalýcý olarak silindi.",
                "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata",
                $"Silme sýrasýnda hata oluþtu: {ex.Message}",
                "Tamam");
        }
    }

    private async void OnTumunuTemizleClicked(object sender, EventArgs e)
    {
        if (arsivListeleri == null || arsivListeleri.Count == 0)
        {
            await DisplayAlert("Bilgi", "Temizlenecek arþiv yok.", "Tamam");
            return;
        }

        bool confirm = await DisplayAlert("Onay",
            $"Tüm arþiv listelerini ({arsivListeleri.Count} adet) kalýcý olarak silmek istediðinizden emin misiniz?",
            "Evet", "Hayýr");

        if (confirm)
        {
            var database = App.Services.GetService<DatabaseService>();
            if (database == null)
            {
                await DisplayAlert("Hata", "Veritabaný servisine eriþilemiyor.", "Tamam");
                return;
            }

            try
            {
                // Tüm arþiv listelerini sil
                foreach (var arsivListe in arsivListeleri.ToList())
                {
                    await database.Database.DeleteAsync(arsivListe);
                }

                arsivListeleri.Clear();

                await DisplayAlert("Baþarýlý",
                    "Tüm arþiv listeleri temizlendi.",
                    "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Temizleme sýrasýnda hata oluþtu: {ex.Message}",
                    "Tamam");
            }
        }
    }
}