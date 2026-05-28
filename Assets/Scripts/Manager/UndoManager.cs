using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    private readonly Stack<System.Action> undoStack = new();

    public void RegisterUndo(System.Action undoAction)
    {
        undoStack.Push(undoAction);
    }

    public void Undo()
    {
        if (undoStack.Count == 0) return;

        System.Action undoAction = undoStack.Pop();
        undoAction.Invoke();
    }
}