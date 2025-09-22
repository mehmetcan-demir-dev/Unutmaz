using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace Unutmaz
{
    public partial class IntroPage : ContentPage
    {
        // Typewriter text content
        private readonly string line1Text = "Unutmaz'a hoþ geldiniz!";
        private readonly string line2Prefix = "Merhaba ben Unutmaz. Kiþisel asistanýnýz olarak hiçbir þeyi ";
        private readonly string line2Special = "unutmam"; // This word will be shown with DancingScript
        private readonly string line2Suffix = ".";

        // Span references for formatted text
        private Span line2PrefixSpan;
        private Span line2SpecialSpan;
        private Span line2SuffixSpan;

        public IntroPage()
        {
            InitializeComponent();
            // Start typewriter effect
            _ = StartTypingAsync();
        }

        // ============================================================
        // Typewriter Effect Implementation
        // ============================================================

        private async Task StartTypingAsync()
        {
            try
            {
                // Create FormattedString and spans for Line2
                line2PrefixSpan = new Span
                {
                    Text = "",
                    FontSize = 20,
                    TextColor = Colors.Black
                };

                line2SpecialSpan = new Span
                {
                    Text = "",
                    FontSize = 28,
                    FontFamily = "DancingScript",
                    TextColor = Colors.Purple
                };

                line2SuffixSpan = new Span
                {
                    Text = "",
                    FontSize = 20,
                    TextColor = Colors.Black
                };

                var formattedString = new FormattedString();
                formattedString.Spans.Add(line2PrefixSpan);
                formattedString.Spans.Add(line2SpecialSpan);
                formattedString.Spans.Add(line2SuffixSpan);

                Line2Label.FormattedText = formattedString;

                // Type Line1
                await TypeTextToLabelAsync(Line1Label, line1Text, 45);

                // Small delay before second line
                await Task.Delay(300);

                // Type Line2 parts sequentially
                await TypeTextToSpanAsync(line2PrefixSpan, line2Prefix, 50);
                await TypeTextToSpanAsync(line2SpecialSpan, line2Special, 80);
                await TypeTextToSpanAsync(line2SuffixSpan, line2Suffix, 50);

                // Show start button after animation completes
                await ShowStartButtonAsync();
            }
            catch (Exception ex)
            {
                // Handle any errors gracefully
                System.Diagnostics.Debug.WriteLine($"Error in typewriter effect: {ex.Message}");
                // Fallback: show all text immediately
                Line1Label.Text = line1Text;
                line2PrefixSpan.Text = line2Prefix;
                line2SpecialSpan.Text = line2Special;
                line2SuffixSpan.Text = line2Suffix;
                await ShowStartButtonAsync();
            }
        }

        private async Task TypeTextToLabelAsync(Label label, string fullText, int delayMs = 50)
        {
            for (int i = 1; i <= fullText.Length; i++)
            {
                string currentText = fullText.Substring(0, i);

                if (MainThread.IsMainThread)
                {
                    label.Text = currentText;
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => label.Text = currentText);
                }

                await Task.Delay(delayMs);
            }
        }

        private async Task TypeTextToSpanAsync(Span span, string fullText, int delayMs = 50)
        {
            for (int i = 1; i <= fullText.Length; i++)
            {
                string currentText = fullText.Substring(0, i);

                if (MainThread.IsMainThread)
                {
                    span.Text = currentText;
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() => span.Text = currentText);
                }

                await Task.Delay(delayMs);
            }
        }

        private async Task ShowStartButtonAsync()
        {
            if (MainThread.IsMainThread)
            {
                await AnimateStartButton();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await AnimateStartButton());
            }
        }

        private async Task AnimateStartButton()
        {
            StartButton.IsVisible = true;
            StartButton.Opacity = 0;
            StartButton.Scale = 0.8;

            await Task.WhenAll(
                StartButton.FadeTo(1, 500),
                StartButton.ScaleTo(1, 500, Easing.BounceOut)
            );
        }

        private async void OnStartClicked(object sender, EventArgs e)
        {
            try
            {
                // Button press animation
                await StartButton.ScaleTo(0.95, 100);
                await StartButton.ScaleTo(1, 100);

                // Navigate to MainPage
                await Navigation.PushAsync(new MainPage());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                // You might want to show an alert to the user
                await DisplayAlert("Error", "Navigation failed. Please try again.", "OK");
            }
        }
    }
}