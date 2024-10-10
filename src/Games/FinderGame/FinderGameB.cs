using Microsoft.AspNetCore.Components;


namespace src.Games.FinderGame
{
    public class FinderGameB
    {

        private readonly NavigationManager _navigationManager;
        private readonly LevelLoader _levelLoader = new();
        private GameState _gameState = GameState.Waiting;

        private int objectsFound = 0;

        private List<Level> levels = [];

        private int currentLevelIndex;

        private static System.Timers.Timer timer;
        private int defaultTime = 20; // Time given for a level
        private int counter = 10;

        public FinderGameB(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            levels = _levelLoader.LoadAllLevels("wwwroot/Levels/FinderGame");
            currentLevelIndex = 0;
            defaultTime = levels[currentLevelIndex].GivenTime;
        }

        public void StartGame()
        {
            _gameState = GameState.Started;
            counter = defaultTime;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += CountDownTimer;
            timer.Enabled = true;
        }

        public Level GetCurrentLevel()
        {
            return levels[currentLevelIndex];
        }

        public void CountDownTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            if (counter > 0 && !CheckIfAllObjectsFound())
            {
                counter -= 1;
            }
            else
            {
                if (counter == 0 && !CheckIfAllObjectsFound())
                {
                    _gameState = GameState.Failed;
                }
                timer.Enabled = false;
                timer.Dispose();
            }
        }

        public void ObjectClicked(GameObject obj)
        {
            if (!obj.IsFound && counter > 0)
            {
                obj.IsFound = true;
                objectsFound++;
            }
        }

        public void ReturnToHomePage()
        {
            _navigationManager.NavigateTo("/");
        }

        public void ReloadLevel()
        {
            foreach (var obj in levels[currentLevelIndex].GameObjects)
            {
                obj.IsFound = false;
            }
            _gameState = GameState.Waiting;
            counter = defaultTime;
            objectsFound = 0;
        }

        public void LoadNextLevel()
        {
            if(currentLevelIndex == levels.Count - 1)
            {
                currentLevelIndex = 0;
                foreach(Level level in levels)
                {
                    foreach(GameObject obj in level.GameObjects)
                    {
                        obj.IsFound = false;
                    }
                }
            }
            else
            {
                currentLevelIndex++;
            }
            defaultTime = levels[currentLevelIndex].GivenTime;
            _gameState = GameState.Waiting;
            objectsFound = 0;
        }

        public bool CheckIfAllObjectsFound()
        {
            return objectsFound == levels[currentLevelIndex].GameObjects.Count;
        }

        public GameState GetCurrentGameState()
        {
            return _gameState;
        }

        public int GetCurrentTimer()
        {
            return counter;
        }
    }
}
