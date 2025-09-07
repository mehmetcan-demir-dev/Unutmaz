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
            await DisplayAlert("Hata", "Veritaban� servisine eri�ilemiyor.", "Tamam");
            return;
        }

        var arsivListesi = await database.Database
            .Table<KisiselAlisverisArsivModel>()
            .OrderByDescending(x => x.ArsivelenmeTarihi)
            .ToListAsync();

        for (int i = 0; i < arsivListesi.Count; i++)
        {
            var arsivListe = arsivListesi[i];
            arsivListe.IsExpanded = false; // Ar�ivde varsay�lan olarak kapal�

            if (!string.IsNullOrEmpty(arsivListe.ListeOgesiJson))
            {
                try
                {
                    var stringListesi = JsonConvert.DeserializeObject<List<string>>(arsivListe.ListeOgesiJson);
                    arsivListe.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>(
                        stringListesi.Select(item => new AlisverisOgesiModel
                        {
                            ItemText = item,
                            IsChecked = true // Ar�ivde t�m ��eler i�aretli
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
            // A��k/Kapal� durumunu de�i�tir
            arsivListe.IsExpanded = !arsivListe.IsExpanded;
        }
    }

    private async void OnMenuTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is KisiselAlisverisArsivModel arsivListe)
        {
            // Men� se�eneklerini g�ster
            var action = await DisplayActionSheet(
                "��lemler",
                "�ptal",
                "Sil",
                "Geri Y�kle"
            );

            switch (action)
            {
                case "Geri Y�kle":
                    await OnGeriYukleClicked(arsivListe);
                    break;
                case "Sil":
                    bool confirm = await DisplayAlert("Onay",
                        $"'{arsivListe.ListeBasligi}' listesini kal�c� olarak silmek istedi�inizden emin misiniz?",
                        "Evet", "Hay�r");
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
            await DisplayAlert("Hata", "Veritaban� servisine eri�ilemiyor.", "Tamam");
            return;
        }

        try
        {
            // Ar�ivden ana listeye geri y�kle
            var yeniListe = new KisiselAlisverisListesiModel
            {
                ListeBasligi = arsivListe.ListeBasligi,
                ListeOgesiJson = arsivListe.ListeOgesiJson,
                Mod = arsivListe.Mod,
                OlusturulmaTarihi = DateTime.Now // Yeni tarih
            };

            await database.Database.InsertAsync(yeniListe);
            await database.Database.DeleteAsync(arsivListe);

            // UI'� g�ncelle
            arsivListeleri.Remove(arsivListe);

            await DisplayAlert("Ba�ar�l�",
                $"'{arsivListe.ListeBasligi}' listesi ana listeye geri y�klendi.",
                "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata",
                $"Geri y�kleme s�ras�nda hata olu�tu: {ex.Message}",
                "Tamam");
        }
    }

    private async Task OnSilClicked(KisiselAlisverisArsivModel arsivListe)
    {
        var database = App.Services.GetService<DatabaseService>();
        if (database == null)
        {
            await DisplayAlert("Hata", "Veritaban� servisine eri�ilemiyor.", "Tamam");
            return;
        }

        try
        {
            await database.Database.DeleteAsync(arsivListe);
            arsivListeleri.Remove(arsivListe);

            await DisplayAlert("Ba�ar�l�",
                $"'{arsivListe.ListeBasligi}' listesi kal�c� olarak silindi.",
                "Tamam");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata",
                $"Silme s�ras�nda hata olu�tu: {ex.Message}",
                "Tamam");
        }
    }

    private async void OnTumunuTemizleClicked(object sender, EventArgs e)
    {
        if (arsivListeleri == null || arsivListeleri.Count == 0)
        {
            await DisplayAlert("Bilgi", "Temizlenecek ar�iv yok.", "Tamam");
            return;
        }

        bool confirm = await DisplayAlert("Onay",
            $"T�m ar�iv listelerini ({arsivListeleri.Count} adet) kal�c� olarak silmek istedi�inizden emin misiniz?",
            "Evet", "Hay�r");

        if (confirm)
        {
            var database = App.Services.GetService<DatabaseService>();
            if (database == null)
            {
                await DisplayAlert("Hata", "Veritaban� servisine eri�ilemiyor.", "Tamam");
                return;
            }

            try
            {
                // T�m ar�iv listelerini sil
                foreach (var arsivListe in arsivListeleri.ToList())
                {
                    await database.Database.DeleteAsync(arsivListe);
                }

                arsivListeleri.Clear();

                await DisplayAlert("Ba�ar�l�",
                    "T�m ar�iv listeleri temizlendi.",
                    "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Temizleme s�ras�nda hata olu�tu: {ex.Message}",
                    "Tamam");
            }
        }
    }
}