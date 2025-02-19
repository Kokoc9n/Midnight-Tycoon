using UnityEngine;

public class LevelSelectButton : ButtonClickHandler
{
    private int levelIndex;
    public void Init(int index) => levelIndex = index;
    public override void OnButtonClicked()
    {
        //if (LevelManager.Instance.IsLevelLocked(levelIndex)) return;
        //LevelManager.Instance.PlayLevel(levelIndex);
        //CanvasManager.SwitchPage(typeof(GameUIPage));
    }
}
