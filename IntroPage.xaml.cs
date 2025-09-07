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
        private string welcomeMessage = "Unutmaz'a ho� geldini.";
        private string introMessage = "Ben Unutmaz. Ki�isel asistan�n�z olarak hi�bir �eyi unutmam.";
        private bool isLastSecond = false;
        private bool zAnimationCompleted = false;

        public IntroPage()
        {
            InitializeComponent();
            StartIntroSequence();
        }

        private async void StartIntroSequence()
        {
            // Saya� ve yaz� animasyonunu paralel ba�lat
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

                // Son 1 saniye kontrol�
                if (remaining.TotalSeconds <= 1.0 && !isLastSecond)
                {
                    isLastSecond = true;
                    _ = StartLastSecondEffects();
                }

                // Son 1 saniyede yan�p s�nme efekti
                if (isLastSecond)
                {
                    await AnimateCountdownLastSecond();
                }

                await Task.Delay(50);
            }

            // Saya� tamamland���nda buton g�ster
            CountdownLabel.Text = "00:00:00";
            ShowStartButton();
        }

        private async Task StartTypingAnimation()
        {
            // Toplam s�reyi iki mesaja b�l
            var firstMessageDuration = TypingDurationMs / 3; // �lk mesaj i�in 2 saniye
            var secondMessageDuration = (TypingDurationMs * 2) / 3; // �kinci mesaj i�in 4 saniye

            // �lk mesaj� yaz (ho� geldini)
            await TypeText(WelcomeLabel, welcomeMessage, firstMessageDuration);

            await Task.Delay(200); // K�sa bir duraklama

            // �kinci mesaj� yaz
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
            // Renk de�i�tirme
            CountdownLabel.TextColor = CountdownLabel.TextColor == Colors.Red ? Colors.Black : Colors.Red;

            // B�y�me k���lme efekti
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

                // Ge�ici animasyon i�in k�rm�z� z harfi g�ster
                var beforeZ = currentText.Substring(0, wordIndex + targetWord.Length);
                WelcomeLabel.Text = beforeZ;

                // Z harfini animasyonlu �ekilde ekle
                await Task.Delay(100);

                // K�rm�z� z harfi efekti
                var originalColor = WelcomeLabel.TextColor;
                WelcomeLabel.TextColor = Colors.Red;
                WelcomeLabel.Text = beforeZ + "z";

                // B�y�me animasyonu
                await WelcomeLabel.ScaleTo(1.1, 200);
                await WelcomeLabel.ScaleTo(1.0, 200);

                await Task.Delay(300);

                // Normal renge d�n ve tam metni g�ster
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

            // MainPage'e ge�i�
            await Navigation.PushAsync(new MainPage());
        }
    }
}