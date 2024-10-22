using Microsoft.AspNetCore.Components;
using System.Text.Json;
using src.Games.ReadingGame;

namespace Components.Pages
{
    public partial class Reading
    {
        public ReadingQuestion[] questions { get; set; } = new ReadingQuestion[100];
        public Action? OnUIUpdate { get; set; }
        public int level = 1;
        public int taskTimer = 60;
        public bool isStartScreen = true;
        public bool isReadingScreen = false;
        public bool isQuestionsScreen = false;
        public bool isEndScreen = false;
        public bool isNextButtonEnabled = false;
        public bool isEndButtonEnabled = false;

        public int currentQuestion = 1;
        public int numberOfQuestions = 5;
        public bool isButtonsDisabled = true;

        public string question = "";
        public string text = "";
        public string answer1 = "";
        public string answer2 = "";
        public string answer3 = "";
        public string answer4 = "";

        public int readingTime = 60;
        public double percentage = 0;
        public int score = 0;
        public string correct = "";

        [Parameter]
        public int Level { get; set; } = 1;

        // Function to initialize the component
        protected override async Task OnInitializedAsync()
        {
            level = Level;
            OnUIUpdate = StateHasChanged;
            await LoadQuestionsAsync(level);
        }

        // Function to load questions from JSON file
        public async Task LoadQuestionsAsync(int levelToLoad, string filePath = "questions.json")

        {
            if (File.Exists(filePath))
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    JsonDocument jsonDocument = await JsonDocument.ParseAsync(fs);

                    var levelsNode = jsonDocument.RootElement.GetProperty("Levels");

                    if (levelsNode.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var levelElement in levelsNode.EnumerateArray())
                        {
                            if (levelElement.GetProperty("Level").GetInt32() == levelToLoad)
                            {
                                readingTime = levelElement.GetProperty("ReadingTime").GetInt32();
                                text = levelElement.GetProperty("Text").GetString();

                                var questionsNode = levelElement.GetProperty("Questions");
                                questions = JsonSerializer.Deserialize<ReadingQuestion[]>(questionsNode.GetRawText());

                                if (questions != null)
                                {
                                    numberOfQuestions = questions.Length;
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        // Function to start the reading level
        public async Task OnStartClick()
        {
            isStartScreen = false;
            isReadingScreen = true;
            await StartTimer(readingTime);
        }

        // Function to start the timer
        public async Task StartTimer(int readingTime = 60)
        {
            taskTimer = readingTime;

            while (taskTimer > 0)
            {
                if (!isReadingScreen)
                    break;
                taskTimer--;
                OnUIUpdate?.Invoke();
                await Task.Delay(1000);
            }

            if (isReadingScreen)
                OnReadingFinished();
        }

        // Function to finish reading
        public void OnReadingFinished()
        {
            PrepareQuestion();
            isReadingScreen = false;
            isQuestionsScreen = true;
            isButtonsDisabled = false;
        }

        // Function to handle answer click
        public void AnswerClick(int answerNumber)
        {
            if (answerNumber == questions[currentQuestion - 1].CorrectAnswer)
            {
                score++;
                correct = "Correct!";
            }
            else
            {
                correct = "Incorrect!";
            }

            if (currentQuestion >= numberOfQuestions)
            {
                isEndButtonEnabled = true;
            }
            else
            {
                isNextButtonEnabled = true;
            }
            isButtonsDisabled = true;
        }

        // Function to move to the next question
        public void OnNextQuestion()
        {
            currentQuestion++;
            PrepareQuestion();
            isNextButtonEnabled = false;
            isButtonsDisabled = false;
        }

        // Function to prepare the question
        public void PrepareQuestion()
        {
            //LINQ usage to iterate through the array
            var current = questions.ElementAtOrDefault(currentQuestion - 1);
            if (current != null)
            {
                question = current.Question;
                answer1 = current.Answers?.ElementAtOrDefault(0) ?? "Default 1";
                answer2 = current.Answers?.ElementAtOrDefault(1) ?? "Default 2";
                answer3 = current.Answers?.ElementAtOrDefault(2) ?? "Default 3";
                answer4 = current.Answers?.ElementAtOrDefault(3) ?? "Default 4";
            }

        }

        // Function to end the level
        public void OnEndLevel()
        {
            percentage = Math.Round(questions.GetCorrectPercentage(score),2);
            isQuestionsScreen = false;
            isEndScreen = true;
        }

        // Function to restart the game
        public void OnRestartClick()
        {
            isEndScreen = false;
            isStartScreen = true;
            isEndButtonEnabled = false;
            score = 0;
            taskTimer = readingTime;
            currentQuestion = 1;
        }
    }
}

