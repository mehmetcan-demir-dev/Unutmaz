using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Unutmaz
{
    public partial class IntroPage : ContentPage
    {
        private const int TotalSeconds = 7;
        private const int TypingDurationMs = 6000; // 6 saniye
        private string welcomeMessage = "Unutmaz'a hoþ geldini.";
        private string introMessage = "Ben Unutmaz. Kiþisel asistanýnýz olarak hiçbir þeyi unutmam.";
        private bool isLastSecond = false;
        private bool zAnimationCompleted = false;

        public IntroPage()
        {
            InitializeComponent();
            StartIntroSequence();
        }

        private async void StartIntroSequence()
        {
            // Sayaç ve yazý animasyonunu paralel baþlat
            var countdownTask = StartCountdown();
            var typingTask = StartTypingAnimation();

            await Task.WhenAll(countdownTask, typingTask);
        }

        private async Task StartCountdown()
        {
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(TotalSeconds);

            while (DateTime.Now < endTime)
            {
                var remaining = endTime - DateTime.Now;
                var minutes = remaining.Minutes;
                var seconds = remaining.Seconds;
                var centiseconds = remaining.Milliseconds / 10;

                // Format: MM:SS:CC
                CountdownLabel.Text = $"{minutes:D2}:{seconds:D2}:{centiseconds:D2}";

                // Son 1 saniye kontrolü
                if (remaining.TotalSeconds <= 1.0 && !isLastSecond)
                {
                    isLastSecond = true;
                    _ = StartLastSecondEffects();
                }

                // Son 1 saniyede yanýp sönme efekti
                if (isLastSecond)
                {
                    await AnimateCountdownLastSecond();
                }

                await Task.Delay(50);
            }

            // Sayaç tamamlandýðýnda buton göster
            CountdownLabel.Text = "00:00:00";
            ShowStartButton();
        }

        private async Task StartTypingAnimation()
        {
            // Toplam süreyi iki mesaja böl
            var firstMessageDuration = TypingDurationMs / 3; // Ýlk mesaj için 2 saniye
            var secondMessageDuration = (TypingDurationMs * 2) / 3; // Ýkinci mesaj için 4 saniye

            // Ýlk mesajý yaz (hoþ geldini)
            await TypeText(WelcomeLabel, welcomeMessage, firstMessageDuration);

            await Task.Delay(200); // Kýsa bir duraklama

            // Ýkinci mesajý yaz
            await TypeText(IntroLabel, introMessage, secondMessageDuration);
        }

        private async Task TypeText(Label label, string text, int durationMs)
        {
            label.Text = "";
            int charDelay = durationMs / text.Length;

            foreach (char c in text)
            {
                label.Text += c;
                await Task.Delay(charDelay);
            }
        }

        private async Task StartLastSecondEffects()
        {
            // Z harfi animasyonu
            await AnimateZLetter();
        }

        private async Task AnimateCountdownLastSecond()
        {
            // Renk deðiþtirme
            CountdownLabel.TextColor = CountdownLabel.TextColor == Colors.Red ? Colors.Black : Colors.Red;

            // Büyüme küçülme efekti
            await CountdownLabel.ScaleTo(1.2, 100, Easing.CubicInOut);
            await CountdownLabel.ScaleTo(1.0, 100, Easing.CubicInOut);
        }

        private async Task AnimateZLetter()
        {
            if (zAnimationCompleted) return;
            zAnimationCompleted = true;

            // Mevcut metindeki "geldini" pozisyonunu bul
            var currentText = WelcomeLabel.Text;
            var targetWord = "geldini";
            var wordIndex = currentText.IndexOf(targetWord);

            if (wordIndex >= 0)
            {
                // "z" harfini ekle
                var newText = currentText.Replace("geldini", "geldiniz");

                // Geçici animasyon için kýrmýzý z harfi göster
                var beforeZ = currentText.Substring(0, wordIndex + targetWord.Length);
                WelcomeLabel.Text = beforeZ;

                // Z harfini animasyonlu þekilde ekle
                await Task.Delay(100);

                // Kýrmýzý z harfi efekti
                var originalColor = WelcomeLabel.TextColor;
                WelcomeLabel.TextColor = Colors.Red;
                WelcomeLabel.Text = beforeZ + "z";

                // Büyüme animasyonu
                await WelcomeLabel.ScaleTo(1.1, 200);
                await WelcomeLabel.ScaleTo(1.0, 200);

                await Task.Delay(300);

                // Normal renge dön ve tam metni göster
                WelcomeLabel.TextColor = originalColor;
                WelcomeLabel.Text = newText;
            }
        }

        private void ShowStartButton()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                StartButton.IsVisible = true;
                StartButton.Opacity = 0;
                StartButton.Scale = 0.8;

                await Task.WhenAll(
                    StartButton.FadeTo(1, 500),
                    StartButton.ScaleTo(1, 500, Easing.BounceOut)
                );
            });
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            // Buton animasyonu
            await StartButton.ScaleTo(0.95, 100);
            await StartButton.ScaleTo(1, 100);

            // MainPage'e geçiþ
            await Navigation.PushAsync(new MainPage());
        }
    }
}