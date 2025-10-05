using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Unutmaz.Models;
using Unutmaz.Pages;
using Unutmaz.Services;

namespace Unutmaz.Pages;

public partial class KisiselAlisveris : ContentPage
{
    private bool siralamaTariheGore = false;
    private ObservableCollection<KisiselAlisverisListesiModel> kayitliListeler;

    public KisiselAlisveris()
    {
        InitializeComponent();
        // Yükleme OnAppearing'de yapılacak, bu satır kaldırıldı.
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        YükleKayitliListeleri();
    }

    private async void YükleKayitliListeleri()
    {
        var database = App.Services.GetService<DatabaseService>();
        if (database == null)
        {
            await DisplayAlert("Hata", "Veritabanı servisine erişilemiyor.", "Tamam");
            return;
        }

        List<KisiselAlisverisListesiModel> listeler;
        if (!siralamaTariheGore)
        {
            // En son eklenen önce
            listeler = await database.Database
                .Table<KisiselAlisverisListesiModel>()
                .OrderByDescending(x => x.Id) // Id'ye göre en son eklenen
                .ToListAsync();
        }
        else
        {
            // Tarihe göre en yakına göre (bugüne en yakın tarih)
            DateTime bugun = DateTime.Today;
            listeler = (await database.Database
                .Table<KisiselAlisverisListesiModel>()
                .ToListAsync())
                .OrderBy(x => Math.Abs((x.OlusturulmaTarihi - bugun).Ticks))
                .ToList();
        }

        for (int i = 0; i < listeler.Count; i++)
        {
            var liste = listeler[i];
            liste.IsExpanded = (i == 0);
            if (!string.IsNullOrEmpty(liste.ListeOgesiJson))
            {
                try
                {
                    var stringListesi = JsonConvert.DeserializeObject<List<string>>(liste.ListeOgesiJson);
                    liste.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>(
                        stringListesi.Select(item => new AlisverisOgesiModel
                        {
                            ItemText = item,
                            IsChecked = false
                        })
                    );
                }
                catch
                {
                    liste.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>();
                }
            }
            else
            {
                liste.ListeOgeleri = new ObservableCollection<AlisverisOgesiModel>();
            }

            foreach (var ogesi in liste.ListeOgeleri)
            {
                ogesi.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(AlisverisOgesiModel.IsChecked))
                    {
                        liste.NotifyPropertyChanged(nameof(liste.ListeOgeleri));
                    }
                };
            }
        }

        kayitliListeler = new ObservableCollection<KisiselAlisverisListesiModel>(listeler);
        ListeCollectionView.ItemsSource = kayitliListeler;
    }

    private void BtnSiralama_Clicked(object sender, EventArgs e)
    {
        // Popup'ı aç/kapat
        SiralamaPopup.IsVisible = !SiralamaPopup.IsVisible;
    }

    private void TariheGoreSirala_Clicked(object sender, EventArgs e)
    {
        // Tarihe göre sıralama
        siralamaTariheGore = true;
        YükleKayitliListeleri();

        // Popup'ı kapat
        SiralamaPopup.IsVisible = false;
    }

    private void KayitSirasinaGoreSirala_Clicked(object sender, EventArgs e)
    {
        // Kayıt sırasına göre sıralama
        siralamaTariheGore = false;
        YükleKayitliListeleri();

        // Popup'ı kapat
        SiralamaPopup.IsVisible = false;
    }

    private void OnListeBasligTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is KisiselAlisverisListesiModel liste)
        {
            // Açık/Kapalı durumunu değiştir
            liste.IsExpanded = !liste.IsExpanded;
        }
    }

    // Maddeye tıklandığında checkbox'ı toggle etmek için yeni method
    private void OnMaddeTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is AlisverisOgesiModel madde)
        {
            madde.IsChecked = !madde.IsChecked;
        }
    }

    // Yeni Liste Oluştur butonu için yeni method
    private async void OnYeniListeOlusturClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KisiselAlisverisOlustur());
    }

    private async void OnMenuTapped(object sender, EventArgs e)
    {
        if (sender is Label label && label.BindingContext is KisiselAlisverisListesiModel liste)
        {
            // Menü seçeneklerini göster
            var action = await DisplayActionSheet(
                "İşlemler",
                "İptal",
                "Sil",
                "Düzenle",
                "Arşive Gönder"
            );

            switch (action)
            {
                case "Arşive Gönder":
                    await OnArsiveGonderClicked(liste);
                    break;
                case "Düzenle":
                    // Düzenleme için KisiselAlisverisListesi sayfasına yönlendir
                    await OnDuzenleClicked(liste);
                    break;
                case "Sil":
                    // Silme işlemi için kod eklenebilir
                    bool confirm = await DisplayAlert("Onay", $"'{liste.ListeBasligi}' listesini silmek istediğinizden emin misiniz?", "Evet", "Hayır");
                    if (confirm)
                    {
                        // Silme işlemi burada yapılacak
                        await DisplayAlert("Bilgi", "Silme özelliği yakında eklenecek.", "Tamam");
                    }
                    break;
            }
        }
    }

    private async Task OnDuzenleClicked(KisiselAlisverisListesiModel liste)
    {
        try
        {
            // KisiselAlisverisOlustur sayfasına düzenleme için yönlendir
            var duzenlemeSayfasi = new KisiselAlisverisOlustur(liste);
            await Navigation.PushAsync(duzenlemeSayfasi);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hata", $"Düzenleme sayfasına yönlendirme sırasında hata oluştu: {ex.Message}", "Tamam");
        }
    }

    private async void OnArsivButonuClicked(object sender, EventArgs e)
    {
        // Ana arşiv butonu tıklandığında
        await Navigation.PushAsync(new KisiselAlisverisArsiv());
    }

    private async Task OnArsiveGonderClicked(KisiselAlisverisListesiModel liste)
    {
        // Önce tüm maddelerin işaretli olup olmadığını kontrol et
        if (liste.ListeOgeleri.Count == 0)
        {
            await DisplayAlert("Uyarı", "Boş liste arşivlenemez.", "Tamam");
            return;
        }

        if (!liste.ListeOgeleri.All(x => x.IsChecked))
        {
            await DisplayAlert("Uyarı",
                "Sadece tüm maddeleri tamamlanmış listeler arşivlenebilir.",
                "Tamam");
            return;
        }

        // Tek liste arşive gönderme
        bool confirm = await DisplayAlert("Onay",
            $"'{liste.ListeBasligi}' listesini arşive göndermek istediğinizden emin misiniz?",
            "Evet", "Hayır");

        if (confirm)
        {
            var database = App.Services.GetService<DatabaseService>();
            if (database == null)
            {
                await DisplayAlert("Hata", "Veritabanı servisine erişilemiyor.", "Tamam");
                return;
            }

            try
            {
                // Arşiv modeline dönüştür
                var arsivListe = new KisiselAlisverisArsivModel
                {
                    ListeBasligi = liste.ListeBasligi,
                    ListeOgesiJson = liste.ListeOgesiJson,
                    Mod = liste.Mod,
                    OlusturulmaTarihi = liste.OlusturulmaTarihi,
                    ArsivelenmeTarihi = DateTime.Now
                };

                // Arşive ekle
                await database.Database.InsertAsync(arsivListe);

                // Ana listeden sil
                await database.Database.DeleteAsync(liste);

                // UI'ı güncelle
                kayitliListeler.Remove(liste);

                string listeBasligi = liste.ListeBasligi;
                await DisplayAlert("Başarılı",
                    $"'{listeBasligi}' listesi arşive gönderildi.",
                    "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata",
                    $"Arşivleme sırasında hata oluştu: {ex.Message}",
                    "Tamam");
            }
        }
    }
}