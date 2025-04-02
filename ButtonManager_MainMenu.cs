using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class ButtonManager_MainMenu
{
    public List<Button_MainMenu> Buttons { get; private set; }
    public int SelectedPlayers { get; private set; } = 0;

    // Додаємо подію для сповіщення про вибір гравців
    public event Action<int> PlayersSelected;

    public ButtonManager_MainMenu(SpriteFont font, Texture2D texture, int screenWidth, int screenHeight)
    {
        Buttons = new List<Button_MainMenu>();

        string[] labels = { "2 Players", "3 Players", "4 Players" };
        int buttonWidth = 200;
        int buttonHeight = 60;
        int spacing = 20;

        int totalHeight = (buttonHeight + spacing) * labels.Length - spacing;
        int startY = (screenHeight - totalHeight) / 2;

        for (int i = 0; i < labels.Length; i++)
        {
            int x = (screenWidth - buttonWidth) / 2;
            int y = startY + i * (buttonHeight + spacing);

            Buttons.Add(new Button_MainMenu(new Rectangle(x, y, buttonWidth, buttonHeight), labels[i], font, texture));
        }
    }

    public void Update(MouseState mouseState, MouseState previousMouseState)
    {
        foreach (var button in Buttons)
        {
            // Передаємо попередній стан миші в метод Update кожної кнопки
            button.Update(mouseState, previousMouseState);

            if (button.IsSelected)
            {
                foreach (var otherButton in Buttons)
                {
                    if (otherButton != button) otherButton.Deselect();
                }

                if (button.Text == "2 Players") SelectedPlayers = 2;
                else if (button.Text == "3 Players") SelectedPlayers = 3;
                else if (button.Text == "4 Players") SelectedPlayers = 4;

                PlayersSelected?.Invoke(SelectedPlayers);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var button in Buttons)
            button.Draw(spriteBatch);
    }
}