﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using UIInfoSuite.Infrastructure;
using UIInfoSuite.Infrastructure.Extensions;

namespace UIInfoSuite.UIElements
{
    class ShowSeasonalBerry : IDisposable
    {
        #region Properties

        Rectangle? _berrySpriteLocation;
        private float _spriteScale = 8 / 3f;
        private string _hoverText;
        private ClickableTextureComponent _berryIcon;

        private readonly IModHelper _helper;
        public bool ShowHazelnut { get; set; }

        #endregion

        #region Lifecycle

        public ShowSeasonalBerry(IModHelper helper)
        {
            _helper = helper;
        }

        public void Dispose()
        {
            ToggleOption(false);
        }

        public void ToggleOption(bool showSeasonalBerry)
        {
            _berrySpriteLocation = null;
            _helper.Events.GameLoop.DayStarted -= OnDayStarted;
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Display.RenderedHud -= OnRenderedHud;

            if (showSeasonalBerry)
            {
                UpdateBerryForDay();

                _helper.Events.GameLoop.DayStarted += OnDayStarted;
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Display.RenderedHud += OnRenderedHud;
            }
        }

        public void ToggleHazelnutOption(bool showHazelnut)
        {
            ShowHazelnut = showHazelnut;

            UpdateBerryForDay();
        }

        #endregion

        #region Event subscriptions

        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UpdateBerryForDay();
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            // Draw icon
            if (Game1.eventUp || !_berrySpriteLocation.HasValue) return;

            var iconPosition = IconHandler.Handler.GetNewIconPosition();
            _berryIcon =
                new ClickableTextureComponent(
                    new Rectangle(iconPosition.X, iconPosition.Y, 40, 40),
                    Game1.objectSpriteSheet,
                    _berrySpriteLocation.Value,
                    _spriteScale
                );
            _berryIcon.draw(Game1.spriteBatch);
        }

        private void OnRenderedHud(object sender, RenderedHudEventArgs e)
        {
            // Show text on hover
            var hasMouse = _berryIcon?.containsPoint(Game1.getMouseX(), Game1.getMouseY()) ?? false;
            var hasText = !string.IsNullOrEmpty(_hoverText);
            if (_berrySpriteLocation.HasValue && hasMouse && hasText)
            {
                IClickableMenu.drawHoverText(
                    Game1.spriteBatch,
                    _hoverText,
                    Game1.dialogueFont
                );
            }
        }

        #endregion

        #region Logic

        private void UpdateBerryForDay()
        {
            var season = Game1.currentSeason;
            var day = Game1.dayOfMonth;
            switch (season)
            {
                case "spring" when day is >= 15 and <= 18:
                    _berrySpriteLocation = new Rectangle(128, 193, 15, 15);
                    _hoverText = _helper.SafeGetString(LanguageKeys.CanFindSalmonberry);
                    _spriteScale = 8 / 3f;
                    break;
                case "fall" when day is >= 8 and <= 11:
                    _berrySpriteLocation = new Rectangle(32, 272, 16, 16);
                    _hoverText = _helper.SafeGetString(LanguageKeys.CanFindBlackberry);
                    _spriteScale = 5 / 2f;
                    break;
                case "fall" when day >= 15 && ShowHazelnut:
                    _berrySpriteLocation = new Rectangle(1, 274, 14, 14);
                    _hoverText = _helper.SafeGetString(LanguageKeys.CanFindHazelnut);
                    _spriteScale = 20 / 7f;
                    break;
                default:
                    _berrySpriteLocation = null;
                    break;
            }
        }

        #endregion
    }
}