using k;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

public class ButtonManagerChooseGame
{
    public List<ButtonImage> Buttons { get; private set; }
    public int SelectedGame { get; private set; } = -1;

    public ButtonManagerChooseGame(Texture[] images, uint screenWidth, uint screenHeight)
    {
        Buttons = new List<ButtonImage>();
        float width = 100, height = 100, spacing = 20;
        float totalHeight = (height + spacing) * images.Length - spacing;
        float startY = (screenHeight - totalHeight) / 2;

        for (int i = 0; i < images.Length; i++)
        {
            var button = new ButtonImage(
                new Vector2f((screenWidth - width) / 2, startY + i * (height + spacing)),
                new Vector2f(width, height),
                images[i]
            );
            Buttons.Add(button);
        }
    }

    public bool Update(Vector2i mousePos, bool isClicked)
    {
        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].Update(mousePos, isClicked);
            if (Buttons[i].IsSelected)
            {
                for (int j = 0; j < Buttons.Count; j++)
                    if (j != i) Buttons[j].Deselect();

                SelectedGame = i;
                return true;
            }
        }
        return false;
    }

    public void Draw(RenderWindow window)
    {
        foreach (var button in Buttons)
            button.Draw(window);
    }

    public void ResetSelectedGame()
    {
        SelectedGame = -1;
        foreach (var b in Buttons) b.Deselect();
    }
}
