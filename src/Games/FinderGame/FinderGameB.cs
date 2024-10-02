using Microsoft.AspNetCore.Components;


namespace src.Games.FinderGame
{
    public class FinderGameB
    {

        private readonly NavigationManager _navigationManager;
        private GameState _gameState = GameState.waiting;

        private int objectsFound = 0;

        private List<Level> levels = new()
        {
            new Level { id = 0, image = "images/low-res-jars-background.jpg", givenTime = 60, gameObjects = new()
            {
                new GameObject { name = "Kitchen Knife", image = "images/pixelated-knife.png", positionX = 17, positionY = 15 },
                new GameObject { name = "Santa Claus", image = "images/pixelated-santa.png", positionX = 40, positionY = 30 },
                new GameObject { name = "Kerosene Lamp", image = "images/pixelated-lamp.png", positionX = 50, positionY = 10 }
            }},
            new Level { id = 1, image = "images/low-res-jars-background.jpg", givenTime = 40, gameObjects = new()
            {
                new GameObject { name = "Kitchen Knife", image = "images/pixelated-knife.png", positionX = 40, positionY = 60 },
                new GameObject { name = "Santa Claus", image = "images/pixelated-santa.png", positionX = 50, positionY = 50 },
                new GameObject { name = "Kerosene Lamp", image = "images/pixelated-lamp.png", positionX = 50, positionY = 10 }
            }}
        };

        private int currentLevelIndex;

        private static System.Timers.Timer timer;
        private int defaultTime = 20; // Time given for a level
        private int counter = 10;

        public FinderGameB(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            currentLevelIndex = 0;
            defaultTime = levels[currentLevelIndex].givenTime;
        }

        public void StartGame()
        {
            _gameState = GameState.started;
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
                    _gameState = GameState.failed;
                }
                timer.Enabled = false;
                timer.Dispose();
            }
        }

        public void ObjectClicked(GameObject obj)
        {
            if (!obj.isFound && counter > 0)
            {
                obj.isFound = true;
                objectsFound++;
            }
        }

        public void ReturnToHomePage()
        {
            _navigationManager.NavigateTo("/");
        }

        public void ReloadLevel()
        {
            foreach (var obj in levels[currentLevelIndex].gameObjects)
            {
                obj.isFound = false;
            }
            _gameState = GameState.waiting;
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
                    foreach(GameObject obj in level.gameObjects)
                    {
                        obj.isFound = false;
                    }
                }
            }
            else
            {
                currentLevelIndex++;
            }
            defaultTime = levels[currentLevelIndex].givenTime;
            _gameState = GameState.waiting;
            objectsFound = 0;
        }

        public bool CheckIfAllObjectsFound()
        {
            return objectsFound == levels[currentLevelIndex].gameObjects.Count;
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
