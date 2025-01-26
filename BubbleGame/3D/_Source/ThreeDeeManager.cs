using BubbleGame._3D;
using BubbleGame.Common.SceneManagement;
using Godot;
using System;

public partial class ThreeDeeManager : Node
{

    [Export] private SpawnManager _spawnManager;

    private float _gameTimeElapsed = 0;
    private float _timeLimitForGame = -1;

    public override void _Ready()
    {
        _gameTimeElapsed = 0;

        Godot.Collections.Array timeStamps = SceneManager.Instance.GetTimeStampsCachedData();
        if (timeStamps != null && timeStamps.Count == 0)
        {
            _timeLimitForGame = SceneManager.Instance.GetTimeElapsedCachedData();
        }

        if (_spawnManager == null)
        {
            return;
        }

        _spawnManager.NoMoreBugsLeftHandler += OnFinishGame;
    }

    public override void _ExitTree()
    {
        if (_spawnManager != null)
        {
            _spawnManager.NoMoreBugsLeftHandler -= OnFinishGame;
        }
    }

    public override void _Process(double delta)
    {
        if (_timeLimitForGame != -1)
        {
            _gameTimeElapsed += (float)delta;

            if (_gameTimeElapsed > _timeLimitForGame)
            {
                OnFinishGame();
            }
        }
    }

    private void OnFinishGame()
    {
        SceneManager.Instance.LoadScene("WelcomeScene");
    }
}
