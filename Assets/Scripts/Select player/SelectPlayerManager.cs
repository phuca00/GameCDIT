using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPlayerManager : MonoBehaviour
{
    public Transform selectFrame;          // object Select di chuyển
    public Transform[] playerSlots;        // danh sách vị trí các icon
    private int currentIndex = 0;

    void Update()
    {
        HandleKeyboard();
        HandleMouse();
    }

    // ---------------------- CLICK MOUSE ------------------------
    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null)
            {
                SelectablePlayer sel = hit.GetComponent<SelectablePlayer>();
                if (sel != null)
                {
                    MoveSelectFrame(sel.index);
                }
            }
        }
    }

    // ---------------------- KEYBOARD ---------------------------
    void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveSelectFrame(Mathf.Min(currentIndex + 1, playerSlots.Length - 1));

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveSelectFrame(Mathf.Max(currentIndex - 1, 0));

        if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveSelectFrame(Mathf.Min(currentIndex + 2, playerSlots.Length - 1));

        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveSelectFrame(Mathf.Max(currentIndex - 2, 0));

        // Nhấn Enter hoặc Space để chọn
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            SelectPlayer();
    }

    // ---------------------- MOVE SELECT FRAME ------------------
    void MoveSelectFrame(int newIndex)
    {
        currentIndex = newIndex;
        selectFrame.position = playerSlots[currentIndex].position;
    }

    // ---------------------- SELECT PLAYER ----------------------
    void SelectPlayer()
    {
        PlayerPrefs.SetInt("SelectedPlayerIndex", currentIndex);
        PlayerPrefs.Save();

        Debug.Log("Chosen Player: " + currentIndex);

        // Load scene chơi game
        SceneManager.LoadScene("level1");
    }
}
