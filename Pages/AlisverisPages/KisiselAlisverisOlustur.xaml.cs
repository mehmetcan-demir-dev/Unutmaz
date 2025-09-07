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
        private int duzenlenenIndex = -1; // D�zenleme i�in se�ili index, -1 ise d�zenleme yok
        private int? _duzenlenecekListeId = null; // D�zenleme i�in liste ID'si

        // Varsay�lan constructor - yeni liste olu�turma
        public KisiselAlisverisOlustur()
        {
            InitializeComponent();
            InitializeForm();
        }

        // D�zenleme constructor - mevcut liste d�zenleme
        public KisiselAlisverisOlustur(KisiselAlisverisListesiModel liste)
        {
            InitializeComponent();
            InitializeForm();

            // D�zenleme moduna ge�
            _duzenlenecekListeId = liste.Id;

            // Formu doldur
            EntryListeBasligi.Text = liste.ListeBasligi;

            // Modu ayarla ve tarihi yans�t
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

            // JSON'dan liste ��elerini ��z�mle ve ekle
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
                DisplayAlert("Hata", $"Liste verileri y�klenirken hata olu�tu: {ex.Message}", "Tamam");
            }

            // Sayfa ba�l���n� g�ncelle
            Title = "Liste D�zenle";
        }

        private void InitializeForm()
        {
            alisverisListesi = new ObservableCollection<string>();
            ListViewAlisveris.ItemsSource = alisverisListesi;

            // Varsay�lan olarak Serbest mod a��k
            LayoutSerbest.IsVisible = true;
            LayoutStandart.IsVisible = false;

            // �rnek kategoriler ve �r�nler
            PickerKategori.ItemsSource = new List<string> { "Kahvalt�l�k", "��ecek", "Temizlik", "At��t�rmal�k" };
            PickerUrun.ItemsSource = new List<string> { "S�t", "Peynir", "Zeytin", "�ay", "Kola", "Bisk�vi", "Deterjan" };

            // Tarih se�icilerin varsay�lan tarih ayar� (bug�nden 7 g�n sonras�)
            DatePickerSerbestTarih.Date = DateTime.Today.AddDays(7);
            DatePickerStandartTarih.Date = DateTime.Today.AddDays(7);
        }

        // JSON'a �evirme metodu
        private string ConvertToJson()
        {
            return JsonSerializer.Serialize(alisverisListesi);
        }

        // SERBEST moddan �r�n ekle veya d�zenle
        private async void BtnListeyeEkle_Clicked(object sender, EventArgs e)
        {
            string urun = EntryUrun.Text?.Trim();
            string miktar = EntryMiktar.Text?.Trim();

            if (string.IsNullOrEmpty(urun) || string.IsNullOrEmpty(miktar))
            {
                await DisplayAlert("Uyar�", "L�tfen hem �r�n hem miktar bilgisini giriniz.", "Tamam");
                return;
            }

            string yeniKalem = $"{miktar} {urun}";

            if (duzenlenenIndex >= 0)
            {
                // D�zenleme modu - listedeki ��eyi g�ncelle
                alisverisListesi[duzenlenenIndex] = yeniKalem;

                // D�zenleme bitti, index s�f�rla
                duzenlenenIndex = -1;

                // Buton metnini tekrar de�i�tir
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
                DisplayAlert("Uyar�", "L�tfen kategori, �r�n ve miktar se�iniz.", "Tamam");
                return;
            }

            string yeniKalem = $"{kategori} > {urun} > {miktar}";

            if (duzenlenenIndex >= 0)
            {
                // D�zenleme i�lemi
                alisverisListesi[duzenlenenIndex] = yeniKalem;
                duzenlenenIndex = -1;

                // Buton metnini eski haline d�nd�r
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


        // Modlar aras� ge�i�: SERBEST
        private void OnSerbestModClicked(object sender, EventArgs e)
        {
            LayoutSerbest.IsVisible = true;
            LayoutStandart.IsVisible = false;

            BtnSerbestMod.BackgroundColor = Color.FromArgb("#A3C4BC");
            BtnStandartMod.BackgroundColor = Color.FromArgb("#F0F0F0");
        }

        // Modlar aras� ge�i�: STANDART
        private void OnStandartModClicked(object sender, EventArgs e)
        {
            LayoutSerbest.IsVisible = false;
            LayoutStandart.IsVisible = true;

            BtnSerbestMod.BackgroundColor = Color.FromArgb("#F0F0F0");
            BtnStandartMod.BackgroundColor = Color.FromArgb("#A3C4BC");
        }

        // Tek t�klamada se�ili ��eyi sil
        private async void OnItemTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.Text != null)
            {
                bool secim = await DisplayAlert("Sil", $"\"{label.Text}\" ��esini silmek istiyor musunuz?", "Evet", "Hay�r");
                if (secim)
                {
                    alisverisListesi.Remove(label.Text);
                }
            }
        }

        // �ift t�klamada d�zenleme i�in entrylere de�erleri koy
        private void OnItemDoubleTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.Text != null)
            {
                duzenlenenIndex = alisverisListesi.IndexOf(label.Text);
                if (duzenlenenIndex < 0) return;

                string secilen = alisverisListesi[duzenlenenIndex];

                if (secilen.Contains(" > "))
                {
                    // STANDART MOD - Format: "Kategori > �r�n > Miktar"
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

                    BtnListeyeEkleStandart.Text = "D�zenle";

                    OnStandartModClicked(null, null);
                }
                else
                {
                    // SERBEST MOD - Format: "Miktar �r�n"
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

                    BtnListeyeEkle.Text = "D�zenle";

                    OnSerbestModClicked(null, null);
                }
            }
        }

        private async void BtnListeyiTemizle_Clicked(object sender, EventArgs e)
        {
            if (alisverisListesi.Count == 0)
            {
                await DisplayAlert("Bilgi", "Liste zaten bo�.", "Tamam");
            }
            else
            {
                bool onay = await DisplayAlert("Temizle", "T�m listeyi silmek istiyor musunuz?", "Evet", "Hay�r");
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
                await DisplayAlert("Uyar�", "Liste bo� olmamal�. L�tfen �r�n ekleyin.", "Tamam");
                return;
            }

            string baslik = EntryListeBasligi.Text?.Trim();
            if (string.IsNullOrEmpty(baslik))
                baslik = "�simsiz Liste";

            string mod = LayoutSerbest.IsVisible ? "Serbest" : "Standart";
            string jsonListe = ConvertToJson();

            try
            {
                var database = App.Services.GetService<DatabaseService>();
                if (database == null)
                {
                    await DisplayAlert("Hata", "Veritaban� servisine eri�ilemiyor.", "Tamam");
                    return;
                }

                if (_duzenlenecekListeId.HasValue)
                {
                    // D�zenleme modu - mevcut kayd� g�ncelle
                    await GuncelleVeriTabaninda(database, baslik, mod, jsonListe);
                }
                else
                {
                    // Yeni kay�t modu
                    await KaydetVeriTabaninda(database, baslik, mod, jsonListe);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"��lem tamamlanamad�.\n\n{ex.Message}", "Tamam");
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
            await DisplayAlert("Ba�ar�l�", "Liste ba�ar�yla kaydedildi!", "Tamam");

            // Liste temizle
            alisverisListesi.Clear();
            EntryListeBasligi.Text = string.Empty;

            // Geri y�nlendir
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
                // Kullan�c�n�n se�ti�i tarihi oku
                DateTime secilenTarih = LayoutSerbest.IsVisible
                    ? DatePickerSerbestTarih.Date
                    : DatePickerStandartTarih.Date;

                // Kayd� g�ncelle
                mevcutKayit.ListeBasligi = baslik;
                mevcutKayit.Mod = mod;
                mevcutKayit.ListeOgesiJson = jsonListe;
                mevcutKayit.OlusturulmaTarihi = secilenTarih; // ?? Tarih burada al�nmal�

                await database.Database.UpdateAsync(mevcutKayit);
                await DisplayAlert("Ba�ar�l�", "Liste ba�ar�yla g�ncellendi!", "Tamam");

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Hata", "G�ncellenecek kay�t bulunamad�.", "Tamam");
            }
        }


    }
}