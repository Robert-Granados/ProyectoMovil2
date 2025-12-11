using System;
using System.ComponentModel;
using VideoJuego.ViewModels;
using ViewExtensions = Microsoft.Maui.Controls.ViewExtensions;

namespace VideoJuego.Views;

public partial class BattlePage : ContentPage
{
    private readonly BattleViewModel _viewModel;
    private double _lastPlayer1Health;
    private double _lastPlayer2Health;
    private bool _animationsReady;

    public BattlePage(BattleViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _animationsReady = false;
        await _viewModel.LoadAsync();
        _lastPlayer1Health = _viewModel.Player1Health;
        _lastPlayer2Health = _viewModel.Player2Health;
        _animationsReady = true;
    }

    protected override void OnDisappearing()
    {
        _animationsReady = false;
        base.OnDisappearing();
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!_animationsReady)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(BattleViewModel.Player1Health):
                await HandleHealthChangeAsync(Player1Image, _lastPlayer1Health, _viewModel.Player1Health, v => _lastPlayer1Health = v);
                break;
            case nameof(BattleViewModel.Player2Health):
                await HandleHealthChangeAsync(Player2Image, _lastPlayer2Health, _viewModel.Player2Health, v => _lastPlayer2Health = v);
                break;
        }
    }

    private async Task HandleHealthChangeAsync(Image targetImage, double previousValue, double newValue, Action<double> updatePrevious)
    {
        var delta = newValue - previousValue;
        updatePrevious(newValue);

        if (Math.Abs(delta) < 0.01)
        {
            return;
        }

        ViewExtensions.CancelAnimations(targetImage);

        if (delta < 0)
        {
            await AnimateDamageAsync(targetImage);
        }
        else
        {
            await AnimateHealAsync(targetImage);
        }
    }

    // Blink/quick shake to indicate received damage on the avatar.
    private async Task AnimateDamageAsync(Image targetImage)
    {
        await targetImage.FadeTo(0.4, 80);
        await targetImage.FadeTo(1.0, 80);
        await targetImage.TranslateTo(-6, 0, 60);
        await targetImage.TranslateTo(0, 0, 60);
    }

    // Brief scale bounce to signal a healing effect on the avatar.
    private async Task AnimateHealAsync(Image targetImage)
    {
        await targetImage.ScaleTo(1.08, 100);
        await targetImage.ScaleTo(1.0, 120);
    }
}
