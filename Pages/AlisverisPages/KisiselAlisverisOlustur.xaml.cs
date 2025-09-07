using System.Text.Json;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Unutmaz.Services;
using Unutmaz.Models;
using System.Linq;

namespace Unutmaz.Pages
{
    public partial class KisiselAlisverisOlustur : ContentPage
    {
        private ObservableCollection<string> alisverisListesi;
        private int duzenlenenIndex = -1; // Düzenleme için seçili index, -1 ise düzenleme yok
        private int? _duzenlenecekListeId = null; // Düzenleme için liste ID'si

        // Varsayýlan constructor - yeni liste oluþturma
        public KisiselAlisverisOlustur()
        {
            InitializeComponent();
            InitializeForm();
        }

        // Düzenleme constructor - mevcut liste düzenleme
        public KisiselAlisverisOlustur(KisiselAlisverisListesiModel liste)
        {
            InitializeComponent();
            InitializeForm();

            // Düzenleme moduna geç
            _duzenlenecekListeId = liste.Id;

            // Formu doldur
            EntryListeBasligi.Text = liste.ListeBasligi;

            // Modu ayarla ve tarihi yansýt
            if (liste.Mod == "Serbest")
            {
                OnSerbestModClicked(null, null);
                DatePickerSerbestTarih.Date = liste.OlusturulmaTarihi;
            }
            else
            {
                OnStandartModClicked(null, null);
                DatePickerStandartTarih.Date = liste.OlusturulmaTarihi;
            }

            // JSON'dan liste öðelerini çözümle ve ekle
            try
            {
                var listeMaddeleri = JsonSerializer.Deserialize<List<string>>(liste.ListeOgesiJson);
                if (listeMaddeleri != null)
                {
                    foreach (var madde in listeMaddeleri)
                    {
                        if (!string.IsNullOrWhiteSpace(madde))
                        {
                            alisverisListesi.Add(madde.Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Hata", $"Liste verileri yüklenirken hata oluþtu: {ex.Message}", "Tamam");
            }

            // Sayfa baþlýðýný güncelle
            Title = "Liste Düzenle";
        }

        private void InitializeForm()
        {
            alisverisListesi = new ObservableCollection<string>();
            ListViewAlisveris.ItemsSource = alisverisListesi;

            // Varsayýlan olarak Serbest mod açýk
            LayoutSerbest.IsVisible = true;
            LayoutStandart.IsVisible = false;

            // Örnek kategoriler ve ürünler
            PickerKategori.ItemsSource = new List<string> { "Kahvaltýlýk", "Ýçecek", "Temizlik", "Atýþtýrmalýk" };
            PickerUrun.ItemsSource = new List<string> { "Süt", "Peynir", "Zeytin", "Çay", "Kola", "Bisküvi", "Deterjan" };

            // Tarih seçicilerin varsayýlan tarih ayarý (bugünden 7 gün sonrasý)
            DatePickerSerbestTarih.Date = DateTime.Today.AddDays(7);
            DatePickerStandartTarih.Date = DateTime.Today.AddDays(7);
        }

        // JSON'a çevirme metodu
        private string ConvertToJson()
        {
            return JsonSerializer.Serialize(alisverisListesi);
        }

        // SERBEST moddan ürün ekle veya düzenle
        private async void BtnListeyeEkle_Clicked(object sender, EventArgs e)
        {
            string urun = EntryUrun.Text?.Trim();
            string miktar = EntryMiktar.Text?.Trim();

            if (string.IsNullOrEmpty(urun) || string.IsNullOrEmpty(miktar))
            {
                await DisplayAlert("Uyarý", "Lütfen hem ürün hem miktar bilgisini giriniz.", "Tamam");
                return;
            }

            string yeniKalem = $"{miktar} {urun}";

            if (duzenlenenIndex >= 0)
            {
                // Düzenleme modu - listedeki öðeyi güncelle
                alisverisListesi[duzenlenenIndex] = yeniKalem;

                // Düzenleme bitti, index sýfýrla
                duzenlenenIndex = -1;

                // Buton metnini tekrar deðiþtir
                BtnListeyeEkle.Text = "Listeye Ekle";
            }
            else
            {
                // Yeni ekleme
                alisverisListesi.Insert(0, yeniKalem);
            }

            // Entry'leri temizle
            EntryUrun.Text = string.Empty;
            EntryMiktar.Text = string.Empty;
        }

        private void OnListeyeEkle_Standart_Clicked(object sender, EventArgs e)
        {
            string kategori = PickerKategori.SelectedItem?.ToString();
            string urun = PickerUrun.SelectedItem?.ToString();
            string miktar = EntryStandartMiktar.Text?.Trim();

            if (kategori == null || urun == null || string.IsNullOrEmpty(miktar))
            {
                DisplayAlert("Uyarý", "Lütfen kategori, ürün ve miktar seçiniz.", "Tamam");
                return;
            }

            string yeniKalem = $"{kategori} > {urun} > {miktar}";

            if (duzenlenenIndex >= 0)
            {
                // Düzenleme iþlemi
                alisverisListesi[duzenlenenIndex] = yeniKalem;
                duzenlenenIndex = -1;

                // Buton metnini eski haline döndür
                BtnListeyeEkleStandart.Text = "Listeye Ekle";
            }
            else
            {
                // Yeni ekleme
                alisverisListesi.Insert(0, yeniKalem);
            }

            // Girdileri temizle
            EntryStandartMiktar.Text = string.Empty;
            PickerKategori.SelectedItem = null;
            PickerUrun.SelectedItem = null;
        }


        // Modlar arasý geçiþ: SERBEST
        private void OnSerbestModClicked(object sender, EventArgs e)
        {
            LayoutSerbest.IsVisible = true;
            LayoutStandart.IsVisible = false;

            BtnSerbestMod.BackgroundColor = Color.FromArgb("#A3C4BC");
            BtnStandartMod.BackgroundColor = Color.FromArgb("#F0F0F0");
        }

        // Modlar arasý geçiþ: STANDART
        private void OnStandartModClicked(object sender, EventArgs e)
        {
            LayoutSerbest.IsVisible = false;
            LayoutStandart.IsVisible = true;

            BtnSerbestMod.BackgroundColor = Color.FromArgb("#F0F0F0");
            BtnStandartMod.BackgroundColor = Color.FromArgb("#A3C4BC");
        }

        // Tek týklamada seçili öðeyi sil
        private async void OnItemTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.Text != null)
            {
                bool secim = await DisplayAlert("Sil", $"\"{label.Text}\" öðesini silmek istiyor musunuz?", "Evet", "Hayýr");
                if (secim)
                {
                    alisverisListesi.Remove(label.Text);
                }
            }
        }

        // Çift týklamada düzenleme için entrylere deðerleri koy
        private void OnItemDoubleTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.Text != null)
            {
                duzenlenenIndex = alisverisListesi.IndexOf(label.Text);
                if (duzenlenenIndex < 0) return;

                string secilen = alisverisListesi[duzenlenenIndex];

                if (secilen.Contains(" > "))
                {
                    // STANDART MOD - Format: "Kategori > Ürün > Miktar"
                    var parcalar = secilen.Split(" > ");
                    if (parcalar.Length != 3)
                    {
                        duzenlenenIndex = -1;
                        return;
                    }

                    string kategori = parcalar[0];
                    string urun = parcalar[1];
                    string miktar = parcalar[2];

                    PickerKategori.SelectedItem = kategori;
                    PickerUrun.SelectedItem = urun;
                    EntryStandartMiktar.Text = miktar;

                    BtnListeyeEkleStandart.Text = "Düzenle";

                    OnStandartModClicked(null, null);
                }
                else
                {
                    // SERBEST MOD - Format: "Miktar Ürün"
                    var kelimeler = secilen.Split(' ');

                    if (kelimeler.Length < 2)
                    {
                        duzenlenenIndex = -1;
                        return;
                    }

                    string urun = kelimeler[^1];
                    string miktar = string.Join(' ', kelimeler, 0, kelimeler.Length - 1);

                    EntryUrun.Text = urun;
                    EntryMiktar.Text = miktar;

                    BtnListeyeEkle.Text = "Düzenle";

                    OnSerbestModClicked(null, null);
                }
            }
        }

