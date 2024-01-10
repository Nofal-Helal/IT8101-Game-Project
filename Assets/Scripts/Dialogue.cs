using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;
using UnityEngine.Splines;
using Object = UnityEngine.Object;

[Serializable]
public struct DialogueItem
{
    public string text;
    public float screenTime;
}

public class Dialogue : MonoBehaviour
{
    [NaughtyAttributes.Required]
    public SplineContainer railTrack;
    [HideInInspector]
    public bool inProgress;
    public float dialogStartDistance = 0f;
    private Spline spline;
    private SplineData<Object> splineData;
    private DataPoint<Object> dataPoint;
    public DialogueItem[] dialogueList = new DialogueItem[0];
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueTextUI;

    private bool attached = false;
    public float Distance => dataPoint.Index;

    private void OnEnable()
    {
        if (railTrack == null)
        {
            Debug.LogError("Dialogue has no reference to a rail track.");
            return;
        }

        if (dialogueBox == null)
        {
            Debug.LogError("There is no dialogue box to display the text.");
            return;
        }

        if (dialogueTextUI == null)
        {
            Debug.LogError("There is no dialogue text object to show the text.");
            return;
        }

        if (dialogueList == null)
        {
            Debug.LogError("Dialogue has no dialogue to display");
            return;
        }

        spline = railTrack.Spline;
        splineData = spline.GetOrCreateObjectData("dialogues");
        dialogueBox.SetActive(false);
        AddDataPoint();
        attached = true;
    }

    // Start is called before the first frame update
    public void Start()
    {
        Debug.Assert(attached, "Dialogue has not attached to a rail track");
    }

    public void OnDestroy()
    {
        _ = splineData.RemoveDataPoint(dataPoint.Index);
    }

    // Update is called once per frame
    // void Update() { }

    public void Nearest(out float3 nearest, out float t)
    {
        float3 point = transform.position - railTrack.transform.position;
        _ = SplineUtility.GetNearestPoint(spline, point, out nearest, out t);
    }

    private void AddDataPoint()
    {
        Nearest(out _, out float t);
        float t_dist = SplineUtility.ConvertIndexUnit(spline, t, PathIndexUnit.Distance);
        t_dist += dialogStartDistance;
        dataPoint = new DataPoint<Object>(t_dist, this);
        _ = splineData.Add(dataPoint);
    }

    private void OnDrawGizmos()
    {
        if (railTrack)
        {
            spline ??= railTrack.Spline;
            Nearest(out _, out float t);
            float t_dist = spline.ConvertIndexUnit(t, PathIndexUnit.Normalized, PathIndexUnit.Distance);
            t_dist += dialogStartDistance;
            float3 position = spline.EvaluatePosition(spline.ConvertIndexUnit(t_dist, PathIndexUnit.Distance, PathIndexUnit.Normalized));
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.3f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, (Vector3)position + railTrack.transform.position);
            Gizmos.DrawSphere((Vector3)position + railTrack.transform.position, 0.1f);
        }
    }
    public void StartDialogue()
    {
        StartCoroutine(DisplayAllDialogues());
    }

    IEnumerator DisplayAllDialogues()
    {
        inProgress = true;
        dialogueBox.SetActive(true);
        foreach (DialogueItem dialogue in dialogueList)
        {
            yield return DisplayDialogue(dialogue);
        }
        inProgress = false;
        Destroy(gameObject.transform.parent.gameObject);
    }

    IEnumerator DisplayDialogue(DialogueItem dialogueItem)
    {
        dialogueTextUI.text = dialogueItem.text;
        yield return new WaitForSeconds(dialogueItem.screenTime);
    }
}
