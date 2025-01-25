using UnityEngine;

public class Bubble : MonoBehaviour {
    public Color bubbleColor;
    public bool isSelectedBubble;
    public LevelManager manager;

    void Awake() {
        isSelectedBubble = false;       
    }

    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("bubble")) {
            Debug.Log("collided with something other than a bubble");
            return;
        }
        if (!isSelectedBubble) {
            Debug.Log("you are not the main bubble, stay put");
            return;
        }

        Debug.Log("Merging bubbles");
        manager.MergingBubbles(transform.gameObject, collision.gameObject);
    }
}
