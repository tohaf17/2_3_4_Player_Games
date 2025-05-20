using SFML.System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using k;

public class ButtonManagerMainMenu
{
    public List<ButtonText> Buttons { get; private set; }
    public int SelectedPlayers { get; set; } = 0;
    public bool ViewHistoryClicked { get; set; } = false;

    private RectangleShape background; // New: Background shape

    public ButtonManagerMainMenu(Font font, uint screenWidth, uint screenHeight)
    {
        Buttons = new List<ButtonText>();
        string[] labels = { "2 Players", "3 Players", "4 Players", "View history" };

        float width = 250, height = 70, spacing = 25; // Slightly larger buttons
        float startY = (screenHeight - (height + spacing) * labels.Length + spacing) / 2;

        for (int i = 0; i < labels.Length; i++)
        {
            var button = new ButtonText(
                new Vector2f((screenWidth - width) / 2, startY + i * (height + spacing)),
                new Vector2f(width, height),
                labels[i],
                font
            );
            Buttons.Add(button);
        }

        // Initialize the background
        background = new RectangleShape(new Vector2f(screenWidth, screenHeight))
        {
            FillColor = new Color(40, 40, 40) // Darker background color (e.g., dark gray)
        };
    }

    public bool Update(Vector2i mousePos, bool isClicked)
    {
        bool selectionMade = false;
        ViewHistoryClicked = false;

        for (int i = 0; i < Buttons.Count; i++)
        {
            Buttons[i].Update(mousePos, isClicked);
            if (Buttons[i].IsSelected)
            {
                if (i < 3)
                {
                    foreach (var b in Buttons)
                        if (b != Buttons[i]) b.Deselect();

                    SelectedPlayers = 2 + i;
                    selectionMade = true;
                }
                else if (i == 3)
                {
                    ViewHistoryClicked = true;
                    foreach (var b in Buttons)
                        if (b != Buttons[i]) b.Deselect();
                    selectionMade = true;
                }
            }
        }
        return selectionMade;
    }

    public void Draw(RenderWindow window)
    {
        window.Draw(background); // Draw the background first
        foreach (var button in Buttons)
            button.Draw(window);
    }
}