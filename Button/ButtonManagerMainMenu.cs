using SFML.System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using k;
using static SFML.Window.Mouse;

public class ButtonManagerMainMenu
{
    public List<ButtonText> Buttons { get; set; }
    public int SelectedPlayers { get; set; } = 0;

    private bool viewHistoryClicked = false;
    private bool howToPlayClicked = false;
    private RectangleShape background;

    public bool IsViewHistoryClickedAndReset()
    {
        if (viewHistoryClicked)
        {
            viewHistoryClicked = false;
            return true;
        }
        return false;
    }

    public bool IsHowToPlayClickedAndReset()
    {
        if (howToPlayClicked)
        {
            howToPlayClicked = false; 
            return true;
        }
        return false;
    }


    

    public ButtonManagerMainMenu(Font font, uint screenWidth, uint screenHeight)
    {
        Buttons = new List<ButtonText>();
        string[] labels = { "2 Players", "3 Players", "4 Players", "View history", "How to play" };

        float width = 250, height = 70, spacing = 25;
        float startY = (screenHeight - (height * labels.Length + spacing * (labels.Length - 1))) / 2;


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

        background = new RectangleShape(new Vector2f(screenWidth, screenHeight))
        {
            FillColor = new Color(40, 40, 40)
        };
    }

    public bool Update(Vector2i mousePosition, bool isClicked)
    {
        bool anyButtonWasActivatedThisFrame = false;

        if (isClicked)
        {
            viewHistoryClicked = false;
            howToPlayClicked = false;
            SelectedPlayers = 0; 
        }

        foreach (var button in Buttons)
        {
            button.Update(mousePosition, isClicked);

            if (button.IsSelected && isClicked)
            {
                anyButtonWasActivatedThisFrame = true;

                if (button.GetText() == "How to play")
                {
                    howToPlayClicked = true;
                }
                else if (button.GetText() == "View history")
                {
                    viewHistoryClicked = true;
                }
                else if (button.GetText().Contains("Players"))
                {
                    if (int.TryParse(button.GetText().Split(' ')[0], out int numPlayers))
                    {
                        SelectedPlayers = numPlayers;
                    }
                }
                foreach (var otherButton in Buttons)
                {
                    if (otherButton != button)
                    {
                        otherButton.Deselect();
                    }
                }
            }
            else if (!button.IsHovered)
            {
                button.Deselect();
            }
        }

        return anyButtonWasActivatedThisFrame;
    }

    public void Draw(RenderWindow window)
    {
        window.Draw(background);
        foreach (var button in Buttons)
            button.Draw(window);
    }
}