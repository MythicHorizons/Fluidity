using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtils : MonoBehaviour
{
    public Scene NextScene;
    private const string _GameSceneName = "Game";
    private const string _MainMenuSceneName = "MainMenu";
    private const string _SettingsMenuSceneName = "SettingsMenu";
    private Scene _GameScene;
    private Scene _MainMenuScene;
    private Scene _SettingsMenuScene;
    public Scene GameScene
    {
        get
        {
            if (!_GameScene.IsValid())
                _GameScene = SceneManager.GetSceneByName(_GameSceneName);

            return _GameScene;
        }
    }
    public Scene MainMenuScene
    {
        get
        {
            if (!_MainMenuScene.IsValid())
                _MainMenuScene = SceneManager.GetSceneByName(_MainMenuSceneName);

            return _MainMenuScene;
        }
    }
    public Scene SettingsMenuScene
    {
        get
        {
            if (!_SettingsMenuScene.IsValid())
            _SettingsMenuScene = SceneManager.GetSceneByName(_SettingsMenuSceneName);

            return _SettingsMenuScene;
        }
    }

    void Awake()
    {
        if (NextScene == null)
            NextScene = SceneManager.GetSceneByBuildIndex(GetNextSceneBuildIndex());
    }

    public void PlayGame() => LoadScene(_GameSceneName);
    public void OpenMainMenu() => LoadScene(_MainMenuSceneName);
    public void OpenSettingsMenu() => LoadScene(_SettingsMenuSceneName);

    public void ApplicationExit() => Application.Quit(0);

    public int GetNextSceneBuildIndex()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = activeSceneIndex + (SceneManager.sceneCount - activeSceneIndex + 1);

        Debug.Log($"SceneUtils.GetNextSceneIndex() returned: \"{SceneManager.GetSceneAt(nextSceneIndex).name}({nextSceneIndex})\"");
        return nextSceneIndex;
    }

    public void LoadNextScene()
    {
        LoadScene(NextScene.buildIndex);
    }

    private void LoadScene(int buildIndex) => StartCoroutine(LoadSceneAsync(buildIndex, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None)));

    private void LoadScene(string sceneName) => StartCoroutine(LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None)));

    private void LoadScene(int buildIndex, LoadSceneMode loadSceneMode) => StartCoroutine(LoadSceneAsync(buildIndex, new LoadSceneParameters(loadSceneMode, LocalPhysicsMode.None)));

    private void LoadScene(string sceneName, LoadSceneMode loadSceneMode) => StartCoroutine(LoadSceneAsync(sceneName, new LoadSceneParameters(loadSceneMode, LocalPhysicsMode.None)));

    private IEnumerator LoadSceneAsync(int buildIndex, LoadSceneParameters loadSceneParameters)
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        while(!asyncLoadScene.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, LoadSceneParameters loadSceneParameters)
    {
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(sceneName, loadSceneParameters);

        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }
    }
}
