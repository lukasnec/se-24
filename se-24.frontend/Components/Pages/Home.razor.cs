namespace se_24.frontend.Components.Pages;

public partial class Home
{
    public int FinderLevels { get; set; } = 0;
    public int ReadingLevels { get; set; } = 0;
    public bool isLoading = true;
    public bool errorHappened = false;
    public string errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        isLoading = true;
        try
        {
            HttpResponseMessage finderResponse = await Http.GetAsync("FinderLevels/count");
            string finderRaw = await finderResponse.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(finderRaw))
            {
                FinderLevels = int.Parse(finderRaw);
            }
            else
            {
                throw new Exception("Finder Levels API returned an empty response.");
            }

            HttpResponseMessage readingResponse = await Http.GetAsync("ReadingLevels/count");
            string readingRaw = await readingResponse.Content.ReadAsStringAsync();

            if (!string.IsNullOrWhiteSpace(readingRaw))
            {
                ReadingLevels = int.Parse(readingRaw);
            }
            else
            {
                throw new Exception("Reading Levels API returned an empty response.");
            }

            isLoading = false;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error fetching levels: {ex.Message}");
            errorHappened = true;
            errorMessage = $"Failed to load levels: {ex.Message}";
            isLoading = false;
        }
    }
}
