﻿using System;
using TheSyndicate.Actions;

namespace TheSyndicate
{
    public class Scene
    {
        Player player = Player.Instance();
        public string Id { get; private set; }
        public string Text { get; private set; }
        public string[] Options { get; private set; }
        public string[] Destinations { get; private set; }
        public string ActualDestinationId { get; private set; }
        public bool Start { get; private set; }
        public IAction Action { get; set; }

        public Scene(string id, string text, string[] options, string[] destinations, bool start)
        {
            this.Id = id;
            this.Text = text;
            this.Options = options;
            this.Destinations = destinations;
            this.ActualDestinationId = null;
            this.Start = start;
        }
        
        public void Play()
        {
            TextBox sceneTextBox = RenderText();
            RenderOptions(sceneTextBox);
            GetUserInput(sceneTextBox);
        }

        TextBox RenderText()
        {
            ClearConsole();

            //TextBox is instantiated to pass this.Text and get access to TextBox Width and Height properties 

            TextBox dialogBox = new TextBox(this.Text, Console.WindowWidth * 3 / 4, 2, (Console.WindowWidth - (Console.WindowWidth * 3 / 4)) / 2, 2) ;
            dialogBox.FormatText(this.Text);
            dialogBox.DrawDialogBox(this.Text);

            //returning dialogBox for information about height of dialog box

            return dialogBox; 
        }
        
        void RenderOptions(TextBox sceneTextBox)
        {
            //checks for end scene
            if (this.Options.Length > 0) 
            {
                RenderUserOptions(sceneTextBox);
            }
            else
            {
                RenderQuitMessage(sceneTextBox);
                player.EmptySaveStateJSONfile();
            }
        }

        private void RenderUserOptions(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);

            RenderInstructions(sceneTextBox);

            for (int i = 0; i < this.Options.Length; i++)
            {
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);

                Console.WriteLine($"{i + 1}: {this.Options[i]}");
                sceneTextBox.TextBoxY += 2;
            }
            sceneTextBox.SetBoxPosition(Console.WindowWidth - (Console.WindowWidth / 4), Console.WindowHeight - 2);
            Console.WriteLine($"Press 0 at any point to save");
        }

        private void RenderInstructions(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);

            Console.WriteLine("What will you do next? Enter the number next to the option and press enter:");
        }

        private void RenderQuitMessage(TextBox sceneTextBox)
        {
            sceneTextBox.TextBoxY += 2;
            sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY);
            Console.WriteLine("You have reached the end of your journey. Press CTRL + C to end.");
            Console.ForegroundColor = ConsoleColor.Green;
        }

        void GetUserInput(TextBox sceneTextBox)
        {
            int selectedOption;

            do
            {
                sceneTextBox.SetBoxPosition(sceneTextBox.TextBoxX, sceneTextBox.TextBoxY + 2);
                Int32.TryParse(Console.ReadLine(), out selectedOption);
            }
            while (!IsValidInput(selectedOption));

            if (selectedOption == 0) 
            {
                //func 
                player.SaveIDFunc(this.Id);
                Console.WriteLine("Saved!");
                Console.WriteLine("Continue when you are ready!!!!");
                //GetUserInput();
            } 
            else 
            {
                SetDestinationId(selectedOption);
            }           
        }

        bool IsValidInput(int selectedOption)
        {
            int numberOfOptions = this.Options.Length;
            return selectedOption >= 0 && selectedOption <= numberOfOptions;
        }

        void ClearConsole()
        {
            Console.Clear();
        }

        void SetDestinationId(int selectedOption)
        {
            this.ActualDestinationId = this.Destinations[selectedOption - 1];
            if (this.ActualDestinationId.Equals("fight"))
            {
                this.Action = new FightAction();
                Action.ExecuteAction();
                if (Action.DidPlayerSucceed())
                {
                    this.ActualDestinationId = "recyclerTruck";
                }
                else
                {
                    this.ActualDestinationId = "dead";
                }
            }
            else if (this.Id.Equals("upload") || 
                (this.Id.Equals("recyclerTruck") && this.ActualDestinationId.Equals("city")))
            {
                this.Action = new KeyPressAction();
                Action.ExecuteAction();
                if (!Action.DidPlayerSucceed())
                {
                    this.ActualDestinationId = "dead";
                }
            }
        }
        
        public bool HasNextScenes()
        {
            return Destinations.Length > 0;
        }
    }
}