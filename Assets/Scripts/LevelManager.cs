using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour {
    private GameObject selectedObject;
    public GameObject bubblePrefab;
    public List<GameObject> bubbles;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("mouse button down");
            if (selectedObject == null) {
                Debug.Log("selected obj null");
                RaycastHit hit;
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

                if (hit.collider != null) {
                    Debug.Log("hit collider not null");
                    if (!hit.collider.CompareTag("bubble")) {
                        Debug.Log("tag not bubble");
                        return;
                    }
                    Debug.Log("tag is bubble");
                    selectedObject = hit.collider.gameObject;
                    foreach (var bubble in bubbles) {
                        if (bubble.name == selectedObject.name) {
                            bubble.GetComponent<Bubble>().isSelectedBubble = true;
                        }
                    }
                    Cursor.visible = false;
                }
            }
        }
            
        if (Input.GetMouseButtonDown(1)) {
            Cursor.visible = true;
                foreach (var bubble in bubbles) {
                    if (bubble.name == selectedObject.name) {
                        bubble.GetComponent<Bubble>().isSelectedBubble = false;
                    }
                }
            selectedObject = null;
        }

        if (selectedObject != null) {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                selectedObject.transform.position = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
            }
        
    }

    public void MergingBubbles(GameObject bubble1, GameObject bubble2) {
        Debug.Log("MergingBubbles() start");
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
        newBubble.GetComponent<MeshRenderer>().material.color = CalculateNewColor(bubble1Color, bubble2Color);
        bubbles.Add(newBubble);
        selectedObject = newBubble;
        Debug.Log("Created a new bubble");
    }

    private Color CalculateNewColor(Color color1, Color color2) {
        return new Color((color1.r+color2.r) / 2, (color1.g + color2.g) / 2, (color1.b + color2.b) / 2, 1);
    }

    private Vector3 CalculateNewSize(Vector3 scale1, Vector3 scale2) {
        return new Vector3(scale1.x + scale2.x, scale1.y + scale2.y, scale1.z + scale2.z);
    }
}
