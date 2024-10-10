using Microsoft.AspNetCore.Components;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace se_24.src.Games.ReadingGame
{
    public class ReadingB
    {
        public QuestionObject[] questions { get; set; } = new QuestionObject[100];
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

        public int readingTime { get; set; } = 60;
        public int score = 0;
        public string correct = "";


        // Override OnInitializedAsync to load questions
        public async Task OnInitializedAsync(int levelToLoad)
        {
            level = levelToLoad;
            await LoadQuestionsAsync(levelToLoad);
        }

        // Function to load questions from JSON file
        public async Task LoadQuestionsAsync(int levelToLoad)
        {
            var filePath = "questions.json";
            if (File.Exists(filePath))
            {
                string jsonString = await File.ReadAllTextAsync(filePath);

                var levelData = JsonSerializer.Deserialize<LevelObject>(jsonString);

                if (levelData != null && levelData.level == levelToLoad)
                {
                    text = levelData.text;
                    readingTime = levelData.readingTime;
                    questions = levelData.questions;
                    numberOfQuestions = questions.Length;
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
        public async Task StartTimer(int readingTime)
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
            if (answerNumber == questions[currentQuestion - 1].correctAnswer)
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
            if (currentQuestion <= numberOfQuestions)
            {
                question = questions[currentQuestion - 1].question;
                answer1 = questions[currentQuestion - 1].answers[0];
                answer2 = questions[currentQuestion - 1].answers[1];
                answer3 = questions[currentQuestion - 1].answers[2];
                answer4 = questions[currentQuestion - 1].answers[3];
            }
        }

        // Function to end the level
        public void OnEndLevel()
        {
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
