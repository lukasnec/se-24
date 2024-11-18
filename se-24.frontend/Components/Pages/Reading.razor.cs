using Microsoft.AspNetCore.Components;
using src.Games.ReadingGame;
using se_24.shared.src.Games.ReadingGame;
using System.Text.Json;

namespace Components.Pages
{
    public partial class Reading
    {
        private List<ReadingLevel> readingLevels = [];
        public List<ReadingQuestion> questions { get; set; } = [];
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
            readingLevels = await GetReadingLevels(level); //_levelLoader.LoadAllLevels<ReadingLevel>("wwwroot/Levels/ReadingGame");
            ReadingLevel selectedLevel = readingLevels.FirstOrDefault(readingLevel => readingLevel.Level == level);
            if (selectedLevel != null)
            {
                readingTime = selectedLevel.ReadingTime;
                text = selectedLevel.Text;
                questions = selectedLevel.Questions;
                numberOfQuestions = questions.Count;
            }
        }

        public async Task<List<ReadingLevel>> GetReadingLevels(int level)
        {
            string url = $"https://localhost:7077/api/ReadingLevels/{level}";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<ReadingLevel>>(jsonResponse, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching levels: {ex.Message}");
                throw;
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
            if (currentQuestion > 0 && currentQuestion <= questions.Count)
            {
                var current = questions[currentQuestion - 1];

                if (current != null && current.Answers != null && current.Answers.Length >= 4)
                {
                    question = current.Question;
                    answer1 = current.Answers[0];
                    answer2 = current.Answers[1];
                    answer3 = current.Answers[2];
                    answer4 = current.Answers[3];
                }
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