        private async void BtnListeyiTemizle_Clicked(object sender, EventArgs e)
        {
            if (alisverisListesi.Count == 0)
            {
                await DisplayAlert("Bilgi", "Liste zaten boþ.", "Tamam");
            }
            else
            {
                bool onay = await DisplayAlert("Temizle", "Tüm listeyi silmek istiyor musunuz?", "Evet", "Hayýr");
                if (onay)
                {
                    alisverisListesi.Clear();
                }
            }
        }

        private async void BtnListeyiTamamla_Clicked(object sender, EventArgs e)
        {
            if (alisverisListesi.Count == 0)
            {
                await DisplayAlert("Uyarý", "Liste boþ olmamalý. Lütfen ürün ekleyin.", "Tamam");
                return;
            }

            string baslik = EntryListeBasligi.Text?.Trim();
            if (string.IsNullOrEmpty(baslik))
                baslik = "Ýsimsiz Liste";

            string mod = LayoutSerbest.IsVisible ? "Serbest" : "Standart";
            string jsonListe = ConvertToJson();

            try
            {
                var database = App.Services.GetService<DatabaseService>();
                if (database == null)
                {
                    await DisplayAlert("Hata", "Veritabaný servisine eriþilemiyor.", "Tamam");
                    return;
                }

                if (_duzenlenecekListeId.HasValue)
                {
                    // Düzenleme modu - mevcut kaydý güncelle
                    await GuncelleVeriTabaninda(database, baslik, mod, jsonListe);
                }
                else
                {
                    // Yeni kayýt modu
                    await KaydetVeriTabaninda(database, baslik, mod, jsonListe);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Ýþlem tamamlanamadý.\n\n{ex.Message}", "Tamam");
            }
        }

        private async Task KaydetVeriTabaninda(DatabaseService database, string baslik, string mod, string jsonListe)
        {
            DateTime secilenTarih = LayoutSerbest.IsVisible
                ? DatePickerSerbestTarih.Date
                : DatePickerStandartTarih.Date;

            var yeniListe = new KisiselAlisverisListesiModel
            {
                ListeBasligi = baslik,
                Mod = mod,
                ListeOgesiJson = jsonListe,
                OlusturulmaTarihi = secilenTarih
            };

            await database.Database.InsertAsync(yeniListe);
            await DisplayAlert("Baþarýlý", "Liste baþarýyla kaydedildi!", "Tamam");

            // Liste temizle
            alisverisListesi.Clear();
            EntryListeBasligi.Text = string.Empty;

            // Geri yönlendir
            await Navigation.PushAsync(new KisiselAlisveris());
        }


        private async Task GuncelleVeriTabaninda(DatabaseService database, string baslik, string mod, string jsonListe)
        {
            var mevcutKayit = await database.Database
                .Table<KisiselAlisverisListesiModel>()
                .Where(x => x.Id == _duzenlenecekListeId.Value)
                .FirstOrDefaultAsync();

            if (mevcutKayit != null)
            {
                // Kullanýcýnýn seçtiði tarihi oku
                DateTime secilenTarih = LayoutSerbest.IsVisible
                    ? DatePickerSerbestTarih.Date
                    : DatePickerStandartTarih.Date;

                // Kaydý güncelle
                mevcutKayit.ListeBasligi = baslik;
                mevcutKayit.Mod = mod;
                mevcutKayit.ListeOgesiJson = jsonListe;
                mevcutKayit.OlusturulmaTarihi = secilenTarih; // ?? Tarih burada alýnmalý

                await database.Database.UpdateAsync(mevcutKayit);
                await DisplayAlert("Baþarýlý", "Liste baþarýyla güncellendi!", "Tamam");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", "Güncellenecek kayýt bulunamadý.", "Tamam");
            }
        }


    }
}