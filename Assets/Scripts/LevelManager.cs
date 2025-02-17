using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour {
    private GameObject selectedObject;
    public GameObject targetBubble;
    public Button nextLevelButton;
    public GameObject bubblePrefab;
    public List<GameObject> bubbles;
    public AudioClip[] poppingSounds;

    [SerializeField]
    private float smoothSpeed = 11f; // Adjust this for smoother/slower following

    private void Awake() {
        nextLevelButton.gameObject.SetActive(false);
        Cursor.visible = true;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("mouse button down");
            if (selectedObject == null)
            {
                Debug.Log("selected obj null");
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity);

                if (hit.collider != null)
                {
                    Debug.Log("hit collider not null");
                    if (!hit.collider.CompareTag("bubble"))
                    {
                        Debug.Log("tag not bubble");
                        return;
                    }
                    Debug.Log("tag is bubble");
                    selectedObject = hit.collider.gameObject;
                    foreach (var bubble in bubbles)
                    {
                        if (bubble.name == selectedObject.name)
                        {
                            bubble.GetComponent<Bubble>().isSelectedBubble = true;
                            EnableParticleEffect(bubble, true); // Enable particle effect
                        }
                    }
                    Cursor.visible = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.visible = true;
            foreach (var bubble in bubbles)
            {
                if (bubble.name == selectedObject.name)
                {
                    bubble.GetComponent<Bubble>().isSelectedBubble = false;
                    EnableParticleEffect(bubble, false); // Disable particle effect
                }
            }
            selectedObject = null;
        }

        if (selectedObject != null)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // Smoothly interpolate the bubble's position towards the target position
            Vector3 targetPosition = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
            selectedObject.transform.position = Vector3.Lerp(selectedObject.transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        }
    }

    private void EnableParticleEffect(GameObject bubble, bool enable)
    {
        // Get the ParticleSystem component from the child object
        ParticleSystem particleSystem = bubble.GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            var emission = particleSystem.emission;
            emission.rateOverTime = enable ? 9 : 0; // Set emission rate
        }
    }

    public void MergingBubbles(GameObject bubble1, GameObject bubble2) {
        Debug.Log("MergingBubbles() start");
        GetComponent<AudioSource>().PlayOneShot(poppingSounds[Random.Range(0, poppingSounds.Length-1)]);

        Color bubble1Color = new Color(1, 1, 1);
        Color bubble2Color = new Color(1, 1, 1);
        Vector3 bubble1Scale = new Vector3(0, 0, 0);
        Vector3 bubble2Scale = new Vector3(0, 0, 0);
        Vector3 position = new Vector3(0, 0, 0);

        for(int i = 0; i < bubbles.Count; i++) {
            if (bubbles[i].name == bubble1.name) {
                bubble1Color = bubbles[i].GetComponent<Bubble>().bubbleColor;
                bubble1Scale = bubbles[i].transform.localScale;
                position = bubbles[i].transform.position;

                bubbles.Remove(bubble1);
                Destroy(bubble1);
                Debug.Log("Destroyed bubble1");
                break;
            }
        }

        for (int i = 0; i < bubbles.Count; i++) {
            if (bubbles[i].name == bubble2.name) {
                bubble2Color = bubbles[i].GetComponent<Bubble>().bubbleColor;
                bubble2Scale = bubbles[i].transform.localScale;

                bubbles.Remove(bubble2);
                Destroy(bubble2);
                Debug.Log("Destroyed bubble2");
                break;
            }
        }

        GameObject newBubble;
        newBubble = Instantiate(bubblePrefab);
        newBubble.transform.position = position;
        newBubble.GetComponent<Bubble>().bubbleColor = CalculateNewColor(bubble1Color, bubble2Color);
        newBubble.transform.localScale = CalculateNewSize(bubble1Scale, bubble2Scale);
        newBubble.GetComponent<Bubble>().isSelectedBubble = true;
        newBubble.GetComponent<Bubble>().manager = this;
        //newBubble.GetComponent<MeshRenderer>().material.SetColor("_Water_Color", CalculateNewColor(bubble1Color, bubble2Color));
        newBubble.GetComponent<MeshRenderer>().material.color = CalculateNewColor(bubble1Color, bubble2Color);
        bubbles.Add(newBubble);
        selectedObject = newBubble;
        EnableParticleEffect(newBubble, true); // enable particle of new bubble
        Debug.Log("Created a new bubble");

        CheckForSuccess(newBubble);
    }

    private Color CalculateNewColor(Color color1, Color color2) {
        if ((color1.CompareRGB(new Color(1,1,0,1)) && color2.CompareRGB(Color.blue)) || (color1.CompareRGB(Color.blue) && color2.CompareRGB(new Color(1, 1, 0, 1)))) {
            return Color.green;
        }
        return new Color((color1.r+color2.r) / 2, (color1.g + color2.g) / 2, (color1.b + color2.b) / 2, 1);
    }

    private Vector3 CalculateNewSize(Vector3 scale1, Vector3 scale2) {
        return new Vector3(scale1.x + scale2.x, scale1.y + scale2.y, scale1.z + scale2.z);
    }

    private void CheckForSuccess(GameObject latestBubble) {
        if (latestBubble.GetComponent<Bubble>().bubbleColor == targetBubble.GetComponent<Bubble>().bubbleColor) {
            Debug.Log("Congratulations, it's a match!");
            
            nextLevelButton.gameObject.SetActive(true);
        }
    }

    //Put on the reset button
    public void Reset() {
        SceneManager.LoadScene(gameObject.scene.name);
    }
}
