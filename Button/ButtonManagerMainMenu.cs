using SFML.System;
using SFML.Window;
using SFML.Graphics;
using System.Collections.Generic;
using k;
using static SFML.Window.Mouse;

public class ButtonManagerMainMenu
{
    public List<ButtonText> Buttons { get; private set; }
    public int SelectedPlayers { get; set; } = 0;

    // Зробимо ці прапорці приватними і додамо public методи для їх перевірки,
    // щоб краще контролювати їх скидання
    private bool _viewHistoryClicked = false;
    private bool _howToPlayClicked = false;

    public bool IsViewHistoryClickedAndReset()
    {
        if (_viewHistoryClicked)
        {
            _viewHistoryClicked = false; // Скидаємо одразу після перевірки
            return true;
        }
        return false;
    }

    public bool IsHowToPlayClickedAndReset()
    {
        if (_howToPlayClicked)
        {
            _howToPlayClicked = false; // Скидаємо одразу після перевірки
            return true;
        }
        return false;
    }


    private RectangleShape background;

    public ButtonManagerMainMenu(Font font, uint screenWidth, uint screenHeight)
    {
        Buttons = new List<ButtonText>();
        string[] labels = { "2 Players", "3 Players", "4 Players", "View history", "How to play" };

        float width = 250, height = 70, spacing = 25;
        // Corrected startY calculation to use screenHeight
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

        // При кліку миші (isClicked == true), ми повинні скинути всі прапорці та SelectedPlayers
        // до того, як встановити новий, щоб забезпечити взаємне виключення.
        if (isClicked)
        {
            _viewHistoryClicked = false;
            _howToPlayClicked = false;
            SelectedPlayers = 0; // Скидаємо кількість гравців, щоб запобігти випадковим запускам
                                 // якщо інша кнопка була обрана раніше.
        }

        foreach (var button in Buttons)
        {
            // Оновлюємо стан кнопки (наведення, вибір)
            button.Update(mousePosition, isClicked);

            // Якщо ця кнопка щойно була натиснута
            if (button.IsSelected && isClicked) // Перевіряємо IsSelected І isClicked
            {
                anyButtonWasActivatedThisFrame = true;

                // Обробка специфічних дій кнопок та взаємного виключення
                if (button.GetText() == "How to play")
                {
                    _howToPlayClicked = true;
                }
                else if (button.GetText() == "View history")
                {
                    _viewHistoryClicked = true;
                }
                else if (button.GetText().Contains("Players"))
                {
                    if (int.TryParse(button.GetText().Split(' ')[0], out int numPlayers))
                    {
                        SelectedPlayers = numPlayers;
                    }
                }

                // Після того, як ми визначили, яка кнопка була натиснута,
                // ми деселектуємо ВСІ кнопки, крім поточної.
                // Це дуже важливо, щоб кнопки не "залипали" візуально.
                foreach (var otherButton in Buttons)
                {
                    if (otherButton != button)
                    {
                        otherButton.Deselect();
                    }
                }
            }
            // Якщо кнопка не вибрана і на неї не наведено курсор, переконайтеся, що вона деселектована.
            // Це ловить випадки, коли миша відводиться або натискається інша кнопка.
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