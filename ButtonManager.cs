using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class ButtonManager
{
    public List<Button> Buttons { get; private set; }
    public int SelectedPlayers { get; private set; } = 0; // Кількість вибраних гравців

    public ButtonManager(SpriteFont font, Texture2D texture, int screenWidth, int screenHeight)
    {
        Buttons = new List<Button>();

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

            Buttons.Add(new Button(new Rectangle(x, y, buttonWidth, buttonHeight), labels[i], font, texture));
        }
    }

    public void Update(MouseState mouseState)
    {
        foreach (var button in Buttons)
        {
            button.Update(mouseState);

            if (button.IsSelected)
            {
                // Скидаємо вибір у всіх кнопках, крім обраної
                foreach (var otherButton in Buttons)
                {
                    if (otherButton != button) otherButton.Deselect();
                }

                // Записуємо вибір у змінну
                if (button.Text == "2 Players") SelectedPlayers = 2;
                else if (button.Text == "3 Players") SelectedPlayers = 3;
                else if (button.Text == "4 Players") SelectedPlayers = 4;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var button in Buttons)
            button.Draw(spriteBatch);
    }
}