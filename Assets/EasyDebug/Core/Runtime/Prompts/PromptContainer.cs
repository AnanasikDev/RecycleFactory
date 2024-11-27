using System.Collections.Generic;
using UnityEngine;

public class PromptContainer
{
    private GameObject _gameobject;
    private Transform _promptsHandler;
    private Dictionary<string, TextPrompt> _prompts;
    private List<TextPrompt> _sortedPrompts;

    public PromptContainer(GameObject gameobject)
    {
        _sortedPrompts = new List<TextPrompt>();
        _prompts = new Dictionary<string, TextPrompt>();

        _gameobject = gameobject;
        _promptsHandler = new GameObject("TextPrompts").transform;
        _promptsHandler.SetParent(_gameobject.transform);
        _promptsHandler.localPosition = TextPromptManager.StartLocalOffset;
    }

    public void UpdatePrompt(string key, string value, int priority)
    {
        if (_prompts.TryGetValue(key, out var prompt))
        {
            prompt.UpdateValue(value, priority);
        }
        else
        {
            var newPrompt = new TextPrompt(key, value, priority, _promptsHandler);
            _prompts[key] = newPrompt;
            _sortedPrompts.Add(newPrompt);
            SortPrompts();
        }

        UpdatePromptPositions();
    }

    private void SortPrompts()
    {
        _sortedPrompts.Sort((a, b) => b.Priority.CompareTo(a.Priority));
    }

    private void UpdatePromptPositions()
    {
        for (int i = 0; i < _sortedPrompts.Count; i++)
        {
            _sortedPrompts[i].SetLocalPosition(Vector3.up * i * TextPromptManager.PromptDistance);
        }
    }
}
