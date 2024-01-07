using System.Collections;
using UnityEngine;

public class Level1Script : MonoBehaviour
{
    Camera playerCamera;
    FirstPersonCamera firstPersonCamera;
    Camera cutsceneCamera;
    GameObject gunView;
    Moleman moleman;
    Player player;
    RailFollower cartObject;
    Obstacle molemanObstacle;

    void Start()
    {
        playerCamera = GameObject.Find("Player Camera").GetComponent<Camera>();
        firstPersonCamera = FindFirstObjectByType<FirstPersonCamera>();
        cutsceneCamera = GameObject.Find("CutsceneCamera").GetComponent<Camera>();
        gunView = GameObject.Find("Gun View");
        moleman = FindObjectOfType<Moleman>();
        player = FindObjectOfType<Player>();
        cartObject = player.transform.GetChild(0).GetComponent<RailFollower>();
        // molemanObstacle = GameObject.Find("Obstacle_Moleman").GetComponent<Obstacle>();

        cutsceneCamera.gameObject.SetActive(false);
        cartObject.ReachedEndOfTrack += TransitionToNextLevel;
    }

    public void CutsceneStart()
    {
        firstPersonCamera.DisableInput();
        Global.inputActions.Disable();
        StartCoroutine(TweenCamera());
    }

    public void CutscenePart2()
    {
        if (molemanObstacle)
        {
            cartObject.OnRemoveObstacle();
        }
    }

    public void CutsceneEnd()
    {
        gunView.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);
        firstPersonCamera.EnableInput();
        Global.inputActions.Enable();
        // Global.inputActions.gameplay.RemoveObstacle.Disable(); // disable removing the obstacle in the boss fight
        moleman.animator.Play("Idle");
    }

    private IEnumerator TweenCamera()
    {
        Vector3 lookHeight = new(0, 2.25f, 0);
        float duration = 0.5f;
        Quaternion initial = firstPersonCamera.transform.rotation;
        Quaternion target = Quaternion.LookRotation(moleman.transform.position + lookHeight - firstPersonCamera.transform.position);
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            firstPersonCamera.transform.rotation = Quaternion.Lerp(initial, target, time / duration);
            yield return null;
        }

        // switch to cutscene camera
        gunView.SetActive(false);
        // cutsceneCamera.transform.parent.SetPositionAndRotation(playerCamera.transform.position, playerCamera.transform.rotation);
        cutsceneCamera.gameObject.SetActive(true);
    }

    private void TransitionToNextLevel()
    {
        TransitionSceneScript.nextScene = "Scenes/Level_2/Level 2";
        SceneTransition.Fade("Scenes/Transition1to2");
    }

}

