using UnityEngine;

public class SnapScrolling : MonoBehaviour

{
    [Range(1, 15)]
    [Header("Controllers")]
	public int panCount;
	[Range(0 ,500)]
	public int panOffset;
	[Range(0f, 20f)]
	public float snapSpeed;
	[Range(0f, 5f)]
	public float scaleOffset;
	[Header("Other Objects")]
	public GameObject panPrefab;

	private GameObject[] instPans;
	private Vector2[] pansPos;
	private Vector2[] pansScale;

	private RectTransform contentRect;
	private Vector2 contentVector;

	private int selectedPanID;
	private bool isScrolling;

	private void Start () 
	{
		contentRect = GetComponent<RectTransform>();
		instPans = new GameObject[panCount];
		pansPos = new Vector2[panCount];
		pansScale = new Vector2[panCount];
		for (int i=0; i< panCount; i++)

		{
			instPans[i] = Instantiate(panPrefab, transform, false);
			if (i == 0) continue;
			instPans[i].transform.localPosition = new Vector2(instPans[i-1].transform.localPosition.x + panPrefab.GetComponent<RectTransform>().sizeDelta.x + panOffset, instPans[i].transform.localPosition.y);
		    pansPos[i] = -instPans[i].transform.localPosition;
		}
	}
	private void FixedUpdate()
	{
		float nearestPos = float.MaxValue;
		for (int i=0; i < panCount; i++)
		{
			float distance = Mathf.Abs(contentRect.anchoredPosition.x - pansPos[i].x);
			if (distance < nearestPos) 
			{
				nearestPos = distance;
				selectedPanID = i;

			}
			float scale = Mathf.Clamp(1 / (distance / panOffset) * scaleOffset, 0.5f, 1f);
			pansScale[i].x = Mathf.SmoothStep(instPans[i].transform.localScale.x, scale, 6 * Time.fixedDeltaTime);
			pansScale[i].y = Mathf.SmoothStep(instPans[i].transform.localScale.y, scale, 6 * Time.fixedDeltaTime);
			instPans[i].transform.localScale = pansScale[i];

		}
		if (isScrolling) return;
		contentVector.x = Mathf.SmoothStep(contentRect.anchoredPosition.x, pansPos[selectedPanID].x, snapSpeed * Time.fixedDeltaTime);
		contentRect.anchoredPosition = contentVector;
	}
	public void Scrolling(bool scroll)
	{
       isScrolling = scroll;
	}
}