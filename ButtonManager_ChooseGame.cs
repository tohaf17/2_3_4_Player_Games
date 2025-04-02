using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class ButtonManager_ChooseGame
{
    public List<Button_ChooseGame> Buttons { get; private set; }
    public int SelectedGame { get; set; } = -1;

    // Додаємо подію для сповіщення про вибір гри
    public event Action<int> GameSelected;

    public ButtonManager_ChooseGame(Texture2D buttonTexture, Texture2D[] imageTextures, int screenWidth, int screenHeight)
    {
        Buttons = new List<Button_ChooseGame>();

        int buttonWidth = 100;
        int buttonHeight = 100;
        int spacing = 20;

        int totalHeight = (buttonHeight + spacing) * imageTextures.Length - spacing;
        int startY = (screenHeight - totalHeight) / 2;

        for (int i = 0; i < imageTextures.Length; i++)
        {
            int x = (screenWidth - buttonWidth) / 2;
            int y = startY + i * (buttonHeight + spacing);

            Buttons.Add(new Button_ChooseGame(new Vector2(x, y), buttonTexture, imageTextures[i]));
        }
    }

    public void Update(MouseState mouseState, MouseState previousMouseState)
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            // Передаємо попередній стан миші в метод Update кожної кнопки
            Buttons[i].Update(mouseState, previousMouseState);

            if (Buttons[i].IsSelected)
            {
                for (int j = 0; j < Buttons.Count; j++)
                {
                    if (j != i) Buttons[j].Deselect();
                }

                SelectedGame = i;
                GameSelected?.Invoke(SelectedGame);
            }
        }
    }
    public void ResetSelectedGame()
    {
        SelectedGame = -1;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var button in Buttons)
            button.Draw(spriteBatch);
    }
}